using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private int _h = 4;
    [SerializeField] private int _w = 4;
    [SerializeField] private int _playersTurn = 0;
    [SerializeField] private int _aIsTurn = 1;
    [SerializeField] private int _nextTurnIndex = 0;
    [SerializeField] private UnityEvent<Vector3> _boxCapturedEvent;
    private Board _board;
    private int _numOfLinesTotal;
    private System.Random _randomizer = new System.Random();

    public int H => _h;
    public int W => _w;

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

    // Start is called before the first frame update
    void Start()
    {
        _board = new Board(_h, _w);
        _numOfLinesTotal = _h * (_w - 1) + _w * (_h - 1);

    }

    // Update is called once per frame
    void Update()
    {
        if (_nextTurnIndex == _aIsTurn)
        {
            AIsMove();
        }

        if (_board.AvailableLines.Count > 0)
        {
            // GAME OVER
        }
    }

    public void CaptureBox(Vector3 boxCoordAndCapturedBy)
    {
        _boxCapturedEvent.Invoke(boxCoordAndCapturedBy);
    }

    public void PlayersMove(Vector2 p1, Vector2 p2)
    {
        Tuple<Vector2, Vector2> lineToConnect;
        // If Vertical
        if (p1.x == p2.x)
        {
            lineToConnect = p1.y > p2.y ? 
                Tuple.Create(p2, p1) : Tuple.Create(p1, p2);
        }
        else
        {
            lineToConnect = p1.x > p2.x ? 
                Tuple.Create(p2, p1) : Tuple.Create(p1, p2);
        }
        _nextTurnIndex = _board.MakeMove(lineToConnect, _playersTurn, true);
    }

    public void AIsMove()
    {
        Tuple<Vector2, Vector2> chosenLine = null;
        if (_board.AvailableLines.Count >= (int)_numOfLinesTotal / 2)
        {
            // Complete the box if any
            if (_board.LastLineForBoxesWithThreeConnections.Any())
            {
                chosenLine = _board.LastLineForBoxesWithThreeConnections.Dequeue();
            }
            else
            {
                // Otherwise randomly choose
                for (int i = 0; i < 10; i++)
                {
                    var randomLine = _board.AvailableLines.ElementAt(
                        _randomizer.Next(_board.AvailableLines.Count));

                    // Check connections but don't connect the lines yet
                    int[] numConnections = _board.CheckBothBoxConnections(
                        false, _aIsTurn, randomLine, false);

                    if (numConnections[0] < 2 && numConnections[1] < 2)
                    {
                        chosenLine = randomLine;
                        break;
                    }
                }
            }
        }

        if (chosenLine == null)
        {
            (_, chosenLine) = MinMax.getScore(_board, 10, -100000, 100000, _aIsTurn);
        }
        _nextTurnIndex = _board.MakeMove(chosenLine, _aIsTurn, true);
        Debug.Log($"AI: {chosenLine}");
        LineController.Instance.MakeLine(chosenLine.Item1, chosenLine.Item2);

    }
}
