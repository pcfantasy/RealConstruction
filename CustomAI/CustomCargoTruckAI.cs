using ColossalFramework;
using ColossalFramework.Math;
using RealConstruction.NewAI;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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

        public static float GetResourcePrice(TransferManager.TransferReason material)
        {
            //Need to sync with RealCity mod
            float num;
            if (!RealConstructionThreading.reduceVehicle)
            {
                switch (material)
                {
                    case TransferManager.TransferReason.Petrol:
                        num = 3f; break;
                    case TransferManager.TransferReason.Food:
                        num = 1.5f; break;
                    case TransferManager.TransferReason.Lumber:
                        num = 2f; break;
                    case TransferManager.TransferReason.Coal:
                        num = 2.5f; break;
                    default: DebugLog.LogToFileOnly("Error: Unknow material in RealConstruction = " + material.ToString()); num = 0f; break;
                }
            }
            else
            {
                switch (material)
                {
                    case TransferManager.TransferReason.Petrol:
                        num = 3f * RealConstructionThreading.reduceCargoDiv; break;
                    case TransferManager.TransferReason.Food:
                        num = 1.5f * RealConstructionThreading.reduceCargoDiv; break;
                    case TransferManager.TransferReason.Lumber:
                        num = 2f * RealConstructionThreading.reduceCargoDiv; break;
                    case TransferManager.TransferReason.Coal:
                        num = 2.5f * RealConstructionThreading.reduceCargoDiv; break;
                    default: DebugLog.LogToFileOnly("Error: Unknow material in RealConstruction = " + material.ToString()); num = 0f; break;
                }
            }
            return (float)(UniqueFacultyAI.IncreaseByBonus(UniqueFacultyAI.FacultyBonus.Science, 100) / 100f) * num ;
        }
    }
}
