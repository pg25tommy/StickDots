using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public static GridGenerator Instance;
    [SerializeField] private GameObject _drawPointPrefab;
    [SerializeField] private GameObject _boxBackgroundPrefab;
    [SerializeField] private float _distance;
    private BoxComplete _boxCompleteScript;
    private Transform _boxBackgroundsParent;
    private int _gridX;
    private int _gridY;
    private Vector3 _gridOrigin = Vector3.zero;
    private Vector3 _boxOrigin = new Vector3(0.5f, 0.5f, 0);
    private Vector3 _topRightPoint;
    private List<GameObject> _gridPoints = new List<GameObject>();
    private Camera _mainCamera;
    private CameraController _cameraController;
    private Bounds _bounds;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Get the main camera
        _mainCamera = FindFirstObjectByType<Camera>();

        // Get the camera controller
        _cameraController = FindFirstObjectByType<CameraController>();
    }

    private void OnValidate()
    {
        _boxCompleteScript = FindFirstObjectByType<BoxComplete>();
        _boxBackgroundsParent = _boxCompleteScript.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        _gridX = GamePlayManager.Instance.W;
        _gridY = GamePlayManager.Instance.H;

        GenerateGrid();
        GenerateBackgroundBoxes();
        SetCamera();
    }

    // Builds the grid, where _gridX is the number of dots wide, and _gridY is the number of dots tall
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
                    _drawPointPrefab, 
                    spawnLocation, 
                    Quaternion.identity, 
                    Dots.transform);

                Dot dot = instance.GetComponent<Dot>();
                dot.DotCoord = new Vector2(x, y);
                _gridPoints.Add(instance);
            }
        }

        // Gets the vector of the dot that opposes the origin and creates the bounding box size
        _topRightPoint = _gridPoints[_gridPoints.Count - 1].transform.position;
        _bounds.Encapsulate(_topRightPoint);
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
                    _boxBackgroundPrefab, 
                    spawnLocation, 
                    Quaternion.identity,
                    _boxBackgroundsParent);

                boxInstance.SetActive(false);

                _boxCompleteScript.BoxBackgrounds.Add(boxInstance.gameObject);
            }
        }
    }

    // Method that finds the camera controller and sets the starting location, min/max zoom, and movement restrictions of the camera
    public void SetCamera()
    {
        // Null check for the camera
        if (_mainCamera == null) _mainCamera = Camera.main;

        // Moves the camera to the center of the grid
        _mainCamera.transform.position = Vector3.Lerp(_gridOrigin, _topRightPoint, 0.5f);

        // Moves the camera backwards on the z axis to ensure the grid is visible
        _mainCamera.transform.position = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, -5f);

        // Null check for the camera controller
        if (_cameraController == null) _cameraController = FindFirstObjectByType<CameraController>();

        // Add specified starting size, zoom values, starting position, and movement restrictions of the camera
        _cameraController.SetCamera(_bounds.size.x, 
            Vector3.Lerp(_gridOrigin, _topRightPoint, 0.5f),
            1.5f, 
            _bounds.size.x,
            new Vector3(_gridOrigin.x - (_distance / 2), _gridOrigin.y - (_distance / 2), _mainCamera.transform.position.z),
            new Vector3(_topRightPoint.x + (_distance / 2), _topRightPoint.y + (_distance / 2), _mainCamera.transform.position.z));
    }
}

