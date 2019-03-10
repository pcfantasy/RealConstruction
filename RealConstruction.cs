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
    }
}
