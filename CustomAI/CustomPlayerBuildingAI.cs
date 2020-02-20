using ColossalFramework;
using RealConstruction.NewAI;
using RealConstruction.Util;
using System;
using UnityEngine;

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

            if (MainDataStore.operationResourceBuffer[buildingID] < 1000 && CanOperation(buildingID, ref buildingData))
            {
                return 10;
            }
            else
            {
                return Singleton<EconomyManager>.instance.GetBudget(m_info.m_class);
            }
        }

        public static bool CanOperation(ushort buildingID, ref Building buildingData)
        {
            if (ResourceBuildingAI.IsSpecialBuilding(buildingID))
            {
                return false;
            }
            else if (buildingData.Info.m_buildingAI is ParkBuildingAI)
            {
                return false;
            }
            else if (buildingData.Info.m_buildingAI is CampusBuildingAI)
            {
                return false;
            }
            else if (buildingData.Info.m_class.m_service == ItemClass.Service.Beautification)
            {
                return false;
            }
            else
            {
                PlayerBuildingAI AI = buildingData.Info.m_buildingAI as PlayerBuildingAI;
                return AI.RequireRoadAccess();
            }
        }

        public static bool CanConstruction(ushort buildingID, ref Building buildingData)
        {
            if (ResourceBuildingAI.IsSpecialBuilding(buildingID))
            {
                return false;
            }
            else if (buildingData.Info.m_buildingAI is ParkBuildingAI)
            {
                return false;
            }
            else if (buildingData.Info.m_buildingAI is CampusBuildingAI)
            {
                return false;
            }
            else
            {
                PlayerBuildingAI AI = buildingData.Info.m_buildingAI as PlayerBuildingAI;
                return AI.RequireRoadAccess();
            }
        }

        public static bool CanRemoveNoResource(ushort buildingID, ref Building buildingData)
        {
            if (buildingData.Info.m_buildingAI is ProcessingFacilityAI || buildingData.Info.m_buildingAI is UniqueFactoryAI)
            {
                return false;
            }
            return true;
        }
    }
}
