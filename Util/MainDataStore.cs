using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace RealConstruction.Util
{
    public class MainDataStore
    {
        public static ushort[] foodBuffer = new ushort[49152];
        public static ushort[] lumberBuffer = new ushort[49152];
        public static ushort[] coalBuffer = new ushort[49152];
        public static ushort[] petrolBuffer = new ushort[49152];
        public static ushort[] constructionResourceBuffer = new ushort[49152];
        public static ushort[] operationResourceBuffer = new ushort[49152];
        public static bool[] isBuildingReleased = new bool[49152];
        public static bool[] isBuildingLackOfResource = new bool[49152];
        public static byte[] resourceCategory = new byte[49152];
        public static ushort lastBuildingID = 0;
        public static ushort[,] canNotConnectedBuildingID = new ushort[49152, 8];
        public static byte[] refreshCanNotConnectedBuildingIDCount = new byte[49152];
        public static byte[] canNotConnectedBuildingIDCount = new byte[49152];

        public static void DataInit()
        {
            for (int i = 0; i < MainDataStore.foodBuffer.Length; i++)
            {
                foodBuffer[i] = 0;
                lumberBuffer[i] = 0;
                coalBuffer[i] = 0;
                petrolBuffer[i] = 0;
                constructionResourceBuffer[i] = 0;
                operationResourceBuffer[i] = 0;
                isBuildingReleased[i] = false;
                resourceCategory[i] = 0;
                isBuildingLackOfResource[i] = false;
            }
        }

        public static void Save(ref byte[] saveData)
        {
            //835584
            int i = 0;
            SaveAndRestore.SaveData(ref i, foodBuffer, ref saveData);
            SaveAndRestore.SaveData(ref i, lumberBuffer, ref saveData);
            SaveAndRestore.SaveData(ref i, coalBuffer, ref saveData);
            SaveAndRestore.SaveData(ref i, petrolBuffer, ref saveData);
            SaveAndRestore.SaveData(ref i, constructionResourceBuffer, ref saveData);
            SaveAndRestore.SaveData(ref i, operationResourceBuffer, ref saveData);
            SaveAndRestore.SaveData(ref i, resourceCategory, ref saveData);
        }

        public static void Load(byte[] saveData)
        {
            //835584
            int i = 0;
            SaveAndRestore.LoadData(ref i, saveData, ref foodBuffer);
            SaveAndRestore.LoadData(ref i, saveData, ref lumberBuffer);
            SaveAndRestore.LoadData(ref i, saveData, ref coalBuffer);
            SaveAndRestore.LoadData(ref i, saveData, ref petrolBuffer);
            SaveAndRestore.LoadData(ref i, saveData, ref constructionResourceBuffer);
            SaveAndRestore.LoadData(ref i, saveData, ref operationResourceBuffer);
            SaveAndRestore.LoadData(ref i, saveData, ref resourceCategory);
        }
    }
}
