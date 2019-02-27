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

        public UIAlignAnchor Alignment;

        public UIPanel RefPanel;

        public static bool refeshOnce = false;

        public static void PlayerBuildingUIToggle()
        {
            if (!Loader.guiPanel2.isVisible)
            {
                UniqueFactoryUI.refeshOnce = true;
                MainDataStore.last_buildingid = WorldInfoPanel.GetCurrentInstanceID().Building;
                Loader.guiPanel2.Show();
            }
            else
            {
                Loader.guiPanel2.Hide();
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
                PlayerBuildingUIToggle();
            };
        }

        public override void Update()
        {
            MainDataStore.last_buildingid = WorldInfoPanel.GetCurrentInstanceID().Building;
            if (Loader.isGuiRunning)
            {
                base.Show();
                PBButton.text = Language.Strings[12];
                if (MainDataStore.isBuildingLackOfResource[MainDataStore.last_buildingid] && refeshOnce)
                {
                    if (PBButton.textColor == Color.red)
                    {
                        PBButton.textColor = Color.white;
                    }
                    else
                    {
                        refeshOnce = false;
                        PBButton.textColor = Color.red;
                    }
                }
                else
                {
                    refeshOnce = false;
                    PBButton.textColor = Color.white;
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