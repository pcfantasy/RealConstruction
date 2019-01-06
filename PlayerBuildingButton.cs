using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RealConstruction
{
    public class PlayerBuildingButton : UIPanel
    {
        private UIButton PBButton;

        private ItemClass.Availability CurrentMode;

        public static PlayerBuildingButton instance;

        public UIAlignAnchor Alignment;

        public UIPanel RefPanel;

        public static void PlayerBuildingUIToggle()
        {
            if (!Loader.guiPanel4.isVisible)
            {
                PlayerBuildingUI.refeshOnce = true;
                MainDataStore.last_buildingid = WorldInfoPanel.GetCurrentInstanceID().Building;
                Loader.guiPanel4.Show();
            }
            else
            {
                Loader.guiPanel4.Hide();
            }
        }

        public override void Start()
        {
            UIView aView = UIView.GetAView();
            base.name = "PlayerBuildingUIPanel";
            base.width = 30f;
            base.height = 30f;
            this.BringToFront();
            //base.backgroundSprite = "MenuPanel";
            //base.autoLayout = true;
            base.opacity = 1f;
            this.CurrentMode = Singleton<ToolManager>.instance.m_properties.m_mode;
            this.PBButton = base.AddUIComponent<UIButton>();
            this.PBButton.normalBgSprite = "PBButton";
            this.PBButton.hoveredBgSprite = "PBButtonHovered";
            this.PBButton.focusedBgSprite = "PBButtonFocused";
            this.PBButton.pressedBgSprite = "PBButtonPressed";
            this.PBButton.playAudioEvents = true;
            this.PBButton.name = "PBButton";
            this.PBButton.tooltipBox = aView.defaultTooltipBox;
            this.PBButton.text = "B";
            this.PBButton.textScale = 1.4f;
            this.PBButton.size = new Vector2(30f, 30f);
            this.PBButton.relativePosition = new Vector3(0f, 0f);
            base.AlignTo(this.RefPanel, this.Alignment);
            this.PBButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                PlayerBuildingButton.PlayerBuildingUIToggle();
            };
        }
    }
}