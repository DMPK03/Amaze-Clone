using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DM
{
    
    public class UI : MonoBehaviour
    {
        public delegate void UiToggle(bool value);

        public static event UiToggle OnTgUiMode, OnTgVibrate;
        [SerializeField] GameObject _settingsTab;
        [SerializeField] TextMeshProUGUI _levelText;

        private Camera _camera;
        private Color _darkMode = new Color(.18F,.18F,.18F,1);
        private Color _lightMode = new Color(.78f,.78f,.78f,1);



        private void Start() {
            _camera = Camera.main;
            LevelManager.OnLevelLoadedEvent += OnNewLevelLoadedEvent;
        }

        public void OnSettingsOpen()
        {
            OnTgUiMode?.Invoke(true);
            _settingsTab.SetActive(true);
        }

        public void OnSettingsClose()
        {
            _settingsTab.SetActive(false);
            OnTgUiMode?.Invoke(false);
        }

        public void OnSoundToggle(bool toggle)
        {
            Debug.Log($"sound is {toggle}");
        }

        public void OnVibrateToggle(bool vibrate)
        {
            OnTgVibrate?.Invoke(!vibrate);
        }

        public void OnDarkmodeToggle(bool darkmode)
        {
            _camera.backgroundColor = darkmode? _lightMode : _darkMode; 
        }
        
        private void OnNewLevelLoadedEvent(Level level)
        {
            _levelText.text = level.name.ToUpper();
        }
    }
}
