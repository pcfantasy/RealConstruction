using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using RealConstruction.CustomAI;
using RealConstruction.UI;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.IO;
using ColossalFramework.Plugins;
using RealConstruction.NewData;

namespace RealConstruction
{
    public class Loader : LoadingExtensionBase
    {
        public static LoadMode CurrentLoadMode;

        public class Detour
        {
            public MethodInfo OriginalMethod;
            public MethodInfo CustomMethod;
            public RedirectCallsState Redirect;

            public Detour(MethodInfo originalMethod, MethodInfo customMethod)
            {
                this.OriginalMethod = originalMethod;
                this.CustomMethod = customMethod;
                this.Redirect = RedirectionHelper.RedirectCalls(originalMethod, customMethod);
            }
        }

        public static List<Detour> Detours { get; set; }
        public static bool DetourInited = false;
        public static bool HarmonyDetourInited = false;
        public static bool HarmonyDetourFailed = true;
        public static bool isGuiRunning = false;
        public static bool isRealCityRunning = false;
        public static bool isRealGasStationRunning = false;
        public static PlayerBuildingButton PBMenuPanel;
        public static UniqueFactoryButton UBMenuPanel;
        public static WarehouseButton WBMenuPanel;
        public static string m_atlasName = "RealConstruction";
        public static bool m_atlasLoaded;

        public override void OnCreated(ILoading loading)
        {
            Detours = new List<Detour>();
            base.OnCreated(loading);
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            Loader.CurrentLoadMode = mode;
            if (RealConstruction.IsEnabled)
            {
                if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
                {
                    DebugLog.LogToFileOnly("OnLevelLoaded");
                    InitDetour();
                    HarmonyInitDetour();
                    SetupGui();
                    RealConstruction.LoadSetting();
                    if (mode == LoadMode.NewGame)
                    {
                        DebugLog.LogToFileOnly("New Game");
                    }
                }
            }
        }

