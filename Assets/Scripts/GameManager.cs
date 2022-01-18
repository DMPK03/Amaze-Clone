using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DM
{
    public class GameManager : MonoBehaviour, IGameState
    {
        public static GameManager Instance;

        [SerializeField] private CameraControll _cameraControll;
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private UiManager _uiManager;
        [SerializeField] private Movement _movement;

        [SerializeField] private Animation _gridAnimation;
        [SerializeField] private Image _fader;
        
        private Level _levelLoaded;
        private bool _nextLevel;

#region Monobehaviour
        private void OnEnable() {
            LevelManager.OnLevelLoadedEvent += OnLevelLoaded;
            Movement.OnLevelClearedEvent += OnLevelCleared;
        }

        private void OnDisable() {
            LevelManager.OnLevelLoadedEvent += OnLevelLoaded;
            Movement.OnLevelClearedEvent += OnLevelCleared;
        }
        
        void Start() {
            Instance = this;
            LoadNewLevel(LevelType.Level);
        }
#endregion
#region IGameState
        

        public void PrepareLevel(Level level)
        {
            _movement.PrepareLevel(level);
            _uiManager.PrepareLevel(level);
            _cameraControll.PrepareLevel(level);
        }

        public void StartLevel(Level level)
        {
            _uiManager.StartLevel(level);
            _cameraControll.StartLevel(level);
            _movement.StartLevel(level);
        }

        public void ClearLevel()
        {
            _uiManager.ClearLevel();
            _cameraControll.ClearLevel();
            _movement.ClearLevel();
        }
#endregion
#region LoadLevels
        public void LoadNewLevel(LevelType type)    //on game start or mode change
        {
            SaveData loadedData = SaveLoad.Instance.LoadData(type);
            _nextLevel = false;
            _levelManager.LoadTilemap(loadedData.LevelType, loadedData.LevelIndex);
        }

        public void LoadChallenge(LevelType type, int index)
        {
            _nextLevel = false;
            _levelManager.LoadTilemap(type,index);
        }

        private void LoadNextLevel()
        {
            _nextLevel=true;
            _levelManager.LoadTilemap(_levelLoaded.Type, _levelLoaded.LevelIndex +1);
            _gridAnimation.Play();
        }

        public void RestartLevel()
        {
            _nextLevel = false;
            _levelManager.LoadTilemap(_levelLoaded.Type, _levelLoaded.LevelIndex);
        }
#endregion
#region GameState       
        private void OnLevelCleared()
        {
            _uiManager.UpdateChallenges(_levelLoaded);
            ClearLevel();

            if(_levelLoaded.Type == LevelType.Challenge) LoadNewLevel(LevelType.Level); //todo pause, show challenge completed screen, wait for button press
            else LoadNextLevel();
        }

        private void OnLevelLoaded(Level level)
        {
            _levelLoaded = level;
            SaveLoad.Instance.SaveData(level);
            StartCoroutine(LevelLoadSequence(level));
        }

        private IEnumerator LevelLoadSequence(Level level)
        {
            if(_nextLevel)
            {
                PrepareLevel(level);
                yield return new WaitForSeconds(.5f);   //wait for grid animation
                StartLevel(level);
            }
            else
            {
                for (float i = .7f; i <= 1; i += Time.deltaTime)
                {
                    _fader.color = new Color(.27f, .27f, .27f, i);
                    yield return null;
                }

                PrepareLevel(level);
                yield return new WaitForSeconds(.5f);
                
                for (float i = 1; i >= 0; i -= Time.deltaTime)
                {
                    _fader.color = new Color(.27f, .27f, .27f, i);
                    if(level.Type != LevelType.Challenge) _cameraControll.transform.position = level.CameraPosition;
                    /*For some reason camera was not reseting when going back from challenge to normal level, other transitions are fine..
                    cant find whats causing it, so just force it back in place */
                    yield return null;
                } 


                StartLevel(level);   
            }
        }
#endregion        
    }
}
