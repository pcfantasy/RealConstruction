﻿using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;
using ColossalFramework;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using RealConstruction.Util;

namespace RealConstruction.UI
{
    public class WareHouseUI : UIPanel
    {
        public static readonly string cacheName = "WareHouseUI";

        private static readonly float SPACING = 15f;

        private Dictionary<string, UILabel> _valuesControlContainer = new Dictionary<string, UILabel>(16);

        public WarehouseWorldInfoPanel baseBuildingWindow;

        public static bool refeshOnce = false;

        private UILabel Operation1;

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
            this.Operation1 = base.AddUIComponent<UILabel>();
            this.Operation1.text = Language.Strings[8];
            this.Operation1.relativePosition = new Vector3(SPACING, 50f);
            this.Operation1.autoSize = true;
        }

        private void RefreshDisplayData()
        {
            uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
            uint num2 = currentFrameIndex & 255u;

            if (refeshOnce || (MainDataStore.last_buildingid != WorldInfoPanel.GetCurrentInstanceID().Building))
            {
                if (base.isVisible)
                {
                    MainDataStore.last_buildingid = WorldInfoPanel.GetCurrentInstanceID().Building;
                    Building buildingData = Singleton<BuildingManager>.instance.m_buildings.m_buffer[MainDataStore.last_buildingid];
                    this.Operation1.text = string.Format(Language.Strings[8] + " [{0}]", MainDataStore.operationResourceBuffer[MainDataStore.last_buildingid]);
                    refeshOnce = false;
                    this.BringToFront();

                    if (!RealConstructionThreading.canOperation(MainDataStore.last_buildingid, ref buildingData) && !RealConstructionThreading.IsSpecialBuilding(MainDataStore.last_buildingid))
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