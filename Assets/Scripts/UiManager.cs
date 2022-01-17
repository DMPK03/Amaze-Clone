using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DM
{
    
    public class UiManager : MonoBehaviour
    {
        public static event Action<bool> OnTgUiMode;
        public static event Action<Sprite> OnBallSelected;

        [SerializeField] CameraControll _cameraControll;
        [SerializeField] AudioSource _audioSource;
        [SerializeField] GameObject _ballsGO, _limitedTrunsGO, _timeTrialsGO, _gameOverGO, _ltTextGo, _ttTextGo;
        [SerializeField] Transform _frame;
        [SerializeField] TextMeshProUGUI _levelText, _movesText, _timeText;
        [SerializeField] ParticleSystem _particle;

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

        public void ChangeBall(int i)   //find a better way to do this
        {
            if(_challengesCompleted[i] == 't')
            {
                Image image = _ballButtons[i].image;
                OnBallSelected?.Invoke(image.sprite);
                _frame.position = image.transform.position;
                PlayerPrefs.SetString("ball", image.gameObject.name);
            }
            else
            {
                GameManager.Instance.LoadNewLevel(LevelType.Challenge, i);
                _ballsGO.SetActive(false);
                CloseUiElement();
            }

        }

        public void ChangeGameMode(int type)
        {
            if(_currentLevel.Type != (LevelType)type) GameManager.Instance.LoadNewLevel((LevelType)type);
            CloseUiElement();
            if(_timerCorutine != null) StopCoroutine(_timerCorutine); //in case level is changed while timer is runing
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
            _particle.Play();
            if(_timerCorutine != null) StopCoroutine(_timerCorutine);
            _timerCorutine = null;
        }

        private void RefreshUiTexts(Level level)
        {
            _movesRemaining = level.AllowedMoves;
            _timeText.text = "20";  //set time for now, should get it from level 
            _levelText.text = level.name.ToUpper();
            _movesText.text = _movesRemaining.ToString();
        }

        private void OnMoveEvent()
        {
            if(Vibrate) Handheld.Vibrate(); // placeholder untill custom vibrate
            if(_currentLevel.Type == LevelType.LimitedTurn)
            {
                _movesRemaining --;
                _movesText.text = _movesRemaining.ToString();
                if(_movesRemaining == 0) GameOver(true);
            }
            if(_currentLevel.Type == LevelType.TimeTrial && _timerCorutine == null) _timerCorutine = StartCoroutine(Timer(20));
        }
#endregion
#region private methods

        private void LoadChallenges()
        {
            _challengesUnlocked =  SaveLoad.Instance.LoadData("Unlocked").ToCharArray();
            _challengesCompleted = SaveLoad.Instance.LoadData("Completed").ToCharArray();

            for (int i = 0; i < _ballButtons.Length - 1; i++) 
            {
                if(_challengesUnlocked[i] == 't')
                {
                    _ballButtons[i].interactable = true;
                    SetTexts(i, (_challengesCompleted[i] == 't')? "" : "PLAY CHALLENGE");
                }
                else _ballButtons[i].interactable = false;
                _ballButtons[0].interactable = true;
            }
        }
        
        private void SetTexts(int i, string msg)
        {
            var text = _ballButtons[i].gameObject.GetComponentInChildren<TextMeshProUGUI>(true);
            if(text != null) text.text = msg;
        }

        public void UpdateChallenges(Level level)
        {
            if(level.Type == LevelType.Level && level.LevelIndex % 3 == 0)
            {
                int i = level.LevelIndex / 3;
                _challengesUnlocked[i] = 't';
                _ballButtons[i].interactable = true;
                SetTexts(i,"PLAY CHALLENGE");
            }
            else if(level.Type == LevelType.Challenge)
            {
                _challengesCompleted[level.LevelIndex] = 't';
                SetTexts(level.LevelIndex, "");
                ChangeBall(level.LevelIndex);
            }            
        }

        private void SaveGame()
        {
            SaveLoad.Instance.SaveData("Unlocked", new string(_challengesUnlocked));
            SaveLoad.Instance.SaveData("Completed", new string(_challengesCompleted));
        }

        private void OnApplicationQuit() {
            SaveGame();
        }

        private void OnApplicationFocus(bool focusStatus) {
            if(!focusStatus) SaveGame();
        }

        private void LoadLastBall()
        {
            if(PlayerPrefs.HasKey("ball"))
            {
                string ball = PlayerPrefs.GetString("ball");
                foreach (var button in _ballButtons)
                {
                    if(button.name == ball)
                    {
                        OnBallSelected?.Invoke(button.image.sprite);
                        _frame.position = button.transform.position;
                        break;
                    }
                }
            }    
        }

        private IEnumerator Timer(float duration)
        {
            float finishedTime = Time.time + duration;

            while (Time.time < finishedTime)
            {
                _timeText.text = (finishedTime - Time.time).ToString("0");
                if(finishedTime - Time.time < 4) _audioSource.Play();

                yield return new WaitForSeconds(1f);
            }
            _timeText.text = (finishedTime - Time.time).ToString("0");
            _audioSource.Play();
            GameOver(true);
            _timerCorutine = null;
        }

        private void GameOver(bool value)
        {
            _gameOverGO.SetActive(value);
            OnTgUiMode?.Invoke(value);
            _ltTextGo.SetActive(_currentLevel.Type == LevelType.LimitedTurn);
            _ttTextGo.SetActive(_currentLevel.Type == LevelType.TimeTrial);
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
