using System.Collections.Generic;
using ColossalFramework.UI;
using UnityEngine;
using ColossalFramework;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using RealConstruction.Util;
using RealConstruction.CustomAI;

namespace RealConstruction.UI
{
    public class PlayerBuildingUI : UIPanel
    {
        public static readonly string cacheName = "PlayerBuildingUI";
        private static readonly float SPACING = 15f;
        private static readonly float SPACING22 = 22f;
        private Dictionary<string, UILabel> _valuesControlContainer = new Dictionary<string, UILabel>(16);
        public CityServiceWorldInfoPanel baseBuildingWindow;
        public static bool refeshOnce = false;
        private UILabel food;
        private UILabel lumber;
        private UILabel coal;
        private UILabel petrol;
        private UILabel constructionResource;
        private UILabel operationResource;
        public static UICheckBox both;
        public static UICheckBox construction;
        public static UICheckBox operation;
        private UILabel bothText;
        private UILabel constructionText;
        private UILabel operationText;

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
            this.food = base.AddUIComponent<UILabel>();
            this.food.text = Localization.Get("FOOD_STORED");
            this.food.relativePosition = new Vector3(SPACING, 50f);
            this.food.autoSize = true;

            this.lumber = base.AddUIComponent<UILabel>();
            this.lumber.text = Localization.Get("LUMBER_STORED");
            this.lumber.relativePosition = new Vector3(SPACING, this.food.relativePosition.y + SPACING22);
            this.lumber.autoSize = true;

            this.coal = base.AddUIComponent<UILabel>();
            this.coal.text = Localization.Get("COAL_STORED");
            this.coal.relativePosition = new Vector3(SPACING, this.lumber.relativePosition.y + SPACING22);
            this.coal.autoSize = true;

            this.petrol = base.AddUIComponent<UILabel>();
            this.petrol.text = Localization.Get("PETROL_STORED");
            this.petrol.relativePosition = new Vector3(SPACING, this.coal.relativePosition.y + SPACING22);
            this.petrol.autoSize = true;

            this.constructionResource = base.AddUIComponent<UILabel>();
            this.constructionResource.text = Localization.Get("CONSTRUCTION_RESOURCE");
            this.constructionResource.relativePosition = new Vector3(SPACING, this.petrol.relativePosition.y + SPACING22);
            this.constructionResource.autoSize = true;

            this.operationResource = base.AddUIComponent<UILabel>();
            this.operationResource.text = Localization.Get("OPERATION_RESOURCE");
            this.operationResource.relativePosition = new Vector3(SPACING, this.constructionResource.relativePosition.y + SPACING22);
            this.operationResource.autoSize = true;

            both = base.AddUIComponent<UICheckBox>();
            both.relativePosition = new Vector3(15f, operationResource.relativePosition.y + 20f);
            this.bothText = base.AddUIComponent<UILabel>();
            this.bothText.relativePosition = new Vector3(both.relativePosition.x + both.width + 20f, both.relativePosition.y + 5f);
            both.height = 16f;
            both.width = 16f;
            both.label = this.bothText;
            both.text = Localization.Get("GENERATE_BOTH_RESOURCES");
            UISprite uISprite0 = both.AddUIComponent<UISprite>();
            uISprite0.height = 20f;
            uISprite0.width = 20f;
            uISprite0.relativePosition = new Vector3(0f, 0f);
            uISprite0.spriteName = "check-unchecked";
            uISprite0.isVisible = true;
            UISprite uISprite1 = both.AddUIComponent<UISprite>();
            uISprite1.height = 20f;
            uISprite1.width = 20f;
            uISprite1.relativePosition = new Vector3(0f, 0f);
            uISprite1.spriteName = "check-checked";
            both.checkedBoxObject = uISprite1;
            both.isChecked = (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 0) ? true : false;
            both.isEnabled = true;
            both.isVisible = true;
            both.canFocus = true;
            both.isInteractive = true;
            both.eventCheckChanged += delegate (UIComponent component, bool eventParam)
            {
                Both_OnCheckChanged(component, eventParam);
            };

