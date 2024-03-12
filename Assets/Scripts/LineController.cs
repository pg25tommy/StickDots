using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;
    private bool isDragging = false;
    public List<Transform> points = new List<Transform>();
    public Transform lastPoints;

    public Color validColor = Color.green;
    public Color invalidColor = Color.red;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }

    // Function to add a point to the line
    private void MakeLine(Transform point)
    {
        // Add the new point to the list of points
        points.Add(point);

        // Enable the LineRenderer and update the line positions
        lr.enabled = true;
        SetupLine();
    }

    // Function to update the line positions based on the points
    private void SetupLine()
    {
        int pointLength = points.Count;

        // Set the number of positions in the LineRenderer to match the number of points
        lr.positionCount = pointLength;

        // Update the positions of the line to match the points
        for (int i = 0; i < pointLength; i++)
        {
            lr.SetPosition(i, points[i].position);
        }
    }

    void Update()
    {
        // Check for mouse button down to start dragging
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;

            // Cast a ray from the mouse position to interact with objects
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits an object with the "dot" tag
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.CompareTag("dot"))
                {
                    // Start or continue the line with the hit point
                    MakeLine(hit.collider.transform);
                }
            }
        }

        // Check for mouse button held down to continue dragging
        if (isDragging && Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.CompareTag("dot"))
                {
                    // Continue the line with the hit point
                    MakeLine(hit.collider.transform);
                }
            }
        }

        // Check for mouse button release to stop dragging
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    // Animation 
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