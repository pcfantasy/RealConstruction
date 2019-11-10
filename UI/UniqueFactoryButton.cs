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
    public class UniqueFactoryButton : UIButton
    {
        public static bool refeshOnce = false;
        private UIPanel playerBuildingInfo;
        private UniqueFactoryUI uniqueFactoryUI;
        private InstanceID BuildingID = InstanceID.Empty;
        public void PlayerBuildingUIToggle()
        {
            if (!uniqueFactoryUI.isVisible && (BuildingID != InstanceID.Empty))
            {
                UniqueFactoryUI.refeshOnce = true;
                uniqueFactoryUI.position = new Vector3(playerBuildingInfo.size.x, playerBuildingInfo.size.y);
                uniqueFactoryUI.size = new Vector3(playerBuildingInfo.size.x, playerBuildingInfo.size.y);
                uniqueFactoryUI.Show();
            }
            else
            {
                uniqueFactoryUI.Hide();
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
            uniqueFactoryUI = (UniqueFactoryUI)buildingWindowGameObject.AddComponent(typeof(UniqueFactoryUI));
            playerBuildingInfo = UIView.Find<UIPanel>("(Library) UniqueFactoryWorldInfoPanel");
            if (playerBuildingInfo == null)
            {
                DebugLog.LogToFileOnly("UIPanel not found (update broke the mod!): (Library) UniqueFactoryWorldInfoPanel\nAvailable panels are:\n");
            }
            uniqueFactoryUI.transform.parent = playerBuildingInfo.transform;
            uniqueFactoryUI.baseBuildingWindow = playerBuildingInfo.gameObject.transform.GetComponentInChildren<UniqueFactoryWorldInfoPanel>();
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