            construction = base.AddUIComponent<UICheckBox>();
            construction.relativePosition = new Vector3(15f, both.relativePosition.y + 20f);
            this.constructionText = base.AddUIComponent<UILabel>();
            this.constructionText.relativePosition = new Vector3(construction.relativePosition.x + construction.width + 20f, construction.relativePosition.y + 5f);
            construction.height = 16f;
            construction.width = 16f;
            construction.label = this.constructionText;
            construction.text = Localization.Get("GENERATE_CONSTRUCTION_RESOURCES");
            UISprite uISprite2 = construction.AddUIComponent<UISprite>();
            uISprite2.height = 20f;
            uISprite2.width = 20f;
            uISprite2.relativePosition = new Vector3(0f, 0f);
            uISprite2.spriteName = "check-unchecked";
            uISprite2.isVisible = true;
            UISprite uISprite3 = construction.AddUIComponent<UISprite>();
            uISprite3.height = 20f;
            uISprite3.width = 20f;
            uISprite3.relativePosition = new Vector3(0f, 0f);
            uISprite3.spriteName = "check-checked";
            construction.checkedBoxObject = uISprite3;
            construction.isChecked = (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 1) ? true : false;
            construction.isEnabled = true;
            construction.isVisible = true;
            construction.canFocus = true;
            construction.isInteractive = true;
            construction.eventCheckChanged += delegate (UIComponent component, bool eventParam)
            {
                Construction_OnCheckChanged(component, eventParam);
            };

            operation = base.AddUIComponent<UICheckBox>();
            operation.relativePosition = new Vector3(15f, construction.relativePosition.y + 20f);
            this.operationText = base.AddUIComponent<UILabel>();
            this.operationText.relativePosition = new Vector3(operation.relativePosition.x + operation.width + 20f, operation.relativePosition.y + 5f);
            operation.height = 16f;
            operation.width = 16f;
            operation.label = this.operationText;
            operation.text = Localization.Get("GENERATE_OPERATION_RESOURCES");
            UISprite uISprite4 = operation.AddUIComponent<UISprite>();
            uISprite4.height = 20f;
            uISprite4.width = 20f;
            uISprite4.relativePosition = new Vector3(0f, 0f);
            uISprite4.spriteName = "check-unchecked";
            uISprite4.isVisible = true;
            UISprite uISprite5 = operation.AddUIComponent<UISprite>();
            uISprite5.height = 20f;
            uISprite5.width = 20f;
            uISprite5.relativePosition = new Vector3(0f, 0f);
            uISprite5.spriteName = "check-checked";
            operation.checkedBoxObject = uISprite5;
            operation.isChecked = (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 2) ? true : false;
            operation.isEnabled = true;
            operation.isVisible = true;
            operation.canFocus = true;
            operation.isInteractive = true;
            operation.eventCheckChanged += delegate (UIComponent component, bool eventParam)
            {
                Operation_OnCheckChanged(component, eventParam);
            };
        }

        public static void Both_OnCheckChanged(UIComponent UIComp, bool bValue)
        {
            MainDataStore.lastBuildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
            if (bValue)
            {
                MainDataStore.resourceCategory[MainDataStore.lastBuildingID] = 0;
                operation.isChecked = false;
                construction.isChecked = false;
                both.isChecked = true;
            }
            else
            {
                if (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 0)
                {
                    both.isChecked = true;
                    operation.isChecked = false;
                    construction.isChecked = false;
                }
                else if (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 1)
                {
                    both.isChecked = false;
                    operation.isChecked = false;
                    construction.isChecked = true;
                }
                else if (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 2)
                {
                    both.isChecked = false;
                    operation.isChecked = true;
                    construction.isChecked = false;
                }
            }
        }

        public static void Construction_OnCheckChanged(UIComponent UIComp, bool bValue)
        {
            MainDataStore.lastBuildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
            if (bValue)
            {
                MainDataStore.resourceCategory[MainDataStore.lastBuildingID] = 1;
                operation.isChecked = false;
                construction.isChecked = true;
                both.isChecked = false;
            }
            else
            {
                if (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 0)
                {
                    both.isChecked = true;
                    operation.isChecked = false;
                    construction.isChecked = false;
                }
                else if (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 1)
                {
                    both.isChecked = false;
                    operation.isChecked = false;
                    construction.isChecked = true;
                }
                else if (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 2)
                {
                    both.isChecked = false;
                    operation.isChecked = true;
                    construction.isChecked = false;
                }
            }
        }

