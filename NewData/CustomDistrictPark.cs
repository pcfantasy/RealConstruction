using ColossalFramework;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealConstruction.NewData
{
    public static class CustomDistrictPark
    {
        public static float GetAcademicYearProgress(ref DistrictPark districtParkData)
        {
            BuildingManager instance = Singleton<BuildingManager>.instance;
            //DebugLog.LogToFileOnly("GetAcademicYearProgress");
            if (instance.m_buildings.m_buffer[districtParkData.m_mainGate].m_flags.IsFlagSet(Building.Flags.Completed))
            {
                //DebugLog.LogToFileOnly("GetAcademicYearProgress and building is completed");
                ushort eventIndex = instance.m_buildings.m_buffer[districtParkData.m_mainGate].m_eventIndex;
                AcademicYearAI academicYearAI = (AcademicYearAI)Singleton<EventManager>.instance.m_events.m_buffer[eventIndex].Info.m_eventAI;
                return academicYearAI.GetYearProgress(eventIndex, ref Singleton<EventManager>.instance.m_events.m_buffer[eventIndex]);
            }
            else
            {
                //DebugLog.LogToFileOnly("GetAcademicYearProgress and building is not completed");
                return 0;
            }
        }
    }
}
