using ColossalFramework;
using ColossalFramework.UI;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RealConstruction.UI
{
    public class WarehouseButton : UIButton
    {
        public static bool refeshOnce = false;
        private UIPanel playerBuildingInfo;
        private WareHouseUI wareHouseUI;
        private InstanceID BuildingID = InstanceID.Empty;
        public void PlayerBuildingUIToggle()
        {
            if (!wareHouseUI.isVisible && (BuildingID != InstanceID.Empty))
            {
                WareHouseUI.refeshOnce = true;
                wareHouseUI.position = new Vector3(playerBuildingInfo.size.x, playerBuildingInfo.size.y);
                wareHouseUI.size = new Vector3(playerBuildingInfo.size.x, playerBuildingInfo.size.y);
                wareHouseUI.Show();
            }
            else
            {
                wareHouseUI.Hide();
            }
        }

        public override void Start()
        {
            base.normalBgSprite = "ToolbarIconGroup1Nomarl";
            base.hoveredBgSprite = "ToolbarIconGroup1Hovered";
            base.focusedBgSprite = "ToolbarIconGroup1Focused";
            base.pressedBgSprite = "ToolbarIconGroup1Pressed";
            base.playAudioEvents = true;
            base.name = "PBButton";
            base.relativePosition = new Vector3(90f, 0f);
            UISprite internalSprite = base.AddUIComponent<UISprite>();
            internalSprite.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
            internalSprite.spriteName = "Pic";
            internalSprite.relativePosition = new Vector3(0, 0);
            internalSprite.width = internalSprite.height = 40f;
            base.size = new Vector2(40f, 40f);
            //Setup UniqueFactoryUI
            var buildingWindowGameObject = new GameObject("buildingWindowObject");
            wareHouseUI = (WareHouseUI)buildingWindowGameObject.AddComponent(typeof(WareHouseUI));
            playerBuildingInfo = UIView.Find<UIPanel>("(Library) WarehouseWorldInfoPanel");
            if (playerBuildingInfo == null)
            {
                DebugLog.LogToFileOnly("UIPanel not found (update broke the mod!): (Library) WarehouseWorldInfoPanel\nAvailable panels are:\n");
            }
            wareHouseUI.transform.parent = playerBuildingInfo.transform;
            wareHouseUI.baseBuildingWindow = playerBuildingInfo.gameObject.transform.GetComponentInChildren<WarehouseWorldInfoPanel>();
            base.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                PlayerBuildingUIToggle();
            };
        }

        public override void Update()
        {
            if (Loader.isGuiRunning)
            {
                if (WorldInfoPanel.GetCurrentInstanceID() != InstanceID.Empty)
                {
                    BuildingID = WorldInfoPanel.GetCurrentInstanceID();
                }
                base.Show();
            }
            else
            {
                base.Hide();
            }
            base.Update();
        }
    }
}