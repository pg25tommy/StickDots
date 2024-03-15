using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] GameObject _drawPoint;
    [SerializeField] int _gridSize;
    private int _gridX;
    private int _gridY;
    [SerializeField] float _distance;
    [SerializeField] Vector3 _gridOrigin = Vector3.zero;
    [SerializeField] Vector3 _topRightPoint;
    private List<GameObject> _gridPoints = new List<GameObject>();
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = FindFirstObjectByType<Camera>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _gridX = _gridSize;
        _gridY = _gridSize;

        GenerateGrid();

        SetCamera();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < _gridX; x++)
        {
            for (int y = 0; y < _gridY; y++)
            {
                Vector3 spawnLocation = new Vector3(x * _distance, y * _distance, 0f) + _gridOrigin;
                GameObject instance = Instantiate(_drawPoint, spawnLocation, Quaternion.identity);
               _gridPoints.Add(instance);
            }
        }

        _topRightPoint = _gridPoints[_gridPoints.Count - 1].transform.position;
    }

    void SetCamera()
    {
        _mainCamera.transform.position = Vector3.Lerp(_gridOrigin, _topRightPoint, 0.5f);
        _mainCamera.transform.position = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, -5f);
        _mainCamera.orthographicSize = _gridSize / _mainCamera.aspect;
    }
}

