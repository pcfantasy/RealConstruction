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
using CitiesHarmony.API;
using ColossalFramework.Globalization;
using UnityEngine.SocialPlatforms;

namespace RealConstruction
{
    public class Loader : LoadingExtensionBase
    {
        public static LoadMode CurrentLoadMode;
        public static bool HarmonyDetourInited = false;
        public static bool HarmonyDetourFailed = true;
        public static bool isGuiRunning = false;
        public static bool isRealCityRunning = false;
        public static PlayerBuildingButton PBMenuPanel;
        public static UniqueFactoryButton UBMenuPanel;
        public static WarehouseButton WBMenuPanel;
        public static string m_atlasName = "RealConstruction";
        public static bool m_atlasLoaded;

        public override void OnCreated(ILoading loading)
        {
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
                    HarmonyInitDetour();
                    SetupGui();
                    SetupLocalization();

                    
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
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                if (!HarmonyDetourInited)
                {
                    DebugLog.LogToFileOnly("Init harmony detours");
                    HarmonyDetours.Apply();
                    HarmonyDetourInited = true;
                }
            }
        }

        public void HarmonyRevertDetour()
        {
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                if (HarmonyDetourInited)
                {
                    DebugLog.LogToFileOnly("Revert harmony detours");
                    HarmonyDetours.DeApply();
                    HarmonyDetourFailed = true;
                    HarmonyDetourInited = false;
                }
            }
        }

        private void SetupLocalization()
        {
            var delivery_construction_key = new Locale.Key
            {
                m_Identifier = "VEHICLE_STATUS_CARGOTRUCK_DELIVER",
                m_Key = "124",
                m_Index = 0
            };
            var delivery_operation_key = new Locale.Key
            {
                m_Identifier = "VEHICLE_STATUS_CARGOTRUCK_DELIVER",
                m_Key = "125",
                m_Index = 0
            };

            var loc = new Locale();
            loc.AddLocalizedString(delivery_construction_key, Localization.Get("TRANSFER_CONSTRUCTION_RESOURCE_TO"));
            loc.AddLocalizedString(delivery_operation_key, Localization.Get("TRANSFER_OPERATION_RESOURCE_TO"));

            loc.appendOverride = true;
            Locale.LocaleOverride = loc;
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
            return this.Check3rdPartyModLoaded("RealCity", false);
        }
    }
}
