using System;
using UnityEngine;

namespace DM
{
    [Serializable]
    public class SaveData 
    { 
        public LevelType LevelType; 
        public int LevelIndex;

        public SaveData(LevelType type, int index)
        {
            LevelType = type;
            LevelIndex = index;
        }

        public string SaveToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
