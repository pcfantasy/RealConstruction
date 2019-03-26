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
    public class PlayerBuildingButton : UIButton
    {
        public static bool refeshOnce = false;

        public static void PlayerBuildingUIToggle()
        {
            if (!Loader.playerBuildingPanel.isVisible)
            {
                PlayerBuildingUI.refeshOnce = true;
                MainDataStore.lastBuildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
                Loader.playerBuildingPanel.Show();
            }
            else
            {
                Loader.playerBuildingPanel.Hide();
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
            base.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                PlayerBuildingUIToggle();
            };
        }

        public override void Update()
        {
            MainDataStore.lastBuildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
            if (Loader.isGuiRunning)
            {
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