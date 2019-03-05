using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using RealConstruction.CustomAI;
using RealConstruction.UI;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public static bool isGuiRunning = false;
        public static bool isRealCityRunning = false;
        public static bool isRealGasStationRunning = false;

        public static UIPanel playerBuildingInfo;
        public static UIPanel uniqueFactoryInfo;
        public static UIPanel wareHouseInfo;

        public static UniqueFactoryUI guiPanel2;
        public static WareHouseUI guiPanel3;
        public static PlayerBuildingUI guiPanel4;

        public static PlayerBuildingButton PBMenuPanel;
        public static UniqueFactoryButton UBMenuPanel;
        public static WarehouseButton WBMenuPanel;

        public static GameObject PlayerbuildingWindowGameObject;
        public static GameObject UniqueFactoryWindowGameObject;
        public static GameObject WareHouseWindowGameObject;

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
                    SetupGui();
                    //RealConstruction.LoadSetting();
                    if (mode == LoadMode.NewGame)
                    {
                        DebugLog.LogToFileOnly("New Game");
                    }
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


        public static void SetupGui()
        {
            SetupPlayerBuidingGui();
            SetupPlayerBuildingButton();
            SetupWareHouseGui();
            SetupWareHouseButton();
            SetupUniqueFactoryGui();
            SetupUniqueFactoryButton();
            Loader.isGuiRunning = true;
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
            if (guiPanel2 != null)
            {
                if (guiPanel2.parent != null)
                {
                    guiPanel2.parent.eventVisibilityChanged -= uniqueFactoryInfo_eventVisibilityChanged;
                }
            }
            if (guiPanel3 != null)
            {
                if (guiPanel3.parent != null)
                {
                    guiPanel3.parent.eventVisibilityChanged -= wareHouseInfo_eventVisibilityChanged;
                }
            }
            if (guiPanel4 != null)
            {
                if (guiPanel4.parent != null)
                {
                    guiPanel4.parent.eventVisibilityChanged -= playerbuildingInfo_eventVisibilityChanged;
                }
            }

            if (PlayerbuildingWindowGameObject != null)
            {
                UnityEngine.Object.Destroy(PlayerbuildingWindowGameObject);
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
            guiPanel2 = (UniqueFactoryUI)UniqueFactoryWindowGameObject.AddComponent(typeof(UniqueFactoryUI));


            uniqueFactoryInfo = UIView.Find<UIPanel>("(Library) UniqueFactoryWorldInfoPanel");
            if (uniqueFactoryInfo == null)
            {
                DebugLog.LogToFileOnly("UIPanel not found (update broke the mod!): (Library) UniqueFactoryWorldInfoPanel\nAvailable panels are:\n");
            }
            guiPanel2.transform.parent = uniqueFactoryInfo.transform;
            guiPanel2.size = new Vector3(uniqueFactoryInfo.size.x, uniqueFactoryInfo.size.y);
            guiPanel2.baseBuildingWindow = uniqueFactoryInfo.gameObject.transform.GetComponentInChildren<UniqueFactoryWorldInfoPanel>();
            guiPanel2.position = new Vector3(uniqueFactoryInfo.size.x, uniqueFactoryInfo.size.y);
            uniqueFactoryInfo.eventVisibilityChanged += uniqueFactoryInfo_eventVisibilityChanged;
        }

        public static void SetupWareHouseGui()
        {
            WareHouseWindowGameObject = new GameObject("WareHouseWindowGameObject");
            guiPanel3 = (WareHouseUI)WareHouseWindowGameObject.AddComponent(typeof(WareHouseUI));


            wareHouseInfo = UIView.Find<UIPanel>("(Library) WarehouseWorldInfoPanel");
            if (wareHouseInfo == null)
            {
                DebugLog.LogToFileOnly("UIPanel not found (update broke the mod!): (Library) WarehouseWorldInfoPanel\nAvailable panels are:\n");
            }
            guiPanel3.transform.parent = wareHouseInfo.transform;
            guiPanel3.size = new Vector3(wareHouseInfo.size.x, wareHouseInfo.size.y);
            guiPanel3.baseBuildingWindow = wareHouseInfo.gameObject.transform.GetComponentInChildren<WarehouseWorldInfoPanel>();
            guiPanel3.position = new Vector3(wareHouseInfo.size.x, wareHouseInfo.size.y);
            wareHouseInfo.eventVisibilityChanged += wareHouseInfo_eventVisibilityChanged;
        }

        public static void SetupPlayerBuidingGui()
        {
            PlayerbuildingWindowGameObject = new GameObject("PlayerbuildingWindowGameObject");
            guiPanel4 = (PlayerBuildingUI)PlayerbuildingWindowGameObject.AddComponent(typeof(PlayerBuildingUI));


            playerBuildingInfo = UIView.Find<UIPanel>("(Library) CityServiceWorldInfoPanel");
            if (playerBuildingInfo == null)
            {
                DebugLog.LogToFileOnly("UIPanel not found (update broke the mod!): (Library) CityServiceWorldInfoPanel\nAvailable panels are:\n");
            }
            guiPanel4.transform.parent = playerBuildingInfo.transform;
            guiPanel4.size = new Vector3(playerBuildingInfo.size.x, playerBuildingInfo.size.y);
            guiPanel4.baseBuildingWindow = playerBuildingInfo.gameObject.transform.GetComponentInChildren<CityServiceWorldInfoPanel>();
            guiPanel4.position = new Vector3(playerBuildingInfo.size.x, playerBuildingInfo.size.y);
            playerBuildingInfo.eventVisibilityChanged += playerbuildingInfo_eventVisibilityChanged;
        }


        public static void SetupPlayerBuildingButton()
        {
            if (PBMenuPanel == null)
            {
                PBMenuPanel = (playerBuildingInfo.AddUIComponent(typeof(PlayerBuildingButton)) as PlayerBuildingButton);
            }
            PBMenuPanel.RefPanel = playerBuildingInfo;
            PBMenuPanel.Alignment = UIAlignAnchor.TopLeft;
            PBMenuPanel.Show();
        }

        public static void SetupUniqueFactoryButton()
        {
            if (UBMenuPanel == null)
            {
                UBMenuPanel = (uniqueFactoryInfo.AddUIComponent(typeof(UniqueFactoryButton)) as UniqueFactoryButton);
            }
            UBMenuPanel.RefPanel = uniqueFactoryInfo;
            UBMenuPanel.Alignment = UIAlignAnchor.TopLeft;
            UBMenuPanel.Show();
        }

        public static void SetupWareHouseButton()
        {
            if (WBMenuPanel == null)
            {
                WBMenuPanel = (wareHouseInfo.AddUIComponent(typeof(WarehouseButton)) as WarehouseButton);
            }
            WBMenuPanel.RefPanel = wareHouseInfo;
            WBMenuPanel.Alignment = UIAlignAnchor.TopLeft;
            WBMenuPanel.Show();
        }

        public static void playerbuildingInfo_eventVisibilityChanged(UIComponent component, bool value)
        {
            guiPanel4.isEnabled = value;
            if (value)
            {
                Loader.guiPanel4.transform.parent = Loader.playerBuildingInfo.transform;
                Loader.guiPanel4.size = new Vector3(Loader.playerBuildingInfo.size.x, Loader.playerBuildingInfo.size.y);
                Loader.guiPanel4.baseBuildingWindow = Loader.playerBuildingInfo.gameObject.transform.GetComponentInChildren<CityServiceWorldInfoPanel>();
                Loader.guiPanel4.position = new Vector3(Loader.playerBuildingInfo.size.x, Loader.playerBuildingInfo.size.y);
            }
            else
            {
                guiPanel4.Hide();
            }
        }

        public static void uniqueFactoryInfo_eventVisibilityChanged(UIComponent component, bool value)
        {
            guiPanel2.isEnabled = value;
            if (value)
            {
                Loader.guiPanel2.transform.parent = Loader.uniqueFactoryInfo.transform;
                Loader.guiPanel2.size = new Vector3(Loader.uniqueFactoryInfo.size.x, Loader.uniqueFactoryInfo.size.y);
                Loader.guiPanel2.baseBuildingWindow = Loader.uniqueFactoryInfo.gameObject.transform.GetComponentInChildren<UniqueFactoryWorldInfoPanel>();
                Loader.guiPanel2.position = new Vector3(Loader.uniqueFactoryInfo.size.x, Loader.uniqueFactoryInfo.size.y);
            }
            else
            {
                guiPanel2.Hide();
            }
        }

        public static void wareHouseInfo_eventVisibilityChanged(UIComponent component, bool value)
        {
            guiPanel3.isEnabled = value;
            if (value)
            {
                Loader.guiPanel3.transform.parent = Loader.wareHouseInfo.transform;
                Loader.guiPanel3.size = new Vector3(Loader.wareHouseInfo.size.x, Loader.wareHouseInfo.size.y);
                Loader.guiPanel3.baseBuildingWindow = Loader.wareHouseInfo.gameObject.transform.GetComponentInChildren<WarehouseWorldInfoPanel>();
                Loader.guiPanel3.position = new Vector3(Loader.wareHouseInfo.size.x, Loader.wareHouseInfo.size.y);
            }
            else
            {
                guiPanel3.Hide();
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
                DebugLog.LogToFileOnly("Detour PlayerBuildingAI::SimulationStep calls");
                try
                {
                    Detours.Add(new Detour(typeof(PlayerBuildingAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Building.Frame).MakeByRefType() }, null),
                                           typeof(CustomPlayerBuildingAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Building.Frame).MakeByRefType() }, null)));
                }
                catch (Exception)
                {
                    DebugLog.LogToFileOnly("Could not detour PlayerBuildingAI::SimulationStep ");
                    detourFailed = true;
                }

                //4.1
                DebugLog.LogToFileOnly("Detour PrivateBuildingAI::SimulationStep calls");
                try
                {
                    Detours.Add(new Detour(typeof(PrivateBuildingAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Building.Frame).MakeByRefType() }, null),
                                           typeof(CustomPrivateBuildingAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Building.Frame).MakeByRefType() }, null)));
                }
                catch (Exception)
                {
                    DebugLog.LogToFileOnly("Could not detour PrivateBuildingAI::SimulationStep ");
                    detourFailed = true;
                }

                isRealCityRunning = CheckRealCityIsLoaded();
                isRealGasStationRunning = CheckRealGasStationIsLoaded();
                if (isRealCityRunning)
                {
                    DebugLog.LogToFileOnly("realCity is Running");
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
