using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LineSpriteController : MonoBehaviour
{
    [SerializeField] public GameObject line;
    [SerializeField] public GameObject startPoint;
    [SerializeField] public GameObject endPoint;
    public void SetScale(float scale)
    {
        Vector3 lineScale = gameObject.transform.localScale;
        lineScale.x = scale;
        lineScale.y = scale;
        gameObject.transform.localScale = lineScale;
    }

    public void SetLine(Vector2 start,Vector2 end)
    {
        Vector2 lookAtVector = end - start;
        float distance = lookAtVector.magnitude;
        Vector3 lineScale = line.transform.localScale;
        lineScale.x = distance;
        lineScale.y = distance;
        line.transform.localScale = lineScale;

        startPoint.transform.position = start; 
        endPoint.transform.position = end;
    }
}
