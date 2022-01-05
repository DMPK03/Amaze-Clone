using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

namespace DM
{

    public class Movement : MonoBehaviour
    {
        [SerializeField] Tilemap _groundTilemap;
        [SerializeField] Tile _originalTile, _yellowTile;
        [SerializeField] LevelManager _levelManager;
        
        [SerializeField] ParticleSystem _particle;
        Controls _controls;
        Animator _animator;
        
        float _timeToMove = .2f;
        int _tilesToMove = 1;
        bool _isMoving;

        int _movingHash, _directionHash;

        private void Awake() {
            _controls = new Controls();
        }

        private void Start() {
            _animator = GetComponent<Animator>();
            _groundTilemap.SetTile(_groundTilemap.WorldToCell(transform.position), _yellowTile);   // set begining tile color, could also be done while creating the tilemap
            _movingHash = Animator.StringToHash("moving"); //for performance
            _directionHash = Animator.StringToHash("direction");
        }

        private void OnEnable() {
            _controls.Gameplay.Enable();
            _controls.Gameplay.Vertical.performed += OnMoveInputVertical;
            _controls.Gameplay.Horizontal.performed += OnMoveInputHorizontal;
            LevelManager.OnMapLoadedEvent += OnNewMapLoaded;
        }

        private void OnDisable() {
            _controls.Gameplay.Disable();
            _controls.Gameplay.Vertical.performed -= OnMoveInputVertical;
            _controls.Gameplay.Horizontal.performed -= OnMoveInputHorizontal;
            LevelManager.OnMapLoadedEvent -= OnNewMapLoaded;
        }

        private void OnMoveInputVertical(InputAction.CallbackContext context)
        {
            Vector2 _verticalInput = new Vector2 (0, context.ReadValue<float>());
            if(!_isMoving) StartCoroutine(Move(_verticalInput, 1));
        }

        private void OnMoveInputHorizontal(InputAction.CallbackContext context)
        {
            Vector2 _horizontalInput = new Vector2 (context.ReadValue<float>(), 0);
            if(!_isMoving) StartCoroutine(Move(_horizontalInput, 0));
        }

        private bool CanMove(Vector3Int location)
        {
            return _groundTilemap.HasTile(location);
        }

        private Vector3 GetPosition(Vector2 inputDirection, float direction)
        {
            if(CanMove(_groundTilemap.WorldToCell(transform.position + (Vector3)inputDirection)))
            {
                bool willMove = true;
                _tilesToMove = 1;
                _animator.SetBool(_movingHash, _isMoving);
                _animator.SetFloat(_directionHash, direction);
                _particle.Play();
                while(willMove)
                {
                    if(CanMove(_groundTilemap.WorldToCell(transform.position + (Vector3)inputDirection * (_tilesToMove + 1)))){
                        _tilesToMove+=1;
                    }
                    else    willMove = false;
                    if(_tilesToMove > 25) break; //just in case 
                }
                return transform.position + (Vector3)inputDirection * _tilesToMove;
            }
            return transform.position;
        }

        private IEnumerator Move(Vector2 inputDirection, float direction)
        {
            _isMoving = true;
            Vector3 newPosition = GetPosition(inputDirection, direction);
            float elapsedTime = 0;
            while(elapsedTime < _timeToMove)
            {
                transform.position = Vector3.Lerp(transform.position, newPosition, elapsedTime / (_timeToMove * _tilesToMove));
                _groundTilemap.SetTile(_groundTilemap.WorldToCell(transform.position), _yellowTile);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = newPosition; //just in case it doesnt line up perfectly
            _isMoving = false;
            _animator.SetBool(_movingHash, _isMoving);
            if(!_groundTilemap.ContainsTile(_originalTile)) _levelManager.LoadNewLevel();
        }

        private void OnNewMapLoaded(Vector3 value)
        {
            _groundTilemap.SetTile(_groundTilemap.WorldToCell(value), _yellowTile);
            Vector3 newPos = new Vector3(value.x + .5f, value.y + .5f, 0);
            transform.position = newPos;
        }
    }
}