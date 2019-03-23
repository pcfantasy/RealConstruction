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
    public class UniqueFactoryButton : UIPanel
    {
        public static UIButton PBButton;
        private ItemClass.Availability CurrentMode;
        public static UniqueFactoryButton instance;
        public UIPanel RefPanel;
        public static bool refeshOnce = false;

        public static void PlayerBuildingUIToggle()
        {
            if (!Loader.uniqueFactoryPanel.isVisible)
            {
                UniqueFactoryUI.refeshOnce = true;
                MainDataStore.lastBuildingID = WorldInfoPanel.GetCurrentInstanceID().Building;
                Loader.uniqueFactoryPanel.Show();
            }
            else
            {
                Loader.uniqueFactoryPanel.Hide();
            }
        }

        public override void Start()
        {
            UIView aView = UIView.GetAView();
            base.name = "PlayerBuildingUIPanel";
            base.width = 200f;
            base.height = 15f;
            base.opacity = 1f;
            this.CurrentMode = Singleton<ToolManager>.instance.m_properties.m_mode;
            PBButton = base.AddUIComponent<UIButton>();
            PBButton.normalBgSprite = "PBButton";
            PBButton.hoveredBgSprite = "PBButtonHovered";
            PBButton.focusedBgSprite = "PBButtonFocused";
            PBButton.pressedBgSprite = "PBButtonPressed";
            PBButton.playAudioEvents = true;
            PBButton.name = "PBButton";
            PBButton.tooltipBox = aView.defaultTooltipBox;
            this.relativePosition = new Vector3(90f, 0f);
            if (Loader.m_atlasLoaded)
            {
                UISprite internalSprite = PBButton.AddUIComponent<UISprite>();
                internalSprite.atlas = SpriteUtilities.GetAtlas(Loader.m_atlasName);
                internalSprite.spriteName = "Pic";
                internalSprite.relativePosition = new Vector3(0, 0);
                internalSprite.width = internalSprite.height = 40f;
                PBButton.size = new Vector2(40f, 40f);
            }
            else
            {
                PBButton.text = Localization.Get("REALCONSTRUCTION_UI");
                PBButton.textScale = 0.9f;
                PBButton.size = new Vector2(100f, 15f);
            }
            PBButton.relativePosition = new Vector3(0f, 0f);
            PBButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
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
                if (!Loader.m_atlasLoaded)
                {
                    PBButton.text = Localization.Get("REALCONSTRUCTION_UI");
                }
            }
            else
            {
                base.Hide();
            }
            base.Update();
        }
    }
}