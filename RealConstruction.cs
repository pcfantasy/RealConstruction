using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.Plugins;
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
        public static string AsmPath
        {
            get
            {
                return RealConstruction.PluginInfo.modPath;
            }
        }

        private static PluginManager.PluginInfo PluginInfo
        {
            get
            {
                PluginManager instance = Singleton<PluginManager>.instance;
                IEnumerable<PluginManager.PluginInfo> pluginsInfo = instance.GetPluginsInfo();
                foreach (PluginManager.PluginInfo current in pluginsInfo)
                {
                    try
                    {
                        IUserMod[] instances = current.GetInstances<IUserMod>();
                        bool flag = !(instances.FirstOrDefault<IUserMod>() is RealConstruction);
                        if (!flag)
                        {
                            return current;
                        }
                    }
                    catch
                    {
                    }
                }
                throw new Exception("Could not find assembly");
            }
        }

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
        }

        public void OnDisabled()
        {
            IsEnabled = false;
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
            UIHelperBase group = helper.AddGroup(Localization.Get("DEBUG_MODE"));
            group.AddCheckbox(Localization.Get("SHOW_LACK_OF_RESOURCE"), debugMode, (index) => debugModeEnable(index));
            SaveSetting();
        }

        public void debugModeEnable(bool index)
        {
            debugMode = index;
            SaveSetting();
        }
    }
}
