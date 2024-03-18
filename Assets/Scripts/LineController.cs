using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public static LineController Instance;
    private GameObject _lineParent;
    private Vector3[] _dotPositions = new Vector3[2];
    private Vector2 _p1;
    private Vector2 _p2;
    [SerializeField] private LineRenderer _lineRendererPrefab;

    //public Color validColor = Color.green;
    //public Color invalidColor = Color.red;
    private bool newLine = false;

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

    private void Start()
    {
        _lineParent = new GameObject("LineDrawings");
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
                    _dotPositions[0] = hit.collider.transform.position;
                    _p1 = hit.collider.transform.GetComponent<Dot>().DotCoord;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            newLine = false;
            // Check if the ray hits an object with the "dot" tag
            if (Physics.Raycast(ray, out hit))
            {
                // if raycast hit dot and 
                if (hit.collider != null && hit.collider.CompareTag("dot"))
                {
                    _dotPositions[1] = hit.collider.transform.position;
                    _p2 = hit.collider.transform.GetComponent<Dot>().DotCoord;
                    if (_p1.y == _p2.y && Mathf.Abs(_p1.x - _p2.x) == 1 ||
                        _p1.x == _p2.x && Mathf.Abs(_p1.y - _p2.y) == 1)
                    {
                        newLine = true;
                    }
                }
            }
        }

        if (newLine)
        {
            LineRenderer lineRenderer = Instantiate(_lineRendererPrefab, _lineParent.transform);
            lineRenderer.SetPositions(_dotPositions);
            Debug.Log($"Human: {_p1}, {_p2}");
            GameManager.Instance.PlayersMove(_p1, _p2);
            newLine = false;
        }
    }

    public void MakeLine(Vector2 p1, Vector2 p2)
    {
        LineRenderer lineRenderer = Instantiate(_lineRendererPrefab, _lineParent.transform);
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