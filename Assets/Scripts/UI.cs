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

        [SerializeField] GameObject _settingsTab, _privacyTab, _ballsGO;
        [SerializeField] TextMeshProUGUI _levelText;
        [SerializeField] Sprite[] _sprites;

        private Camera _camera;
        private Color _darkMode = new Color(.18F,.18F,.18F,1);
        private Color _lightMode = new Color(.78f,.78f,.78f,1);

        private int _levelsCompleted = 0;
        private Button[] _ballButtons;


        private void Start() {
            _camera = Camera.main;
            _ballButtons = _ballsGO.GetComponentsInChildren<Button>(true);
            LevelManager.OnLevelLoadedEvent += OnNewLevelLoadedEvent;
            RefreshOwnedBalls();
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
            if(level.LevelIndex > _levelsCompleted) _levelsCompleted = level.LevelIndex;
            RefreshOwnedBalls(); 
        }
        
        public void ChangeBall()
        {
            Sprite sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
            if(sprite != null) OnBallSelected?.Invoke(sprite);
        }

        private void RefreshOwnedBalls()
        {
            for (int i = 3; i < _ballButtons.Length - 1; i++)
            {
                if(_levelsCompleted > i+2)
                {
                    _ballButtons[i].interactable = true; // unlock new ball every 2 levels
                    _ballButtons[i].image.sprite = _sprites[i-3];
                }
                else _ballButtons[i].interactable = false;
            }
        }

        public void ChangeGameMode(int type)
        {
            GameManager.Instance.LoadNewLevel(type);
            CloseUiElement();
        }
    }
}
