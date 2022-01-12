using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace DM
{
    
    public class UI : MonoBehaviour
    {
        public delegate void UiToggle(bool value);
        public delegate void BallSprite(Sprite sprite);

        public static event UiToggle OnTgUiMode, OnTgVibrate;
        public static event BallSprite OnBallSelected;

        [SerializeField] GameObject _settingsTab, _privacyTab;
        [SerializeField] TextMeshProUGUI _levelText;

        private Camera _camera;
        private Color _darkMode = new Color(.18F,.18F,.18F,1);
        private Color _lightMode = new Color(.78f,.78f,.78f,1);



        private void Start() {
            _camera = Camera.main;
            LevelManager.OnLevelLoadedEvent += OnNewLevelLoadedEvent;
        }

        public void OpenUiElement()
        {
            OnTgUiMode?.Invoke(true);
        }

        public void CloseUiElement()
        {
            OnTgUiMode?.Invoke(false);
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
        
        public void ChangeBall()
        {
            Sprite sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
            if(sprite != null) OnBallSelected?.Invoke(sprite);
        }

    }
}
