using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public static LineController Instance;
    public Transform lineParent;
    public Vector3[] dotPositions = new Vector3[2];
    public Vector2 p1;
    public Vector2 p2;
    public LineRenderer lineRendererPrefab;

    //public Color validColor = Color.green;
    //public Color invalidColor = Color.red;
    public bool newLine = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits an object with the "dot" tag
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.CompareTag("dot"))
                {
                    dotPositions[0] = hit.collider.transform.position;
                    p1 = hit.collider.transform.GetComponent<Dot>().dotCoord;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits an object with the "dot" tag
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.CompareTag("dot"))
                {
                    dotPositions[1] = hit.collider.transform.position;
                    p2 = hit.collider.transform.GetComponent<Dot>().dotCoord;
                }
            }

            newLine = true;
        }

        if (newLine)
        {
            LineRenderer lineRenderer = Instantiate(lineRendererPrefab, lineParent);
            lineRenderer.SetPositions(dotPositions);
            Debug.Log($"Human: {p1}, {p2}");
            GameManager.Instance.PlayersMove(p1, p2);
            newLine = false;
        }
    }

    public void MakeLine(Vector2 p1, Vector2 p2)
    {
        LineRenderer lineRenderer = Instantiate(lineRendererPrefab, lineParent);
        Vector3[] dotsToConnect = new Vector3[2];
        dotsToConnect[0] = p1;
        dotsToConnect[1] = p2;
        lineRenderer.SetPositions(dotsToConnect);
    }

    // Animation 
    //public void StartLineAnimation()
    //{
    //    StartCoroutine(AnimateLine());
    //}

    //IEnumerator AnimateLine()
    //{
    //    for (float t = 0; t <= 1; t += 0.02f)
    //    {
    //        lr.widthCurve = AnimationCurve.Linear(0, 0.2f, 1, 0.2f);
    //        lr.widthMultiplier = 0.2f;
    //        yield return null;
    //    }
    //}
}