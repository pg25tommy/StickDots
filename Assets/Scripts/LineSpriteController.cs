using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LineSpriteController : MonoBehaviour
{
    [SerializeField] public GameObject line;
    [SerializeField] public GameObject startPoint;
    [SerializeField] public GameObject endPoint;
    [SerializeField] public float thickness;

    private void Start()
    {
        //Vector3 lineScale = line.transform.localScale;
        Vector3 lineScale = line.transform.localScale;
        lineScale.y = thickness;
        line.transform.localScale = lineScale;
        Vector3 dotScale = startPoint.transform.localScale;
        dotScale.x = thickness;
        dotScale.y = thickness;
        startPoint.transform.localScale = dotScale;
        endPoint.transform.localScale = dotScale;
        
    }


    public void SetLine(Vector2 start,Vector2 end)
    {
        Vector2 lookAtVector = end - start;
        float distance = lookAtVector.magnitude;
        Vector3 lineScale = line.transform.localScale;
        lineScale.x = distance;
        line.transform.localScale = lineScale;


        startPoint.transform.position = start;
        line.transform.position = start;
        //line.transform.LookAt(lookAtVector);
        transform.right = end - start;


        endPoint.transform.position = end;
    }
}
