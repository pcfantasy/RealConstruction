using CitiesHarmony.API;
using ICities;
using RealConstruction.Util;
using System.IO;

namespace RealConstruction
{
    public class RealConstruction : IUserMod
    {
        public static bool IsEnabled = false;
        public static bool debugMode = false;
        public static bool fixUnRouteTransfer = false;
        public static int operationConsumption = 2;

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
            HarmonyHelper.EnsureHarmonyInstalled();
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
            streamWriter.WriteLine(fixUnRouteTransfer);
            streamWriter.WriteLine(operationConsumption);
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

                if (strLine == "True")
                {
                    debugMode = true;
                }
                else
                {
                    debugMode = false;
                }

                strLine = sr.ReadLine();
                if (strLine == "True")
                {
                    fixUnRouteTransfer = true;
                }
                else
                {
                    fixUnRouteTransfer = false;
                }

                strLine = sr.ReadLine();
                if (strLine == "0")
                {
                    operationConsumption = 0;
                }
                else if (strLine == "1")
                {
                    operationConsumption = 1;
                }
                else
                {
                    operationConsumption = 2;
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
            UIHelperBase group1 = helper.AddGroup(Localization.Get("FIX_UNROUTED_TRANSFER_MATCH_DESCRIPTION"));
            group1.AddCheckbox(Localization.Get("FIX_UNROUTED_TRANSFER_MATCH_ENALBE"), fixUnRouteTransfer, (index) => fixUnRouteTransferEnable(index));
            UIHelperBase group2 = helper.AddGroup(Localization.Get("OPERATION_RESOURCE_CONSUMPTION"));
            group2.AddDropdown(Localization.Get("OPERATION_RESOURCE_CONSUMPTION"), new string[] { Localization.Get("NORMAL"), Localization.Get("HALF"), Localization.Get("NONE") }, operationConsumption, (index) => GetOperationConsumption(index));
            SaveSetting();
        }

        public void debugModeEnable(bool index)
        {
            debugMode = index;
            SaveSetting();
        }

        public void fixUnRouteTransferEnable(bool index)
        {
            fixUnRouteTransfer = index;
            SaveSetting();
        }

        public void GetOperationConsumption(int index)
        {
            operationConsumption = index;
            SaveSetting();
            //MethodInfo method = typeof(OptionsMainPanel).GetMethod("OnLocaleChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            //method.Invoke(UIView.library.Get<OptionsMainPanel>("OptionsPanel"), new object[0]);
        }
    }
}
