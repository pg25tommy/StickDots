using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCamera : MonoBehaviour
{
    public Camera frontCamera; // Asigna la cámara frontal en el Inspector
    public Camera rearCamera; // Asigna la cámara trasera en el Inspector

    private bool isFrontCameraActive = true;

    private void Start()
    {
        // Activa la cámara frontal al inicio
        frontCamera.enabled = true;
        rearCamera.enabled = false;
    }

    public void SwitchCamera()
    {
        // Alterna entre cámaras
        isFrontCameraActive = !isFrontCameraActive;
        frontCamera.enabled = isFrontCameraActive;
        rearCamera.enabled = !isFrontCameraActive;
    }
}
