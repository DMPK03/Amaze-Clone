using UnityEngine;
using UnityEditor;

namespace DM
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerEditor : Editor
    {
        public override void OnInspectorGUI() 
        {
            DrawDefaultInspector();

            AudioManager manager = (AudioManager) target;

            if(GUILayout.Button("ADD")) manager.AddAudio();
            if(GUILayout.Button("REMOVE")) manager.RemoveAudio();
        }
    }
}
