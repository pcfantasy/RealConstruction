using ColossalFramework;
using ColossalFramework.UI;
using ICities;
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

        public static bool isGuiRunning = false;

        public static bool realCityRunning = false;
        public static bool fuelAlarmRunning = false;

        public static UIPanel playerbuildingInfo;

        public static PlayerBuildingUI guiPanel4;

        public static PlayerBuildingButton PBMenuPanel;

        public static GameObject PlayerbuildingWindowGameObject;

        public static RedirectCallsState state1;
        public static RedirectCallsState state2;
        public static RedirectCallsState state3;
        public static RedirectCallsState state4;
        public static RedirectCallsState state5;
        public static RedirectCallsState state6;
        public static RedirectCallsState state7;
        public static RedirectCallsState state8;
        public static RedirectCallsState state9;

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
                    Detour();
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
            Loader.isGuiRunning = true;
        }

        public static void RemoveGui()
        {
            Loader.isGuiRunning = false;
            if (playerbuildingInfo != null)
            {
                UnityEngine.Object.Destroy(PBMenuPanel);
                Loader.PBMenuPanel = null;
            }

            //remove PlayerbuildingUI
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
        }

        public static void SetupPlayerBuidingGui()
        {
            PlayerbuildingWindowGameObject = new GameObject("PlayerbuildingWindowGameObject");
            guiPanel4 = (PlayerBuildingUI)PlayerbuildingWindowGameObject.AddComponent(typeof(PlayerBuildingUI));


            playerbuildingInfo = UIView.Find<UIPanel>("(Library) CityServiceWorldInfoPanel");
            if (playerbuildingInfo == null)
            {
                DebugLog.LogToFileOnly("UIPanel not found (update broke the mod!): (Library) CityServiceWorldInfoPanel\nAvailable panels are:\n");
            }
            guiPanel4.transform.parent = playerbuildingInfo.transform;
            guiPanel4.size = new Vector3(playerbuildingInfo.size.x, playerbuildingInfo.size.y);
            guiPanel4.baseBuildingWindow = playerbuildingInfo.gameObject.transform.GetComponentInChildren<CityServiceWorldInfoPanel>();
            guiPanel4.position = new Vector3(playerbuildingInfo.size.x, playerbuildingInfo.size.y);
            playerbuildingInfo.eventVisibilityChanged += playerbuildingInfo_eventVisibilityChanged;
        }


        public static void SetupPlayerBuildingButton()
        {
            if (PBMenuPanel == null)
            {
                PBMenuPanel = (playerbuildingInfo.AddUIComponent(typeof(PlayerBuildingButton)) as PlayerBuildingButton);
            }
            PBMenuPanel.RefPanel = playerbuildingInfo;
            PBMenuPanel.Alignment = UIAlignAnchor.TopLeft;
            PBMenuPanel.Show();
        }

        public static void playerbuildingInfo_eventVisibilityChanged(UIComponent component, bool value)
        {
            guiPanel4.isEnabled = value;
            if (value)
            {
                Loader.guiPanel4.transform.parent = Loader.playerbuildingInfo.transform;
                Loader.guiPanel4.size = new Vector3(Loader.playerbuildingInfo.size.x, Loader.playerbuildingInfo.size.y);
                Loader.guiPanel4.baseBuildingWindow = Loader.playerbuildingInfo.gameObject.transform.GetComponentInChildren<CityServiceWorldInfoPanel>();
                Loader.guiPanel4.position = new Vector3(Loader.playerbuildingInfo.size.x, Loader.playerbuildingInfo.size.y);
                //DebugLog.LogToFileOnly("select building found!!!!!:\n");
                //comm_data.current_buildingid = 0;
                //PlayerBuildingUI.refesh_once = true;
                //guiPanel4.Show();
            }
            else
            {
                //comm_data.current_buildingid = 0;
                guiPanel4.Hide();
            }
        }


        public void Detour()
        {
            var srcMethod1 = typeof(PlayerBuildingAI).GetMethod("GetConstructionTime", BindingFlags.NonPublic | BindingFlags.Instance);
            var destMethod1 = typeof(CustomPlayerBuildingAI).GetMethod("GetConstructionTime", BindingFlags.NonPublic | BindingFlags.Instance);
            state1 = RedirectionHelper.RedirectCalls(srcMethod1, destMethod1);

            var srcMethod5 = typeof(TransferManager).GetMethod("GetFrameReason", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            var destMethod5 = typeof(CustomTransferManager).GetMethod("GetFrameReason", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            state5 = RedirectionHelper.RedirectCalls(srcMethod5, destMethod5);

            var srcMethod6 = typeof(CargoTruckAI).GetMethod("GetLocalizedStatus", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(InstanceID).MakeByRefType() }, null);
            var destMethod6 = typeof(CustomCargoTruckAI).GetMethod("GetLocalizedStatus", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(InstanceID).MakeByRefType() }, null);
            state6 = RedirectionHelper.RedirectCalls(srcMethod6, destMethod6);

            var srcMethod7 = typeof(PlayerBuildingAI).GetMethod("GetBudget", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType() }, null);
            var destMethod7 = typeof(CustomPlayerBuildingAI).GetMethod("CustomGetBudget", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType() }, null);
            state7 = RedirectionHelper.RedirectCalls(srcMethod7, destMethod7);

            var srcMethod9 = typeof(BuildingAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType() , typeof(Building.Frame).MakeByRefType() }, null);
            var destMethod9 = typeof(CustomCommonBuildingAI).GetMethod("CustomSimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Building.Frame).MakeByRefType() }, null);
            state9 = RedirectionHelper.RedirectCalls(srcMethod9, destMethod9);

            realCityRunning = CheckRealCityIsLoaded();
            fuelAlarmRunning = CheckFuelAlarmIsLoaded();
            if (!realCityRunning)
            {
                var srcMethod2 = typeof(CargoTruckAI).GetMethod("ArriveAtTarget", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null);
                var destMethod2 = typeof(CustomCargoTruckAI).GetMethod("ArriveAtTarget", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null);
                state2 = RedirectionHelper.RedirectCalls(srcMethod2, destMethod2);
                var srcMethod3 = typeof(CargoTruckAI).GetMethod("SetSource", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(ushort) }, null);
                var destMethod3 = typeof(CustomCargoTruckAI).GetMethod("SetSource", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(ushort) }, null);
                state3 = RedirectionHelper.RedirectCalls(srcMethod3, destMethod3);
                var srcMethod4 = typeof(TransferManager).GetMethod("StartTransfer", BindingFlags.NonPublic | BindingFlags.Instance);
                var destMethod4 = typeof(CustomTransferManager).GetMethod("StartTransfer", BindingFlags.NonPublic | BindingFlags.Instance);
                state4 = RedirectionHelper.RedirectCalls(srcMethod4, destMethod4);
                var srcMethod8 = typeof(CommonBuildingAI).GetMethod("ReleaseBuilding", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType() }, null);
                var destMethod8 = typeof(CustomCommonBuildingAI).GetMethod("ReleaseBuilding", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType() }, null);
                state8 = RedirectionHelper.RedirectCalls(srcMethod8, destMethod8);
            }
            else
            {
                DebugLog.LogToFileOnly("realCity is Running");
            }
        }

        public void RevertDetour()
        {
            var srcMethod1 = typeof(PlayerBuildingAI).GetMethod("GetConstructionTime", BindingFlags.NonPublic | BindingFlags.Instance);
            RedirectionHelper.RevertRedirect(srcMethod1, state1);
            var srcMethod5 = typeof(TransferManager).GetMethod("GetFrameReason", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            RedirectionHelper.RevertRedirect(srcMethod5, state5);
            var srcMethod6 = typeof(CargoTruckAI).GetMethod("GetLocalizedStatus", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(InstanceID).MakeByRefType() }, null);
            RedirectionHelper.RevertRedirect(srcMethod6, state6);
            var srcMethod7 = typeof(PlayerBuildingAI).GetMethod("GetBudget", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType() }, null);
            RedirectionHelper.RevertRedirect(srcMethod7, state7);

            var srcMethod9 = typeof(BuildingAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Building.Frame).MakeByRefType() }, null);
            RedirectionHelper.RevertRedirect(srcMethod9, state9);

            if (!realCityRunning)
            {
                var srcMethod2 = typeof(CargoTruckAI).GetMethod("ArriveAtTarget", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null);
                RedirectionHelper.RevertRedirect(srcMethod2, state2);
                var srcMethod3 = typeof(CargoTruckAI).GetMethod("SetSource", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(ushort) }, null);
                RedirectionHelper.RevertRedirect(srcMethod3, state3);
                var srcMethod4 = typeof(TransferManager).GetMethod("StartTransfer", BindingFlags.NonPublic | BindingFlags.Instance);
                RedirectionHelper.RevertRedirect(srcMethod4, state4);
                var srcMethod8 = typeof(CommonBuildingAI).GetMethod("ReleaseBuilding", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType() }, null);
                RedirectionHelper.RevertRedirect(srcMethod8, state8);
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

        private bool CheckFuelAlarmIsLoaded()
        {
            return this.Check3rdPartyModLoaded("FuelAlarm", true);
        }

    }
}