        public void DataInit()
        {
            for (int i = 0; i < 49152; i++)
            {
                MainDataStore.refreshCanNotConnectedBuildingIDCount[i] = 0;
                MainDataStore.canNotConnectedBuildingIDCount[i] = 0;
                for (int j = 0; j < 8; j++)
                {
                    MainDataStore.canNotConnectedBuildingID[i, j] = 0;
                }
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            if (Loader.CurrentLoadMode == LoadMode.LoadGame || Loader.CurrentLoadMode == LoadMode.NewGame)
            {
                if (RealConstruction.IsEnabled)
                {
                    RevertDetour();
                    HarmonyRevertDetour();
                    RealConstructionThreading.isFirstTime = true;
                    if (Loader.isGuiRunning)
                    {
                        RemoveGui();
                    }
                }
            }
            //RealConstruction.SaveSetting();
        }

        public override void OnReleased()
        {
            base.OnReleased();
        }

        private static void LoadSprites()
        {
            if (SpriteUtilities.GetAtlas(m_atlasName) != null) return;
            var modPath = PluginManager.instance.FindPluginInfo(Assembly.GetExecutingAssembly()).modPath;
            m_atlasLoaded = SpriteUtilities.InitialiseAtlas(Path.Combine(modPath, "Icon/RealConstruction.png"), m_atlasName);
            if (m_atlasLoaded)
            {
                var spriteSuccess = true;
                spriteSuccess = SpriteUtilities.AddSpriteToAtlas(new Rect(new Vector2(1, 1), new Vector2(151, 151)), "Pic", m_atlasName)
                             && spriteSuccess;
                if (!spriteSuccess) DebugLog.LogToFileOnly("Some sprites haven't been loaded. This is abnormal; you should probably report this to the mod creator.");
            }
            else DebugLog.LogToFileOnly("The texture atlas (provides custom icons) has not loaded. All icons have reverted to text prompts.");
        }

        public static void SetupGui()
        {
            LoadSprites();
            if (m_atlasLoaded)
            {
                SetupPlayerBuildingButton();
                SetupWareHouseButton();
                SetupUniqueFactoryButton();
                Loader.isGuiRunning = true;
            }
        }

        public static void RemoveGui()
        {
            Loader.isGuiRunning = false;
            if (PBMenuPanel != null)
            {
                UnityEngine.Object.Destroy(PBMenuPanel);
                Loader.PBMenuPanel = null;
            }
            if (UBMenuPanel != null)
            {
                UnityEngine.Object.Destroy(UBMenuPanel);
                Loader.UBMenuPanel = null;
            }
            if (WBMenuPanel != null)
            {
                UnityEngine.Object.Destroy(WBMenuPanel);
                Loader.WBMenuPanel = null;
            }
        }

        public static void SetupPlayerBuildingButton()
        {
            var playerBuildingInfo = UIView.Find<UIPanel>("(Library) CityServiceWorldInfoPanel");
            if (PBMenuPanel == null)
            {
                PBMenuPanel = (playerBuildingInfo.AddUIComponent(typeof(PlayerBuildingButton)) as PlayerBuildingButton);
            }
            PBMenuPanel.Show();
        }

        public static void SetupUniqueFactoryButton()
        {
            var uniqueFactoryInfo = UIView.Find<UIPanel>("(Library) UniqueFactoryWorldInfoPanel");
            if (UBMenuPanel == null)
            {
                UBMenuPanel = (uniqueFactoryInfo.AddUIComponent(typeof(UniqueFactoryButton)) as UniqueFactoryButton);
            }
            UBMenuPanel.Show();
        }

        public static void SetupWareHouseButton()
        {
            var wareHouseInfo = UIView.Find<UIPanel>("(Library) WarehouseWorldInfoPanel");
            if (WBMenuPanel == null)
            {
                WBMenuPanel = (wareHouseInfo.AddUIComponent(typeof(WarehouseButton)) as WarehouseButton);
            }
            WBMenuPanel.Show();
        }

        public void HarmonyInitDetour()
        {
            if (!HarmonyDetourInited)
            {
                DebugLog.LogToFileOnly("Init harmony detours");
                HarmonyDetours.Apply();
                HarmonyDetourInited = true;
            }
        }

        public void HarmonyRevertDetour()
        {
            if (HarmonyDetourInited)
            {
                DebugLog.LogToFileOnly("Revert harmony detours");
                HarmonyDetours.DeApply();
                HarmonyDetourFailed = true;
                HarmonyDetourInited = false;
            }
        }

        public void InitDetour()
        {
            if (!DetourInited)
            {
                DebugLog.LogToFileOnly("Init detours");
                bool detourFailed = false;

                //1
                DebugLog.LogToFileOnly("Detour PlayerBuildingAI::GetConstructionTime calls");
                try
                {
                    Detours.Add(new Detour(typeof(PlayerBuildingAI).GetMethod("GetConstructionTime", BindingFlags.NonPublic | BindingFlags.Instance),
                                           typeof(CustomPlayerBuildingAI).GetMethod("GetConstructionTime", BindingFlags.NonPublic | BindingFlags.Instance)));
                }
                catch (Exception)
                {
                    DebugLog.LogToFileOnly("Could not detour PlayerBuildingAI::GetConstructionTime");
                    detourFailed = true;
                }

                //2
                DebugLog.LogToFileOnly("Detour CargoTruckAI::GetLocalizedStatus calls");
                try
                {
                    Detours.Add(new Detour(typeof(CargoTruckAI).GetMethod("GetLocalizedStatus", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(InstanceID).MakeByRefType() }, null),
                                           typeof(CustomCargoTruckAI).GetMethod("GetLocalizedStatus", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(InstanceID).MakeByRefType() }, null)));
                }
                catch (Exception)
                {
                    DebugLog.LogToFileOnly("Could not detour CargoTruckAI::GetLocalizedStatus");
                    detourFailed = true;
                }

                //3
                DebugLog.LogToFileOnly("Detour PlayerBuildingAI::GetBudget calls");
                try
                {
                    Detours.Add(new Detour(typeof(PlayerBuildingAI).GetMethod("GetBudget", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType() }, null),
                                           typeof(CustomPlayerBuildingAI).GetMethod("CustomGetBudget", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType() }, null)));
                }
                catch (Exception)
                {
                    DebugLog.LogToFileOnly("Could not detour PlayerBuildingAI::GetBudget");
                    detourFailed = true;
                }

