using ColossalFramework;
using ColossalFramework.UI;
using RealConstruction.CustomAI;
using RealConstruction.NewAI;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RealConstruction.UI
{
    public class PlayerBuildingButton : UIButton
    {
        public static bool refeshOnce = false;
        private UIPanel playerBuildingInfo;
        private PlayerBuildingUI playerBuildingUI;
        private InstanceID BuildingID = InstanceID.Empty;
        public void PlayerBuildingUIToggle()
        {
            if ((!playerBuildingUI.isVisible) && (BuildingID != InstanceID.Empty))
            {
                playerBuildingUI.position = new Vector3(playerBuildingInfo.size.x, playerBuildingInfo.size.y);
                playerBuildingUI.size = new Vector3(playerBuildingInfo.size.x, playerBuildingInfo.size.y);
                PlayerBuildingUI.refeshOnce = true;
                playerBuildingUI.Show();
            }
            else
            {
                playerBuildingUI.Hide();
            }
        }

        public override void Start()
        {
            base.normalBgSprite = "ToolbarIconGroup1Normal";
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
            //Setup PlayerBuildingUI
            var buildingWindowGameObject = new GameObject("buildingWindowObject");
            playerBuildingUI = (PlayerBuildingUI)buildingWindowGameObject.AddComponent(typeof(PlayerBuildingUI));
            playerBuildingInfo = UIView.Find<UIPanel>("(Library) CityServiceWorldInfoPanel");
            if (playerBuildingInfo == null)
            {
                DebugLog.LogToFileOnly("UIPanel not found (update broke the mod!): (Library) CityServiceWorldInfoPanel\nAvailable panels are:\n");
            }
            playerBuildingUI.transform.parent = playerBuildingInfo.transform;
            playerBuildingUI.baseBuildingWindow = playerBuildingInfo.gameObject.transform.GetComponentInChildren<CityServiceWorldInfoPanel>();
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