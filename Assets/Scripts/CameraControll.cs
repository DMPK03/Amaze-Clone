using UnityEngine;
using System.Collections;
 
namespace DM
{
    public class CameraControll : MonoBehaviour {
    
        [SerializeField] private Transform _player;

        private Color _darkMode = new Color(.18F,.18F,.18F,1);
        private Color _lightMode = new Color(.78f,.78f,.78f,1);

        private Camera _camera;
        private Vector3 _velocity = Vector3.zero;
        private Vector3 _offset = new Vector3(0, 0, -10);
        private bool _isMovable;

        private void Awake() {
            _camera = Camera.main;
            LevelManager.OnLevelLoadedEvent += OnLevelLoaded;
            Movement.OnLevelClearedEvent += OnLevelCleared;
        }


        private void OnLevelLoaded(Level level)
        {
            StartCoroutine(PositionCamera(level));
        }

        private void OnLevelCleared()
        {
            _isMovable = false;
        }

        public void OnDarkmodeToggle(bool darkmode)
        {
            _camera.backgroundColor = darkmode? _lightMode : _darkMode;
        }


        private IEnumerator PositionCamera(Level level)
        {
            Vector3 targetPosition = level.Type == LevelType.Challenge? _player.position + _offset : level.CameraPosition;
            
            
            float f = 0;
            while (f < .2f)
            {
                f += .01f; 
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, level.CameraSize, f/.2f);
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, targetPosition, f/.2f);
                yield return new WaitForSeconds(.01f);
            }

           _isMovable = level.Type == LevelType.Challenge;
        }



        private void LateUpdate () {
            if(_isMovable) transform.position = Vector3.SmoothDamp(transform.position, _player.position + _offset, ref _velocity, .1f);
        }
    }
}