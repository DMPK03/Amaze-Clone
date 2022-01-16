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

        [SerializeField] CameraControll _cameraControll;
        [SerializeField] GameObject _ballsGO, _limitedTrunsGO, _timeTrialsGO, _gameOverGO;
        [SerializeField] Transform _frame;
        [SerializeField] TextMeshProUGUI _levelText, _movesText, _timeText;
        [SerializeField] Sprite[] _sprites;

        [SerializeField] Toggle[] _toggles;
        private string _toggleString;
        Char[] _challengesUnlocked, _challengesCompleted;


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
            LoadToggleState();
            LoadLastBall();
            LoadChallenges();
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

        public void ChangeBall(int i)
        {
            if(_challengesCompleted[i] == 't')
            {
                Image image = EventSystem.current.currentSelectedGameObject.GetComponent<Image>();
                OnBallSelected?.Invoke(image.sprite);
                _frame.position = image.transform.position;
                PlayerPrefs.SetString("ball", image.gameObject.name);
            }
            else
            {
                GameManager.Instance.LoadNewLevel(LevelType.Challenge);
                _ballsGO.SetActive(false);
            }

        }

        public void ChangeGameMode(int type)
        {
            LevelType targetLevelType = (LevelType)type;
            if(_currentLevel.Type != targetLevelType) GameManager.Instance.LoadNewLevel(targetLevelType);
            CloseUiElement();
            StopAllCoroutines();
        }

#endregion
#region event methods

        private void OnNewLevelLoadedEvent(Level level)
        {
            _currentLevel = level;
            _limitedTrunsGO.SetActive(level.Type == LevelType.LimitedTurn);
            _timeTrialsGO.SetActive(level.Type == LevelType.TimeTrial);
            _cameraControll.enabled = (level.Type == LevelType.Challenge);
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
        }

        private void OnMoveEvent()
        {
            if(Vibrate) Handheld.Vibrate(); // placeholder untill custom vibrate
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

        private void LoadChallenges()
        {
            _challengesUnlocked =  SaveLoad.Instance.LoadData("Unlocked").ToCharArray();
            _challengesCompleted = SaveLoad.Instance.LoadData("Completed").ToCharArray();

            for (int i = 3; i < _ballButtons.Length - 1; i++) _ballButtons[i].interactable = _challengesUnlocked[i-3] == 't';
        }

        public void UpdateChallenges(Level level)
        {
            Debug.Log(level);
            /*if(GameManager.Instance.LevelCleared.Type == LevelType.Level)
            {
                _challengesUnlocked[GameManager.Instance.LevelCleared.LevelIndex] = 't';
                _ballButtons[GameManager.Instance.LevelCleared.LevelIndex].interactable = true;
            }*/
            //else if(GameManager.Instance.LevelCleared.Type == LevelType.Challenge) _challengesCompleted[GameManager.Instance.LevelCleared.LevelIndex] = 't';
        }

        private void LoadLastBall()  //find a better way to do this
        {
            if(PlayerPrefs.HasKey("ball"))
            {
                string ball = PlayerPrefs.GetString("ball");
                foreach (var button in _ballButtons)
                {
                    if(button.name == ball)
                    {
                        OnBallSelected?.Invoke(button.transform.GetComponent<Image>().sprite);
                        _frame.position = button.transform.position;
                        break;
                    }
                }
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
#region PlayerPrefs

        public void SaveToggleState()
        {
            _toggleString = "";
            foreach (var toggle in _toggles)
            {
                _toggleString += toggle.isOn? 't' : 'f';
            }
            PlayerPrefs.SetString("state", _toggleString);
        }

        private void LoadToggleState()
        {
            if(PlayerPrefs.HasKey("state"))
            {
                _toggleString = PlayerPrefs.GetString("state");
                for (int i = 0; i < 3; i++)
                {
                    _toggles[i].isOn = _toggleString[i] == 't';
                }
            }
        }

#endregion
    }
}
