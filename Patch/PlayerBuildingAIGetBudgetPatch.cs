using ColossalFramework;
using HarmonyLib;
using RealConstruction.CustomAI;
using RealConstruction.Util;
using System;
using System.Reflection;

namespace RealConstruction.Patch
{
    [HarmonyPatch]
    public class PlayerBuildingAIGetBudgetPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(PlayerBuildingAI).GetMethod("GetBudget", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType() }, null);
        }
        public static bool Prefix(ushort buildingID, ref Building buildingData, ref int __result)
        {
            ushort eventIndex = buildingData.m_eventIndex;
            if (eventIndex != 0)
            {
                EventManager instance = Singleton<EventManager>.instance;
                EventInfo info = instance.m_events.m_buffer[eventIndex].Info;
                __result = info.m_eventAI.GetBudget(eventIndex, ref instance.m_events.m_buffer[eventIndex]);
            }

            if (MainDataStore.operationResourceBuffer[buildingID] < 1000 && CustomPlayerBuildingAI.CanOperation(buildingID, ref buildingData) && (RealConstruction.operationConsumption != 2))
            {
                __result = 10;
            }
            else
            {
                __result = Singleton<EconomyManager>.instance.GetBudget(buildingData.Info.m_class);
            }
            return false;
        }
    }
}
