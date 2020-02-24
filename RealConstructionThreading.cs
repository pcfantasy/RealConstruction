using ColossalFramework;
using ColossalFramework.Math;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ColossalFramework.Globalization;
using System.Reflection;
using System.IO;
using RealConstruction.CustomAI;
using RealConstruction.Util;
using RealConstruction.UI;
using RealConstruction.CustomManager;
using ColossalFramework.UI;
using Harmony;

namespace RealConstruction
{
    public class RealConstructionThreading : ThreadingExtensionBase
    {
        public static bool isFirstTime = true;
        public static FieldInfo _reduceVehicle = null;
        public static Assembly RealCity = null;
        public static Assembly RealGasStation = null;
        public static Type RealCityClass = null;
        public static object RealCityInstance = null;
        public static bool reduceVehicle = false;
        public static Type MainDataStoreClass = null;
        public static object MainDataStoreInstance = null;
        public static FieldInfo _reduceCargoDiv = null;
        public static int reduceCargoDiv = 1;
        public const int HarmonyPatchNum = 5;

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
                        if (!isFirstTime)
                        {
                            if (Loader.isRealCityRunning)
                            {
                                reduceVehicle = (bool)_reduceVehicle.GetValue(RealCityInstance);
                                if (reduceVehicle)
                                {
                                    reduceCargoDiv = (int)_reduceCargoDiv.GetValue(MainDataStoreInstance);
                                }
                                else
                                {
                                    reduceCargoDiv = 1;
                                }
                            }
                            else
                            {
                                reduceVehicle = false;
                                reduceCargoDiv = 1;
                            }
                            //DebugLog.LogToFileOnly("Info: reduceVehicle = " + reduceVehicle.ToString());
                            //DebugLog.LogToFileOnly("Info: reduceCargoDiv = " + reduceCargoDiv.ToString());
                        }
                    }
                    //CustomSimulationStepImpl for 110 111 TransferReason
                    CustomTransferManager.CustomSimulationStepImpl();
                }
            }
        }

        public void DetourAfterLoad()
        {
            //This is for Detour RealCity method
            DebugLog.LogToFileOnly("Init DetourAfterLoad");
            if (Loader.isRealCityRunning)
            {
                RealCity = Assembly.Load("RealCity");
                RealCityClass = RealCity.GetType("RealCity.RealCity");
                RealCityInstance = Activator.CreateInstance(RealCityClass);
                _reduceVehicle = RealCityClass.GetField("reduceVehicle", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                MainDataStoreClass = RealCity.GetType("RealCity.Util.MainDataStore");
                MainDataStoreInstance = Activator.CreateInstance(MainDataStoreClass);
                _reduceCargoDiv = MainDataStoreClass.GetField("reduceCargoDiv", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            }
        }

        public void CheckDetour()
        {
            if (isFirstTime && Loader.DetourInited && Loader.HarmonyDetourInited)
            {
                isFirstTime = false;
                DetourAfterLoad();
                if (Loader.DetourInited)
                {
                    DebugLog.LogToFileOnly("ThreadingExtension.OnBeforeSimulationFrame: First frame detected. Checking detours.");
                    List<string> list = new List<string>();
                    foreach (Loader.Detour current in Loader.Detours)
                    {
                        if (!RedirectionHelper.IsRedirected(current.OriginalMethod, current.CustomMethod))
                        {
                            list.Add(string.Format("{0}.{1} with {2} parameters ({3})", new object[]
                            {
                    current.OriginalMethod.DeclaringType.Name,
                    current.OriginalMethod.Name,
                    current.OriginalMethod.GetParameters().Length,
                    current.OriginalMethod.DeclaringType.AssemblyQualifiedName
                            }));
                        }
                    }
                    DebugLog.LogToFileOnly(string.Format("ThreadingExtension.OnBeforeSimulationFrame: First frame detected. Detours checked. Result: {0} missing detours", list.Count));
                    if (list.Count > 0)
                    {
                        string error = "RealConstruction detected an incompatibility with another mod! You can continue playing but it's NOT recommended. RealConstruction will not work as expected. Send RealConstruction.txt to Author.";
                        DebugLog.LogToFileOnly(error);
                        string text = "The following methods were overriden by another mod:";
                        foreach (string current2 in list)
                        {
                            text += string.Format("\n\t{0}", current2);
                        }
                        DebugLog.LogToFileOnly(text);
                        UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Incompatibility Issue", text, true);
                    }

                    if (Loader.HarmonyDetourFailed)
                    {
                        string error = "RealConstruction HarmonyDetourInit is failed, Send RealConstruction.txt to Author.";
                        DebugLog.LogToFileOnly(error);
                        UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Incompatibility Issue", error, true);
                    }
                    else
                    {
                        var harmony = HarmonyInstance.Create(HarmonyDetours.ID);
                        var methods = harmony.GetPatchedMethods();
                        int i = 0;
                        foreach (var method in methods)
                        {
                            var info = harmony.GetPatchInfo(method);
                            if (info.Owners?.Contains(harmony.Id) == true)
                            {
                                DebugLog.LogToFileOnly("Harmony patch method = " + method.Name.ToString());
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
}
