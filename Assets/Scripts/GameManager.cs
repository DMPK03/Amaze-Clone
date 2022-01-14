using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DM
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] LevelManager _levelManager;
        [SerializeField] UiManager _uiManager;

        [SerializeField] SpriteRenderer _ballSprite;
        [SerializeField] TrailRenderer _trailRenderer;

        private Level _levelLoaded,_LevelCleared;

        private void Awake() {
            Movement.OnLevelClearedEvent += OnLevelCleared;
            LevelManager.OnLevelLoadedEvent += OnLevelLoaded;
            UiManager.OnBallSelected += OnNewBall;
        }
        
        void Start()
        {
            Instance = this;
            LoadNewLevel(LevelType.Level);
        }

        public void LoadNewLevel(LevelType type)
        {
            SaveData loadedData = SaveLoad.Instance.LoadData(type);

            _levelManager.LoadTilemap(loadedData.LevelType, loadedData.LevelIndex);
        }

        public void RestartLevel()
        {
            _levelManager.LoadTilemap(_levelLoaded.Type, _levelLoaded.LevelIndex);
        }

        private void LoadNextLevel()
        {
            _levelManager.LoadTilemap(_LevelCleared.Type, _LevelCleared.LevelIndex +1);
        }

        private void OnLevelCleared()
        {
            //check for adds here, if no adds ready load next level
            _LevelCleared = _levelLoaded;
            LoadNextLevel();
        }

        private void OnLevelLoaded(Level level)
        {
            _levelLoaded = level;

            _trailRenderer.Clear();
            SaveLoad.Instance.SaveData(level);
        }

        private void OnNewBall(Sprite ballSprite)
        {
            _ballSprite.sprite = ballSprite;
        }
    }
}
