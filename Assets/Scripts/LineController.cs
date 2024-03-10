using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;
    private bool isMouseClicked = false;
    public List<Transform> points = new List<Transform>();
    public Transform lastPoints;
    public Color validColor = Color.green;
    public Color invalidColor = Color.red;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }

    private void MakeLine(Transform finalPoint)
    {
        if (lastPoints == null)
        {
            lastPoints = finalPoint;
            points.Add(lastPoints);
        }
        else
        {
            points.Add(finalPoint);
            lr.enabled = true;
            SetupLine();
        }
    }

    private void SetupLine()
    {
        int pointLength = points.Count;
        lr.positionCount = pointLength;
        for (int i = 0; i < pointLength; i++)
        {
            lr.SetPosition(i, points[i].position);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isMouseClicked)
        {
            isMouseClicked = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.CompareTag("dot"))
                {
                    MakeLine(hit.collider.transform);
                    Debug.Log(hit.collider.name);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseClicked = false;
        }
    }

    public void StartLineAnimation()
    {
        StartCoroutine(AnimateLine());
    }

    IEnumerator AnimateLine()
    {
        for (float t = 0; t <= 1; t += 0.02f)
        {
            lr.widthCurve = AnimationCurve.Linear(0, 0.2f, 1, 0.2f);
            lr.widthMultiplier = 0.2f;
            yield return null;
        }
    }
}