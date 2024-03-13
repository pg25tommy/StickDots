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

    //Check to see if the user is dragging the camera around
    private bool _isDragging;

    //Awake method to initialize the camera
    private void Awake()
    {
        _mainCamera = Camera.main;
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
