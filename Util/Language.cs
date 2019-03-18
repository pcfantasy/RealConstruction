using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealConstruction.Util
{
    public class Language
    {
        public static string[] English =
        {
            "Language",                                                                                      //0
            "Language_Select",                                                                               //1
            "Food Stored",                     //2
            "Lumber Stored",
            "Coal Stored",
            "Petrol Stored",
            "Construction Resource",
            "Operation Resource",//3
            "Operation Resource Left",
            "Transfer construction resource to",
            "Transfer operation resource to",
            "Going for Fuel to",
            "RealConstruction UI",
            "Generate Both Resources",
            "Generate Construction Resource",
            "Generate Operation Resource",
            "Debug Mode",
            "Show lack of resource icon on building which is lack of construction or operation resource"
        };



        public static string[] Chinese =
            {
            "语言",                                                   //0
            "语言选择",                                               //1
            "储存的食物",    //2
            "储存的木材",
            "储存的矿石",
            "储存的石油",
            "建筑材料",
            "日常运营材料",//3
            "剩余的运营材料",
            "运输建筑材料到",
            "运输日常运营材料到",
            "前往加油站",
            "RealConstruction 界面",
            "生成两种资源",
            "生成建筑资源",
            "生成运营资源",
            "调试模式",
            "对缺建筑或者运营材料的建筑显示缺少资源图标提示"
        };


        public static string[] Strings = new string[English.Length];

        public static byte currentLanguage = 255;

        public static void LanguageSwitch(byte language)
        {
            if (language == 1)
            {
                for (int i = 0; i < English.Length; i++)
                {
                    Strings[i] = Chinese[i];
                }
                currentLanguage = 1;
            }
            else if (language == 0)
            {
                for (int i = 0; i < English.Length; i++)
                {
                    Strings[i] = English[i];
                }
                currentLanguage = 0;
            }
            else
            {
                DebugLog.LogToFileOnly("unknow language!! use English");
                for (int i = 0; i < English.Length; i++)
                {
                    Strings[i] = English[i];
                }
                currentLanguage = 0;
            }
            MainDataStore.lastLanguage = currentLanguage;
        }
    }
}
