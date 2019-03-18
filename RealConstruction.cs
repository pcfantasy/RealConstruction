using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using RealConstruction.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RealConstruction
{
    public class RealConstruction : IUserMod
    {
        public static bool IsEnabled = false;
        public static int language_idex = 0;
        public static bool debugMode = false;

        public string Name
        {
            get { return "Real Construction"; }
        }

        public string Description
        {
            get { return "Private building construction will start only after getting enough resource."; }
        }

        public void OnEnabled()
        {
            IsEnabled = true;
            FileStream fs = File.Create("RealConstruction.txt");
            fs.Close();
            Language.LanguageSwitch((byte)language_idex);
        }

        public void OnDisabled()
        {
            IsEnabled = false;
            Language.LanguageSwitch((byte)language_idex);
        }

        public static void SaveSetting()
        {
            //save langugae
            FileStream fs = File.Create("RealConstruction_setting.txt");
            StreamWriter streamWriter = new StreamWriter(fs);
            streamWriter.WriteLine(debugMode);
            streamWriter.Flush();
            fs.Close();
        }

        public static void LoadSetting()
        {
            if (File.Exists("RealConstruction_setting.txt"))
            {
                FileStream fs = new FileStream("RealConstruction_setting.txt", FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string strLine = sr.ReadLine();

                if (strLine == "False")
                {
                    debugMode = false;
                }
                else
                {
                    debugMode = true;
                }

                sr.Close();
                fs.Close();
            }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            LoadSetting();
            if (SingletonLite<LocaleManager>.instance.language.Contains("zh"))
            {
                Language.LanguageSwitch(1);
            }
            else
            {
                Language.LanguageSwitch(0);
            }

            UIHelperBase group1 = helper.AddGroup(Language.Strings[16]);
            group1.AddCheckbox(Language.Strings[17], debugMode, (index) => debugModeEnable(index));
            SaveSetting();
        }

        public void debugModeEnable(bool index)
        {
            debugMode = index;
            SaveSetting();
        }
    }
}
