using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DM
{
    
    public class UiManager : MonoBehaviour, IGameState
    {
        public static event Action<bool> OnTgUiMode;
        public static event Action<Sprite> OnBallSelected;

        
        [SerializeField] private GameObject _limitedTrunsGO, _timeTrialsGO, _gameOverGO, _ballsGO, _ltTextGo, _ttTextGo;        
        [Space(10)][SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _movesText, _timeText;
        [Space(10)][SerializeField] private Transform _frame;
        [SerializeField] private SpriteRenderer _ballSprite;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private Toggle[] _toggles;
        [SerializeField] private Image _fader;

        public bool Vibrate;
        
        private Char[] _challengesUnlocked, _challengesCompleted;
        private string _toggleString;
        private Coroutine _timerCorutine = null;
        private Button[] _ballButtons;
        private Level _currentLevel;
        private int _movesRemaining;

#region mono
        private void OnEnable() {
            Movement.OnMoveEvent += OnMoveEvent;
        }

        private void OnDestroy() {
            Movement.OnMoveEvent -= OnMoveEvent;
        }

        private void Start() {
            _ballButtons = _ballsGO.GetComponentsInChildren<Button>(true);
            LoadToggleState();
            LoadLastBall();
            LoadChallenges();
        }
#endregion
#region public ui
        
        public void ChangeBall(int i)   //find a better way to do this
        {
            if(_challengesCompleted[i] == 't')
            {
                Image image = _ballButtons[i].image;
                _ballSprite.sprite = image.sprite;
                _frame.position = image.transform.position;
                PlayerPrefs.SetString("ball", image.gameObject.name);
            }
            else
            {
                GameManager.Instance.LoadChallenge(LevelType.Challenge, i);
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
#region IGameState
        public void PrepareLevel(Level level)
        {
            _currentLevel = level;
            _limitedTrunsGO.SetActive(level.Type == LevelType.LimitedTurn);
            _timeTrialsGO.SetActive(level.Type == LevelType.TimeTrial);
            RefreshUiTexts(level);
        }

        public void StartLevel(Level level)
        {
            if(_timerCorutine != null) StopCoroutine(_timerCorutine);
            _timerCorutine = null;
        }

        public void ClearLevel()
        {
            _particle.Play();
            if(_timerCorutine != null) StopCoroutine(_timerCorutine);
            _timerCorutine = null;
        }
#endregion
#region public
        public void RestartLevel()
        {
            GameManager.Instance.RestartLevel();
            GameOver(false);
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

        public void OpenUiElement() {   // get rid of this
            OnTgUiMode?.Invoke(true);
        }

        public void CloseUiElement() {
            OnTgUiMode?.Invoke(false);
        }
        
#endregion
#region private
        private void RefreshUiTexts(Level level)
        {
            _movesRemaining = level.AllowedMoves;
            _timeText.text = "20";  //set time for now, should get it from level 
            _levelText.text = level.name.ToUpper();
            _movesText.text = _movesRemaining.ToString();
        }

        private void OnMoveEvent()
        {
            if(!_toggles[1].isOn) Handheld.Vibrate(); // placeholder untill custom vibrate
            if(_currentLevel.Type == LevelType.LimitedTurn)
            {
                _movesRemaining --;
                _movesText.text = _movesRemaining.ToString();
                if(_movesRemaining == 0) GameOver(true);
            }
            if(_currentLevel.Type == LevelType.TimeTrial && _timerCorutine == null) _timerCorutine = StartCoroutine(Timer(20));
        }
           
        private void SetTexts(int i, string msg)
        {
            var text = _ballButtons[i].gameObject.GetComponentInChildren<TextMeshProUGUI>(true);
            if(text != null) text.text = msg;
        }

        private void GameOver(bool value)
        {
            _gameOverGO.SetActive(value);
            OnTgUiMode?.Invoke(value);
            _ltTextGo.SetActive(_currentLevel.Type == LevelType.LimitedTurn);
            _ttTextGo.SetActive(_currentLevel.Type == LevelType.TimeTrial);
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
#endregion
#region SaveLoad
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

#endregion
    }
}
