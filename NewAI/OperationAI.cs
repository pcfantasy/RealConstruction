using ColossalFramework;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RealConstruction.NewAI
{
    public class OperationAI
    {
        public static void ProcessPlayerBuildingOperation(ushort buildingID, ref Building buildingData)
        {
            if (buildingData.m_fireIntensity == 0 && buildingData.m_flags.IsFlagSet(Building.Flags.Completed))
            {
                int num27 = 0;
                int num28 = 0;
                int num29 = 0;
                int value = 0;
                int num34 = 0;
                TransferManager.TransferReason incomingTransferReason = default(TransferManager.TransferReason);
                //operation resource
                incomingTransferReason = (TransferManager.TransferReason)125;
                num27 = 0;
                num28 = 0;
                num29 = 0;
                value = 0;
                num34 = 0;
                if (incomingTransferReason != TransferManager.TransferReason.None)
                {
                    CaculationVehicle.CustomCalculateGuestVehicles(buildingID, ref buildingData, incomingTransferReason, ref num27, ref num28, ref num29, ref value);
                    buildingData.m_tempImport = (byte)Mathf.Clamp(value, (int)buildingData.m_tempImport, 255);
                }

                num34 = 15000 - MainDataStore.operationResourceBuffer[buildingID] - num29;
                if (num34 > 0)
                {
                    TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                    //Higher priority for Electricity and Water sevice.
                    if (buildingData.Info.m_class.m_service == ItemClass.Service.Water || buildingData.Info.m_class.m_service == ItemClass.Service.Electricity || buildingData.Info.m_class.m_service == ItemClass.Service.Garbage)
                    {
                        offer.Priority = num34 / 1000;
                    }
                    else
                    {
                        offer.Priority = num34 / 3000;
                    }
                    if (offer.Priority > 7)
                    {
                        offer.Priority = 7;
                    }
                    offer.Building = buildingID;
                    offer.Position = buildingData.m_position;
                    offer.Amount = 1;
                    offer.Active = false;
                    Singleton<TransferManager>.instance.AddIncomingOffer(incomingTransferReason, offer);
                }
            }
        }
    }
}
