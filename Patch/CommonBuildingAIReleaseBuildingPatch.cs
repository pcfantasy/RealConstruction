using ColossalFramework;
using Harmony;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RealConstruction.Patch
{
    [HarmonyPatch]
    public class CommonBuildingAIReleaseBuildingPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(CommonBuildingAI).GetMethod("ReleaseBuilding");
        }
        public static void Postfix(ushort buildingID, ref Building data)
        {
            MainDataStore.foodBuffer[buildingID] = 0;
            MainDataStore.lumberBuffer[buildingID] = 0;
            MainDataStore.petrolBuffer[buildingID] = 0;
            MainDataStore.coalBuffer[buildingID] = 0;
            MainDataStore.constructionResourceBuffer[buildingID] = 0;
            MainDataStore.operationResourceBuffer[buildingID] = 0;
            TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
            offer.Building = buildingID;
            Singleton<TransferManager>.instance.RemoveOutgoingOffer((TransferManager.TransferReason)124, offer);
            Singleton<TransferManager>.instance.RemoveOutgoingOffer((TransferManager.TransferReason)125, offer);
        }
    }
}
