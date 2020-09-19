using ColossalFramework;
using HarmonyLib;
using RealConstruction.CustomAI;
using RealConstruction.Util;
using System;
using System.Reflection;

namespace RealConstruction.Patch
{
    [HarmonyPatch]
    public class PlayerBuildingAICreateBuildingPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(PlayerBuildingAI).GetMethod("CreateBuilding", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType() }, null);
        }
        public static void Prefix(ushort buildingID, ref Building data)
        {
            if (data.Info.name == "1908304237.City Resource Building_Data")
            {
                //special building default value
                if (RealConstruction.operationConsumption == 2)
                {
                    MainDataStore.resourceCategory[buildingID] = 2;
                }
                else
                {
                    MainDataStore.resourceCategory[buildingID] = 1;
                }
            }
        }
    }
}
