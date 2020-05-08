using RealConstruction.NewAI;
using RealConstruction.Util;
using UnityEngine;

namespace RealConstruction.CustomAI
{
    public class CustomCargoTruckAI: CargoTruckAI
    {
        public static void CargoTruckAISetSourceForRealConstruction(ushort vehicleID, ref Vehicle data, ushort sourceBuilding)
        {
            CargoTruckAI AI = data.Info.m_vehicleAI as CargoTruckAI;
            int num = Mathf.Min(0, (int)data.m_transferSize - AI.m_cargoCapacity);
            //new added begin
            if (ResourceBuildingAI.IsSpecialBuilding(sourceBuilding))
            {
                if ((TransferManager.TransferReason)data.m_transferType == (TransferManager.TransferReason)124)
                {
                    MainDataStore.constructionResourceBuffer[sourceBuilding] -= 8000;
                }
                else if ((TransferManager.TransferReason)data.m_transferType == (TransferManager.TransferReason)125)
                {
                    MainDataStore.operationResourceBuffer[sourceBuilding] -= 8000;
                }
                else
                {
                    DebugLog.LogToFileOnly("find unknow transfor for SpecialBuilding " + data.m_transferType.ToString());
                }
            }
        }
    }
}
