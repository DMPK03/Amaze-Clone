using UnityEngine;
 
namespace DM
{
    public class CameraControll : MonoBehaviour, IGameState
    {    
        [SerializeField] private Transform _player;
        [SerializeField] private TrailRenderer _trailRenderer;

        private Color _darkMode = new Color(.18F,.18F,.18F,1);
        private Color _lightMode = new Color(1,1,1,1);

        private Vector3 _offset = new Vector3(0, 0, -10);
        private Vector3 _velocity = Vector3.zero;
        private Camera _camera;
        private bool _isMovable;

        private void Awake() {
            _camera = Camera.main;
        }

#region IGameState
        public void PrepareLevel(Level level)
        {
            _camera.orthographicSize = level.CameraSize;
            transform.position = level.Type == LevelType.Challenge? _player.position + _offset : level.CameraPosition;
        }
        
        public void StartLevel(Level level)
        {
            _isMovable = level.Type == LevelType.Challenge;
        }

        public void ClearLevel()
        {
            _isMovable = false;
        }
#endregion       
        public void OnDarkmodeToggle(bool darkmode)
        {
            _camera.backgroundColor = darkmode? _lightMode : _darkMode;
        }

        private void LateUpdate () {
            if(_isMovable) transform.position = Vector3.SmoothDamp(transform.position, _player.position + _offset, ref _velocity, .1f);
        }
    }
}