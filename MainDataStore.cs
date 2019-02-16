using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealConstruction
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
        public static byte[] buildingFlag1 = new byte[49152];


        public static ushort last_buildingid = 0;
        public static byte lastLanguage = 0;

        public static byte[] saveData = new byte[835584];

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
                buildingFlag1[i] = 0;
            }
        }

        public static void save()
        {
            int i = 0;
            SaveAndRestore.save_ushorts(ref i, foodBuffer, ref saveData);
            SaveAndRestore.save_ushorts(ref i, lumberBuffer, ref saveData);
            SaveAndRestore.save_ushorts(ref i, coalBuffer, ref saveData);
            SaveAndRestore.save_ushorts(ref i, petrolBuffer, ref saveData);
            SaveAndRestore.save_ushorts(ref i, constructionResourceBuffer, ref saveData);
            SaveAndRestore.save_ushorts(ref i, operationResourceBuffer, ref saveData);
            SaveAndRestore.save_bytes(ref i, buildingFlag1, ref saveData);
        }

        public static void load()
        {
            int i = 0;
            foodBuffer = SaveAndRestore.load_ushorts(ref i, saveData, foodBuffer.Length);
            lumberBuffer = SaveAndRestore.load_ushorts(ref i, saveData, foodBuffer.Length);
            coalBuffer = SaveAndRestore.load_ushorts(ref i, saveData, foodBuffer.Length);
            petrolBuffer = SaveAndRestore.load_ushorts(ref i, saveData, foodBuffer.Length);
            constructionResourceBuffer = SaveAndRestore.load_ushorts(ref i, saveData, foodBuffer.Length);
            operationResourceBuffer = SaveAndRestore.load_ushorts(ref i, saveData, foodBuffer.Length);
            buildingFlag1 = SaveAndRestore.load_bytes(ref i, saveData, buildingFlag1.Length);
        }

    }
}
