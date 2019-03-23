using ColossalFramework;
using RealConstruction.Util;

namespace RealConstruction.CustomAI
{
    public class CustomPrivateBuildingAI
    {
        public static void PrivateBuildingAISimulationStepPostFix(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            // Update problems
            RealConstructionThreading.ProcessBuildingConstruction(buildingID, ref buildingData, ref frameData);
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
