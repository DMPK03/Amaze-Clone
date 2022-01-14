using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace DM
{
    
    public class UiManager : MonoBehaviour
    {
        public static event Action<bool> OnTgUiMode;
        public static event Action<Sprite> OnBallSelected;

        [SerializeField] GameObject _ballsGO, _limitedTrunsGO, _timeTrialsGO, _gameOverGO;
        [SerializeField] TextMeshProUGUI _levelText, _movesText, _timeText;
        [SerializeField] Sprite[] _sprites;

        public bool Vibrate{get; set;}

        private Level _currentLevel;
        private Camera _camera;
        private Color _darkMode = new Color(.18F,.18F,.18F,1);
        private Color _lightMode = new Color(.78f,.78f,.78f,1);
        private int _movesRemaining;
        private Coroutine _timerCorutine = null;
        private Button[] _ballButtons;

#region monobehaviour
        private void Awake() {
            Movement.OnMoveEvent += OnMoveEvent;
            Movement.OnLevelClearedEvent += OnLevelCleared;
            LevelManager.OnLevelLoadedEvent += OnNewLevelLoadedEvent;
        }

        private void OnDestroy() {
            Movement.OnMoveEvent -= OnMoveEvent;
            Movement.OnLevelClearedEvent -= OnLevelCleared;
            LevelManager.OnLevelLoadedEvent -= OnNewLevelLoadedEvent;
        }

        private void Start() {
            _camera = Camera.main;
            _ballButtons = _ballsGO.GetComponentsInChildren<Button>(true);
        }
#endregion
#region public ui methods
        public void OpenUiElement()
        {
            OnTgUiMode?.Invoke(true);
        }

        public void CloseUiElement()
        {
            OnTgUiMode?.Invoke(false);
        }

        public void OnDarkmodeToggle(bool darkmode)
        {
            _camera.backgroundColor = darkmode? _lightMode : _darkMode; 
        }

        public void RestartLevel()
        {
            GameManager.Instance.RestartLevel();
            GameOver(false);
        }

        public void ChangeBall()
        {
            Sprite sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
            if(sprite != null) OnBallSelected?.Invoke(sprite);
        }

        public void ChangeGameMode(int type)
        {
            LevelType targetLevelType = (LevelType)type;
            if(_currentLevel.Type != targetLevelType) GameManager.Instance.LoadNewLevel(targetLevelType);
            CloseUiElement();
        }

#endregion
#region event methods

        private void OnNewLevelLoadedEvent(Level level)
        {
            _currentLevel = level;
            _limitedTrunsGO.SetActive(level.Type == LevelType.LimitedTurn);
            _timeTrialsGO.SetActive(level.Type == LevelType.TimeTrial);
            RefreshUiTexts(level);
        }

        private void OnLevelCleared()
        {
            StopAllCoroutines();
            _timerCorutine = null;
        }

        private void RefreshUiTexts(Level level)
        {
            _movesRemaining = level.AllowedMoves;
            _levelText.text = level.name.ToUpper();
            _movesText.text = _movesRemaining.ToString();
            _timeText.text = "20";
            RefreshOwnedBalls();
        }

        private void OnMoveEvent()
        {
            if(Vibrate) Handheld.Vibrate(); // placeholder untill custom vibrate
            Debug.Log(Vibrate);
            switch (_currentLevel.Type)
            {
                case LevelType.Level:
                break;

                case LevelType.Challenge:
                break;

                case LevelType.LimitedTurn:
                    _movesRemaining --;
                    _movesText.text = _movesRemaining.ToString();
                    if(_movesRemaining == 0) GameOver(true);
                break;

                case LevelType.TimeTrial:
                    if(_timerCorutine == null) _timerCorutine = StartCoroutine(Timer(20));  //todo get timelimit from saved level data, fixed time for now
                break;
                
                default:
                break;
            }
            
        }
#endregion
#region private methods
        private void RefreshOwnedBalls()
        {
            for (int i = 3; i < _ballButtons.Length - 1; i++)
            {
                if(_currentLevel.LevelIndex > i+2)
                {
                    _ballButtons[i].interactable = true; // unlock new ball every 2 levels
                    _ballButtons[i].image.sprite = _sprites[i-3];
                }
                else _ballButtons[i].interactable = false;
            }
        }

        private IEnumerator Timer(float duration)
        {
            float finishedTime = Time.time + duration;
            while (Time.time < finishedTime) {
                _timeText.text = (finishedTime - Time.time).ToString("0");
                yield return null;
            }
            GameOver(true);
            _timerCorutine = null;
        }

        private void GameOver(bool value)
        {
            _gameOverGO.SetActive(value);
            OnTgUiMode?.Invoke(value);
        }
#endregion
    }
}
