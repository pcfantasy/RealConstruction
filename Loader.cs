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
        public static UIPanel playerBuildingInfo;
        public static UIPanel uniqueFactoryInfo;
        public static UIPanel wareHouseInfo;
        public static UniqueFactoryUI uniqueFactoryPanel;
        public static WareHouseUI wareHousePanel;
        public static PlayerBuildingUI playerBuildingPanel;
        public static PlayerBuildingButton PBMenuPanel;
        public static UniqueFactoryButton UBMenuPanel;
        public static WarehouseButton WBMenuPanel;
        public static GameObject PlayerBuildingWindowGameObject;
        public static GameObject UniqueFactoryWindowGameObject;
        public static GameObject WareHouseWindowGameObject;
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
                SetupPlayerBuidingGui();
                SetupPlayerBuildingButton();
                SetupWareHouseGui();
                SetupWareHouseButton();
                SetupUniqueFactoryGui();
                SetupUniqueFactoryButton();
                Loader.isGuiRunning = true;
            }
        }

        public static void RemoveGui()
        {
            Loader.isGuiRunning = false;
            if (playerBuildingInfo != null)
            {
                UnityEngine.Object.Destroy(PBMenuPanel);
                Loader.PBMenuPanel = null;
            }
            if (uniqueFactoryInfo != null)
            {
                UnityEngine.Object.Destroy(UBMenuPanel);
                Loader.UBMenuPanel = null;
            }
            if (wareHouseInfo != null)
            {
                UnityEngine.Object.Destroy(WBMenuPanel);
                Loader.WBMenuPanel = null;
            }

            //remove PlayerbuildingUI
            if (uniqueFactoryPanel != null)
            {
                if (uniqueFactoryPanel.parent != null)
                {
                    uniqueFactoryPanel.parent.eventVisibilityChanged -= uniqueFactoryInfo_eventVisibilityChanged;
                }
            }
            if (wareHousePanel != null)
            {
                if (wareHousePanel.parent != null)
                {
                    wareHousePanel.parent.eventVisibilityChanged -= wareHouseInfo_eventVisibilityChanged;
                }
            }
            if (playerBuildingPanel != null)
            {
                if (playerBuildingPanel.parent != null)
                {
                    playerBuildingPanel.parent.eventVisibilityChanged -= playerbuildingInfo_eventVisibilityChanged;
                }
            }

            if (PlayerBuildingWindowGameObject != null)
            {
                UnityEngine.Object.Destroy(PlayerBuildingWindowGameObject);
            }

            if (WareHouseWindowGameObject != null)
            {
                UnityEngine.Object.Destroy(WareHouseWindowGameObject);
            }

            if (UniqueFactoryWindowGameObject != null)
            {
                UnityEngine.Object.Destroy(UniqueFactoryWindowGameObject);
            }
        }

        public static void SetupUniqueFactoryGui()
        {
            UniqueFactoryWindowGameObject = new GameObject("UniqueFactoryWindowGameObject");
            uniqueFactoryPanel = (UniqueFactoryUI)UniqueFactoryWindowGameObject.AddComponent(typeof(UniqueFactoryUI));


            uniqueFactoryInfo = UIView.Find<UIPanel>("(Library) UniqueFactoryWorldInfoPanel");
            if (uniqueFactoryInfo == null)
            {
                DebugLog.LogToFileOnly("UIPanel not found (update broke the mod!): (Library) UniqueFactoryWorldInfoPanel\nAvailable panels are:\n");
            }
            uniqueFactoryPanel.transform.parent = uniqueFactoryInfo.transform;
            uniqueFactoryPanel.size = new Vector3(uniqueFactoryInfo.size.x, uniqueFactoryInfo.size.y);
            uniqueFactoryPanel.baseBuildingWindow = uniqueFactoryInfo.gameObject.transform.GetComponentInChildren<UniqueFactoryWorldInfoPanel>();
            uniqueFactoryPanel.position = new Vector3(uniqueFactoryInfo.size.x, uniqueFactoryInfo.size.y);
            uniqueFactoryInfo.eventVisibilityChanged += uniqueFactoryInfo_eventVisibilityChanged;
        }

        public static void SetupWareHouseGui()
        {
            WareHouseWindowGameObject = new GameObject("WareHouseWindowGameObject");
            wareHousePanel = (WareHouseUI)WareHouseWindowGameObject.AddComponent(typeof(WareHouseUI));


            wareHouseInfo = UIView.Find<UIPanel>("(Library) WarehouseWorldInfoPanel");
            if (wareHouseInfo == null)
            {
                DebugLog.LogToFileOnly("UIPanel not found (update broke the mod!): (Library) WarehouseWorldInfoPanel\nAvailable panels are:\n");
            }
            wareHousePanel.transform.parent = wareHouseInfo.transform;
            wareHousePanel.size = new Vector3(wareHouseInfo.size.x, wareHouseInfo.size.y);
            wareHousePanel.baseBuildingWindow = wareHouseInfo.gameObject.transform.GetComponentInChildren<WarehouseWorldInfoPanel>();
            wareHousePanel.position = new Vector3(wareHouseInfo.size.x, wareHouseInfo.size.y);
            wareHouseInfo.eventVisibilityChanged += wareHouseInfo_eventVisibilityChanged;
        }

        public static void SetupPlayerBuidingGui()
        {
            PlayerBuildingWindowGameObject = new GameObject("PlayerbuildingWindowGameObject");
            playerBuildingPanel = (PlayerBuildingUI)PlayerBuildingWindowGameObject.AddComponent(typeof(PlayerBuildingUI));


            playerBuildingInfo = UIView.Find<UIPanel>("(Library) CityServiceWorldInfoPanel");
            if (playerBuildingInfo == null)
            {
                DebugLog.LogToFileOnly("UIPanel not found (update broke the mod!): (Library) CityServiceWorldInfoPanel\nAvailable panels are:\n");
            }
            playerBuildingPanel.transform.parent = playerBuildingInfo.transform;
            playerBuildingPanel.size = new Vector3(playerBuildingInfo.size.x, playerBuildingInfo.size.y);
            playerBuildingPanel.baseBuildingWindow = playerBuildingInfo.gameObject.transform.GetComponentInChildren<CityServiceWorldInfoPanel>();
            playerBuildingPanel.position = new Vector3(playerBuildingInfo.size.x, playerBuildingInfo.size.y);
            playerBuildingInfo.eventVisibilityChanged += playerbuildingInfo_eventVisibilityChanged;
        }

        public static void SetupPlayerBuildingButton()
        {
            if (PBMenuPanel == null)
            {
                PBMenuPanel = (playerBuildingInfo.AddUIComponent(typeof(PlayerBuildingButton)) as PlayerBuildingButton);
            }
            PBMenuPanel.Show();
        }

        public static void SetupUniqueFactoryButton()
        {
            if (UBMenuPanel == null)
            {
                UBMenuPanel = (uniqueFactoryInfo.AddUIComponent(typeof(UniqueFactoryButton)) as UniqueFactoryButton);
            }
            UBMenuPanel.Show();
        }

        public static void SetupWareHouseButton()
        {
            if (WBMenuPanel == null)
            {
                WBMenuPanel = (wareHouseInfo.AddUIComponent(typeof(WarehouseButton)) as WarehouseButton);
            }
            WBMenuPanel.Show();
        }

        public static void playerbuildingInfo_eventVisibilityChanged(UIComponent component, bool value)
        {
            playerBuildingPanel.isEnabled = value;
            if (value)
            {
                Loader.playerBuildingPanel.transform.parent = Loader.playerBuildingInfo.transform;
                Loader.playerBuildingPanel.size = new Vector3(Loader.playerBuildingInfo.size.x, Loader.playerBuildingInfo.size.y);
                Loader.playerBuildingPanel.baseBuildingWindow = Loader.playerBuildingInfo.gameObject.transform.GetComponentInChildren<CityServiceWorldInfoPanel>();
                Loader.playerBuildingPanel.position = new Vector3(Loader.playerBuildingInfo.size.x, Loader.playerBuildingInfo.size.y);
            }
            else
            {
                playerBuildingPanel.Hide();
            }
        }

        public static void uniqueFactoryInfo_eventVisibilityChanged(UIComponent component, bool value)
        {
            uniqueFactoryPanel.isEnabled = value;
            if (value)
            {
                Loader.uniqueFactoryPanel.transform.parent = Loader.uniqueFactoryInfo.transform;
                Loader.uniqueFactoryPanel.size = new Vector3(Loader.uniqueFactoryInfo.size.x, Loader.uniqueFactoryInfo.size.y);
                Loader.uniqueFactoryPanel.baseBuildingWindow = Loader.uniqueFactoryInfo.gameObject.transform.GetComponentInChildren<UniqueFactoryWorldInfoPanel>();
                Loader.uniqueFactoryPanel.position = new Vector3(Loader.uniqueFactoryInfo.size.x, Loader.uniqueFactoryInfo.size.y);
            }
            else
            {
                uniqueFactoryPanel.Hide();
            }
        }

        public static void wareHouseInfo_eventVisibilityChanged(UIComponent component, bool value)
        {
            wareHousePanel.isEnabled = value;
            if (value)
            {
                Loader.wareHousePanel.transform.parent = Loader.wareHouseInfo.transform;
                Loader.wareHousePanel.size = new Vector3(Loader.wareHouseInfo.size.x, Loader.wareHouseInfo.size.y);
                Loader.wareHousePanel.baseBuildingWindow = Loader.wareHouseInfo.gameObject.transform.GetComponentInChildren<WarehouseWorldInfoPanel>();
                Loader.wareHousePanel.position = new Vector3(Loader.wareHouseInfo.size.x, Loader.wareHouseInfo.size.y);
            }
            else
            {
                wareHousePanel.Hide();
            }
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
