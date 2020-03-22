using ColossalFramework;
using Harmony;
using System.Reflection;

namespace RealConstruction.Patch
{
    public class DistrictParkGetAcademicYearProgressPatch
    {
        [HarmonyPatch]
        public static MethodBase TargetMethod()
        {
            return typeof(DistrictPark).GetMethod("GetAcademicYearProgress", BindingFlags.Public | BindingFlags.Instance);
        }
        public static bool Prefix(ref DistrictPark districtParkData, ref float __result)
        {
            BuildingManager instance = Singleton<BuildingManager>.instance;
            if (instance.m_buildings.m_buffer[districtParkData.m_mainGate].m_flags.IsFlagSet(Building.Flags.Completed))
            {
                ushort eventIndex = instance.m_buildings.m_buffer[districtParkData.m_mainGate].m_eventIndex;
                AcademicYearAI academicYearAI = (AcademicYearAI)Singleton<EventManager>.instance.m_events.m_buffer[eventIndex].Info.m_eventAI;
                __result = academicYearAI.GetYearProgress(eventIndex, ref Singleton<EventManager>.instance.m_events.m_buffer[eventIndex]);
            }
            else
            {
                __result = 0;
            }
            return false;
        }
    }
}
