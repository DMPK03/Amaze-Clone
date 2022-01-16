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

        private Level _levelLoaded;

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

        public void LoadNewLevel(LevelType type, int index)
        {
            _levelManager.LoadTilemap(type,index);
        }

        public void RestartLevel()
        {
            _levelManager.LoadTilemap(_levelLoaded.Type, _levelLoaded.LevelIndex);
        }

        private void LoadNextLevel()
        {
            _levelManager.LoadTilemap(_levelLoaded.Type, _levelLoaded.LevelIndex +1);
        }

        private void OnLevelCleared()
        {
            _uiManager.UpdateChallenges(_levelLoaded);
            if(_levelLoaded.Type == LevelType.Challenge) LoadNewLevel(LevelType.Level);
            else LoadNextLevel();
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
