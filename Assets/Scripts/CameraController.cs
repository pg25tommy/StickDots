using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    //Initial spot of the camera
    private Vector3 _origin;

    //Calculated difference of the camera distance
    private Vector3 _difference;

    //Reference of the camera
    private Camera _mainCamera;

    private Coroutine _zoomCoroutine;

    [SerializeField] private float _cameraSpeed = 4f;

    //Check to see if the user is dragging the camera around
    private bool _isDragging;

    //Player Touch Inputs 
    private PlayerInputs _controls;

    //Awake method to initialize the camera
    private void Awake()
    {
        _mainCamera = Camera.main;
        _controls = new PlayerInputs();
    }

    //Function that enables touch controls
    private void OnEnable()
    {
        _controls.Enable();
    }

    //Function that disable touch controls
    private void OnDisable()
    {
        _controls.Disable();
    }

    //Start function where we detect the touch controls
    private void Start()
    {
        _controls.CameraMovement.SecondaryTouchContact.started += _ => ZoomStart();
        _controls.CameraMovement.SecondaryTouchContact.canceled += _ => ZoomEnd();
        _controls.CameraMovement.PrimaryTouchContact.canceled += _ => ZoomEnd();
    }

    private void ZoomStart()
    {
        _zoomCoroutine = StartCoroutine(ZoomDetection());
    } 
    
    private void ZoomEnd()
    {
        StopCoroutine(_zoomCoroutine);
    }
    
    IEnumerator ZoomDetection()
    {
        float previousDistance = 0, distance = 0f;

        while (true)
        {
            distance = Vector2.Distance(_controls.CameraMovement.PrimaryFingerPosition.ReadValue<Vector2>(), _controls.CameraMovement.SecondaryFingerPosition.ReadValue<Vector2>());

            if (distance > previousDistance && _mainCamera.orthographicSize > 2.0f)
            {
                _mainCamera.orthographicSize -= Time.deltaTime * _cameraSpeed;
            }

            else if(distance < previousDistance && _mainCamera.orthographicSize < 10.0f)
            {
                _mainCamera.orthographicSize += Time.deltaTime * _cameraSpeed;
            }

            previousDistance = distance;
            yield return null;
        }
    }



    //Method that drags the camera around the scene
    public void OnDrag(InputAction.CallbackContext ctx)
    {
        if (ctx.started) _origin = GetMousePosition();
        _isDragging = ctx.started || ctx.performed;
    }

    //Late update method to check if the user is dragging the camera and updates its position
    private void LateUpdate()
    {
        if (!_isDragging) return;
        
        _difference = GetMousePosition() - transform.position;
        transform.position = _origin - _difference;
    }

    //Method that retrieves the current mouse position
    private Vector3 GetMousePosition()
    {
        Vector3 MousePos = Mouse.current.position.ReadValue();

        MousePos.z = 1;

        return _mainCamera.ScreenToWorldPoint(MousePos);

    }
}
