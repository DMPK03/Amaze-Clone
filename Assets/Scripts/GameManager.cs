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

        private Level _levelLoaded,_LevelCleared;

        private void OnEnable() {
            Movement.OnLevelClearedEvent += OnLevelCleared;
            LevelManager.OnLevelLoadedEvent += OnLevelLoaded;
            UiManager.OnBallSelected += OnNewBall;
        }
        
        void Start()
        {
            Instance = this;
            LoadNewLevel(Level.LevelType.Level);
        }

        public void LoadNewLevel(Level.LevelType type)
        {
            int lastLevelOfThisType = 0;    //get this from saved progress
          
            
            _levelManager.LoadTilemap(type, lastLevelOfThisType + 1);
        }

        private void LoadNextLevel()
        {
            Debug.Log(_LevelCleared);
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
            
        }
        

        private void OnNewBall(Sprite ballSprite)
        {
            _ballSprite.sprite = ballSprite;
        }

    }
}
