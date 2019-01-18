using ColossalFramework;
using ColossalFramework.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RealConstruction
{
    public class CustomCargoTruckAI: CargoTruckAI
    {
        private bool ArriveAtTarget(ushort vehicleID, ref Vehicle data)
        {
            if (data.m_transferType == 112 && Loader.fuelAlarmRunning)
            {
                //DebugLog.LogToFileOnly("vehicle arrive at to gas station for petrol now");
                data.m_transferType = FuelAlarm.MainDataStore.preTranferReason[vehicleID];
                if (FuelAlarm.MainDataStore.petrolBuffer[data.m_targetBuilding] > 400)
                {
                    FuelAlarm.MainDataStore.petrolBuffer[data.m_targetBuilding] -= 400;
                }
                SetTarget(vehicleID, ref data, FuelAlarm.MainDataStore.preTargetBuilding[vehicleID]);
                return true;
            }

            if (data.m_targetBuilding == 0)
            {
                return true;
            }
            int num = 0;
            if ((data.m_flags & Vehicle.Flags.TransferToTarget) != (Vehicle.Flags)0)
            {
                //new added begin
                RealConstructionDetour(vehicleID, ref data);
                if (Loader.fuelAlarmRunning)
                {
                    FuelAlarm.CustomCargoTruckAI.FuelAlarmDetour(vehicleID, ref data);
                }
                //new added end
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


        private void RemoveSource(ushort vehicleID, ref Vehicle data)
        {
            if (data.m_sourceBuilding != 0)
            {
                Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].RemoveOwnVehicle(vehicleID, ref data);
                data.m_sourceBuilding = 0;
            }
        }

        public override void SetSource(ushort vehicleID, ref Vehicle data, ushort sourceBuilding)
        {
            this.RemoveSource(vehicleID, ref data);
            data.m_sourceBuilding = sourceBuilding;
            if (sourceBuilding != 0)
            {
                BuildingManager instance = Singleton<BuildingManager>.instance;
                BuildingInfo info = instance.m_buildings.m_buffer[(int)sourceBuilding].Info;
                data.Unspawn(vehicleID);
                Randomizer randomizer = new Randomizer((int)vehicleID);
                Vector3 vector;
                Vector3 vector2;
                info.m_buildingAI.CalculateSpawnPosition(sourceBuilding, ref instance.m_buildings.m_buffer[(int)sourceBuilding], ref randomizer, this.m_info, out vector, out vector2);
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
                if ((data.m_flags & Vehicle.Flags.TransferToTarget) != (Vehicle.Flags)0)
                {
                    int num = Mathf.Min(0, (int)data.m_transferSize - this.m_cargoCapacity);
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
                    else
                    {
                        info.m_buildingAI.ModifyMaterialBuffer(sourceBuilding, ref instance.m_buildings.m_buffer[(int)sourceBuilding], (TransferManager.TransferReason)data.m_transferType, ref num);
                    }
                    // new added end
                    num = Mathf.Max(0, -num);
                    data.m_transferSize += (ushort)num;
                }
                this.FrameDataUpdated(vehicleID, ref data, ref data.m_frame0);
                instance.m_buildings.m_buffer[(int)sourceBuilding].AddOwnVehicle(vehicleID, ref data);
                if ((instance.m_buildings.m_buffer[(int)sourceBuilding].m_flags & Building.Flags.IncomingOutgoing) != Building.Flags.None)
                {
                    if ((data.m_flags & Vehicle.Flags.TransferToTarget) != (Vehicle.Flags)0)
                    {
                        data.m_flags |= Vehicle.Flags.Importing;
                    }
                    else if ((data.m_flags & Vehicle.Flags.TransferToSource) != (Vehicle.Flags)0)
                    {
                        data.m_flags |= Vehicle.Flags.Exporting;
                    }
                }
            }
        }



        public static void RealConstructionDetour(ushort vehicleID, ref Vehicle vehicleData)
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
                                    if (!Loader.realCityRunning)
                                    {
                                        vehicleData.m_transferSize = 0;
                                        MainDataStore.foodBuffer[vehicleData.m_targetBuilding] += 8000;
                                    }
                                    break;
                                case TransferManager.TransferReason.Lumber:
                                    if (!Loader.realCityRunning)
                                    {
                                        vehicleData.m_transferSize = 0;
                                        MainDataStore.lumberBuffer[vehicleData.m_targetBuilding] += 8000;
                                    }
                                    break;
                                case TransferManager.TransferReason.Coal:
                                    if (!Loader.realCityRunning)
                                    {
                                        vehicleData.m_transferSize = 0;
                                        MainDataStore.coalBuffer[vehicleData.m_targetBuilding] += 8000;
                                    }
                                    break;
                                case TransferManager.TransferReason.Petrol:
                                    if (!Loader.realCityRunning)
                                    {
                                        vehicleData.m_transferSize = 0;
                                        MainDataStore.petrolBuffer[vehicleData.m_targetBuilding] += 8000;
                                    }
                                    break;
                                default:
                                    DebugLog.LogToFileOnly("find a import trade m_transferType error = " + vehicleData.m_transferType.ToString()); break;
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
