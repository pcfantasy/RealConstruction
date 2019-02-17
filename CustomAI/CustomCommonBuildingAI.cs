using ColossalFramework;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealConstruction.CustomAI
{
    public class CustomCommonBuildingAI : BuildingAI
    {
        public static void CustomReleaseBuilding(ushort buildingID)
        {
            MainDataStore.foodBuffer[buildingID] = 0;
            MainDataStore.lumberBuffer[buildingID] = 0;
            MainDataStore.petrolBuffer[buildingID] = 0;
            MainDataStore.coalBuffer[buildingID] = 0;
            MainDataStore.constructionResourceBuffer[buildingID] = 0;
            MainDataStore.operationResourceBuffer[buildingID] = 0;
            if (!Loader.realCityRunning)
            {
                //RealCity Mod will also do this
                TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                offer.Building = buildingID;
                Singleton<TransferManager>.instance.RemoveOutgoingOffer((TransferManager.TransferReason)110, offer);
                Singleton<TransferManager>.instance.RemoveOutgoingOffer((TransferManager.TransferReason)111, offer);
                Singleton<TransferManager>.instance.RemoveOutgoingOffer((TransferManager.TransferReason)112, offer);
            }
        }

        public void CustomSimulationStep(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            if (buildingData.Info.m_animalPlaces != null && buildingData.Info.m_animalPlaces.Length != 0 && (buildingData.m_flags & Building.Flags.Active) != Building.Flags.None)
            {
                this.SpawnAnimals(buildingID, ref buildingData);
            }

            if (buildingData.m_flags.IsFlagSet(Building.Flags.Created) && (!buildingData.m_flags.IsFlagSet(Building.Flags.Deleted)) && (!buildingData.m_flags.IsFlagSet(Building.Flags.Untouchable)))
            {
                if (!(buildingData.Info.m_buildingAI is OutsideConnectionAI) && !((buildingData.Info.m_buildingAI is DecorationBuildingAI)) && !(buildingData.Info.m_buildingAI is WildlifeSpawnPointAI))
                {
                    if (!(buildingData.Info.m_buildingAI is ExtractingDummyAI) && !(buildingData.Info.m_buildingAI is DummyBuildingAI) && !((buildingData.Info.m_buildingAI is PowerPoleAI)) && !(buildingData.Info.m_buildingAI is WaterJunctionAI))
                    {
                        if (!(buildingData.Info.m_buildingAI is IntersectionAI) && !((buildingData.Info.m_buildingAI is CableCarPylonAI)) && !(buildingData.Info.m_buildingAI is MonorailPylonAI))
                        {
                            if (RealConstructionThreading.canOperation(buildingID, ref buildingData) && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
                            {
                                if (MainDataStore.operationResourceBuffer[buildingID] > 1000)
                                {
                                    MainDataStore.operationResourceBuffer[buildingID] -= 100;
                                    Notification.Problem problem = Notification.RemoveProblems(buildingData.m_problems, Notification.Problem.NoResources);
                                    buildingData.m_problems = problem;
                                }
                                else
                                {
                                    if (buildingData.m_problems == Notification.Problem.None)
                                    {
                                        Notification.Problem problem = Notification.AddProblems(buildingData.m_problems, Notification.Problem.NoResources);
                                        buildingData.m_problems = problem;
                                    }
                                }
                            }


                            if (RealConstructionThreading.canConstruction(buildingID, ref buildingData) && !buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
                            {
                                if (MainDataStore.constructionResourceBuffer[buildingID] >= 8000)
                                {
                                    Notification.Problem problem = Notification.RemoveProblems(buildingData.m_problems, Notification.Problem.NoResources);
                                    buildingData.m_problems = problem;
                                }
                                else
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
                }
            }
        }
    }
}
