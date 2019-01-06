using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;
using ColossalFramework;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace RealConstruction
{
    public class PlayerBuildingUI : UIPanel
    {
        public static readonly string cacheName = "PlayerBuildingUI";

        private static readonly float SPACING = 15f;

        private static readonly float SPACING22 = 22f;

        private Dictionary<string, UILabel> _valuesControlContainer = new Dictionary<string, UILabel>(16);

        public CityServiceWorldInfoPanel baseBuildingWindow;

        public static bool refeshOnce = false;

        //1、citizen tax income
        private UILabel Food;
        private UILabel Lumber;
        private UILabel Coal;
        private UILabel Petrol;
        private UILabel Construction;
        private UILabel Operation;
        //private UILabel alivevisitcount;

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
            //base.backgroundSprite = "MenuPanel";
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
            this.Food = base.AddUIComponent<UILabel>();
            this.Food.text = Language.Strings[2];
            this.Food.relativePosition = new Vector3(SPACING, 50f);
            this.Food.autoSize = true;

            this.Lumber = base.AddUIComponent<UILabel>();
            this.Lumber.text = Language.Strings[3];
            this.Lumber.relativePosition = new Vector3(SPACING, this.Food.relativePosition.y + SPACING22);
            this.Lumber.autoSize = true;

            this.Coal = base.AddUIComponent<UILabel>();
            this.Coal.text = Language.Strings[4];
            this.Coal.relativePosition = new Vector3(SPACING, this.Lumber.relativePosition.y + SPACING22);
            this.Coal.autoSize = true;

            this.Petrol = base.AddUIComponent<UILabel>();
            this.Petrol.text = Language.Strings[5];
            this.Petrol.relativePosition = new Vector3(SPACING, this.Coal.relativePosition.y + SPACING22);
            this.Petrol.autoSize = true;

            this.Construction = base.AddUIComponent<UILabel>();
            this.Construction.text = Language.Strings[6];
            this.Construction.relativePosition = new Vector3(SPACING, this.Petrol.relativePosition.y + SPACING22);
            this.Construction.autoSize = true;

            this.Operation = base.AddUIComponent<UILabel>();
            this.Operation.text = Language.Strings[7];
            this.Operation.relativePosition = new Vector3(SPACING, this.Construction.relativePosition.y + SPACING22);
            this.Operation.autoSize = true;
        }

        private void RefreshDisplayData()
        {
            uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
            uint num2 = currentFrameIndex & 255u;

            if (PlayerBuildingUI.refeshOnce || (MainDataStore.last_buildingid != WorldInfoPanel.GetCurrentInstanceID().Building))
            {
                if (base.isVisible)
                {
                    MainDataStore.last_buildingid = WorldInfoPanel.GetCurrentInstanceID().Building;
                    Building buildingData = Singleton<BuildingManager>.instance.m_buildings.m_buffer[MainDataStore.last_buildingid];

                    if (RealConstructionThreading.IsSpecialBuilding(MainDataStore.last_buildingid) == true)
                    {
                        this.Food.text = string.Format(Language.Strings[2] + " [{0}]", MainDataStore.foodBuffer[MainDataStore.last_buildingid]);
                        this.Lumber.text = string.Format(Language.Strings[3] + " [{0}]", MainDataStore.lumberBuffer[MainDataStore.last_buildingid]);
                        this.Coal.text = string.Format(Language.Strings[4] + " [{0}]", MainDataStore.coalBuffer[MainDataStore.last_buildingid]);
                        this.Petrol.text = string.Format(Language.Strings[5] + " [{0}]", MainDataStore.petrolBuffer[MainDataStore.last_buildingid]);
                        this.Operation.text = string.Format(Language.Strings[7] + " [{0}]", MainDataStore.operationResourceBuffer[MainDataStore.last_buildingid]);
                        this.Construction.text = string.Format(Language.Strings[6] + " [{0}]", MainDataStore.constructionResourceBuffer[MainDataStore.last_buildingid]);
                    }
                    else
                    {
                        this.Food.text = "";
                        this.Lumber.text = "";
                        this.Coal.text = "";
                        this.Petrol.text = "";
                        this.Construction.text = "";
                        this.Operation.text = string.Format(Language.Strings[8] + " [{0}]", MainDataStore.operationResourceBuffer[MainDataStore.last_buildingid]);
                    }
                    PlayerBuildingUI.refeshOnce = false;
                    this.BringToFront();
                }
                else
                {
                    this.Hide();
                }
            }

        }

    }
}