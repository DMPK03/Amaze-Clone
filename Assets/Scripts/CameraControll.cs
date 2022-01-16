using UnityEngine;
 
public class CameraControll : MonoBehaviour {
 
    [SerializeField] private Transform player;

    private Camera _camera;
    private Vector3 _normalPos;
    private Vector3 _offset = new Vector3(0, 1.5f, -10);
    private Vector3 _velocity = Vector3.zero;

    private void Awake() {
        _camera = Camera.main;
        _normalPos = transform.position;
        this.enabled = false;
    }

    private void OnEnable() {
        SetupCamera();
    }
 
    private void OnDisable() {
        RestoreCamera();
    }

    private void SetupCamera()
    {
        _camera.orthographicSize = 8;
        transform.position = _offset + player.position;
    }

    private void RestoreCamera()
    {
        _camera.orthographicSize = 12;
        transform.position = _normalPos;
    }

    private void LateUpdate () {
        transform.position = Vector3.SmoothDamp(transform.position, player.position + _offset, ref _velocity, .1f);
    }
}
