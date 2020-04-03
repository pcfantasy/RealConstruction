using RealConstruction.NewAI;
using RealConstruction.Util;

namespace RealConstruction.CustomAI
{
    public class CustomPlayerBuildingAI
    {
        public static bool CanOperation(ushort buildingID, ref Building buildingData, bool userReject = true)
        {
            if ((MainDataStore.resourceCategory[buildingID] == 4) && userReject)
            {
                return false;
            }

            if (ResourceBuildingAI.IsSpecialBuilding(buildingID))
            {
                return false;
            }

            if (buildingData.Info.m_buildingAI is ParkBuildingAI)
            {
                return false;
            }

            if (buildingData.Info.m_buildingAI is CampusBuildingAI)
            {
                return false;
            }

            if (buildingData.Info.m_class.m_service == ItemClass.Service.Beautification)
            {
                return false;
            }
            PlayerBuildingAI AI = buildingData.Info.m_buildingAI as PlayerBuildingAI;
            return AI.RequireRoadAccess();
        }

        public static bool CanConstruction(ushort buildingID, ref Building buildingData, bool userReject = true)
        {
            if ((MainDataStore.resourceCategory[buildingID] == 4) && userReject)
            {
                return false;
            }

            if (ResourceBuildingAI.IsSpecialBuilding(buildingID))
            {
                return false;
            }

            if (buildingData.Info.m_buildingAI is ParkBuildingAI)
            {
                return false;
            }

            if (buildingData.Info.m_buildingAI is CampusBuildingAI)
            {
                return false;
            }

            PlayerBuildingAI AI = buildingData.Info.m_buildingAI as PlayerBuildingAI;
            return AI.RequireRoadAccess();
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