                //4
                DebugLog.LogToFileOnly("Detour CustomDistrictPark::GetAcademicYearProgress calls");
                try
                {
                    Detours.Add(new Detour(typeof(DistrictPark).GetMethod("GetAcademicYearProgress", BindingFlags.Public | BindingFlags.Instance), 
                                           typeof(CustomDistrictPark).GetMethod("GetAcademicYearProgress", BindingFlags.Public | BindingFlags.Static)));
                }
                catch (Exception)
                {
                    DebugLog.LogToFileOnly("Could not redirect CustomDistrictPark::GetAcademicYearProgress");
                    detourFailed = true;
                }

                isRealCityRunning = CheckRealCityIsLoaded();
                isRealGasStationRunning = CheckRealGasStationIsLoaded();
                if (isRealCityRunning || isRealGasStationRunning)
                {
                    DebugLog.LogToFileOnly("RealCity or RealGasStation is Running");
                }
                else
                {
                    //5
                    DebugLog.LogToFileOnly("Detour CargoTruckAI::ArriveAtTarget calls");
                    try
                    {
                        Detours.Add(new Detour(typeof(CargoTruckAI).GetMethod("ArriveAtTarget", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null),
                                               typeof(CustomCargoTruckAI).GetMethod("ArriveAtTarget", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null)));
                    }
                    catch (Exception)
                    {
                        DebugLog.LogToFileOnly("Could not detour CargoTruckAI::ArriveAtTarget");
                        detourFailed = true;
                    }
                }

                if (detourFailed)
                {
                    DebugLog.LogToFileOnly("Detours failed");
                }
                else
                {
                    DebugLog.LogToFileOnly("Detours successful");
                }
                DetourInited = true;
            }
        }

        public void RevertDetour()
        {
            if (DetourInited)
            {
                DebugLog.LogToFileOnly("Revert detours");
                Detours.Reverse();
                foreach (Detour d in Detours)
                {
                    RedirectionHelper.RevertRedirect(d.OriginalMethod, d.Redirect);
                }
                DetourInited = false;
                Detours.Clear();
                DebugLog.LogToFileOnly("Reverting detours finished.");
            }
        }

        private bool Check3rdPartyModLoaded(string namespaceStr, bool printAll = false)
        {
            bool thirdPartyModLoaded = false;

            var loadingWrapperLoadingExtensionsField = typeof(LoadingWrapper).GetField("m_LoadingExtensions", BindingFlags.NonPublic | BindingFlags.Instance);
            List<ILoadingExtension> loadingExtensions = (List<ILoadingExtension>)loadingWrapperLoadingExtensionsField.GetValue(Singleton<LoadingManager>.instance.m_LoadingWrapper);

            if (loadingExtensions != null)
            {
                foreach (ILoadingExtension extension in loadingExtensions)
                {
                    if (printAll)
                        DebugLog.LogToFileOnly($"Detected extension: {extension.GetType().Name} in namespace {extension.GetType().Namespace}");
                    if (extension.GetType().Namespace == null)
                        continue;

                    var nsStr = extension.GetType().Namespace.ToString();
                    if (namespaceStr.Equals(nsStr))
                    {
                        DebugLog.LogToFileOnly($"The mod '{namespaceStr}' has been detected.");
                        thirdPartyModLoaded = true;
                        break;
                    }
                }
            }
            else
            {
                DebugLog.LogToFileOnly("Could not get loading extensions");
            }

            return thirdPartyModLoaded;
        }

        private bool CheckRealCityIsLoaded()
        {
            return this.Check3rdPartyModLoaded("RealCity", true);
        }

        private bool CheckRealGasStationIsLoaded()
        {
            return this.Check3rdPartyModLoaded("RealGasStation", true);
        }
    }
}
