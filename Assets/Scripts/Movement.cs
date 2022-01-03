using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

namespace DM
{

    public class Movement : MonoBehaviour
    {
        [SerializeField] Tilemap groundTilemap;
        [SerializeField] Tile originalTile;
        [SerializeField] Tile yellowTile;
        
        [SerializeField] ParticleSystem particleLocal;
        private Controls controls;
        private Animator animator;
        
        private float timeToMove = .2f;
        private int tilesToMove = 1;
        private bool isMoving;

        int movingHash;
        int directionHash;

        private void Awake() {
            controls = new Controls();
        }

        private void Start() {
            animator = GetComponent<Animator>();
            groundTilemap.SetTile(groundTilemap.WorldToCell(transform.position), yellowTile);   // set begining tile color, could also be done while creating the tilemap
            movingHash = Animator.StringToHash("moving"); //for performance
            directionHash = Animator.StringToHash("direction");
        }

        private void OnEnable() {
            controls.Gameplay.Enable();
            controls.Gameplay.Vertical.performed += OnMoveInputVertical;
            controls.Gameplay.Horizontal.performed += OnMoveInputHorizontal;
        }

        private void OnDisable() {
            controls.Gameplay.Disable();
            controls.Gameplay.Vertical.performed -= OnMoveInputVertical;
            controls.Gameplay.Horizontal.performed -= OnMoveInputHorizontal;
        }

        private void OnMoveInputVertical(InputAction.CallbackContext context)
        {
            Vector2 verticalInput = new Vector2 (0, context.ReadValue<float>());
            if(!isMoving) StartCoroutine(Move(verticalInput, 1));
        }

        private void OnMoveInputHorizontal(InputAction.CallbackContext context)
        {
            Vector2 horizontalInput = new Vector2 (context.ReadValue<float>(), 0);
            if(!isMoving) StartCoroutine(Move(horizontalInput, 0));
        }

        private bool CanMove(Vector3Int location)
        {
            return groundTilemap.HasTile(location);
        }

        private Vector3 GetPosition(Vector2 inputDirection, float direction)
        {
            if(CanMove(groundTilemap.WorldToCell(transform.position + (Vector3)inputDirection)))
            {
                bool willMove = true;
                tilesToMove = 1;
                animator.SetBool(movingHash, isMoving);
                animator.SetFloat(directionHash, direction);
                particleLocal.Play();
                while(willMove)
                {
                    if(CanMove(groundTilemap.WorldToCell(transform.position + (Vector3)inputDirection * (tilesToMove + 1)))){
                        tilesToMove+=1;
                    }
                    else    willMove = false;
                    if(tilesToMove > 25) break; //just in case 
                }
                return transform.position + (Vector3)inputDirection * tilesToMove;
            }
            return transform.position;
        }

        private IEnumerator Move(Vector2 inputDirection, float direction)
        {
            isMoving = true;
            Vector3 newPosition = GetPosition(inputDirection, direction);
            float elapsedTime = 0;
            while(elapsedTime < timeToMove)
            {
                transform.position = Vector3.Lerp(transform.position, newPosition, elapsedTime / (timeToMove * tilesToMove));
                groundTilemap.SetTile(groundTilemap.WorldToCell(transform.position), yellowTile);
                elapsedTime += Time.deltaTime;
                Debug.Log(groundTilemap.ContainsTile(originalTile));
                yield return null;
            }
            transform.position = newPosition; //just in case it doesnt line up perfectly
            isMoving = false;
            animator.SetBool(movingHash, isMoving);
        }
    }
}