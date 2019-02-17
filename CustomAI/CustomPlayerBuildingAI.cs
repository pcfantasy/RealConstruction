using ColossalFramework;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealConstruction.CustomAI
{
    public class CustomPlayerBuildingAI: CommonBuildingAI
    {
        protected override int GetConstructionTime()
        {
            if ((Singleton<ToolManager>.instance.m_properties.m_mode & ItemClass.Availability.AssetEditor) != ItemClass.Availability.None)
            {
                return 0;
            }
            return 100;
        }

        public int CustomGetBudget(ushort buildingID, ref Building buildingData)
        {
            ushort eventIndex = buildingData.m_eventIndex;
            if (eventIndex != 0)
            {
                EventManager instance = Singleton<EventManager>.instance;
                EventInfo info = instance.m_events.m_buffer[eventIndex].Info;
                return info.m_eventAI.GetBudget(eventIndex, ref instance.m_events.m_buffer[eventIndex]);
            }

            if (MainDataStore.operationResourceBuffer[buildingID] < 1000 && RealConstructionThreading.canOperation(buildingID, ref buildingData))
            {
                return 10;
            }
            else
            {
                return Singleton<EconomyManager>.instance.GetBudget(m_info.m_class);
            }
        }
    }
}
