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
    // Start is called before the first frame update
    void Start()
    {
        _gridX = _gridSize;
        _gridY = _gridSize;

        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < _gridX; x++)
        {
            for (int y = 0; y < _gridY; y++)
            {
                Vector3 spawnLocation = new Vector3(x * _distance, y * _distance, 0f) + _gridOrigin;
                Instantiate(_drawPoint, spawnLocation, Quaternion.identity);
            }
        }
    }
}
