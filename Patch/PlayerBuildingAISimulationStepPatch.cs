﻿using ColossalFramework;
using HarmonyLib;
using RealConstruction.CustomAI;
using RealConstruction.NewAI;
using RealConstruction.Util;
using System;
using System.Reflection;

namespace RealConstruction.Patch
{
    [HarmonyPatch]
    public class PlayerBuildingAISimulationStepPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(PlayerBuildingAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Building.Frame).MakeByRefType() }, null);
        }
        public static void Postfix(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            // Update problems
            if (CustomPlayerBuildingAI.CanOperation(buildingID, ref buildingData) && buildingData.m_flags.IsFlagSet(Building.Flags.Completed) && (RealConstruction.operationConsumption != 2))
            {
                OperationAI.ProcessPlayerBuildingOperation(buildingID, ref buildingData);
                if (MainDataStore.operationResourceBuffer[buildingID] > 100)
                {
                    if (buildingData.Info.m_class.m_service == ItemClass.Service.PlayerIndustry)
                    {
                        if (buildingData.Info.m_class.m_subService == ItemClass.SubService.PlayerIndustryFarming)
                        {
                            if (RealConstruction.operationConsumption == 1)
                                MainDataStore.operationResourceBuffer[buildingID] -= 5;
                            else
                                MainDataStore.operationResourceBuffer[buildingID] -= 10;
                        }
                        else if (buildingData.Info.m_class.m_subService == ItemClass.SubService.PlayerIndustryForestry)
                        {
                            if (RealConstruction.operationConsumption == 1)
                                MainDataStore.operationResourceBuffer[buildingID] -= 10;
                            else
                                MainDataStore.operationResourceBuffer[buildingID] -= 20;
                        }
                        else if (buildingData.Info.m_class.m_subService == ItemClass.SubService.PlayerIndustryOre)
                        {
                            if (RealConstruction.operationConsumption == 1)
                                MainDataStore.operationResourceBuffer[buildingID] -= 15;
                            else
                                MainDataStore.operationResourceBuffer[buildingID] -= 30;
                        }
                        else if (buildingData.Info.m_class.m_subService == ItemClass.SubService.PlayerIndustryOil)
                        {
                            if (RealConstruction.operationConsumption == 1)
                                MainDataStore.operationResourceBuffer[buildingID] -= 20;
                            else
                                MainDataStore.operationResourceBuffer[buildingID] -= 40;
                        }
                        else
                        {
                            if (RealConstruction.operationConsumption == 1)
                                MainDataStore.operationResourceBuffer[buildingID] -= 25;
                            else
                                MainDataStore.operationResourceBuffer[buildingID] -= 50;
                        }
                    }
                    else
                    {
                        if (RealConstruction.operationConsumption == 1)
                            MainDataStore.operationResourceBuffer[buildingID] -= 50;
                        else
                            MainDataStore.operationResourceBuffer[buildingID] -= 100;
                    }

                    if (CustomPlayerBuildingAI.CanRemoveNoResource(buildingID, ref buildingData))
                    {
                        Notification.Problem1 problem = Notification.RemoveProblems(buildingData.m_problems, Notification.Problem1.NoResources);
                        buildingData.m_problems = problem;
                    }
                }
                else
                {
                    MainDataStore.operationResourceBuffer[buildingID] = 0;
                    if (RealConstruction.debugMode)
                    {
                        if (buildingData.m_problems == Notification.Problem1.None)
                        {
                            Notification.Problem1 problem = Notification.AddProblems(buildingData.m_problems, Notification.Problem1.NoResources);
                            buildingData.m_problems = problem;
                        }
                    }
                    else
                    {
                        if (CustomPlayerBuildingAI.CanRemoveNoResource(buildingID, ref buildingData))
                        {
                            Notification.Problem1 problem = Notification.RemoveProblems(buildingData.m_problems, Notification.Problem1.NoResources);
                            buildingData.m_problems = problem;
                        }
                    }
                }
            }
            else
            {
                if (CustomPlayerBuildingAI.CanRemoveNoResource(buildingID, ref buildingData))
                {
                    Notification.Problem1 problem = Notification.RemoveProblems(buildingData.m_problems, Notification.Problem1.NoResources);
                    buildingData.m_problems = problem;
                }
            }

            if (CustomPlayerBuildingAI.CanConstruction(buildingID, ref buildingData))
            {
                if (!buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
                {
                    ConstructionAI.ProcessBuildingConstruction(buildingID, ref buildingData, ref frameData);
                    if (MainDataStore.constructionResourceBuffer[buildingID] >= 8000)
                    {
                        Notification.Problem1 problem = Notification.RemoveProblems(buildingData.m_problems, Notification.Problem1.NoResources);
                        buildingData.m_problems = problem;
                    }
                    else
                    {
                        if (RealConstruction.debugMode)
                        {
                            if (buildingData.m_problems == Notification.Problem1.None)
                            {
                                Notification.Problem1 problem = Notification.AddProblems(buildingData.m_problems, Notification.Problem1.NoResources);
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
