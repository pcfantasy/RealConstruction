using System;
using ICities;
using System.IO;

namespace RealConstruction.Util
{
    public class SaveAndRestore : SerializableDataExtensionBase
    { 
        private static ISerializableData _serializableData;

        public static void SaveData(ref int idex, byte[] item, ref byte[] container)
        {
            int j;
            for (j = 0; j < item.Length; j++)
            {
                container[idex + j] = item[j];
            }
            idex += item.Length;
        }

        public static void LoadData(ref int idex, byte[] container, ref byte[] item)
        {
            int i;
            if (idex < container.Length)
            {
                for (i = 0; i < item.Length; i++)
                {
                    item[i] = container[idex];
                    idex++;
                }
            }
            else
            {
                for (i = 0; i < item.Length; i++)
                {
                    idex++;
                }
                DebugLog.LogToFileOnly("load data is too short, please check" + container.Length.ToString());
            }
        }

        public static void SaveData(ref int idex, ushort[] item, ref byte[] container)
        {
            int i; int j;
            byte[] bytes;
            for (j = 0; j < item.Length; j++)
            {
                bytes = BitConverter.GetBytes(item[j]);
                for (i = 0; i < bytes.Length; i++)
                {
                    container[idex + i] = bytes[i];
                }
                idex += bytes.Length;
            }
        }

        public static void LoadData(ref int idex, byte[] container, ref ushort[] item)
        {
            int i;
            if (idex < container.Length)
            {
                for (i = 0; i < item.Length; i++)
                {
                    item[i] = BitConverter.ToUInt16(container, idex);
                    idex += 2;
                }
            }
            else
            {
                DebugLog.LogToFileOnly("load data is too short, please check" + container.Length.ToString());
                for (i = 0; i < item.Length; i++)
                {
                    idex += 2;
                }
            }
        }

        public override void OnCreated(ISerializableData serializableData)
        {
            SaveAndRestore._serializableData = serializableData;
        }

        public override void OnSaveData()
        {
            if (Loader.CurrentLoadMode == LoadMode.LoadGame || Loader.CurrentLoadMode == LoadMode.NewGame)
            {
                DebugLog.LogToFileOnly("StartSave");
                var saveData = new byte[638976];
                MainDataStore.Save(ref saveData);
                SaveAndRestore._serializableData.SaveData("RealConstruction MainDataStore", saveData);
            }
        }

        public override void OnLoadData()
        {
            MainDataStore.DataInit();
            DebugLog.LogToFileOnly("StartLoad");

            var saveData = SaveAndRestore._serializableData.LoadData("RealConstruction MainDataStore");
            if (saveData == null)
            {
                DebugLog.LogToFileOnly("no RealConstruction MainDataStore save data, please check");
            }
            else
            {
                MainDataStore.Load(saveData);
            }
        }
    }
}
