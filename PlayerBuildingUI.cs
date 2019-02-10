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
        private UILabel Construction1;
        private UILabel Operation1;
        public static UICheckBox Both;
        public static UICheckBox Construction;
        public static UICheckBox Operation;

        private UILabel BothText;
        private UILabel ConstructionText;
        private UILabel OperationText;
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

            this.Construction1 = base.AddUIComponent<UILabel>();
            this.Construction1.text = Language.Strings[6];
            this.Construction1.relativePosition = new Vector3(SPACING, this.Petrol.relativePosition.y + SPACING22);
            this.Construction1.autoSize = true;

            this.Operation1 = base.AddUIComponent<UILabel>();
            this.Operation1.text = Language.Strings[7];
            this.Operation1.relativePosition = new Vector3(SPACING, this.Construction1.relativePosition.y + SPACING22);
            this.Operation1.autoSize = true;

            Both = base.AddUIComponent<UICheckBox>();
            Both.relativePosition = new Vector3(15f, Operation1.relativePosition.y + 20f);
            this.BothText = base.AddUIComponent<UILabel>();
            this.BothText.relativePosition = new Vector3(Both.relativePosition.x + Both.width + 20f, Both.relativePosition.y + 5f);
            Both.height = 16f;
            Both.width = 16f;
            Both.label = this.BothText;
            Both.text = Language.Strings[13];
            UISprite uISprite0 = Both.AddUIComponent<UISprite>();
            uISprite0.height = 20f;
            uISprite0.width = 20f;
            uISprite0.relativePosition = new Vector3(0f, 0f);
            uISprite0.spriteName = "check-unchecked";
            uISprite0.isVisible = true;
            UISprite uISprite1 = Both.AddUIComponent<UISprite>();
            uISprite1.height = 20f;
            uISprite1.width = 20f;
            uISprite1.relativePosition = new Vector3(0f, 0f);
            uISprite1.spriteName = "check-checked";
            Both.checkedBoxObject = uISprite1;
            Both.isChecked = (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 0) ? true : false;
            Both.isEnabled = true;
            Both.isVisible = true;
            Both.canFocus = true;
            Both.isInteractive = true;
            Both.eventCheckChanged += delegate (UIComponent component, bool eventParam)
            {
                Both_OnCheckChanged(component, eventParam);
            };

            Construction = base.AddUIComponent<UICheckBox>();
            Construction.relativePosition = new Vector3(15f, Both.relativePosition.y + 20f);
            this.ConstructionText = base.AddUIComponent<UILabel>();
            this.ConstructionText.relativePosition = new Vector3(Construction.relativePosition.x + Construction.width + 20f, Construction.relativePosition.y + 5f);
            Construction.height = 16f;
            Construction.width = 16f;
            Construction.label = this.ConstructionText;
            Construction.text = Language.Strings[14];
            UISprite uISprite2 = Construction.AddUIComponent<UISprite>();
            uISprite2.height = 20f;
            uISprite2.width = 20f;
            uISprite2.relativePosition = new Vector3(0f, 0f);
            uISprite2.spriteName = "check-unchecked";
            uISprite2.isVisible = true;
            UISprite uISprite3 = Construction.AddUIComponent<UISprite>();
            uISprite3.height = 20f;
            uISprite3.width = 20f;
            uISprite3.relativePosition = new Vector3(0f, 0f);
            uISprite3.spriteName = "check-checked";
            Construction.checkedBoxObject = uISprite3;
            Construction.isChecked = (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 1) ? true : false;
            Construction.isEnabled = true;
            Construction.isVisible = true;
            Construction.canFocus = true;
            Construction.isInteractive = true;
            Construction.eventCheckChanged += delegate (UIComponent component, bool eventParam)
            {
                Construction_OnCheckChanged(component, eventParam);
            };

            Operation = base.AddUIComponent<UICheckBox>();
            Operation.relativePosition = new Vector3(15f, Construction.relativePosition.y + 20f);
            this.OperationText = base.AddUIComponent<UILabel>();
            this.OperationText.relativePosition = new Vector3(Operation.relativePosition.x + Operation.width + 20f, Operation.relativePosition.y + 5f);
            Operation.height = 16f;
            Operation.width = 16f;
            Operation.label = this.OperationText;
            Operation.text = Language.Strings[15];
            UISprite uISprite4 = Operation.AddUIComponent<UISprite>();
            uISprite4.height = 20f;
            uISprite4.width = 20f;
            uISprite4.relativePosition = new Vector3(0f, 0f);
            uISprite4.spriteName = "check-unchecked";
            uISprite4.isVisible = true;
            UISprite uISprite5 = Operation.AddUIComponent<UISprite>();
            uISprite5.height = 20f;
            uISprite5.width = 20f;
            uISprite5.relativePosition = new Vector3(0f, 0f);
            uISprite5.spriteName = "check-checked";
            Operation.checkedBoxObject = uISprite5;
            Operation.isChecked = (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 2) ? true : false;
            Operation.isEnabled = true;
            Operation.isVisible = true;
            Operation.canFocus = true;
            Operation.isInteractive = true;
            Operation.eventCheckChanged += delegate (UIComponent component, bool eventParam)
            {
                Operation_OnCheckChanged(component, eventParam);
            };
        }


        public static void Both_OnCheckChanged(UIComponent UIComp, bool bValue)
        {
            MainDataStore.last_buildingid = WorldInfoPanel.GetCurrentInstanceID().Building;
            if (bValue)
            {
                MainDataStore.buildingFlag1[MainDataStore.last_buildingid] = 0;
                Operation.isChecked = false;
                Construction.isChecked = false;
                Both.isChecked = true;
            }
            else
            {
                if (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 0)
                {
                    Both.isChecked = true;
                    Operation.isChecked = false;
                    Construction.isChecked = false;
                }
                else if (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 1)
                {
                    Both.isChecked = false;
                    Operation.isChecked = false;
                    Construction.isChecked = true;
                }
                else if (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 2)
                {
                    Both.isChecked = false;
                    Operation.isChecked = true;
                    Construction.isChecked = false;
                }
            }
        }

        public static void Construction_OnCheckChanged(UIComponent UIComp, bool bValue)
        {
            MainDataStore.last_buildingid = WorldInfoPanel.GetCurrentInstanceID().Building;
            if (bValue)
            {
                MainDataStore.buildingFlag1[MainDataStore.last_buildingid] = 1;
                Operation.isChecked = false;
                Construction.isChecked = true;
                Both.isChecked = false;
            }
            else
            {
                if (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 0)
                {
                    Both.isChecked = true;
                    Operation.isChecked = false;
                    Construction.isChecked = false;
                }
                else if (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 1)
                {
                    Both.isChecked = false;
                    Operation.isChecked = false;
                    Construction.isChecked = true;
                }
                else if (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 2)
                {
                    Both.isChecked = false;
                    Operation.isChecked = true;
                    Construction.isChecked = false;
                }
            }
        }

        public static void Operation_OnCheckChanged(UIComponent UIComp, bool bValue)
        {
            MainDataStore.last_buildingid = WorldInfoPanel.GetCurrentInstanceID().Building;
            if (bValue)
            {
                MainDataStore.buildingFlag1[MainDataStore.last_buildingid] = 2;
                Operation.isChecked = true;
                Construction.isChecked = false;
                Both.isChecked = false;
            }
            else
            {
                if (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 0)
                {
                    Both.isChecked = true;
                    Operation.isChecked = false;
                    Construction.isChecked = false;
                }
                else if (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 1)
                {
                    Both.isChecked = false;
                    Operation.isChecked = false;
                    Construction.isChecked = true;
                }
                else if (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 2)
                {
                    Both.isChecked = false;
                    Operation.isChecked = true;
                    Construction.isChecked = false;
                }
            }
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
                        this.Operation1.text = string.Format(Language.Strings[7] + " [{0}]", MainDataStore.operationResourceBuffer[MainDataStore.last_buildingid]);
                        this.Construction1.text = string.Format(Language.Strings[6] + " [{0}]", MainDataStore.constructionResourceBuffer[MainDataStore.last_buildingid]);
                        Both.isChecked = (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 0) ? true : false;
                        Construction.isChecked = (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 1) ? true : false;
                        Operation.isChecked = (MainDataStore.buildingFlag1[MainDataStore.last_buildingid] == 2) ? true : false;
                        Both.isVisible = true;
                        Construction.isVisible = true;
                        Operation.isVisible = true;
                        Both.text = Language.Strings[13];
                        Construction.text = Language.Strings[14];
                        Operation.text = Language.Strings[15];
                    }
                    else
                    {
                        this.Food.text = "";
                        this.Lumber.text = "";
                        this.Coal.text = "";
                        this.Petrol.text = "";
                        this.Construction1.text = "";
                        this.Operation1.text = string.Format(Language.Strings[8] + " [{0}]", MainDataStore.operationResourceBuffer[MainDataStore.last_buildingid]);
                        Both.isVisible = false;
                        Construction.isVisible = false;
                        Operation.isVisible = false;
                        Both.text = "";
                        Construction.text = "";
                        Operation.text = "";
                    }
                    PlayerBuildingUI.refeshOnce = false;
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