using NUnit;
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
    [SerializeField] public AnimationCurve _animationCurve;

    

    private bool strinking = false;
    private float length = 0.0f;
    private float timer = 0.0f;


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

    public void SetLength(float scaledLength)
    {
        Vector3 lineScale = line.transform.localScale;
        lineScale.x = scaledLength;
        line.transform.localScale = lineScale;

        Vector3 endPos = endPoint.transform.localPosition;
        endPos.x = scaledLength;
        endPoint.transform.localPosition = endPos;

    }

    public void SetLine(Vector2 start,Vector2 end)
    {
        Vector2 lookAtVector = end - start;
        length = lookAtVector.magnitude;
        Vector3 lineScale = line.transform.localScale;
        lineScale.x = length;
        line.transform.localScale = lineScale;


        transform.position = start;
        transform.right = end - start;

        endPoint.transform.position = end;
    }

    public void startStrinking()
    {
        strinking = true;
    }

    public void stopStrinking()
    {
        strinking = false;
        timer = 0;
    }

    public void Update()
    {
        if(strinking)
        {
            float scaledMultiplier = _animationCurve.Evaluate(timer);
            SetLength(scaledMultiplier * length);

            if(scaledMultiplier <= 0)
            {
                stopStrinking();
            }

            timer += Time.deltaTime;
        }
    }
}
