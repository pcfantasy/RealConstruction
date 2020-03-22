using RealConstruction.NewAI;

namespace RealConstruction.CustomAI
{
    public class CustomPlayerBuildingAI
    {
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
