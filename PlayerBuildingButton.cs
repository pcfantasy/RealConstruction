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
        public static UIButton PBButton;

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
            base.width = 200f;
            base.height = 15f;
            //this.MoveBackward();
            //base.backgroundSprite = "MenuPanel";
            //base.autoLayout = true;
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
            PBButton.text = Language.Strings[12];
            PBButton.textScale = 0.9f;
            PBButton.size = new Vector2(200f, 15f);
            PBButton.relativePosition = new Vector3(0f, 0f);
            base.AlignTo(this.RefPanel, this.Alignment);
            PBButton.eventClick += delegate (UIComponent component, UIMouseEventParameter eventParam)
            {
                PlayerBuildingButton.PlayerBuildingUIToggle();
            };
        }

        public override void Update()
        {
            MainDataStore.last_buildingid = WorldInfoPanel.GetCurrentInstanceID().Building;
            if (Loader.isGuiRunning)
            {
                base.Show();
                PlayerBuildingButton.PBButton.text = Language.Strings[12];
            }
            else
            {
                base.Hide();
            }
            base.Update();
        }
    }
}