using ColossalFramework;
using ICities;
using System;
using System.Reflection;
using RealConstruction.Util;
using RealConstruction.UI;
using RealConstruction.CustomManager;
using ColossalFramework.UI;
using HarmonyLib;

namespace RealConstruction
{
    public class RealConstructionThreading : ThreadingExtensionBase
    {
        public static bool isFirstTime = true;
        public const int HarmonyPatchNum = 9;

        public override void OnBeforeSimulationFrame()
        {
            base.OnBeforeSimulationFrame();
            if (Loader.CurrentLoadMode == LoadMode.LoadGame || Loader.CurrentLoadMode == LoadMode.NewGame)
            {
                if (RealConstruction.IsEnabled)
                {
                    CheckDetour();
                }
            }
        }

        public override void OnAfterSimulationFrame()
        {
            base.OnAfterSimulationFrame();
            if (Loader.CurrentLoadMode == LoadMode.LoadGame || Loader.CurrentLoadMode == LoadMode.NewGame)
            {
                uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
                int num4 = (int)(currentFrameIndex & 255u);
                if (RealConstruction.IsEnabled)
                {
                    BuildingManager instance = Singleton<BuildingManager>.instance;
                    if (num4 == 255)
                    {
                        PlayerBuildingUI.refeshOnce = true;
                        UniqueFactoryUI.refeshOnce = true;
                        UniqueFactoryButton.refeshOnce = true;
                        WarehouseButton.refeshOnce = true;
                        WareHouseUI.refeshOnce = true;
                        PlayerBuildingButton.refeshOnce = true;
                    }
                    //CustomSimulationStepImpl for 124 125 TransferReason
                    CustomTransferManager.CustomSimulationStepImpl();
                }
            }
        }

        public void CheckDetour()
        {
            if (isFirstTime && Loader.HarmonyDetourInited)
            {
                isFirstTime = false;

                if (Loader.HarmonyDetourFailed)
                {
                    string error = "RealConstruction HarmonyDetourInit is failed, Send RealConstruction.txt to Author.";
                    DebugLog.LogToFileOnly(error);
                    UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Incompatibility Issue", error, true);
                }
                else
                {
                    var harmony = new Harmony(HarmonyDetours.Id);
                    var methods = harmony.GetPatchedMethods();
                    int i = 0;
                    foreach (var method in methods)
                    {
                        var info = Harmony.GetPatchInfo(method);
                        if (info.Owners?.Contains(harmony.Id) == true)
                        {
                            DebugLog.LogToFileOnly($"Harmony patch method = {method.FullDescription()}");
                            if (info.Prefixes.Count != 0)
                            {
                                DebugLog.LogToFileOnly("Harmony patch method has PreFix");
                            }
                            if (info.Postfixes.Count != 0)
                            {
                                DebugLog.LogToFileOnly("Harmony patch method has PostFix");
                            }
                            i++;
                        }
                    }

                    if (i != HarmonyPatchNum)
                    {
                        string error = $"RealConstruction HarmonyDetour Patch Num is {i}, Right Num is {HarmonyPatchNum} Send RealConstruction.txt to Author.";
                        DebugLog.LogToFileOnly(error);
                        UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Incompatibility Issue", error, true);
                    }
                }
            }
        }
    }
}
