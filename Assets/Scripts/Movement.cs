using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DM
{

    public class Movement : MonoBehaviour
    {
        public delegate void MOVEMENT();
        public static event MOVEMENT OnLevelClearedEvent;
        public static event MOVEMENT OnMoveEvent;
        
        [SerializeField] Tilemap _groundTilemap;
        [SerializeField] Tile _originalTile, _coloredTile;
        [SerializeField] LevelManager _levelManager;
        [SerializeField] ParticleSystem _particle;
        Animator _animator;
        
        float _moveSpeed = 20f;
        int _tilesToMove = 1;
        bool _isMoving, _vibrate;

        int _movingHash, _directionHash;

        private void Start() {
            _animator = GetComponent<Animator>();
            _movingHash = Animator.StringToHash("moving");
            _directionHash = Animator.StringToHash("direction");
        }

        private void OnEnable() {
            LevelManager.OnLevelLoadedEvent += OnNewLevelLoadedEvent;
            UiManager.OnTgUiMode += OnUIMode;
            UiManager.OnTgVibrate += OnVibrate;
        }

        private void OnDisable() {
            LevelManager.OnLevelLoadedEvent -= OnNewLevelLoadedEvent;
        }

        private void Update()
        {
            if(!_isMoving && Input.touchCount > 0) GetMovementDirection();
        }

        private void GetMovementDirection() //todo better way to detect swipe, works for testing
        {
            Vector2 touch = Input.GetTouch(0).deltaPosition;
            if (Vector2.SqrMagnitude(touch) > 100)
            {
                if (Mathf.Abs(touch.x) > Mathf.Abs(touch.y))
                {
                    float inputX = touch.x / Mathf.Abs(touch.x);
                    StartCoroutine(Move(new Vector3(inputX, 0, 0), 0));
                }
                else
                {
                    float inputY = touch.y / Mathf.Abs(touch.y);
                    StartCoroutine(Move(new Vector3(0, inputY, 0), 1));
                }
            }
        }

        private bool CanMove(Vector3Int location)
        {
            return _groundTilemap.HasTile(location);
        }

        private Vector3 GetPosition(Vector3 inputDirection)
        {
            while(CanMove(_groundTilemap.WorldToCell(transform.position + inputDirection * (_tilesToMove + 1))))
            {
                _tilesToMove+=1;
                if(_tilesToMove > 20) break;
            }    
            return transform.position + inputDirection * _tilesToMove;
        }

        private IEnumerator Move(Vector3 inputDirection, float direction)
        {
            if(CanMove(_groundTilemap.WorldToCell(transform.position + inputDirection)))
            {
                _isMoving = true;
                _tilesToMove = 1;

                Vector3 newPosition = GetPosition(inputDirection);
                StartAnimationsAndParticles(direction);

                while (transform.position != newPosition)
                {
                    float step = _moveSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, newPosition, step);
                    _groundTilemap.SetTile(_groundTilemap.WorldToCell(transform.position), _coloredTile);
                    yield return null;
                }

                OnMoveEvent?.Invoke();
                if(_vibrate) Handheld.Vibrate();

                if (!_groundTilemap.ContainsTile(_originalTile)) OnLevelClearedEvent?.Invoke();  
                else {
                    _isMoving = false;
                    _animator.SetBool(_movingHash, _isMoving);
                }
            }
        }

        private void StartAnimationsAndParticles(float direction)
        {
            _animator.SetBool(_movingHash, _isMoving);
            _animator.SetFloat(_directionHash, direction);
            _particle.emission.SetBurst(0, new ParticleSystem.Burst(0, 25, 25, _tilesToMove + 2, .04f));
            _particle.Play();
        }

        private void OnNewLevelLoadedEvent(Level level)
        {
            Vector3 newPos = new Vector3(level.GroundTiles[0].Position.x + .5f, level.GroundTiles[0].Position.y + .5f, 0);
            transform.position = newPos;
            _isMoving = false;
            _animator.SetBool(_movingHash, _isMoving);
        }

        private void OnUIMode(bool value)
        {
            _isMoving = value;
        }

        private void OnVibrate(bool value)
        {
            _vibrate = value;
        }
        
    }
}