        public static void Operation_OnCheckChanged(UIComponent UIComp, bool bValue)
        {
            MainDataStore.lastBuildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
            if (bValue)
            {
                MainDataStore.resourceCategory[MainDataStore.lastBuildingID] = 2;
                operation.isChecked = true;
                construction.isChecked = false;
                both.isChecked = false;
            }
            else
            {
                if (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 0)
                {
                    both.isChecked = true;
                    operation.isChecked = false;
                    construction.isChecked = false;
                }
                else if (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 1)
                {
                    both.isChecked = false;
                    operation.isChecked = false;
                    construction.isChecked = true;
                }
                else if (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 2)
                {
                    both.isChecked = false;
                    operation.isChecked = true;
                    construction.isChecked = false;
                }
            }
        }

        private void RefreshDisplayData()
        {
            uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
            uint num2 = currentFrameIndex & 255u;

            if (PlayerBuildingUI.refeshOnce || (MainDataStore.lastBuildingID != WorldInfoPanel.GetCurrentInstanceID().Building))
            {
                if (base.isVisible)
                {
                    MainDataStore.lastBuildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
                    Building buildingData = Singleton<BuildingManager>.instance.m_buildings.m_buffer[MainDataStore.lastBuildingID];

                    if (RealConstructionThreading.IsSpecialBuilding(MainDataStore.lastBuildingID) == true)
                    {
                        this.food.text = string.Format(Localization.Get("FOOD_STORED") + " [{0}]", MainDataStore.foodBuffer[MainDataStore.lastBuildingID]);
                        this.lumber.text = string.Format(Localization.Get("LUMBER_STORED") + " [{0}]", MainDataStore.lumberBuffer[MainDataStore.lastBuildingID]);
                        this.coal.text = string.Format(Localization.Get("COAL_STORED") + " [{0}]", MainDataStore.coalBuffer[MainDataStore.lastBuildingID]);
                        this.petrol.text = string.Format(Localization.Get("PETROL_STORED") + " [{0}]", MainDataStore.petrolBuffer[MainDataStore.lastBuildingID]);
                        this.operationResource.text = string.Format(Localization.Get("OPERATION_RESOURCE") + " [{0}]", MainDataStore.operationResourceBuffer[MainDataStore.lastBuildingID]);
                        this.constructionResource.text = string.Format(Localization.Get("CONSTRUCTION_RESOURCE") + " [{0}]", MainDataStore.constructionResourceBuffer[MainDataStore.lastBuildingID]);
                        both.isChecked = (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 0) ? true : false;
                        construction.isChecked = (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 1) ? true : false;
                        operation.isChecked = (MainDataStore.resourceCategory[MainDataStore.lastBuildingID] == 2) ? true : false;
                        both.isVisible = true;
                        construction.isVisible = true;
                        operation.isVisible = true;
                        both.text = Localization.Get("GENERATE_BOTH_RESOURCES");
                        construction.text = Localization.Get("CONSTRUCTION_RESOURCE");
                        operation.text = Localization.Get("OPERATION_RESOURCE");
                    }
                    else
                    {
                        this.food.text = "";
                        this.lumber.text = "";
                        this.coal.text = "";
                        this.petrol.text = "";
                        this.constructionResource.text = "";
                        this.operationResource.text = string.Format(Localization.Get("OPERATION_RESOURCE_LEFT") + " [{0}]", MainDataStore.operationResourceBuffer[MainDataStore.lastBuildingID]);
                        both.isVisible = false;
                        construction.isVisible = false;
                        operation.isVisible = false;
                        both.text = "";
                        construction.text = "";
                        operation.text = "";
                    }
                    PlayerBuildingUI.refeshOnce = false;
                    this.BringToFront();

                    if (!CustomPlayerBuildingAI.CanOperation(MainDataStore.lastBuildingID, ref buildingData) && !RealConstructionThreading.IsSpecialBuilding(MainDataStore.lastBuildingID))
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