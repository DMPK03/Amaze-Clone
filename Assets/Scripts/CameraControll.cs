using UnityEngine;
using System.Collections;
 
namespace DM
{
    public class CameraControll : MonoBehaviour {
    
        [SerializeField] private Transform _player;

        private Color _darkMode = new Color(.18F,.18F,.18F,1);
        private Color _lightMode = new Color(.78f,.78f,.78f,1);
        [SerializeField] private TrailRenderer _trailRenderer;

        private Vector3 _offset = new Vector3(0, 0, -10);
        private Vector3 _velocity = Vector3.zero;
        private Camera _camera;
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
            _camera.orthographicSize = level.CameraSize;

            transform.position = level.Type == LevelType.Challenge? _player.position + _offset : level.CameraPosition;

            yield return new WaitForSeconds(.45f);  //wait for the grid animation
           _isMovable = level.Type == LevelType.Challenge;
        }



        private void LateUpdate () {
            if(_isMovable) transform.position = Vector3.SmoothDamp(transform.position, _player.position + _offset, ref _velocity, .1f);
        }
    }
}