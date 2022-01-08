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
        [SerializeField] Tile _originalTile, _coloredTile;
        [SerializeField] LevelManager _levelManager;        
        
        [SerializeField] ParticleSystem _particle;
        Controls _controls;
        Animator _animator;
        
        float _moveSpeed = 20f;
        int _tilesToMove = 1;
        bool _isMoving;

        int _movingHash, _directionHash;

        private void Awake() {
            _controls = new Controls();
        }

        private void Start() {
            _animator = GetComponent<Animator>();
            _groundTilemap.SetTile(_groundTilemap.WorldToCell(transform.position), _coloredTile);   // set begining tile color, could also be done while creating the tilemap
            _movingHash = Animator.StringToHash("moving"); //for performance
            _directionHash = Animator.StringToHash("direction");
        }

        private void OnEnable() {
            _controls.Gameplay.Enable();
            _controls.Gameplay.Vertical.performed += OnMoveInputVertical;
            _controls.Gameplay.Horizontal.performed += OnMoveInputHorizontal;
            LevelManager.OnLevelLoadedEvent += OnNewLevelLoadedEvent;
        }

        private void OnDisable() {
            _controls.Gameplay.Disable();
            _controls.Gameplay.Vertical.performed -= OnMoveInputVertical;
            _controls.Gameplay.Horizontal.performed -= OnMoveInputHorizontal;
            LevelManager.OnLevelLoadedEvent -= OnNewLevelLoadedEvent;
        }

        private void OnMoveInputVertical(InputAction.CallbackContext context)
        {
            Vector2 _verticalInput = new Vector2 (0, context.ReadValue<float>());
            if(!_isMoving) StartCoroutine(Move((Vector3)_verticalInput, 1));
        }

        private void OnMoveInputHorizontal(InputAction.CallbackContext context)
        {
            Vector2 _horizontalInput = new Vector2 (context.ReadValue<float>(), 0);
            if(!_isMoving) StartCoroutine(Move((Vector3)_horizontalInput, 0));
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