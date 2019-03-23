using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;
using ColossalFramework;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using RealConstruction.Util;
using RealConstruction.CustomAI;
using RealConstruction.NewAI;

namespace RealConstruction.UI
{
    public class WareHouseUI : UIPanel
    {
        public static readonly string cacheName = "WareHouseUI";

        private static readonly float SPACING = 15f;
        private Dictionary<string, UILabel> _valuesControlContainer = new Dictionary<string, UILabel>(16);
        public WarehouseWorldInfoPanel baseBuildingWindow;
        public static bool refeshOnce = false;
        private UILabel operationResourceLeft;

        public override void Update()
        {
            this.RefreshDisplayData();
            base.Update();
        }

        public override void Awake()
        {
            base.Awake();
            this.DoOnStartup();
        }

        public override void Start()
        {
            base.Start();
            this.canFocus = true;
            this.isInteractive = true;
            base.isVisible = true;
            this.BringToFront();
            base.opacity = 1f;
            base.cachedName = cacheName;
            this.RefreshDisplayData();
            base.Hide();
        }

        private void DoOnStartup()
        {
            this.ShowOnGui();
            base.Hide();
        }

        private void ShowOnGui()
        {
            this.operationResourceLeft = base.AddUIComponent<UILabel>();
            this.operationResourceLeft.text = Localization.Get("OPERATION_RESOURCE_LEFT");
            this.operationResourceLeft.relativePosition = new Vector3(SPACING, 50f);
            this.operationResourceLeft.autoSize = true;
        }

        private void RefreshDisplayData()
        {
            uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
            uint num2 = currentFrameIndex & 255u;

            if (refeshOnce || (MainDataStore.lastBuildingID != WorldInfoPanel.GetCurrentInstanceID().Building))
            {
                if (base.isVisible)
                {
                    MainDataStore.lastBuildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
                    Building buildingData = Singleton<BuildingManager>.instance.m_buildings.m_buffer[MainDataStore.lastBuildingID];
                    this.operationResourceLeft.text = string.Format(Localization.Get("OPERATION_RESOURCE_LEFT") + " [{0}]", MainDataStore.operationResourceBuffer[MainDataStore.lastBuildingID]);
                    refeshOnce = false;
                    this.BringToFront();

                    if (!CustomPlayerBuildingAI.CanOperation(MainDataStore.lastBuildingID, ref buildingData) && !ResourceBuildingAI.IsSpecialBuilding(MainDataStore.lastBuildingID))
                    {
                        this.Hide();
                    }
                }
                else
                {
                    this.Hide();
                }
            }

        }

    }
}