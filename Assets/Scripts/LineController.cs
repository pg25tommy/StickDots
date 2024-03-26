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
    private LineSpriteController _lineDrawableSpriteController;
    //[SerializeField] private LineRenderer _lineRendererPrefab;
    [SerializeField] private GameObject _LineDrawablePrefab;
    [SerializeField] private GameObject _LineDrawable;
    [SerializeField] private GameObject _LineStaticPrefab;


    //public Color validColor = Color.green;
    //public Color invalidColor = Color.red;
    private bool drawing = false;

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

        //instantiate lineDrawable
        if (_LineDrawable == null)
        {
            _LineDrawable = Instantiate(_LineDrawablePrefab);
            _lineDrawableSpriteController = _LineDrawable.GetComponent<LineSpriteController>();
            _LineDrawable.SetActive(false);
        }

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
                    //record first position
                    _dotPositions[0] = hit.collider.transform.position;
                    _p1 = hit.collider.transform.GetComponent<Dot>().DotCoord;

                    //draw sprite line from position to mouse
                    _LineDrawable.SetActive(true);
                    drawing = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            //stop drawing lineDrawable
            drawing = false;

            //strink lineDrawable
            _lineDrawableSpriteController.startStrinking();
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits an object with the "dot" tag
            if (Physics.Raycast(ray, out hit))
            {
                // if raycast hit dot and 
                if (hit.collider != null && hit.collider.CompareTag("dot"))
                {
                    //record second position
                    _dotPositions[1] = hit.collider.transform.position;
                    _p2 = hit.collider.transform.GetComponent<Dot>().DotCoord;

                    //drawline if distance between two dots position is 1
                    if (_p1.y == _p2.y && Mathf.Abs(_p1.x - _p2.x) == 1 ||
                        _p1.x == _p2.x && Mathf.Abs(_p1.y - _p2.y) == 1)
                    {
                        MakeLine(_dotPositions[0], _dotPositions[1]);


                        Debug.Log($"Human: {_p1}, {_p2}");
                        GameManager.Instance.PlayersMove(_p1, _p2);

                        //hide lineDrawable if creating new line
                        _LineDrawable.SetActive(false);
                    }
                }
            }
            
        }

        if ( drawing )
        {
            //safty check
            if (_LineDrawable.activeSelf)
            {
                //draw lineDrawable
                _lineDrawableSpriteController.SetLine(_dotPositions[0], mousePositionOnProjection());
            }
        }
    }

    /*
    ************* Input Event *****************
    */
    

    /*
    *******************************************
    */

    Vector3 mousePositionOnProjection()
    {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = Camera.main.orthographicSize;

        return Camera.main.ScreenToWorldPoint(screenPoint);
    }


    public void MakeLine(Vector2 p1, Vector2 p2)
    {
        /*
        LineRenderer lineRenderer = Instantiate(_lineRendererPrefab, _lineParent.transform);
        */
        GameObject lineSprite = Instantiate(_LineStaticPrefab, _lineParent.transform);
        //Vector3[] dotsToConnect = new Vector3[2];
        //dotsToConnect[0] = p1;
        //dotsToConnect[1] = p2;
        lineSprite.GetComponent<LineSpriteController>().SetLine(p1, p2);
        //Debug.Log($"{p1} , { p2} ");
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