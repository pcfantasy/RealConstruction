using ColossalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealConstruction
{
    public class CustomTransferManager: TransferManager
    {
        public void StartSpecialBuildingTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
                //DebugLog.LogToFileOnly("find valid SpecialBuilding");
                VehicleInfo vehicleInfo = null;
                if (material == (TransferManager.TransferReason)110)
                {
                    //DebugLog.LogToFileOnly("find valid construction resource transfer");
                    vehicleInfo = Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, ItemClass.Service.Industrial, ItemClass.SubService.IndustrialForestry, ItemClass.Level.Level1);
                }
                else if (material == (TransferManager.TransferReason)111)
                {
                    //DebugLog.LogToFileOnly("find valid operation resource transfer");
                    vehicleInfo = Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, ItemClass.Service.Industrial, ItemClass.SubService.IndustrialFarming, ItemClass.Level.Level1);
                }


                if (vehicleInfo != null)
                {
                    //DebugLog.LogToFileOnly("find valid vehicleInfo");
                    Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                    ushort num16;
                    if (Singleton<VehicleManager>.instance.CreateVehicle(out num16, ref Singleton<SimulationManager>.instance.m_randomizer, vehicleInfo, data.m_position, material, false, true))
                    {
                        //DebugLog.LogToFileOnly("find valid CreateVehicle");
                        vehicleInfo.m_vehicleAI.SetSource(num16, ref vehicles.m_buffer[(int)num16], buildingID);
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

        private void StartTransfer(TransferManager.TransferReason material, TransferManager.TransferOffer offerOut, TransferManager.TransferOffer offerIn, int delta)
        {
            bool active = offerIn.Active;
            bool active2 = offerOut.Active;
            if (active && offerIn.Vehicle != 0)
            {
                Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                ushort vehicle = offerIn.Vehicle;
                VehicleInfo info = vehicles.m_buffer[(int)vehicle].Info;
                offerOut.Amount = delta;
                info.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[(int)vehicle], material, offerOut);
            }
            else if (active2 && offerOut.Vehicle != 0)
            {
                Array16<Vehicle> vehicles2 = Singleton<VehicleManager>.instance.m_vehicles;
                ushort vehicle2 = offerOut.Vehicle;
                VehicleInfo info2 = vehicles2.m_buffer[(int)vehicle2].Info;
                offerIn.Amount = delta;
                info2.m_vehicleAI.StartTransfer(vehicle2, ref vehicles2.m_buffer[(int)vehicle2], material, offerIn);
            }
            else if (active && offerIn.Citizen != 0u)
            {
                Array32<Citizen> citizens = Singleton<CitizenManager>.instance.m_citizens;
                uint citizen = offerIn.Citizen;
                CitizenInfo citizenInfo = citizens.m_buffer[(int)((UIntPtr)citizen)].GetCitizenInfo(citizen);
                if (citizenInfo != null)
                {
                    offerOut.Amount = delta;
                    citizenInfo.m_citizenAI.StartTransfer(citizen, ref citizens.m_buffer[(int)((UIntPtr)citizen)], material, offerOut);
                }
            }
            else if (active2 && offerOut.Citizen != 0u)
            {
                Array32<Citizen> citizens2 = Singleton<CitizenManager>.instance.m_citizens;
                uint citizen2 = offerOut.Citizen;
                CitizenInfo citizenInfo2 = citizens2.m_buffer[(int)((UIntPtr)citizen2)].GetCitizenInfo(citizen2);
                if (citizenInfo2 != null)
                {
                    offerIn.Amount = delta;
                    citizenInfo2.m_citizenAI.StartTransfer(citizen2, ref citizens2.m_buffer[(int)((UIntPtr)citizen2)], material, offerIn);
                }
            }
            else if (active2 && offerOut.Building != 0)
            {
                Array16<Building> buildings = Singleton<BuildingManager>.instance.m_buildings;
                ushort building = offerOut.Building;
                BuildingInfo info3 = buildings.m_buffer[(int)building].Info;
                offerIn.Amount = delta;
                //DebugLog.LogToFileOnly("find valid StartTransfer");
                if (RealConstructionThreading.IsSpecialBuilding(building))
                {
                    StartSpecialBuildingTransfer(building, ref buildings.m_buffer[(int)building], material, offerIn);
                }
                else
                {
                    info3.m_buildingAI.StartTransfer(building, ref buildings.m_buffer[(int)building], material, offerIn);
                }
            }
            else if (active && offerIn.Building != 0)
            {
                Array16<Building> buildings2 = Singleton<BuildingManager>.instance.m_buildings;
                ushort building2 = offerIn.Building;
                BuildingInfo info4 = buildings2.m_buffer[(int)building2].Info;
                offerOut.Amount = delta;
                info4.m_buildingAI.StartTransfer(building2, ref buildings2.m_buffer[(int)building2], material, offerOut);
            }
        }


        // global::TransferManager
        private static TransferReason GetFrameReason(int frameIndex)
        {
            switch (frameIndex)
            {
                case 1:
                    return TransferReason.Snow;
                case 2:
                    //constuction
                    return (TransferReason)110;
                case 3:
                    return TransferReason.Garbage;
                case 4:
                    //operation
                    return (TransferReason)111;
                case 5:
                    return TransferReason.Metals;
                case 7:
                    return TransferReason.Worker0;
                case 9:
                    return TransferReason.TouristA;
                case 11:
                    return TransferReason.Fire;
                case 15:
                    return TransferReason.Goods;
                case 17:
                    return TransferReason.Sick2;
                case 19:
                    return TransferReason.Shopping;
                case 23:
                    return TransferReason.DummyCar;
                case 25:
                    return TransferReason.IncomingMail;
                case 27:
                    return TransferReason.Single0;
                case 29:
                    return TransferReason.EvacuateA;
                case 31:
                    return TransferReason.LeaveCity0;
                case 33:
                    return TransferReason.ForestFire;
                case 35:
                    return TransferReason.Entertainment;
                case 39:
                    return TransferReason.PartnerYoung;
                case 41:
                    return TransferReason.UnsortedMail;
                case 43:
                    return TransferReason.Grain;
                case 47:
                    return TransferReason.Family0;
                case 49:
                    return TransferReason.FloodWater;
                case 51:
                    return TransferReason.ShoppingB;
                case 55:
                    return TransferReason.PassengerShip;
                case 57:
                    return TransferReason.AnimalProducts;
                case 59:
                    return TransferReason.Single1;
                case 61:
                    return TransferReason.EvacuateVipA;
                case 63:
                    return TransferReason.Oil;
                case 65:
                    return TransferReason.RoadMaintenance;
                case 67:
                    return TransferReason.Crime;
                case 69:
                    return TransferReason.LuxuryProducts;
                case 71:
                    return TransferReason.Worker1;
                case 73:
                    return TransferReason.TouristB;
                case 75:
                    return TransferReason.Bus;
                case 79:
                    return TransferReason.Student1;
                case 81:
                    return TransferReason.Ferry;
                case 83:
                    return TransferReason.ShoppingC;
                case 87:
                    return TransferReason.DummyTrain;
                case 89:
                    return TransferReason.Flours;
                case 91:
                    return TransferReason.Single2;
                case 93:
                    return TransferReason.EvacuateB;
                case 95:
                    return TransferReason.LeaveCity1;
                case 97:
                    return TransferReason.Collapsed;
                case 99:
                    return TransferReason.EntertainmentB;
                case 103:
                    return TransferReason.GarbageMove;
                case 105:
                    return TransferReason.Mail;
                case 107:
                    return TransferReason.Lumber;
                case 111:
                    return TransferReason.Family1;
                case 113:
                    return TransferReason.CableCar;
                case 115:
                    return TransferReason.ShoppingD;
                case 119:
                    return TransferReason.CriminalMove;
                case 121:
                    return TransferReason.Paper;
                case 123:
                    return TransferReason.Single3;
                case 125:
                    return TransferReason.EvacuateVipB;
                case 127:
                    return TransferReason.Coal;
                case 129:
                    return TransferReason.SnowMove;
                case 131:
                    return TransferReason.Sick;
                case 135:
                    return TransferReason.Worker2;
                case 137:
                    return TransferReason.TouristC;
                case 139:
                    return TransferReason.PassengerTrain;
                case 143:
                    return TransferReason.Student2;
                case 145:
                    return TransferReason.Blimp;
                case 147:
                    return TransferReason.ShoppingE;
                case 151:
                    return TransferReason.DummyShip;
                case 153:
                    return TransferReason.PlanedTimber;
                case 155:
                    return TransferReason.Single0B;
                case 157:
                    return TransferReason.EvacuateC;
                case 159:
                    return TransferReason.LeaveCity2;
                case 161:
                    return TransferReason.Collapsed2;
                case 163:
                    return TransferReason.EntertainmentC;
                case 167:
                    return TransferReason.PartnerAdult;
                case 169:
                    return TransferReason.SortedMail;
                case 171:
                    return TransferReason.Food;
                case 175:
                    return TransferReason.Family2;
                case 177:
                    return TransferReason.Monorail;
                case 179:
                    return TransferReason.ShoppingF;
                case 183:
                    return TransferReason.Taxi;
                case 185:
                    return TransferReason.Petroleum;
                case 187:
                    return TransferReason.Single1B;
                case 189:
                    return TransferReason.EvacuateVipC;
                case 191:
                    return TransferReason.Petrol;
                case 193:
                    return TransferReason.SickMove;
                case 195:
                    return TransferReason.Dead;
                case 199:
                    return TransferReason.Worker3;
                case 201:
                    return TransferReason.TouristD;
                case 203:
                    return TransferReason.MetroTrain;
                case 207:
                    return TransferReason.Student3;
                case 209:
                    return TransferReason.TouristBus;
                case 211:
                    return TransferReason.ShoppingG;
                case 215:
                    return TransferReason.DummyPlane;
                case 217:
                    return TransferReason.Plastics;
                case 219:
                    return TransferReason.Single2B;
                case 221:
                    return TransferReason.EvacuateD;
                case 223:
                    return TransferReason.PassengerPlane;
                case 225:
                    return TransferReason.Fire2;
                case 227:
                    return TransferReason.EntertainmentD;
                case 231:
                    return TransferReason.DeadMove;
                case 233:
                    return TransferReason.OutgoingMail;
                case 235:
                    return TransferReason.Logs;
                case 239:
                    return TransferReason.Family3;
                case 241:
                    return TransferReason.ParkMaintenance;
                case 243:
                    return TransferReason.ShoppingH;
                case 247:
                    return TransferReason.Tram;
                case 249:
                    return TransferReason.Glass;
                case 251:
                    return TransferReason.Single3B;
                case 253:
                    return TransferReason.EvacuateVipD;
                case 255:
                    return TransferReason.Ore;
                default:
                    return TransferReason.None;
            }
        }


    }
}
