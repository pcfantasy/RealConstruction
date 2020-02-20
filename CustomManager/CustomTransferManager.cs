using ColossalFramework;
using ColossalFramework.Plugins;
using RealConstruction.CustomAI;
using RealConstruction.NewAI;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace RealConstruction.CustomManager
{
    public class CustomTransferManager: TransferManager
    {
        public static bool _init = false;
        public static void StartSpecialBuildingTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            VehicleInfo vehicleInfo = null;
            if (material == (TransferManager.TransferReason)110)
            {
                vehicleInfo = Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, ItemClass.Service.Industrial, ItemClass.SubService.IndustrialForestry, ItemClass.Level.Level1);
            }
            else if (material == (TransferManager.TransferReason)111)
            {
                vehicleInfo = Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, ItemClass.Service.Industrial, ItemClass.SubService.IndustrialFarming, ItemClass.Level.Level1);
            }


            if (vehicleInfo != null)
            {
                Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                ushort num16;
                if (Singleton<VehicleManager>.instance.CreateVehicle(out num16, ref Singleton<SimulationManager>.instance.m_randomizer, vehicleInfo, data.m_position, material, false, true))
                {
                    vehicleInfo.m_vehicleAI.SetSource(num16, ref vehicles.m_buffer[(int)num16], buildingID);
                    if (vehicleInfo.m_vehicleAI is CargoTruckAI)
                    {
                        CargoTruckAI AI = vehicleInfo.m_vehicleAI as CargoTruckAI;
                        CustomCargoTruckAI.CargoTruckAISetSourceForRealConstruction(num16, ref vehicles.m_buffer[(int)num16], buildingID);
                        vehicles.m_buffer[(int)num16].m_transferSize = (ushort)AI.m_cargoCapacity;
                    }
                    else
                    {
                        DebugLog.LogToFileOnly("Error: vehicleInfo is not cargoTruckAI " + vehicleInfo.m_vehicleAI.ToString());
                    }
                    vehicleInfo.m_vehicleAI.StartTransfer(num16, ref vehicles.m_buffer[(int)num16], material, offer);
                    ushort building4 = offer.Building;
                    if (building4 != 0)
                    {
                        int amount;
                        int num17;
                        vehicleInfo.m_vehicleAI.GetSize(num16, ref vehicles.m_buffer[(int)num16], out amount, out num17);
                    }
                }
            }
        }

        public static void StartTransfer(TransferManager.TransferReason material, TransferManager.TransferOffer offerOut, TransferManager.TransferOffer offerIn, int delta)
        {
            bool active = offerIn.Active;
            bool active2 = offerOut.Active;
            if (active && offerIn.Vehicle != 0)
            {
                DebugLog.LogToFileOnly("Error: active && offerIn.Vehicle");
            }
            else if (active2 && offerOut.Vehicle != 0)
            {
                DebugLog.LogToFileOnly("Error: active2 && offerOut.Vehicle");
            }
            else if (active && offerIn.Citizen != 0u)
            {
                DebugLog.LogToFileOnly("Error: active && offerIn.Citizen");
            }
            else if (active2 && offerOut.Citizen != 0u)
            {
                DebugLog.LogToFileOnly("Error: active2 && offerOut.Citizen");
            }
            else if (active2 && offerOut.Building != 0)
            {
                Array16<Building> buildings = Singleton<BuildingManager>.instance.m_buildings;
                ushort building = offerOut.Building;
                BuildingInfo info3 = buildings.m_buffer[(int)building].Info;
                offerIn.Amount = delta;
                if (ResourceBuildingAI.IsSpecialBuilding(building))
                {
                    StartSpecialBuildingTransfer(building, ref buildings.m_buffer[(int)building], material, offerIn);
                }
                else
                {
                    DebugLog.LogToFileOnly("Error: active2 && offerOut.Building");
                }
            }
            else if (active && offerIn.Building != 0)
            {
                DebugLog.LogToFileOnly("Error: active && offerIn.Building");
            }
        }

        public static void ForgetFailedBuilding(ushort targetBuilding, int idex)
        {
            if (RealConstruction.fixUnRouteTransfer)
            {
                if (targetBuilding != 0)
                {
                    if (MainDataStore.canNotConnectedBuildingIDCount[targetBuilding] != 0)
                    {
                        if (MainDataStore.refreshCanNotConnectedBuildingIDCount[targetBuilding] > 64)
                        {
                            //After several times we can refresh fail building list.
                            MainDataStore.canNotConnectedBuildingIDCount[targetBuilding]--;
                            MainDataStore.canNotConnectedBuildingID[targetBuilding, idex] = MainDataStore.canNotConnectedBuildingID[targetBuilding, MainDataStore.canNotConnectedBuildingIDCount[targetBuilding]];
                            MainDataStore.canNotConnectedBuildingID[targetBuilding, MainDataStore.canNotConnectedBuildingIDCount[targetBuilding]] = 0;
                            MainDataStore.refreshCanNotConnectedBuildingIDCount[targetBuilding] = 0;
                        }
                        else
                        {
                            MainDataStore.refreshCanNotConnectedBuildingIDCount[targetBuilding]++;
                        }
                    }
                }
            }
        }

        private static bool IsUnRoutedMatch(TransferOffer offerIn, TransferOffer offerOut, TransferReason material)
        {
            if (!RealConstruction.fixUnRouteTransfer)
            {
                return false;
            }

            bool active = offerIn.Active;
            bool active2 = offerOut.Active;
            VehicleManager instance1 = Singleton<VehicleManager>.instance;
            BuildingManager instance = Singleton<BuildingManager>.instance;
            if (active && offerIn.Vehicle != 0)
            {
                ushort targetBuilding = 0;
                ushort sourceBuilding = instance1.m_vehicles.m_buffer[offerIn.Vehicle].m_sourceBuilding;
                targetBuilding = offerOut.Building;

                if ((targetBuilding != 0) && (sourceBuilding != 0))
                {
                    for (int j = 0; j < MainDataStore.canNotConnectedBuildingIDCount[targetBuilding]; j++)
                    {
                        if (MainDataStore.canNotConnectedBuildingID[targetBuilding, j] == sourceBuilding)
                        {
                            ForgetFailedBuilding(targetBuilding, j);
                            return true;
                        }
                    }
                }
                return false;
                //info.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[(int)vehicle], material, offerOut);
            }
            else if (active2 && offerOut.Vehicle != 0)
            {
                ushort targetBuilding = 0;
                ushort sourceBuilding = instance1.m_vehicles.m_buffer[offerOut.Vehicle].m_sourceBuilding;
                targetBuilding = offerIn.Building;

                if ((targetBuilding != 0) && (sourceBuilding != 0))
                {
                    for (int j = 0; j < MainDataStore.canNotConnectedBuildingIDCount[targetBuilding]; j++)
                    {
                        if (MainDataStore.canNotConnectedBuildingID[targetBuilding, j] == sourceBuilding)
                        {
                            ForgetFailedBuilding(targetBuilding, j);
                            return true;
                        }
                    }
                }
                return false;
                //info2.m_vehicleAI.StartTransfer(vehicle2, ref vehicles2.m_buffer[(int)vehicle2], material, offerIn);
            }
            else if (active && offerIn.Citizen != 0u)
            {
                DebugLog.LogToFileOnly("Error: No such case active && offerIn.Citizen != 0u");
                return false;
            }
            else if (active2 && offerOut.Citizen != 0u)
            {
                DebugLog.LogToFileOnly("Error: No such case active && offerOut.Citizen != 0u");
                return false;
            }
            else if (active2 && offerOut.Building != 0)
            {
                ushort targetBuilding = 0;
                ushort sourceBuilding = offerOut.Building;
                targetBuilding = offerIn.Building;

                if ((targetBuilding != 0) && (sourceBuilding != 0))
                {
                    for (int j = 0; j < MainDataStore.canNotConnectedBuildingIDCount[targetBuilding]; j++)
                    {
                        if (MainDataStore.canNotConnectedBuildingID[targetBuilding, j] == sourceBuilding)
                        {
                            ForgetFailedBuilding(targetBuilding, j);
                            return true;
                        }
                    }
                }
                return false;
                //info3.m_buildingAI.StartTransfer(building, ref buildings.m_buffer[(int)building], material, offerIn);
            }
            else if (active && offerIn.Building != 0)
            {
                ushort targetBuilding = 0;
                ushort sourceBuilding = offerIn.Building;
                targetBuilding = offerOut.Building;

                if ((targetBuilding != 0) && (sourceBuilding != 0))
                {
                    for (int j = 0; j < MainDataStore.canNotConnectedBuildingIDCount[targetBuilding]; j++)
                    {
                        if (MainDataStore.canNotConnectedBuildingID[targetBuilding, j] == sourceBuilding)
                        {
                            ForgetFailedBuilding(targetBuilding, j);
                            return true;
                        }
                    }
                }
                return false;
                //info4.m_buildingAI.StartTransfer(building2, ref buildings2.m_buffer[(int)building2], material, offerOut);
            }
            return false;
        }

        private static void Init()
        {
            var inst = Singleton<TransferManager>.instance;
            var incomingCount = typeof(TransferManager).GetField("m_incomingCount", BindingFlags.NonPublic | BindingFlags.Instance);
            var incomingOffers = typeof(TransferManager).GetField("m_incomingOffers", BindingFlags.NonPublic | BindingFlags.Instance);
            var incomingAmount = typeof(TransferManager).GetField("m_incomingAmount", BindingFlags.NonPublic | BindingFlags.Instance);
            var outgoingCount = typeof(TransferManager).GetField("m_outgoingCount", BindingFlags.NonPublic | BindingFlags.Instance);
            var outgoingOffers = typeof(TransferManager).GetField("m_outgoingOffers", BindingFlags.NonPublic | BindingFlags.Instance);
            var outgoingAmount = typeof(TransferManager).GetField("m_outgoingAmount", BindingFlags.NonPublic | BindingFlags.Instance);
            if (inst == null)
            {
                CODebugBase<LogChannel>.Error(LogChannel.Core, "No instance of TransferManager found!");
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, "No instance of TransferManager found!");
                return;
            }
            m_incomingCount = incomingCount.GetValue(inst) as ushort[];
            m_incomingOffers = incomingOffers.GetValue(inst) as TransferManager.TransferOffer[];
            m_incomingAmount = incomingAmount.GetValue(inst) as int[];
            m_outgoingCount = outgoingCount.GetValue(inst) as ushort[];
            m_outgoingOffers = outgoingOffers.GetValue(inst) as TransferManager.TransferOffer[];
            m_outgoingAmount = outgoingAmount.GetValue(inst) as int[];
        }

        private static TransferManager.TransferOffer[] m_outgoingOffers;

        private static TransferManager.TransferOffer[] m_incomingOffers;

        private static ushort[] m_outgoingCount;

        private static ushort[] m_incomingCount;

        private static int[] m_outgoingAmount;

        private static int[] m_incomingAmount;


        public static void CustomSimulationStepImpl()
        {
            int frameIndex = (int)(Singleton<SimulationManager>.instance.m_currentFrameIndex & 255u);
            if (frameIndex == 205)
            {
                //construction resource matchoffer
                MatchOffers((TransferReason)110);
            } 
            else if (frameIndex == 213)
            {
                //operation resource matchoffer
                MatchOffers((TransferReason)111);
            }
        }

        private static bool CanUseNewMatchOffers(ushort buildingID, TransferReason material)
        {
            //For RealConstruction Mod, always use new matchoffers
            return true;
        }

        private static byte MatchOffersMode(TransferReason material)
        {
            //For RealConstruction Mod, always use incoming first mode
            //incoming first mode 0
            //outgoing first mode 1
            //balanced mode 2
            return 0;
        }

        private static void MatchOffers(TransferReason material)
        {
            if (!_init)
            {
                Init();
                _init = true;
            }

            if (material != TransferReason.None)
            {
                float distanceMultiplier = 1E-07f;
                float maxDistance = (distanceMultiplier == 0f) ? 0f : (0.01f / distanceMultiplier);
                for (int priority = 7; priority >= 0; priority--)
                {
                    int offerIdex = (int)material * 8 + priority;
                    int incomingCount = m_incomingCount[offerIdex];
                    int outgoingCount = m_outgoingCount[offerIdex];
                    int incomingIdex = 0;
                    int outgoingIdex = 0;
                    int oldPriority = priority;
                    // NON-STOCK CODE START
                    byte matchOffersMode = MatchOffersMode(material);
                    bool isLoopValid = false;
                    if (matchOffersMode == 2)
                    {
                        isLoopValid = (incomingIdex < incomingCount || outgoingIdex < outgoingCount);
                    }
                    else if (matchOffersMode == 1)
                    {
                        isLoopValid = (outgoingIdex < outgoingCount);
                    }
                    else if (matchOffersMode == 0)
                    {
                        isLoopValid = (incomingIdex < incomingCount);
                    }

                    // NON-STOCK CODE END
                    while (isLoopValid)
                    {
                        //use incomingOffer to match outgoingOffer
                        if (incomingIdex < incomingCount && (matchOffersMode != 1))
                        {
                            TransferOffer incomingOffer = m_incomingOffers[offerIdex * 256 + incomingIdex];
                            // NON-STOCK CODE START
                            Vector3 incomingPositionNew = Vector3.zero;
                            bool canUseNewMatchOffers = CanUseNewMatchOffers(incomingOffer.Building, material);
                            if (canUseNewMatchOffers)
                            {
                                if (Singleton<BuildingManager>.instance.m_buildings.m_buffer[incomingOffer.Building].m_flags.IsFlagSet(Building.Flags.Untouchable))
                                {
                                    incomingPositionNew = Singleton<BuildingManager>.instance.m_buildings.m_buffer[incomingOffer.Building].m_position;
                                }
                                else
                                {
                                    incomingPositionNew = incomingOffer.Position;
                                }
                            }
                            // NON-STOCK CODE END
                            Vector3 incomingPosition = incomingOffer.Position;
                            int incomingOfferAmount = incomingOffer.Amount;
                            do
                            {
                                int incomingPriority = Mathf.Max(0, 2 - priority);
                                // NON-STOCK CODE START
                                float currentShortestDistance = -1f;
                                if (canUseNewMatchOffers)
                                {
                                    priority = 7;
                                    incomingPriority = 0;
                                }
                                else
                                {
                                    priority = oldPriority;
                                    incomingPriority = Mathf.Max(0, 2 - priority);
                                }
                                // NON-STOCK CODE END
                                int incomingPriorityExclude = (!incomingOffer.Exclude) ? incomingPriority : Mathf.Max(0, 3 - priority);
                                int validPriority = -1;
                                int validOutgoingIdex = -1;
                                float distanceOffsetPre = -1f;
                                int outgoingIdexInsideIncoming = outgoingIdex;
                                for (int incomingPriorityInside = priority; incomingPriorityInside >= incomingPriority; incomingPriorityInside--)
                                {
                                    int outgoingIdexWithPriority = (int)material * 8 + incomingPriorityInside;
                                    int outgoingCountWithPriority = m_outgoingCount[outgoingIdexWithPriority];
                                    //To let incomingPriorityInsideFloat!=0
                                    float incomingPriorityInsideFloat = (float)incomingPriorityInside + 0.1f;
                                    //Higher priority will get more chance to match
                                    //UseNewMatchOffers to find the shortest transfer building
                                    if ((distanceOffsetPre >= incomingPriorityInsideFloat) && !canUseNewMatchOffers)
                                    {
                                        break;
                                    }
                                    //Find the nearest offer to match in every priority.
                                    for (int i = outgoingIdexInsideIncoming; i < outgoingCountWithPriority; i++)
                                    {
                                        TransferOffer outgoingOfferPre = m_outgoingOffers[outgoingIdexWithPriority * 256 + i];
                                        if (incomingOffer.m_object != outgoingOfferPre.m_object && (!outgoingOfferPre.Exclude || incomingPriorityInside >= incomingPriorityExclude))
                                        {
                                            float incomingOutgoingDistance = Vector3.SqrMagnitude(outgoingOfferPre.Position - incomingPosition);
                                            // NON-STOCK CODE START
                                            Vector3 outgoingPositionNew = Vector3.zero;
                                            float incomingOutgoingDistanceNew = 0;
                                            if (canUseNewMatchOffers)
                                            {
                                                if (Singleton<BuildingManager>.instance.m_buildings.m_buffer[outgoingOfferPre.Building].m_flags.IsFlagSet(Building.Flags.Untouchable))
                                                {
                                                    outgoingPositionNew = Singleton<BuildingManager>.instance.m_buildings.m_buffer[outgoingOfferPre.Building].m_position;
                                                }
                                                else
                                                {
                                                    outgoingPositionNew = outgoingOfferPre.Position;
                                                }
                                                incomingOutgoingDistanceNew = Vector3.SqrMagnitude(outgoingPositionNew - incomingPositionNew);
                                                if ((incomingOutgoingDistanceNew < currentShortestDistance) || currentShortestDistance == -1)
                                                {
                                                    if (!IsUnRoutedMatch(incomingOffer, outgoingOfferPre, material))
                                                    {
                                                        validPriority = incomingPriorityInside;
                                                        validOutgoingIdex = i;
                                                        currentShortestDistance = incomingOutgoingDistanceNew;
                                                    }
                                                }
                                            }
                                            // NON-STOCK CODE END
                                            float distanceOffset = (!(distanceMultiplier < 0f)) ? (incomingPriorityInsideFloat / (1f + incomingOutgoingDistance * distanceMultiplier)) : (incomingPriorityInsideFloat - incomingPriorityInsideFloat / (1f - incomingOutgoingDistance * distanceMultiplier));
                                            if ((distanceOffset > distanceOffsetPre) && !canUseNewMatchOffers)
                                            {
                                                validPriority = incomingPriorityInside;
                                                validOutgoingIdex = i;
                                                distanceOffsetPre = distanceOffset;
                                                if ((incomingOutgoingDistance < maxDistance))
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    outgoingIdexInsideIncoming = 0;
                                }
                                // NON-STOCK CODE START
                                if (canUseNewMatchOffers)
                                {
                                    priority = oldPriority;
                                }
                                // NON-STOCK CODE END
                                if (validPriority == -1)
                                {
                                    break;
                                }
                                //Find a validPriority, get outgoingOffer
                                int matchedOutgoingOfferIdex = (int)material * 8 + validPriority;
                                TransferOffer outgoingOffer = m_outgoingOffers[matchedOutgoingOfferIdex * 256 + validOutgoingIdex];
                                int outgoingOfferAmount = outgoingOffer.Amount;
                                int matchedOfferAmount = Mathf.Min(incomingOfferAmount, outgoingOfferAmount);
                                if (matchedOfferAmount != 0)
                                {
                                    StartTransfer(material, outgoingOffer, incomingOffer, matchedOfferAmount);
                                }
                                incomingOfferAmount -= matchedOfferAmount;
                                outgoingOfferAmount -= matchedOfferAmount;
                                //matched outgoingOffer is empty now
                                if (outgoingOfferAmount == 0)
                                {
                                    int outgoingCountPost = m_outgoingCount[matchedOutgoingOfferIdex] - 1;
                                    m_outgoingCount[matchedOutgoingOfferIdex] = (ushort)outgoingCountPost;
                                    m_outgoingOffers[matchedOutgoingOfferIdex * 256 + validOutgoingIdex] = m_outgoingOffers[matchedOutgoingOfferIdex * 256 + outgoingCountPost];
                                    if (matchedOutgoingOfferIdex == offerIdex)
                                    {
                                        outgoingCount = outgoingCountPost;
                                    }
                                }
                                else
                                {
                                    outgoingOffer.Amount = outgoingOfferAmount;
                                    m_outgoingOffers[matchedOutgoingOfferIdex * 256 + validOutgoingIdex] = outgoingOffer;
                                }
                                incomingOffer.Amount = incomingOfferAmount;
                            }
                            while (incomingOfferAmount != 0);
                            //matched incomingOffer is empty now
                            if (incomingOfferAmount == 0)
                            {
                                incomingCount--;
                                m_incomingCount[offerIdex] = (ushort)incomingCount;
                                m_incomingOffers[offerIdex * 256 + incomingIdex] = m_incomingOffers[offerIdex * 256 + incomingCount];
                            }
                            else
                            {
                                incomingOffer.Amount = incomingOfferAmount;
                                m_incomingOffers[offerIdex * 256 + incomingIdex] = incomingOffer;
                                incomingIdex++;
                            }
                        }
                        //For RealConstruction, We only satisify incoming building
                        //use outgoingOffer to match incomingOffer
                        if (outgoingIdex < outgoingCount && (matchOffersMode != 0))
                        {
                            TransferOffer outgoingOffer = m_outgoingOffers[offerIdex * 256 + outgoingIdex];
                            // NON-STOCK CODE START
                            bool canUseNewMatchOffers = CanUseNewMatchOffers(outgoingOffer.Building, material);
                            Vector3 outgoingPositionNew = Vector3.zero;
                            if (canUseNewMatchOffers)
                            {
                                if (Singleton<BuildingManager>.instance.m_buildings.m_buffer[outgoingOffer.Building].m_flags.IsFlagSet(Building.Flags.Untouchable))
                                {
                                    outgoingPositionNew = Singleton<BuildingManager>.instance.m_buildings.m_buffer[outgoingOffer.Building].m_position;
                                }
                                else
                                {
                                    outgoingPositionNew = outgoingOffer.Position;
                                }
                            }
                            // NON-STOCK CODE END
                            Vector3 outgoingPosition = outgoingOffer.Position;
                            int outgoingOfferAmount = outgoingOffer.Amount;
                            do
                            {
                                int outgoingPriority = Mathf.Max(0, 2 - priority);
                                // NON-STOCK CODE START
                                float currentShortestDistance = -1f;
                                if (canUseNewMatchOffers)
                                {
                                    priority = 7;
                                    outgoingPriority = 0;
                                }
                                else
                                {
                                    priority = oldPriority;
                                    outgoingPriority = Mathf.Max(0, 2 - priority);
                                }
                                // NON-STOCK CODE END
                                int outgoingPriorityExclude = (!outgoingOffer.Exclude) ? outgoingPriority : Mathf.Max(0, 3 - priority);
                                int validPriority = -1;
                                int validIncomingIdex = -1;
                                float distanceOffsetPre = -1f;
                                int incomingIdexInsideOutgoing = incomingIdex;
                                for (int outgoingPriorityInside = priority; outgoingPriorityInside >= outgoingPriority; outgoingPriorityInside--)
                                {
                                    int incomingIdexWithPriority = (int)material * 8 + outgoingPriorityInside;
                                    int incomingCountWithPriority = m_incomingCount[incomingIdexWithPriority];
                                    //To let outgoingPriorityInsideFloat!=0
                                    float outgoingPriorityInsideFloat = (float)outgoingPriorityInside + 0.1f;
                                    //Higher priority will get more chance to match
                                    if ((distanceOffsetPre >= outgoingPriorityInsideFloat) && !canUseNewMatchOffers)
                                    {
                                        break;
                                    }
                                    for (int j = incomingIdexInsideOutgoing; j < incomingCountWithPriority; j++)
                                    {
                                        TransferOffer incomingOfferPre = m_incomingOffers[incomingIdexWithPriority * 256 + j];
                                        if (outgoingOffer.m_object != incomingOfferPre.m_object && (!incomingOfferPre.Exclude || outgoingPriorityInside >= outgoingPriorityExclude))
                                        {
                                            float incomingOutgoingDistance = Vector3.SqrMagnitude(incomingOfferPre.Position - outgoingPosition);
                                            // NON-STOCK CODE START
                                            Vector3 incomingPositionNew = Vector3.zero;
                                            float incomingOutgoingDistanceNew = 0;
                                            if (canUseNewMatchOffers)
                                            {
                                                if (Singleton<BuildingManager>.instance.m_buildings.m_buffer[incomingOfferPre.Building].m_flags.IsFlagSet(Building.Flags.Untouchable))
                                                {
                                                    incomingPositionNew = Singleton<BuildingManager>.instance.m_buildings.m_buffer[incomingOfferPre.Building].m_position;
                                                }
                                                else
                                                {
                                                    incomingPositionNew = incomingOfferPre.Position;
                                                }
                                                incomingOutgoingDistanceNew = Vector3.SqrMagnitude(outgoingPositionNew - incomingPositionNew);
                                                if ((incomingOutgoingDistanceNew < currentShortestDistance) || currentShortestDistance == -1)
                                                {
                                                    if (!IsUnRoutedMatch(incomingOfferPre, outgoingOffer, material))
                                                    {
                                                        validPriority = outgoingPriorityInside;
                                                        validIncomingIdex = j;
                                                        currentShortestDistance = incomingOutgoingDistanceNew;
                                                    }
                                                }
                                            }
                                            // NON-STOCK CODE END
                                            float distanceOffset = (!(distanceMultiplier < 0f)) ? (outgoingPriorityInsideFloat / (1f + incomingOutgoingDistance * distanceMultiplier)) : (outgoingPriorityInsideFloat - outgoingPriorityInsideFloat / (1f - incomingOutgoingDistance * distanceMultiplier));
                                            if ((distanceOffset > distanceOffsetPre) && !canUseNewMatchOffers)
                                            {
                                                validPriority = outgoingPriorityInside;
                                                validIncomingIdex = j;
                                                distanceOffsetPre = distanceOffset;
                                                if (incomingOutgoingDistance < maxDistance)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    incomingIdexInsideOutgoing = 0;
                                }
                                // NON-STOCK CODE START
                                if (canUseNewMatchOffers)
                                {
                                    priority = oldPriority;
                                }
                                // NON-STOCK CODE END
                                if (validPriority == -1)
                                {
                                    break;
                                }
                                //Find a validPriority, get incomingOffer
                                int matchedIncomingOfferIdex = (int)material * 8 + validPriority;
                                TransferOffer incomingOffers = m_incomingOffers[matchedIncomingOfferIdex * 256 + validIncomingIdex];
                                int incomingOffersAmount = incomingOffers.Amount;
                                int matchedOfferAmount = Mathf.Min(outgoingOfferAmount, incomingOffersAmount);
                                if (matchedOfferAmount != 0)
                                {
                                    StartTransfer(material, outgoingOffer, incomingOffers, matchedOfferAmount);
                                }
                                outgoingOfferAmount -= matchedOfferAmount;
                                incomingOffersAmount -= matchedOfferAmount;
                                //matched incomingOffer is empty now
                                if (incomingOffersAmount == 0)
                                {
                                    int incomingCountPost = m_incomingCount[matchedIncomingOfferIdex] - 1;
                                    m_incomingCount[matchedIncomingOfferIdex] = (ushort)incomingCountPost;
                                    m_incomingOffers[matchedIncomingOfferIdex * 256 + validIncomingIdex] = m_incomingOffers[matchedIncomingOfferIdex * 256 + incomingCountPost];
                                    if (matchedIncomingOfferIdex == offerIdex)
                                    {
                                        incomingCount = incomingCountPost;
                                    }
                                }
                                else
                                {
                                    incomingOffers.Amount = incomingOffersAmount;
                                    m_incomingOffers[matchedIncomingOfferIdex * 256 + validIncomingIdex] = incomingOffers;
                                }
                                outgoingOffer.Amount = outgoingOfferAmount;
                            }
                            while (outgoingOfferAmount != 0);
                            //matched outgoingOffer is empty now
                            if (outgoingOfferAmount == 0)
                            {
                                outgoingCount--;
                                m_outgoingCount[offerIdex] = (ushort)outgoingCount;
                                m_outgoingOffers[offerIdex * 256 + outgoingIdex] = m_outgoingOffers[offerIdex * 256 + outgoingCount];
                            }
                            else
                            {
                                outgoingOffer.Amount = outgoingOfferAmount;
                                m_outgoingOffers[offerIdex * 256 + outgoingIdex] = outgoingOffer;
                                outgoingIdex++;
                            }
                        }

                        // NON-STOCK CODE START
                        if (matchOffersMode == 2)
                        {
                            isLoopValid = (incomingIdex < incomingCount || outgoingIdex < outgoingCount);
                        }
                        else if (matchOffersMode == 1)
                        {
                            isLoopValid = (outgoingIdex < outgoingCount);
                        }
                        else if (matchOffersMode == 0)
                        {
                            isLoopValid = (incomingIdex < incomingCount);
                        }
                        // NON-STOCK CODE END
                    }
                }
                for (int k = 0; k < 8; k++)
                {
                    int num40 = (int)material * 8 + k;
                    m_incomingCount[num40] = 0;
                    m_outgoingCount[num40] = 0;
                }
                m_incomingAmount[(int)material] = 0;
                m_outgoingAmount[(int)material] = 0;
            }
        }
    }
}
