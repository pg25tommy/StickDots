using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public static GridGenerator Instance;
    public BoxComplete boxCompleteScript;
    [SerializeField] Transform _boxBackground;
    [SerializeField] GameObject _drawPoint;
    [SerializeField] GameObject _box;
    [SerializeField] int _gridSize;
    private int _gridX;
    private int _gridY;
    [SerializeField] float _distance;
    [SerializeField] Vector3 _gridOrigin = Vector3.zero;
    private Vector3 _boxOrigin = new Vector3(0.5f, 0.5f, 0);
    [SerializeField] Vector3 _topRightPoint;
    private List<GameObject> _gridPoints = new List<GameObject>();
    private Camera _mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _mainCamera = FindFirstObjectByType<Camera>();
    }

    private void OnValidate()
    {
        boxCompleteScript = FindFirstObjectByType<BoxComplete>();
        _boxBackground = boxCompleteScript.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        _gridX = _gridSize;
        _gridY = _gridSize;

        GenerateGrid();
        GenerateBackgroundBoxes();
        SetCamera();
    }

    public void GenerateGrid()
    {
        GameObject Dots = new GameObject("Dots");
        for (int x = 0; x < _gridX; x++)
        {
            for (int y = 0; y < _gridY; y++)
            {
                Vector3 spawnLocation = new Vector3(
                    x * _distance, 
                    y * _distance, 
                    0f) + _gridOrigin;

                GameObject instance = Instantiate(
                    _drawPoint, 
                    spawnLocation, 
                    Quaternion.identity, 
                    Dots.transform);

                Dot dot = instance.GetComponent<Dot>();
                dot.dotCoord = new Vector2(x, y);
                _gridPoints.Add(instance);
            }
        }

        _topRightPoint = _gridPoints[_gridPoints.Count - 1].transform.position;
    }

    public void GenerateBackgroundBoxes()
    {
        for (int x = 0; x < _gridX - 1; x++)
        {
            for (int y = 0 ; y < _gridY - 1; y++)
            {
                Vector3 spawnLocation = 
                    new Vector3(y * _distance, x * _distance, 0f) + _boxOrigin;

                GameObject boxInstance = Instantiate(
                    _box, 
                    spawnLocation, 
                    Quaternion.identity,
                    _boxBackground);


                boxCompleteScript.boxes.Add(boxInstance.gameObject);
            }
        }
    }

    public void SetCamera()
    {
        _mainCamera.transform.position = Vector3.Lerp(_gridOrigin, _topRightPoint, 0.5f);
        _mainCamera.transform.position = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, -5f);
        _mainCamera.orthographicSize = _gridSize / _mainCamera.aspect;
    }
}

