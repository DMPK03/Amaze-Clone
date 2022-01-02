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
        [SerializeField] Tilemap wallTilemap;
        private bool isMoving;
        private int tilesToMove = 1;
        float timeToMove = .2f;
        [SerializeField] Tile yellowTile;

        private Controls controls;

        private void Awake() {
            controls = new Controls();
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
            if(!isMoving) StartCoroutine(Move(verticalInput));
        }

        private void OnMoveInputHorizontal(InputAction.CallbackContext context)
        {
            Vector2 horizontalInput = new Vector2 (context.ReadValue<float>(), 0);
            if(!isMoving) StartCoroutine(Move(horizontalInput));
        }

        private bool CanMove(Vector3Int location)
        {
            return groundTilemap.HasTile(location) && !wallTilemap.HasTile(location);
        }

        private Vector3 GetPosition(Vector2 inputDirection)
        {
            if(CanMove(groundTilemap.WorldToCell(transform.position + (Vector3)inputDirection)))
            {
                bool willMove = true;
                tilesToMove = 1;
                while(willMove)
                {
                    if(CanMove(groundTilemap.WorldToCell(transform.position + (Vector3)inputDirection * (tilesToMove + 1)))){
                        tilesToMove+=1;
                    }
                    else    willMove = false;
                    if(tilesToMove > 10) break;
                }
                return transform.position + (Vector3)inputDirection * tilesToMove;
            }
            return transform.position;
        }

        private IEnumerator Move(Vector2 inputDirection)
        {
            isMoving = true;
            Vector3 newPosition = GetPosition(inputDirection);
            float elapsedTime = 0;
            while(elapsedTime < timeToMove)
            {
                transform.position = Vector3.Lerp(transform.position, newPosition, elapsedTime / (timeToMove * tilesToMove));
                groundTilemap.SetTile(groundTilemap.WorldToCell(transform.position), yellowTile);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = newPosition;
            isMoving = false;
        }
    }
}