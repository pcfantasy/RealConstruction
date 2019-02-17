using ColossalFramework;
using ColossalFramework.Math;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RealConstruction.CustomAI
{
    public class CustomCargoTruckAI: CargoTruckAI
    {
        public void CargoTruckAIArriveAtTargetForRealGasStationPre(ushort vehicleID, ref Vehicle data)
        {
            DebugLog.LogToFileOnly("Error: Should be detour by RealGasStation @ CargoTruckAIArriveAtTargetForRealGasStationPre");
        }

        public void CargoTruckAIArriveAtTargetForRealGasStationPost(ushort vehicleID, ref Vehicle data)
        {
            DebugLog.LogToFileOnly("Error: Should be detour by RealGasStation @ CargoTruckAIArriveAtTargetForRealGasStationPost");
        }

        private bool ArriveAtTarget(ushort vehicleID, ref Vehicle data)
        {
            // NON-STOCK CODE START
            // 112 means fuel demand, see more in RealGasStation mod
            if (data.m_transferType == 112 && Loader.fuelAlarmRunning)
            {
                /*data.m_transferType = FuelAlarm.MainDataStore.preTranferReason[vehicleID];
                if (FuelAlarm.MainDataStore.petrolBuffer[data.m_targetBuilding] > 400)
                {
                    FuelAlarm.MainDataStore.petrolBuffer[data.m_targetBuilding] -= 400;
                }
                SetTarget(vehicleID, ref data, FuelAlarm.MainDataStore.preTargetBuilding[vehicleID]);*/
                CargoTruckAIArriveAtTargetForRealGasStationPre(vehicleID, ref data);
                return true;
            }
            /// NON-STOCK CODE END ///
            if (data.m_targetBuilding == 0)
            {
                return true;
            }
            int num = 0;
            if ((data.m_flags & Vehicle.Flags.TransferToTarget) != (Vehicle.Flags)0)
            {
                // NON-STOCK CODE START
                CargoTruckAIArriveAtTargetForRealConstruction(vehicleID, ref data);
                if (Loader.fuelAlarmRunning)
                {
                    CargoTruckAIArriveAtTargetForRealGasStationPost(vehicleID, ref data);
                }
                /// NON-STOCK CODE END ///
                num = (int)data.m_transferSize;
            }
            if ((data.m_flags & Vehicle.Flags.TransferToSource) != (Vehicle.Flags)0)
            {
                num = Mathf.Min(0, (int)data.m_transferSize - this.m_cargoCapacity);
            }
            BuildingManager instance = Singleton<BuildingManager>.instance;
            BuildingInfo info = instance.m_buildings.m_buffer[(int)data.m_targetBuilding].Info;
            info.m_buildingAI.ModifyMaterialBuffer(data.m_targetBuilding, ref instance.m_buildings.m_buffer[(int)data.m_targetBuilding], (TransferManager.TransferReason)data.m_transferType, ref num);
            if ((data.m_flags & Vehicle.Flags.TransferToTarget) != (Vehicle.Flags)0)
            {
                data.m_transferSize = (ushort)Mathf.Clamp((int)data.m_transferSize - num, 0, (int)data.m_transferSize);
                if (data.m_sourceBuilding != 0)
                {
                    IndustryBuildingAI.ExchangeResource((TransferManager.TransferReason)data.m_transferType, num, data.m_sourceBuilding, data.m_targetBuilding);
                }
            }
            if ((data.m_flags & Vehicle.Flags.TransferToSource) != (Vehicle.Flags)0)
            {
                data.m_transferSize += (ushort)Mathf.Max(0, -num);
            }
            if (data.m_sourceBuilding != 0 && (instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Outgoing)
            {
                BuildingInfo info2 = instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].Info;
                ushort num2 = instance.FindBuilding(instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].m_position, 200f, info2.m_class.m_service, ItemClass.SubService.None, Building.Flags.Incoming, Building.Flags.Outgoing);
                if (num2 != 0)
                {
                    instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].RemoveOwnVehicle(vehicleID, ref data);
                    data.m_sourceBuilding = num2;
                    instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].AddOwnVehicle(vehicleID, ref data);
                }
            }
            if ((instance.m_buildings.m_buffer[(int)data.m_targetBuilding].m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Incoming)
            {
                ushort num3 = instance.FindBuilding(instance.m_buildings.m_buffer[(int)data.m_targetBuilding].m_position, 200f, info.m_class.m_service, ItemClass.SubService.None, Building.Flags.Outgoing, Building.Flags.Incoming);
                if (num3 != 0)
                {
                    data.Unspawn(vehicleID);
                    BuildingInfo info3 = instance.m_buildings.m_buffer[(int)num3].Info;
                    Randomizer randomizer = new Randomizer((int)vehicleID);
                    Vector3 vector;
                    Vector3 vector2;
                    info3.m_buildingAI.CalculateSpawnPosition(num3, ref instance.m_buildings.m_buffer[(int)num3], ref randomizer, this.m_info, out vector, out vector2);
                    Quaternion rotation = Quaternion.identity;
                    Vector3 forward = vector2 - vector;
                    if (forward.sqrMagnitude > 0.01f)
                    {
                        rotation = Quaternion.LookRotation(forward);
                    }
                    data.m_frame0 = new Vehicle.Frame(vector, rotation);
                    data.m_frame1 = data.m_frame0;
                    data.m_frame2 = data.m_frame0;
                    data.m_frame3 = data.m_frame0;
                    data.m_targetPos0 = vector;
                    data.m_targetPos0.w = 2f;
                    data.m_targetPos1 = vector2;
                    data.m_targetPos1.w = 2f;
                    data.m_targetPos2 = data.m_targetPos1;
                    data.m_targetPos3 = data.m_targetPos1;
                    this.FrameDataUpdated(vehicleID, ref data, ref data.m_frame0);
                    this.SetTarget(vehicleID, ref data, 0);
                    return true;
                }
            }
            this.SetTarget(vehicleID, ref data, 0);
            return false;
        }


        public static void CargoTruckAISetSourceForRealConstruction(ushort vehicleID, ref Vehicle data, ushort sourceBuilding)
        {
            CargoTruckAI AI = data.Info.m_vehicleAI as CargoTruckAI;
            int num = Mathf.Min(0, (int)data.m_transferSize - AI.m_cargoCapacity);
            //new added begin
            if (RealConstructionThreading.IsSpecialBuilding(sourceBuilding))
            {
                if ((TransferManager.TransferReason)data.m_transferType == (TransferManager.TransferReason)110)
                {
                    MainDataStore.constructionResourceBuffer[sourceBuilding] -= 8000;
                }
                else if ((TransferManager.TransferReason)data.m_transferType == (TransferManager.TransferReason)111)
                {
                    MainDataStore.operationResourceBuffer[sourceBuilding] -= 8000;
                }
                else
                {
                    DebugLog.LogToFileOnly("find unknow transfor for SpecialBuilding " + data.m_transferType.ToString());
                }
            }
        }

        public static float GetResourcePrice(TransferManager.TransferReason material)
        {
            //need to sync with realcity mod
            switch (material)
            {
                case TransferManager.TransferReason.Petrol:
                    return 3f;
                case TransferManager.TransferReason.Food:
                    return 1.5f;
                case TransferManager.TransferReason.Lumber:
                    return 2f;
                case TransferManager.TransferReason.Coal:
                    return 2.5f;
                default: DebugLog.LogToFileOnly("Error: Unknow material in realconstruction = " + material.ToString()); return 0f;
            }
        }

        public void CargoTruckAIArriveAtTargetForRealConstruction(ushort vehicleID, ref Vehicle vehicleData)
        {
            BuildingManager instance = Singleton<BuildingManager>.instance;
            if (vehicleData.m_targetBuilding != 0)
            {
                Building buildingData = instance.m_buildings.m_buffer[vehicleData.m_targetBuilding];
                if (!(buildingData.Info.m_buildingAI is OutsideConnectionAI))
                {
                    if (buildingData.m_flags.IsFlagSet(Building.Flags.Created) && (!buildingData.m_flags.IsFlagSet(Building.Flags.Completed)) && (!buildingData.m_flags.IsFlagSet(Building.Flags.Deleted)))
                    {
                        if (vehicleData.m_transferType == 110)
                        {
                            vehicleData.m_transferSize = 0;
                            MainDataStore.constructionResourceBuffer[vehicleData.m_targetBuilding] = 8000;
                        }
                    }
                    else
                    {
                        if (RealConstructionThreading.IsSpecialBuilding(vehicleData.m_targetBuilding) == true)
                        {
                            switch ((TransferManager.TransferReason)vehicleData.m_transferType)
                            {
                                case TransferManager.TransferReason.Food:                                    
                                    vehicleData.m_transferSize = 0;
                                    MainDataStore.foodBuffer[vehicleData.m_targetBuilding] += 8000;
                                    if (Loader.realCityRunning)
                                    {
                                        float productionValue1 = 8000 * GetResourcePrice((TransferManager.TransferReason)vehicleData.m_transferType);
                                        Singleton<EconomyManager>.instance.FetchResource(EconomyManager.Resource.ResourcePrice, (int)productionValue1, ItemClass.Service.PlayerIndustry, ItemClass.SubService.PlayerIndustryFarming, ItemClass.Level.Level1);
                                    }
                                    break;
                                case TransferManager.TransferReason.Lumber:
                                    
                                    vehicleData.m_transferSize = 0;
                                    MainDataStore.lumberBuffer[vehicleData.m_targetBuilding] += 8000;
                                    if (Loader.realCityRunning)
                                    {
                                        float productionValue1 = 8000 * GetResourcePrice((TransferManager.TransferReason)vehicleData.m_transferType);
                                        Singleton<EconomyManager>.instance.FetchResource(EconomyManager.Resource.ResourcePrice, (int)productionValue1, ItemClass.Service.PlayerIndustry, ItemClass.SubService.PlayerIndustryForestry, ItemClass.Level.Level1);
                                    }
                                    break;
                                case TransferManager.TransferReason.Coal:
                                    vehicleData.m_transferSize = 0;
                                    MainDataStore.coalBuffer[vehicleData.m_targetBuilding] += 8000;
                                    if (Loader.realCityRunning)
                                    {
                                        float productionValue1 = 8000 * GetResourcePrice((TransferManager.TransferReason)vehicleData.m_transferType);
                                        Singleton<EconomyManager>.instance.FetchResource(EconomyManager.Resource.ResourcePrice, (int)productionValue1, ItemClass.Service.PlayerIndustry, ItemClass.SubService.PlayerIndustryOre, ItemClass.Level.Level1);
                                    }
                                    break;
                                case TransferManager.TransferReason.Petrol:                                    
                                    vehicleData.m_transferSize = 0;
                                    MainDataStore.petrolBuffer[vehicleData.m_targetBuilding] += 8000;
                                    if (Loader.realCityRunning)
                                    {
                                        float productionValue1 = 8000 * GetResourcePrice((TransferManager.TransferReason)vehicleData.m_transferType);
                                        Singleton<EconomyManager>.instance.FetchResource(EconomyManager.Resource.ResourcePrice, (int)productionValue1, ItemClass.Service.PlayerIndustry, ItemClass.SubService.PlayerIndustryOil, ItemClass.Level.Level1);
                                    }
                                    break;
                                default:
                                    DebugLog.LogToFileOnly("Error: Unknow m_transferType in realconstruction = " + vehicleData.m_transferType.ToString()); break;
                            }
                        }
                        else
                        {
                            if (vehicleData.m_transferType == 111)
                            {
                                vehicleData.m_transferSize = 0;
                                MainDataStore.operationResourceBuffer[vehicleData.m_targetBuilding] += 8000;
                            }
                        }
                    }
                }
            }
        }


        public override string GetLocalizedStatus(ushort vehicleID, ref Vehicle data, out InstanceID target)
        {
            if ((data.m_flags & Vehicle.Flags.TransferToTarget) != 0)
            {
                ushort targetBuilding = data.m_targetBuilding;
                if ((data.m_flags & Vehicle.Flags.GoingBack) != 0)
                {
                    target = InstanceID.Empty;
                    TransferManager.TransferReason transferType = (TransferManager.TransferReason)data.m_transferType;
                    if (transferType == (TransferManager.TransferReason)112)
                    {
                        return Language.Strings[11];
                    }
                    return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CARGOTRUCK_RETURN");
                }
                if ((data.m_flags & Vehicle.Flags.WaitingTarget) != 0)
                {
                    target = InstanceID.Empty;
                    return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CARGOTRUCK_UNLOAD");
                }
                if (targetBuilding != 0)
                {
                    Building.Flags flags = Singleton<BuildingManager>.instance.m_buildings.m_buffer[targetBuilding].m_flags;
                    TransferManager.TransferReason transferType = (TransferManager.TransferReason)data.m_transferType;
                    if ((data.m_flags & Vehicle.Flags.Exporting) != 0 || (flags & Building.Flags.IncomingOutgoing) != 0)
                    {
                        target = InstanceID.Empty;
                        if (transferType == (TransferManager.TransferReason)112)
                        {
                            return Language.Strings[11];
                        }
                        return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CARGOTRUCK_EXPORT", transferType.ToString());
                    }
                    if ((data.m_flags & Vehicle.Flags.Importing) != 0)
                    {
                        target = InstanceID.Empty;
                        target.Building = targetBuilding;
                        if (transferType == (TransferManager.TransferReason)112)
                        {
                            return Language.Strings[11];
                        }
                        return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CARGOTRUCK_IMPORT", transferType.ToString());
                    }
                    target = InstanceID.Empty;
                    target.Building = targetBuilding;
                    if (transferType == (TransferManager.TransferReason)110)
                    {
                        return Language.Strings[9];
                    }
                    else if (transferType == (TransferManager.TransferReason)111)
                    {
                        return Language.Strings[10];
                    }
                    else if (transferType == (TransferManager.TransferReason)112)
                    {
                        return Language.Strings[11];
                    }
                    else
                    {
                        return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CARGOTRUCK_DELIVER", transferType.ToString());
                    }
                }
            }
            target = InstanceID.Empty;
            return ColossalFramework.Globalization.Locale.Get("VEHICLE_STATUS_CONFUSED");
        }

    }
}
