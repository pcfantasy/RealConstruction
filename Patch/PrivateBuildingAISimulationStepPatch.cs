using ColossalFramework;
using Harmony;
using RealConstruction.NewAI;
using RealConstruction.Util;
using System;
using System.Reflection;

namespace RealConstruction.Patch
{
    [HarmonyPatch]
    public class PrivateBuildingAISimulationStepPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(PrivateBuildingAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Building.Frame).MakeByRefType() }, null);
        }
        public static void Postfix(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            // Update problems
            ConstructionAI.ProcessBuildingConstruction(buildingID, ref buildingData, ref frameData);
            if (!buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
            {
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
            else
            {
                Notification.Problem problem = Notification.RemoveProblems(buildingData.m_problems, Notification.Problem.NoResources);
                buildingData.m_problems = problem;
            }
        }
    }
}
