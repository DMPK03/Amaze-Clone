using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DM
{
    
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] LevelManager _levelManager;
        [SerializeField] SpriteRenderer _ballSprite;

        private int _currentLevelIndex;
        private Level _currentLevel;

        private void OnEnable() {
            Movement.OnLevelClearedEvent += OnLevelCleared;
            LevelManager.OnLevelLoadedEvent += OnLevelLoaded;
        }
        
        void Start()
        {
            Instance = this;
            UI.OnBallSelected += OnNewBall;
        }

        public void LoadNewLevel(int type)
        {
            Level.LevelType levelType = (Level.LevelType)type;
            int lastLevelOfThisType = 0;    //get this from saved progress
            _levelManager.LoadTilemap(levelType, lastLevelOfThisType);
        }

        private void LoadNextLevel()
        {
            _levelManager.LoadTilemap(_currentLevel.Type, _currentLevelIndex +1);
        }

        private void OnLevelCleared()
        {
            //check for adds here, if no adds ready load next level
            LoadNextLevel();
        }

        private void OnLevelLoaded(Level level)
        {
            _currentLevel = level;
            _currentLevelIndex = level.LevelIndex;
        }
        

        private void OnNewBall(Sprite ballSprite)
        {
            _ballSprite.sprite = ballSprite;
        }

    }
}
