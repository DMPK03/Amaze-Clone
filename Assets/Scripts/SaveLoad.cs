using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace DM
{
    public class SaveLoad : MonoBehaviour
    {
        public static SaveLoad Instance;
        private float time;

        private void Awake() {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        
        public void SaveData(Level level)
        {
            SaveData newData = new SaveData(level.Type, level.LevelIndex);
            string saveString = newData.SaveToString();

            string path = Path.Combine(Application.persistentDataPath, $"{level.Type}.json");
            File.WriteAllText(path, saveString);
        }

        public SaveData LoadData(LevelType levelType)
        {
            string path = Path.Combine(Application.persistentDataPath, $"{levelType}.json");
            if(System.IO.File.Exists(path))
            {
                string loadData = File.ReadAllText(path);
                return JsonUtility.FromJson<SaveData>(loadData);
            }
            else return new SaveData(levelType, 0);
        }
    }
}
