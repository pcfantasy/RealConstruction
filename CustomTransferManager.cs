using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RealConstruction
{
    public class CustomTransferManager: TransferManager
    {
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
                        CustomCargoTruckAI.CargoTruckAISetSourceForRealConstruction(num16, ref vehicles.m_buffer[(int)num16], buildingID);
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
                if (RealConstructionThreading.IsSpecialBuilding(building))
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


        public static void Init()
        {
            DebugLog.LogToFileOnly("Init fake transfer manager");
            try
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
                    DebugLog.LogToFileOnly("No instance of TransferManager found!");
                    return;
                }
                _incomingCount = incomingCount.GetValue(inst) as ushort[];
                _incomingOffers = incomingOffers.GetValue(inst) as TransferManager.TransferOffer[];
                _incomingAmount = incomingAmount.GetValue(inst) as int[];
                _outgoingCount = outgoingCount.GetValue(inst) as ushort[];
                _outgoingOffers = outgoingOffers.GetValue(inst) as TransferManager.TransferOffer[];
                _outgoingAmount = outgoingAmount.GetValue(inst) as int[];
                if (_incomingCount == null || _incomingOffers == null || _incomingAmount == null || _outgoingCount == null || _outgoingOffers == null || _outgoingAmount == null)
                {
                    DebugLog.LogToFileOnly("Arrays are null");
                }
            }
            catch (Exception ex)
            {
                DebugLog.LogToFileOnly("Exception: " + ex.Message);
            }
        }

        private static TransferManager.TransferOffer[] _incomingOffers;
        private static ushort[] _incomingCount;
        private static int[] _incomingAmount;
        private static TransferManager.TransferOffer[] _outgoingOffers;
        private static ushort[] _outgoingCount;
        private static int[] _outgoingAmount;
        private static bool _init;


        public static void CustomSimulationStepImpl()
        {
            if (!_init)
            {
                _init = true;
                Init();
            }

            int frameIndex = (int)(Singleton<SimulationManager>.instance.m_currentFrameIndex & 255u);
            if (frameIndex == 2)
            {
                //construction resource matchoffer
                MatchOffers((TransferReason)110);
            } 
            else if (frameIndex == 4)
            {
                //operation resource matchoffer
                MatchOffers((TransferReason)111);
            }
        }

        // TransferManager
        public static void MatchOffers(TransferManager.TransferReason material)
        {
            if (material == TransferManager.TransferReason.None)
            {
                return;
            }
            float distanceMultiplier = 1E-07f;
            float num;
            if (distanceMultiplier != 0f)
            {
                num = 0.01f / distanceMultiplier;
            }
            else
            {
                num = 0f;
            }
            for (int i = 7; i >= 0; i--)
            {
                int num2 = (int)((int)material * 8 + i);
                int num3 = (int)_incomingCount[num2];
                int num4 = (int)_outgoingCount[num2];
                int num5 = 0;
                int num6 = 0;
                while (num5 < num3 || num6 < num4)
                {
                    if (num5 < num3)
                    {
                        TransferManager.TransferOffer transferOffer = _incomingOffers[num2 * 256 + num5];
                        Vector3 position = transferOffer.Position;
                        int num7 = transferOffer.Amount;
                        do
                        {
                            int num8 = Mathf.Max(0, 2 - i);
                            int num9 = (!transferOffer.Exclude) ? num8 : Mathf.Max(0, 3 - i);
                            int num10 = -1;
                            int num11 = -1;
                            float num12 = -1f;
                            int num13 = num6;
                            for (int j = i; j >= num8; j--)
                            {
                                int num14 = (int)((int)material * 8 + j);
                                int num15 = (int)_outgoingCount[num14];
                                float num16 = (float)j + 0.1f;
                                if (num12 >= num16)
                                {
                                    break;
                                }
                                for (int k = num13; k < num15; k++)
                                {
                                    TransferManager.TransferOffer transferOffer2 = _outgoingOffers[num14 * 256 + k];
                                    if (transferOffer.m_object != transferOffer2.m_object && (!transferOffer2.Exclude || j >= num9))
                                    {
                                        float num17 = Vector3.SqrMagnitude(transferOffer2.Position - position);
                                        float num18;
                                        if (distanceMultiplier < 0f)
                                        {
                                            num18 = num16 - num16 / (1f - num17 * distanceMultiplier);
                                        }
                                        else
                                        {
                                            num18 = num16 / (1f + num17 * distanceMultiplier);
                                        }
                                        if (num18 > num12)
                                        {
                                            num10 = j;
                                            num11 = k;
                                            num12 = num18;
                                            if (num17 < num)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                num13 = 0;
                            }
                            if (num10 == -1)
                            {
                                break;
                            }
                            int num19 = (int)((int)material * 8 + num10);
                            TransferManager.TransferOffer transferOffer3 = _outgoingOffers[num19 * 256 + num11];
                            int num20 = transferOffer3.Amount;
                            int num21 = Mathf.Min(num7, num20);
                            if (num21 != 0)
                            {
                                StartTransfer(material, transferOffer3, transferOffer, num21);
                            }
                            num7 -= num21;
                            num20 -= num21;
                            if (num20 == 0)
                            {
                                int num22 = (int)(_outgoingCount[num19] - 1);
                                _outgoingCount[num19] = (ushort)num22;
                                _outgoingOffers[num19 * 256 + num11] = _outgoingOffers[num19 * 256 + num22];
                                if (num19 == num2)
                                {
                                    num4 = num22;
                                }
                            }
                            else
                            {
                                transferOffer3.Amount = num20;
                                _outgoingOffers[num19 * 256 + num11] = transferOffer3;
                            }
                            transferOffer.Amount = num7;
                        }
                        while (num7 != 0);
                        IL_2E8:
                        if (num7 == 0)
                        {
                            num3--;
                            _incomingCount[num2] = (ushort)num3;
                            _incomingOffers[num2 * 256 + num5] = _incomingOffers[num2 * 256 + num3];
                            goto IL_364;
                        }
                        transferOffer.Amount = num7;
                        _incomingOffers[num2 * 256 + num5] = transferOffer;
                        num5++;
                        goto IL_364;
                        goto IL_2E8;
                    }
                    IL_364:
                    if (num6 < num4)
                    {
                        TransferManager.TransferOffer transferOffer4 = _outgoingOffers[num2 * 256 + num6];
                        Vector3 position2 = transferOffer4.Position;
                        int num23 = transferOffer4.Amount;
                        do
                        {
                            int num24 = Mathf.Max(0, 2 - i);
                            int num25 = (!transferOffer4.Exclude) ? num24 : Mathf.Max(0, 3 - i);
                            int num26 = -1;
                            int num27 = -1;
                            float num28 = -1f;
                            int num29 = num5;
                            for (int l = i; l >= num24; l--)
                            {
                                int num30 = (int)((int)material * 8 + l);
                                int num31 = (int)_incomingCount[num30];
                                float num32 = (float)l + 0.1f;
                                if (num28 >= num32)
                                {
                                    break;
                                }
                                for (int m = num29; m < num31; m++)
                                {
                                    TransferManager.TransferOffer transferOffer5 = _incomingOffers[num30 * 256 + m];
                                    if (transferOffer4.m_object != transferOffer5.m_object && (!transferOffer5.Exclude || l >= num25))
                                    {
                                        float num33 = Vector3.SqrMagnitude(transferOffer5.Position - position2);
                                        float num34;
                                        if (distanceMultiplier < 0f)
                                        {
                                            num34 = num32 - num32 / (1f - num33 * distanceMultiplier);
                                        }
                                        else
                                        {
                                            num34 = num32 / (1f + num33 * distanceMultiplier);
                                        }
                                        if (num34 > num28)
                                        {
                                            num26 = l;
                                            num27 = m;
                                            num28 = num34;
                                            if (num33 < num)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                num29 = 0;
                            }
                            if (num26 == -1)
                            {
                                break;
                            }
                            int num35 = (int)((int)material * 8 + num26);
                            TransferManager.TransferOffer transferOffer6 = _incomingOffers[num35 * 256 + num27];
                            int num36 = transferOffer6.Amount;
                            int num37 = Mathf.Min(num23, num36);
                            if (num37 != 0)
                            {
                                StartTransfer(material, transferOffer4, transferOffer6, num37);
                            }
                            num23 -= num37;
                            num36 -= num37;
                            if (num36 == 0)
                            {
                                int num38 = (int)(_incomingCount[num35] - 1);
                                _incomingCount[num35] = (ushort)num38;
                                _incomingOffers[num35 * 256 + num27] = _incomingOffers[num35 * 256 + num38];
                                if (num35 == num2)
                                {
                                    num3 = num38;
                                }
                            }
                            else
                            {
                                transferOffer6.Amount = num36;
                                _incomingOffers[num35 * 256 + num27] = transferOffer6;
                            }
                            transferOffer4.Amount = num23;
                        }
                        while (num23 != 0);
                        IL_5EF:
                        if (num23 == 0)
                        {
                            num4--;
                            _outgoingCount[num2] = (ushort)num4;
                            _outgoingOffers[num2 * 256 + num6] = _outgoingOffers[num2 * 256 + num4];
                            continue;
                        }
                        transferOffer4.Amount = num23;
                        _outgoingOffers[num2 * 256 + num6] = transferOffer4;
                        num6++;
                        continue;
                        goto IL_5EF;
                    }
                }
            }
            for (int n = 0; n < 8; n++)
            {
                int num39 = (int)((int)material * 8 + n);
                _incomingCount[num39] = 0;
                _outgoingCount[num39] = 0;
            }
            _incomingAmount[(int)material] = 0;
            _outgoingAmount[(int)material] = 0;
        }


    }
}
