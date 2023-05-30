using ColossalFramework;
using HarmonyLib;
using RealConstruction.CustomManager;
using RealConstruction.Util;
using System.Reflection;
using static ItemClass;

namespace RealConstruction.Patch
{
    [HarmonyPatch]
    public class MatchOffersPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(TransferManager).GetMethod("MatchOffers", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        // Specify the order of execution, since this mod will not work if TransferManagerCE is executed first.
        [HarmonyBefore(new string[] { "Sleepy.TransferManagerCE" })]
        public static void Prefix(TransferManager.TransferReason material)
        {
            if (material == (TransferManager.TransferReason)124 ||
                material == (TransferManager.TransferReason)125)
            {
                MethodInfo func = typeof(CustomTransferManager).GetMethod("MatchOffers", BindingFlags.NonPublic | BindingFlags.Static);
                func.Invoke(null, new object[] { material });
            }
        }
    }
}
