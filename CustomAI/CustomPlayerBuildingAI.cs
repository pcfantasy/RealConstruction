using ColossalFramework;
using RealConstruction.NewAI;
using RealConstruction.Util;
using System;
using UnityEngine;

namespace RealConstruction.CustomAI
{
    public class CustomPlayerBuildingAI: CommonBuildingAI
    {
        protected override int GetConstructionTime()
        {
            if ((Singleton<ToolManager>.instance.m_properties.m_mode & ItemClass.Availability.AssetEditor) != ItemClass.Availability.None)
            {
                return 0;
            }
            return 100;
        }

        public int CustomGetBudget(ushort buildingID, ref Building buildingData)
        {
            ushort eventIndex = buildingData.m_eventIndex;
            if (eventIndex != 0)
            {
                EventManager instance = Singleton<EventManager>.instance;
                EventInfo info = instance.m_events.m_buffer[eventIndex].Info;
                return info.m_eventAI.GetBudget(eventIndex, ref instance.m_events.m_buffer[eventIndex]);
            }

            if (MainDataStore.operationResourceBuffer[buildingID] < 1000 && CanOperation(buildingID, ref buildingData))
            {
                return 10;
            }
            else
            {
                return Singleton<EconomyManager>.instance.GetBudget(m_info.m_class);
            }
        }

        public static bool CanOperation(ushort buildingID, ref Building buildingData)
        {
            if (ResourceBuildingAI.IsSpecialBuilding(buildingID))
            {
                return false;
            }
            else if (buildingData.Info.m_buildingAI is ParkBuildingAI)
            {
                return false;
            }
            else
            {
                PlayerBuildingAI AI = buildingData.Info.m_buildingAI as PlayerBuildingAI;
                return AI.RequireRoadAccess();
            }
        }

        public static bool CanConstruction(ushort buildingID, ref Building buildingData)
        {
            if (ResourceBuildingAI.IsSpecialBuilding(buildingID))
            {
                return false;
            }
            else if (buildingData.Info.m_buildingAI is ParkBuildingAI)
            {
                return false;
            }
            else
            {
                PlayerBuildingAI AI = buildingData.Info.m_buildingAI as PlayerBuildingAI;
                return AI.RequireRoadAccess();
            }
        }

        // PlayerBuildingAI
        public static void PlayerBuildingAISimulationStepPostFix(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            // Update problems
            if (CanOperation(buildingID, ref buildingData) && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
            {
                OperationAI.ProcessPlayerBuildingOperation(buildingID, ref buildingData);
                if (MainDataStore.operationResourceBuffer[buildingID] > 100)
                {
                    MainDataStore.isBuildingLackOfResource[buildingID] = false;
                    MainDataStore.operationResourceBuffer[buildingID] -= 100;
                    Notification.Problem problem = Notification.RemoveProblems(buildingData.m_problems, Notification.Problem.NoResources);
                    buildingData.m_problems = problem;
                }
                else
                {
                    MainDataStore.operationResourceBuffer[buildingID] = 0;
                    MainDataStore.isBuildingLackOfResource[buildingID] = true;
                    if (RealConstruction.debugMode)
                    {
                        if (buildingData.m_problems == Notification.Problem.None)
                        {
                            Notification.Problem problem = Notification.AddProblems(buildingData.m_problems, Notification.Problem.NoResources);
                            buildingData.m_problems = problem;
                        }
                    }
                }
            }

            if (CanConstruction(buildingID, ref buildingData))
            {
                if (!buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
                {
                    ConstructionAI.ProcessBuildingConstruction(buildingID, ref buildingData, ref frameData);
                    if (MainDataStore.constructionResourceBuffer[buildingID] >= 8000)
                    {
                        Notification.Problem problem = Notification.RemoveProblems(buildingData.m_problems, Notification.Problem.NoResources);
                        buildingData.m_problems = problem;
                    }
                    else
                    {
                        if (RealConstruction.debugMode)
                        {
                            if (buildingData.m_problems == Notification.Problem.None)
                            {
                                Notification.Problem problem = Notification.AddProblems(buildingData.m_problems, Notification.Problem.NoResources);
                                buildingData.m_problems = problem;
                            }
                        }
                    }
                }
            }

            //
            if (ResourceBuildingAI.IsSpecialBuilding((ushort)buildingID))
            {
                if (buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
                {
                    ResourceBuildingAI.ProcessCityResourceDepartmentBuildingGoods(buildingID, ref buildingData);
                    ResourceBuildingAI.ProcessCityResourceDepartmentBuildingOutgoing(buildingID, ref buildingData);
                    ResourceBuildingAI.ProcessCityResourceDepartmentBuildingIncoming(buildingID, ref buildingData);
                }
            }
        }
    }
}
