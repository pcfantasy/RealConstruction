using ColossalFramework;
using ColossalFramework.Math;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ColossalFramework.Globalization;
using System.Reflection;
using System.IO;
using RealConstruction.CustomAI;
using RealConstruction.Util;
using RealConstruction.UI;
using RealConstruction.CustomManager;
using ColossalFramework.UI;

namespace RealConstruction
{
    public class RealConstructionThreading : ThreadingExtensionBase
    {
        public static bool isFirstTime = true;

        public override void OnBeforeSimulationFrame()
        {
            base.OnBeforeSimulationFrame();
            if (Loader.CurrentLoadMode == LoadMode.LoadGame || Loader.CurrentLoadMode == LoadMode.NewGame)
            {
                if (RealConstruction.IsEnabled)
                {
                    CheckDetour();
                }
            }
        }

        public override void OnAfterSimulationFrame()
        {
            base.OnAfterSimulationFrame();
            if (Loader.CurrentLoadMode == LoadMode.LoadGame || Loader.CurrentLoadMode == LoadMode.NewGame)
            {
                uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
                int num4 = (int)(currentFrameIndex & 255u);
                int num5 = num4 * 192;
                int num6 = (num4 + 1) * 192 - 1;
                //DebugLog.LogToFileOnly("currentFrameIndex num2 = " + currentFrameIndex.ToString());
                if (RealConstruction.IsEnabled)
                {
                    BuildingManager instance = Singleton<BuildingManager>.instance;
                    if (num4 == 255)
                    {
                        PlayerBuildingUI.refeshOnce = true;
                        UniqueFactoryUI.refeshOnce = true;
                        UniqueFactoryButton.refeshOnce = true;
                        WarehouseButton.refeshOnce = true;
                        WareHouseUI.refeshOnce = true;
                        PlayerBuildingButton.refeshOnce = true;
                    }
                    //CustomSimulationStepImpl for 110 111 TransferReason
                    CustomTransferManager.CustomSimulationStepImpl();
                    for (int i = num5; i <= num6; i = i + 1)
                    {
                        if (instance.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Created) && (!instance.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Deleted)))
                        {
                            MainDataStore.isBuildingReleased[i] = false;
                            if (IsSpecialBuilding((ushort)i))
                            {
                                if (instance.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Completed))
                                {
                                    ProcessCityResourceDepartmentBuildingGoods((ushort)i, instance.m_buildings.m_buffer[i]);
                                    ProcessCityResourceDepartmentBuildingOutgoing((ushort)i, instance.m_buildings.m_buffer[i]);
                                    ProcessCityResourceDepartmentBuildingIncoming((ushort)i, instance.m_buildings.m_buffer[i]);
                                }
                            }
                        }
                        /*else
                        {
                            if (!MainDataStore.isBuildingReleased[i])
                            {
                                MainDataStore.isBuildingReleased[i] = true;
                                CustomCommonBuildingAI.CustomReleaseBuilding((ushort)i);
                            }
                        }*/
                    }
                }
            }
        }

        public void DetourAfterLoad()
        {
            //This is for Detour RealCity method
            DebugLog.LogToFileOnly("Init DetourAfterLoad");
            bool detourFailed = false;

            if (Loader.isRealCityRunning)
            {
                Assembly as1 = Assembly.Load("RealCity");
                //1
                DebugLog.LogToFileOnly("Detour RealCityCargoTruckAI::CargoTruckAIArriveAtTargetForRealConstruction calls");
                try
                {
                    Loader.Detours.Add(new Loader.Detour(as1.GetType("RealCity.CustomAI.RealCityCargoTruckAI").GetMethod("CargoTruckAIArriveAtTargetForRealConstruction", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null),
                                           typeof(CustomCargoTruckAI).GetMethod("CargoTruckAIArriveAtTargetForRealConstruction", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null)));
                }
                catch (Exception)
                {
                    DebugLog.LogToFileOnly("Could not detour RealCityCargoTruckAI::CargoTruckAIArriveAtTargetForRealConstruction");
                    detourFailed = true;
                }

                if (detourFailed)
                {
                    DebugLog.LogToFileOnly("DetourAfterLoad failed");
                }
                else
                {
                    DebugLog.LogToFileOnly("DetourAfterLoad successful");
                }
            }
        }

        public void CheckDetour()
        {
            if (isFirstTime && Loader.DetourInited)
            {
                isFirstTime = false;
                DetourAfterLoad();
                if (Loader.DetourInited)
                {
                    DebugLog.LogToFileOnly("ThreadingExtension.OnBeforeSimulationFrame: First frame detected. Checking detours.");
                    List<string> list = new List<string>();
                    foreach (Loader.Detour current in Loader.Detours)
                    {
                        if (!RedirectionHelper.IsRedirected(current.OriginalMethod, current.CustomMethod))
                        {
                            list.Add(string.Format("{0}.{1} with {2} parameters ({3})", new object[]
                            {
                    current.OriginalMethod.DeclaringType.Name,
                    current.OriginalMethod.Name,
                    current.OriginalMethod.GetParameters().Length,
                    current.OriginalMethod.DeclaringType.AssemblyQualifiedName
                            }));
                        }
                    }
                    DebugLog.LogToFileOnly(string.Format("ThreadingExtension.OnBeforeSimulationFrame: First frame detected. Detours checked. Result: {0} missing detours", list.Count));
                    if (list.Count > 0)
                    {
                        string error = "RealConstruction detected an incompatibility with another mod! You can continue playing but it's NOT recommended. RealConstruction will not work as expected. See RealConstruction.log for technical details.";
                        DebugLog.LogToFileOnly(error);
                        string text = "The following methods were overriden by another mod:";
                        foreach (string current2 in list)
                        {
                            text += string.Format("\n\t{0}", current2);
                        }
                        DebugLog.LogToFileOnly(text);
                        UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Incompatibility Issue", text, true);
                    }
                }
            }
        }

        public static bool IsSpecialBuilding(ushort id)
        {
            BuildingManager instance = Singleton<BuildingManager>.instance;
            int num = instance.m_buildings.m_buffer[id].Info.m_buildingAI.GetConstructionCost();
            if (num == 208600)
            {
                return true;
            }
            return false;
        }

        public static void ProcessBuildingConstruction(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            if (MainDataStore.constructionResourceBuffer[buildingID] < 8000 && (!IsSpecialBuilding(buildingID)))
            {
                System.Random rand = new System.Random();
                if (buildingData.m_flags.IsFlagSet(Building.Flags.Created) && (!buildingData.m_flags.IsFlagSet(Building.Flags.Completed)) && (!buildingData.m_flags.IsFlagSet(Building.Flags.Deleted)))
                {
                    frameData.m_constructState = 10;
                    int num27 = 0;
                    int num28 = 0;
                    int num29 = 0;
                    int value = 0;
                    int num34 = 0;
                    TransferManager.TransferReason incomingTransferReason = default(TransferManager.TransferReason);
                    //construction resource
                    incomingTransferReason = (TransferManager.TransferReason)110;
                    num27 = 0;
                    num28 = 0;
                    num29 = 0;
                    value = 0;
                    num34 = 0;
                    if (incomingTransferReason != TransferManager.TransferReason.None)
                    {
                        CalculateGuestVehicles(buildingID, ref buildingData, incomingTransferReason, ref num27, ref num28, ref num29, ref value);
                        buildingData.m_tempImport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempImport, 255);
                    }

                    num34 = 8000 - MainDataStore.constructionResourceBuffer[buildingID] - num29;
                    if (num34 > 0)
                    {
                        TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                        offer.Priority = rand.Next(8);
                        if ((buildingData.Info.m_class.m_service != ItemClass.Service.Residential) && (buildingData.Info.m_class.m_service != ItemClass.Service.Industrial) && (buildingData.Info.m_class.m_service != ItemClass.Service.Commercial) && (buildingData.Info.m_class.m_service != ItemClass.Service.Office))
                        {
                            offer.Priority = 7;
                        }
                        offer.Building = buildingID;
                        offer.Position = buildingData.m_position;
                        offer.Amount = 1;
                        offer.Active = false;
                        Singleton<TransferManager>.instance.AddIncomingOffer(incomingTransferReason, offer);
                    }
                }
            }
            else
            {
                if (!IsSpecialBuilding(buildingID) && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
                {
                    MainDataStore.constructionResourceBuffer[buildingID] = 0;
                }
            }
        }

        public static void ProcessPlayerBuildingOperation(ushort buildingID, ref Building buildingData)
        {
            if (buildingData.m_fireIntensity == 0 && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
            {
                int num27 = 0;
                int num28 = 0;
                int num29 = 0;
                int value = 0;
                int num34 = 0;
                TransferManager.TransferReason incomingTransferReason = default(TransferManager.TransferReason);
                //operation resource
                incomingTransferReason = (TransferManager.TransferReason)111;
                num27 = 0;
                num28 = 0;
                num29 = 0;
                value = 0;
                num34 = 0;
                if (incomingTransferReason != TransferManager.TransferReason.None)
                {
                    CalculateGuestVehicles(buildingID, ref buildingData, incomingTransferReason, ref num27, ref num28, ref num29, ref value);
                    buildingData.m_tempImport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempImport, 255);
                }

                num34 = 15000 - MainDataStore.operationResourceBuffer[buildingID] - num29;
                if (num34 > 0)
                {
                    TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                    //Higher priority for Electricity and Water sevice.
                    if (buildingData.Info.m_class.m_service == ItemClass.Service.Water || buildingData.Info.m_class.m_service == ItemClass.Service.Electricity || buildingData.Info.m_class.m_service == ItemClass.Service.Garbage)
                    {
                        offer.Priority = num34 / 1000;
                    }
                    else
                    {
                        offer.Priority = num34 / 3000;
                    }
                    if (offer.Priority > 7)
                    {
                        offer.Priority = 7;
                    }
                    offer.Building = buildingID;
                    offer.Position = buildingData.m_position;
                    offer.Amount = 1;
                    offer.Active = false;
                    Singleton<TransferManager>.instance.AddIncomingOffer(incomingTransferReason, offer);
                }
            }
        }

        void ProcessCityResourceDepartmentBuildingGoods(ushort buildingID, Building buildingData)
        {
            if (buildingData.m_fireIntensity == 0 && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
            {
                if (MainDataStore.lumberBuffer[buildingID] > 40 && MainDataStore.coalBuffer[buildingID] > 40 && MainDataStore.constructionResourceBuffer[buildingID] < 64000)
                {
                    if (MainDataStore.resourceCategory[buildingID] == 0 || MainDataStore.resourceCategory[buildingID] == 1)
                    {
                        MainDataStore.lumberBuffer[buildingID] -= 40;
                        MainDataStore.coalBuffer[buildingID] -= 40;
                        MainDataStore.constructionResourceBuffer[buildingID] += 800;
                    }
                }

                if (MainDataStore.petrolBuffer[buildingID] > 40 && MainDataStore.foodBuffer[buildingID] > 40 && MainDataStore.operationResourceBuffer[buildingID] < 64000)
                {
                    if (MainDataStore.resourceCategory[buildingID] == 0 || MainDataStore.resourceCategory[buildingID] == 2)
                    {
                        MainDataStore.petrolBuffer[buildingID] -= 40;
                        MainDataStore.foodBuffer[buildingID] -= 40;
                        MainDataStore.operationResourceBuffer[buildingID] += 800;
                    }
                }
            }
        }


        void ProcessCityResourceDepartmentBuildingOutgoing(ushort buildingID, Building buildingData)
        {
            int num27 = 0;
            int num28 = 0;
            int num29 = 0;
            int value = 0;
            TransferManager.TransferReason outgoingTransferReason = default(TransferManager.TransferReason);

            //constructionResource
            System.Random rand = new System.Random();
            outgoingTransferReason = (TransferManager.TransferReason)110;
            if (outgoingTransferReason != TransferManager.TransferReason.None)
            {
                CalculateOwnVehicles(buildingID, ref buildingData, outgoingTransferReason, ref num27, ref num28, ref num29, ref value);
                buildingData.m_tempExport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempExport, 255);
            }

            if (buildingData.m_fireIntensity == 0 && outgoingTransferReason != TransferManager.TransferReason.None && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
            {
                int num36 = 10;
                int customBuffer = MainDataStore.constructionResourceBuffer[buildingID];
                if (customBuffer >= 8000 && num27 < num36)
                {
                    TransferManager.TransferOffer offer2 = default(TransferManager.TransferOffer);
                    offer2.Priority = rand.Next(8);
                    offer2.Building = buildingID;
                    offer2.Position = buildingData.m_position;
                    offer2.Amount = Mathf.Min(customBuffer / 8000, num36 - num27);
                    offer2.Active = true;
                    Singleton<TransferManager>.instance.AddOutgoingOffer(outgoingTransferReason, offer2);
                }
            }

            //operationResource
            outgoingTransferReason = (TransferManager.TransferReason)111;
            if (outgoingTransferReason != TransferManager.TransferReason.None)
            {
                CalculateOwnVehicles(buildingID, ref buildingData, outgoingTransferReason, ref num27, ref num28, ref num29, ref value);
                buildingData.m_tempExport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempExport, 255);
            }

            if (buildingData.m_fireIntensity == 0 && outgoingTransferReason != TransferManager.TransferReason.None && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
            {
                int num36 = 10;
                int customBuffer = MainDataStore.operationResourceBuffer[buildingID];
                if (customBuffer >= 8000 && num27 < num36)
                {
                    TransferManager.TransferOffer offer2 = default(TransferManager.TransferOffer);
                    offer2.Priority = rand.Next(8);
                    offer2.Building = buildingID;
                    offer2.Position = buildingData.m_position;
                    offer2.Amount = Mathf.Min((customBuffer) / 8000, num36 - num27);
                    offer2.Active = true;
                    Singleton<TransferManager>.instance.AddOutgoingOffer(outgoingTransferReason, offer2);
                }
            }
        }

        public void ProcessCityResourceDepartmentBuildingIncoming(ushort buildingID, Building buildingData)
        {
            int num27 = 0;
            int num28 = 0;
            int num29 = 0;
            int value = 0;
            int num34 = 0;
            TransferManager.TransferReason incomingTransferReason = default(TransferManager.TransferReason);
            //Foods
            if (MainDataStore.resourceCategory[buildingID] == 0 || MainDataStore.resourceCategory[buildingID] == 2)
            {
                incomingTransferReason = TransferManager.TransferReason.Food;
                if (incomingTransferReason != TransferManager.TransferReason.None)
                {
                    CalculateGuestVehicles(buildingID, ref buildingData, incomingTransferReason, ref num27, ref num28, ref num29, ref value);
                    buildingData.m_tempImport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempImport, 255);
                }

                num34 = 4000 - MainDataStore.foodBuffer[buildingID] - num29;
                if (buildingData.m_fireIntensity == 0 && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
                {
                    if (num34 >= 0)
                    {
                        TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                        offer.Priority = 7;
                        offer.Building = buildingID;
                        offer.Position = buildingData.m_position;
                        offer.Amount = 1;
                        offer.Active = false;
                        Singleton<TransferManager>.instance.AddIncomingOffer(incomingTransferReason, offer);
                    }
                }
                //Petrol
                incomingTransferReason = TransferManager.TransferReason.Petrol;
                num27 = 0;
                num28 = 0;
                num29 = 0;
                value = 0;
                num34 = 0;
                if (incomingTransferReason != TransferManager.TransferReason.None && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
                {
                    CalculateGuestVehicles(buildingID, ref buildingData, incomingTransferReason, ref num27, ref num28, ref num29, ref value);
                    buildingData.m_tempImport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempImport, 255);
                }

                num34 = 4000 - MainDataStore.petrolBuffer[buildingID] - num29;
                if (buildingData.m_fireIntensity == 0)
                {
                    if (num34 >= 0)
                    {
                        TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                        offer.Priority = 7;
                        offer.Building = buildingID;
                        offer.Position = buildingData.m_position;
                        offer.Amount = 1;
                        offer.Active = false;
                        Singleton<TransferManager>.instance.AddIncomingOffer(incomingTransferReason, offer);
                    }
                }
            }
            //Coal
            if (MainDataStore.resourceCategory[buildingID] == 0 || MainDataStore.resourceCategory[buildingID] == 1)
            {
                incomingTransferReason = TransferManager.TransferReason.Coal;
                num27 = 0;
                num28 = 0;
                num29 = 0;
                value = 0;
                num34 = 0;
                if (incomingTransferReason != TransferManager.TransferReason.None && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
                {
                    CalculateGuestVehicles(buildingID, ref buildingData, incomingTransferReason, ref num27, ref num28, ref num29, ref value);
                    buildingData.m_tempImport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempImport, 255);
                }

                num34 = 4000 - MainDataStore.coalBuffer[buildingID] - num29;
                if (buildingData.m_fireIntensity == 0)
                {
                    if (num34 >= 0)
                    {
                        TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                        offer.Priority = 7;
                        offer.Building = buildingID;
                        offer.Position = buildingData.m_position;
                        offer.Amount = 1;
                        offer.Active = false;
                        Singleton<TransferManager>.instance.AddIncomingOffer(incomingTransferReason, offer);
                    }
                }

                //Lumber
                incomingTransferReason = TransferManager.TransferReason.Lumber;
                num27 = 0;
                num28 = 0;
                num29 = 0;
                value = 0;
                num34 = 0;
                if (incomingTransferReason != TransferManager.TransferReason.None)
                {
                    CalculateGuestVehicles(buildingID, ref buildingData, incomingTransferReason, ref num27, ref num28, ref num29, ref value);
                    buildingData.m_tempImport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempImport, 255);
                }

                num34 = 4000 - MainDataStore.lumberBuffer[buildingID] - num29;
                if (buildingData.m_fireIntensity == 0 && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
                {
                    if (num34 >= 0)
                    {
                        TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                        offer.Priority = 7;
                        offer.Building = buildingID;
                        offer.Position = buildingData.m_position;
                        offer.Amount = 1;
                        offer.Active = false;
                        Singleton<TransferManager>.instance.AddIncomingOffer(incomingTransferReason, offer);
                    }
                }
            }
        }

        public static void CalculateGuestVehicles(ushort buildingID, ref Building data, TransferManager.TransferReason material, ref int count, ref int cargo, ref int capacity, ref int outside)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort num = data.m_guestVehicles;
            int num2 = 0;
            while (num != 0)
            {
                if ((TransferManager.TransferReason)instance.m_vehicles.m_buffer[(int)num].m_transferType == material)
                {
                    VehicleInfo info = instance.m_vehicles.m_buffer[(int)num].Info;
                    int a;
                    int num3;
                    info.m_vehicleAI.GetSize(num, ref instance.m_vehicles.m_buffer[(int)num], out a, out num3);
                    cargo += Mathf.Min(a, num3);
                    capacity += num3;
                    count++;
                    if ((instance.m_vehicles.m_buffer[(int)num].m_flags & (Vehicle.Flags.Importing | Vehicle.Flags.Exporting)) != (Vehicle.Flags)0)
                    {
                        outside++;
                    }
                }
                num = instance.m_vehicles.m_buffer[(int)num].m_nextGuestVehicle;
                if (++num2 > 16384)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }

        public static void CalculateOwnVehicles(ushort buildingID, ref Building data, TransferManager.TransferReason material, ref int count, ref int cargo, ref int capacity, ref int outside)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort num = data.m_ownVehicles;
            int num2 = 0;
            while (num != 0)
            {
                if ((TransferManager.TransferReason)instance.m_vehicles.m_buffer[(int)num].m_transferType == material)
                {
                    VehicleInfo info = instance.m_vehicles.m_buffer[(int)num].Info;
                    int a;
                    int num3;
                    info.m_vehicleAI.GetSize(num, ref instance.m_vehicles.m_buffer[(int)num], out a, out num3);
                    cargo += Mathf.Min(a, num3);
                    capacity += num3;
                    count++;
                    if ((instance.m_vehicles.m_buffer[(int)num].m_flags & (Vehicle.Flags.Importing | Vehicle.Flags.Exporting)) != (Vehicle.Flags)0)
                    {
                        outside++;
                    }
                }
                num = instance.m_vehicles.m_buffer[(int)num].m_nextOwnVehicle;
                if (++num2 > 16384)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }
    }
}
