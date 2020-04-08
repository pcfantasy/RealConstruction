using ColossalFramework;
using HarmonyLib;
using System.Reflection;

namespace RealConstruction.Patch
{
    [HarmonyPatch]
    public class PlayerBuildingAIGetConstructionTimePatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(PlayerBuildingAI).GetMethod("GetConstructionTime", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        public static bool Prefix(ref int __result)
        {
            if ((Singleton<ToolManager>.instance.m_properties.m_mode & ItemClass.Availability.AssetEditor) != ItemClass.Availability.None)
            {
                __result = 0;
            }
            __result = 100;
            return false;
        }
    }
}
