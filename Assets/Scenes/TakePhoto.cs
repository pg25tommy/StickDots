using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePhoto : MonoBehaviour
{
    public RawImage cameraPreview; // Asigna el objeto RawImage en el Inspector

    private WebCamTexture webcamTexture;

    private void Start()
    {
        // Inicializa la cámara
        webcamTexture = new WebCamTexture();
        cameraPreview.texture = webcamTexture;
        webcamTexture.Play();
    }

    public void CapturePhoto()
    {
        // Captura la imagen (puedes guardarla como Texture2D o archivo PNG)
        Texture2D photo = new Texture2D(webcamTexture.width, webcamTexture.height);
        photo.SetPixels(webcamTexture.GetPixels());
        photo.Apply();

        // Aquí puedes usar 'photo' en tu proyecto
        // Por ejemplo, asignarla a un material o guardarla en disco
    }
}
