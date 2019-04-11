using ColossalFramework;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealConstruction.CustomAI
{
    public class CustomCarAI
    {
        public static void CarAIPathfindFailurePostFix(ushort vehicleID, ref Vehicle data)
        {
            RecordFailedBuilding(vehicleID, ref data);
        }

        public static bool NeedCheckPathFind(TransferManager.TransferReason material)
        {
            switch (material)
            {
                case (TransferManager.TransferReason)110:
                case (TransferManager.TransferReason)111:
                    return true;
                default: return false;
            }
        }

        public static void RecordFailedBuilding(ushort vehicleID, ref Vehicle data)
        {
            if (RealConstruction.fixUnRouteTransfer)
            {
                if (NeedCheckPathFind((TransferManager.TransferReason)data.m_transferType))
                {
                    if (data.m_targetBuilding != 0)
                    {
                        if (data.m_sourceBuilding != 0)
                        {
                            bool alreadyHaveFailedBuilding = false;
                            for (int j = 0; j < MainDataStore.canNotConnectedBuildingIDCount[data.m_targetBuilding]; j++)
                            {
                                if (MainDataStore.canNotConnectedBuildingID[data.m_targetBuilding, j] == data.m_sourceBuilding)
                                {
                                    alreadyHaveFailedBuilding = true;
                                    break;
                                }
                            }

                            if (!alreadyHaveFailedBuilding)
                            {
                                if (MainDataStore.canNotConnectedBuildingIDCount[data.m_targetBuilding] < 8)
                                {
                                    MainDataStore.canNotConnectedBuildingID[data.m_targetBuilding, MainDataStore.canNotConnectedBuildingIDCount[data.m_targetBuilding]] = data.m_sourceBuilding;
                                    MainDataStore.canNotConnectedBuildingIDCount[data.m_targetBuilding]++;
                                }
                                else
                                { 
                                    DebugLog.LogToFileOnly("Error: Max canNotConnectedBuildingIDCount 8 reached, Please check your roadnetwork");
                                    var building1 = Singleton<BuildingManager>.instance.m_buildings.m_buffer[data.m_targetBuilding];
                                    DebugLog.LogToFileOnly("DebugInfo: building m_class is " + building1.Info.m_class.ToString());
                                    DebugLog.LogToFileOnly("DebugInfo: building name is " + building1.Info.name.ToString());
                                    DebugLog.LogToFileOnly("DebugInfo: building id is " + data.m_targetBuilding.ToString());
                                    DebugLog.LogToFileOnly("Error: Max canNotConnectedBuildingIDCount 8 reached, End");
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ForgetFailedBuilding(ushort vehicleID, ref Vehicle data)
        {
            if (RealConstruction.fixUnRouteTransfer)
            {
                if (data.m_targetBuilding != 0)
                {
                    if (MainDataStore.canNotConnectedBuildingIDCount[data.m_targetBuilding] != 0)
                    {
                        if (MainDataStore.refreshCanNotConnectedBuildingIDCount[data.m_targetBuilding] > 64)
                        {
                            //After several times we can refresh fail building list.
                            MainDataStore.canNotConnectedBuildingIDCount[data.m_targetBuilding]--;
                            MainDataStore.canNotConnectedBuildingID[data.m_targetBuilding, MainDataStore.canNotConnectedBuildingIDCount[data.m_targetBuilding]] = 0;
                            MainDataStore.refreshCanNotConnectedBuildingIDCount[data.m_targetBuilding] = 0;
                        }
                        else
                        {
                            MainDataStore.refreshCanNotConnectedBuildingIDCount[data.m_targetBuilding]++;
                        }
                    }
                }
            }
        }

        public static void CarAIPathfindSuccessPostFix(ushort vehicleID, ref Vehicle data)
        {
            ForgetFailedBuilding(vehicleID, ref data);
        }
    }
}
