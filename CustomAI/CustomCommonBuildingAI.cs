using ColossalFramework;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealConstruction.CustomAI
{
    public class CustomCommonBuildingAI
    {
        public static void CommonBuildingAIReleaseBuildingPostfix(ushort buildingID, ref Building data)
        {
            MainDataStore.foodBuffer[buildingID] = 0;
            MainDataStore.lumberBuffer[buildingID] = 0;
            MainDataStore.petrolBuffer[buildingID] = 0;
            MainDataStore.coalBuffer[buildingID] = 0;
            MainDataStore.constructionResourceBuffer[buildingID] = 0;
            MainDataStore.operationResourceBuffer[buildingID] = 0;
            TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
            offer.Building = buildingID;
            Singleton<TransferManager>.instance.RemoveOutgoingOffer((TransferManager.TransferReason)110, offer);
            Singleton<TransferManager>.instance.RemoveOutgoingOffer((TransferManager.TransferReason)111, offer);
            Singleton<TransferManager>.instance.RemoveOutgoingOffer((TransferManager.TransferReason)112, offer);
            DebugLog.LogToFileOnly("HarmonyDetours CommonBuildingAIReleaseBuildingPostfix");
        }
    }
}
