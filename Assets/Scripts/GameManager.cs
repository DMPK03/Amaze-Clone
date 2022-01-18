using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DM
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private UiManager _uiManager;
        
        [SerializeField] private Animation _gridAnimation;

        private Level _levelLoaded;

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

        public void LoadNewLevel(LevelType type)
        {
            SaveData loadedData = SaveLoad.Instance.LoadData(type);
            StartCoroutine(_uiManager.Fade());

            _levelManager.LoadTilemap(loadedData.LevelType, loadedData.LevelIndex);
        }

        public void LoadNewLevel(LevelType type, int index)
        {
            StartCoroutine(_uiManager.Fade());
            _levelManager.LoadTilemap(type,index);
        }

        public void RestartLevel()
        {
            StartCoroutine(_uiManager.Fade());
            _levelManager.LoadTilemap(_levelLoaded.Type, _levelLoaded.LevelIndex);
        }

        private void LoadNextLevel()
        {
            _levelManager.LoadTilemap(_levelLoaded.Type, _levelLoaded.LevelIndex +1);
            _gridAnimation.Play();
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
            SaveLoad.Instance.SaveData(level);
        }
    }
}
