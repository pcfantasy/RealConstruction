using ColossalFramework;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (RealConstructionThreading.IsSpecialBuilding(buildingID))
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
            if (RealConstructionThreading.IsSpecialBuilding(buildingID))
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
        public override void SimulationStep(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            base.SimulationStep(buildingID, ref buildingData, ref frameData);
            // NON-STOCK CODE START
            // Update problems
            if (CanOperation(buildingID, ref buildingData) && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
            {
                RealConstructionThreading.ProcessPlayerBuildingOperation(buildingID, ref buildingData);
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
                    RealConstructionThreading.ProcessBuildingConstruction(buildingID, ref buildingData, ref frameData);
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
            /// NON-STOCK CODE END
            if ((buildingData.m_flags & Building.Flags.Completed) != Building.Flags.None)
            {
                DistrictManager instance = Singleton<DistrictManager>.instance;
                byte district = instance.GetDistrict(buildingData.m_position);
                District[] expr_43_cp_0_cp_0 = instance.m_districts.m_buffer;
                byte expr_43_cp_0_cp_1 = district;
                expr_43_cp_0_cp_0[(int)expr_43_cp_0_cp_1].m_playerData.m_tempBuildingCount = (ushort)(expr_43_cp_0_cp_0[(int)expr_43_cp_0_cp_1].m_playerData.m_tempBuildingCount + 1);
                District[] expr_67_cp_0_cp_0 = instance.m_districts.m_buffer;
                byte expr_67_cp_0_cp_1 = district;
                expr_67_cp_0_cp_0[(int)expr_67_cp_0_cp_1].m_playerData.m_tempBuildingArea = expr_67_cp_0_cp_0[(int)expr_67_cp_0_cp_1].m_playerData.m_tempBuildingArea + (uint)(buildingData.Width * buildingData.Length);
                if ((buildingData.m_flags & Building.Flags.Collapsed) != Building.Flags.None && frameData.m_fireDamage == 255)
                {
                    District[] expr_B7_cp_0_cp_0 = instance.m_districts.m_buffer;
                    byte expr_B7_cp_0_cp_1 = district;
                    expr_B7_cp_0_cp_0[(int)expr_B7_cp_0_cp_1].m_playerData.m_tempBurnedCount = (ushort)(expr_B7_cp_0_cp_0[(int)expr_B7_cp_0_cp_1].m_playerData.m_tempBurnedCount + 1);
                }
                else if ((buildingData.m_flags & Building.Flags.Collapsed) != Building.Flags.None && frameData.m_fireDamage != 255)
                {
                    District[] expr_101_cp_0_cp_0 = instance.m_districts.m_buffer;
                    byte expr_101_cp_0_cp_1 = district;
                    expr_101_cp_0_cp_0[(int)expr_101_cp_0_cp_1].m_playerData.m_tempCollapsedCount = (ushort)(expr_101_cp_0_cp_0[(int)expr_101_cp_0_cp_1].m_playerData.m_tempCollapsedCount + 1);
                }
            }
            if ((ulong)(Singleton<SimulationManager>.instance.m_currentFrameIndex >> 8 & 15u) == (ulong)((long)(buildingID & 15)))
            {
                int constructionCost = this.GetConstructionCost();
                if (constructionCost != 0)
                {
                    StatisticBase statisticBase = Singleton<StatisticsManager>.instance.Acquire<StatisticInt64>(StatisticType.CityValue);
                    if (statisticBase != null)
                    {
                        statisticBase.Add(constructionCost);
                    }
                }
            }
            if ((buildingData.m_flags & Building.Flags.Active) == Building.Flags.None)
            {
                buildingData.m_flags &= ~Building.Flags.EventActive;
                base.EmptyBuilding(buildingID, ref buildingData, CitizenUnit.Flags.Created, false);
            }
            else if (buildingData.m_eventIndex != 0)
            {
                if ((Singleton<EventManager>.instance.m_events.m_buffer[(int)buildingData.m_eventIndex].m_flags & EventData.Flags.Active) != EventData.Flags.None)
                {
                    buildingData.m_flags |= Building.Flags.EventActive;
                }
                else
                {
                    buildingData.m_flags &= ~Building.Flags.EventActive;
                }
            }
        }

    }
}
