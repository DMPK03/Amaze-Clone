using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DM
{

    public class Movement : MonoBehaviour
    {
        [SerializeField] Tilemap _groundTilemap;
        [SerializeField] Tile _originalTile, _coloredTile;
        [SerializeField] LevelManager _levelManager;
        [SerializeField] ParticleSystem _particle;
        Animator _animator;
        
        float _moveSpeed = 20f;
        int _tilesToMove = 1;
        bool _isMoving;

        int _movingHash, _directionHash;

        private void Start() {
            _animator = GetComponent<Animator>();
            _groundTilemap.SetTile(_groundTilemap.WorldToCell(transform.position), _coloredTile);   // set begining tile color, could also be done while creating the tilemap
            _movingHash = Animator.StringToHash("moving");
            _directionHash = Animator.StringToHash("direction");
        }

        private void OnEnable() {
            LevelManager.OnLevelLoadedEvent += OnNewLevelLoadedEvent;
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
            Touch touch = Input.GetTouch(0);
            if (Vector2.SqrMagnitude(touch.deltaPosition) > 55)
            {
                if (Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y))
                {
                    float inputX = touch.deltaPosition.x / Mathf.Abs(touch.deltaPosition.x);
                    StartCoroutine(Move(new Vector3(inputX, 0, 0), 0));
                }
                else
                {
                    float inputY = touch.deltaPosition.y / Mathf.Abs(touch.deltaPosition.y);
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

                _isMoving = false;
                _animator.SetBool(_movingHash, _isMoving);

                if (!_groundTilemap.ContainsTile(_originalTile)) _levelManager.LoadNewLevel();   //todo  transform into levelEnd Event!
            }
        }

        private void StartAnimationsAndParticles(float direction)
        {
            _animator.SetBool(_movingHash, _isMoving);
            _animator.SetFloat(_directionHash, direction);
            _particle.emission.SetBurst(0, new ParticleSystem.Burst(0, 25, 25, _tilesToMove + 2, .04f));
            _particle.Play();
        }

        private void OnNewLevelLoadedEvent(Vector3 value)
        {
            Vector3 newPos = new Vector3(value.x + .5f, value.y + .5f, 0);
            transform.position = newPos;
            _groundTilemap.SetTile(_groundTilemap.WorldToCell(value), _coloredTile);
        }

        
    }
}