using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace DM
{
    [CustomEditor(typeof(LevelManager))]
    public class LevelManagerEditor : Editor 
    {
        public override void OnInspectorGUI() 
        {
            DrawDefaultInspector();

            LevelManager manager = (LevelManager) target;

            if(GUILayout.Button("Save")) manager.SaveTilemap();
            
            if(GUILayout.Button("Clear")) manager.ClearTilemap();

            if(GUILayout.Button("Load")) manager.LoadTilemap();
        }
    }
    
}

