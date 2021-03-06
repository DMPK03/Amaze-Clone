using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DM
{
    public class Movement : MonoBehaviour, IGameState
    {
        public static event Action OnLevelClearedEvent, OnMoveEvent;
        
        [SerializeField] private Tile _originalTile, _coloredTile;
        [SerializeField] private Tilemap _groundTilemap;
        [SerializeField] private ParticleSystem _hitParticle, _trailParticle;
        [SerializeField] private TrailRenderer _trail;
        
        private Animator _animator;
        private float _moveSpeed = 20f;
        private int _tilesToMove = 1;
        private bool _isMoving, _uiMode;
        private int _movingHash, _directionHash;

#region monobehaviour
        private void Start() {
            _animator = GetComponent<Animator>();
            _movingHash = Animator.StringToHash("moving");
            _directionHash = Animator.StringToHash("direction");
        }

        private void OnEnable() {
            UiManager.OnTgUiMode += OnUIMode;
        }

        private void OnDisable() {
            UiManager.OnTgUiMode -= OnUIMode;
        }

        private void Update()
        {
            if(!_isMoving && !_uiMode && Input.touchCount > 0) GetMovementDirection();
        }
#endregion
#region Movement
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
                if(_tilesToMove > 30) break;    //remove after testing
            }    
            return transform.position + inputDirection * _tilesToMove;
        }

        private IEnumerator Move(Vector3 inputDirection, float direction)
        {
            if(CanMove(_groundTilemap.WorldToCell(transform.position + inputDirection)))    // todo move check before starting routine
            {
                _isMoving = true;
                _tilesToMove = 1;

                Vector3 newPosition = GetPosition(inputDirection);
                _animator.SetBool(_movingHash, _isMoving);
                _animator.SetFloat(_directionHash, direction);

                while (transform.position != newPosition)
                {
                    float step = _moveSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, newPosition, step);
                    _groundTilemap.SetTile(_groundTilemap.WorldToCell(transform.position), _coloredTile);
                    yield return null;
                }
                
                if (!_groundTilemap.ContainsTile(_originalTile))
                {
                    OnLevelClearedEvent?.Invoke(); 
                    yield break;
                }

                OnMoveEvent?.Invoke();
                _hitParticle.Play();
                _animator.SetBool(_movingHash, false);
                
                yield return new WaitForSeconds(.13f);   // fake slam into wall delay
                _isMoving = false;
            }
        }

        private void OnUIMode(bool value) {_uiMode = value;}
#endregion
#region IGameState
        
        public void PrepareLevel(Level level)
        {            
            _animator.SetBool(_movingHash, false);
            transform.position = new Vector3(level.GroundTiles[0].Position.x + .5f, level.GroundTiles[0].Position.y + .5f, 0);
            _groundTilemap.SetTile(_groundTilemap.WorldToCell(transform.position), _coloredTile);
        }
        
        public void StartLevel(Level level)
        {
            _isMoving = false;
            _animator.SetBool(_movingHash, _isMoving);
            _trail.emitting = true;
            _trailParticle.Play();
        }

        public void ClearLevel()
        {
            _isMoving = true;
            _trail.emitting = false;
            _trailParticle.Stop();
        }
#endregion               
    }
}