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
    public class UniqueFactoryUI : UIPanel
    {
        public static readonly string cacheName = "UniqueFactoryUI";
        private static readonly float SPACING = 15f;
        private static readonly float SPACING22 = 22f;
        private Dictionary<string, UILabel> _valuesControlContainer = new Dictionary<string, UILabel>(16);
        public UniqueFactoryWorldInfoPanel baseBuildingWindow;
        public static bool refeshOnce = false;
        private UILabel food;
        private UILabel lumber;
        private UILabel coal;
        private UILabel petrol;
        private UILabel constructionResource;
        private UILabel operationResource;
        private UILabel buildingType;
        private UIDropDown buildingTypeDD;
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

            buildingType = AddUIComponent<UILabel>();
            buildingType.text = Localization.Get("BUILDING_TYPE");
            buildingType.relativePosition = new Vector3(15f, operationResource.relativePosition.y + 20f);
            buildingType.autoSize = true;

            buildingTypeDD = UIUtil.CreateDropDown(this);
            buildingTypeDD.items = new string[] { Localization.Get("NORMAL_BUILDING"), Localization.Get("GENERATE_BOTH_RESOURCES"), Localization.Get("GENERATE_CONSTRUCTION_RESOURCES"), Localization.Get("GENERATE_OPERATION_RESOURCES"), Localization.Get("NONEED_RESOURCE") };
            buildingTypeDD.selectedIndex = MainDataStore.resourceCategory[MainDataStore.lastBuildingID];
            buildingTypeDD.size = new Vector2(250f, 25f);
            buildingTypeDD.relativePosition = new Vector3(15f, buildingType.relativePosition.y + 20f);
            buildingTypeDD.eventSelectedIndexChanged += delegate (UIComponent c, int sel)
            {
                MainDataStore.resourceCategory[MainDataStore.lastBuildingID] = (byte)sel;
            };
        }

        private void RefreshDisplayData()
        {
            if (refeshOnce || (MainDataStore.lastBuildingID != WorldInfoPanel.GetCurrentInstanceID().Building))
            {
                if (base.isVisible)
                {
                    MainDataStore.lastBuildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
                    Building buildingData = Singleton<BuildingManager>.instance.m_buildings.m_buffer[MainDataStore.lastBuildingID];

                    if (ResourceBuildingAI.IsSpecialBuilding(MainDataStore.lastBuildingID) == true)
                    {
                        this.food.text = string.Format(Localization.Get("FOOD_STORED") + " [{0}]", MainDataStore.foodBuffer[MainDataStore.lastBuildingID]);
                        this.lumber.text = string.Format(Localization.Get("LUMBER_STORED") + " [{0}]", MainDataStore.lumberBuffer[MainDataStore.lastBuildingID]);
                        this.coal.text = string.Format(Localization.Get("COAL_STORED") + " [{0}]", MainDataStore.coalBuffer[MainDataStore.lastBuildingID]);
                        this.petrol.text = string.Format(Localization.Get("PETROL_STORED") + " [{0}]", MainDataStore.petrolBuffer[MainDataStore.lastBuildingID]);
                        this.operationResource.text = string.Format(Localization.Get("OPERATION_RESOURCE") + " [{0}]", MainDataStore.operationResourceBuffer[MainDataStore.lastBuildingID]);
                        this.constructionResource.text = string.Format(Localization.Get("CONSTRUCTION_RESOURCE") + " [{0}]", MainDataStore.constructionResourceBuffer[MainDataStore.lastBuildingID]);
                        buildingType.text = Localization.Get("BUILDING_TYPE");
                        buildingTypeDD.items = new string[] { Localization.Get("NORMAL_BUILDING"), Localization.Get("GENERATE_BOTH_RESOURCES"), Localization.Get("GENERATE_CONSTRUCTION_RESOURCES"), Localization.Get("GENERATE_OPERATION_RESOURCES"), Localization.Get("NONEED_RESOURCE") };
                        if (buildingTypeDD.selectedIndex != MainDataStore.resourceCategory[MainDataStore.lastBuildingID])
                            buildingTypeDD.selectedIndex = MainDataStore.resourceCategory[MainDataStore.lastBuildingID];
                    }
                    else
                    {
                        this.food.text = "";
                        this.lumber.text = "";
                        this.coal.text = "";
                        this.petrol.text = "";
                        this.constructionResource.text = "";
                        this.operationResource.text = string.Format(Localization.Get("OPERATION_RESOURCE_LEFT") + " [{0}]", MainDataStore.operationResourceBuffer[MainDataStore.lastBuildingID]);
                        buildingType.text = Localization.Get("BUILDING_TYPE");
                        buildingTypeDD.items = new string[] { Localization.Get("NORMAL_BUILDING"), Localization.Get("GENERATE_BOTH_RESOURCES"), Localization.Get("GENERATE_CONSTRUCTION_RESOURCES"), Localization.Get("GENERATE_OPERATION_RESOURCES"), Localization.Get("NONEED_RESOURCE") };
                        if (buildingTypeDD.selectedIndex != MainDataStore.resourceCategory[MainDataStore.lastBuildingID])
                            buildingTypeDD.selectedIndex = MainDataStore.resourceCategory[MainDataStore.lastBuildingID];
                    }
                    refeshOnce = false;
                    this.BringToFront();

                    if (!CustomPlayerBuildingAI.CanOperation(MainDataStore.lastBuildingID, ref buildingData, false) && !ResourceBuildingAI.IsSpecialBuilding(MainDataStore.lastBuildingID))
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