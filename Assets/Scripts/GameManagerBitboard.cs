using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class GameManagerBitboard : MonoBehaviour
{
    public static GameManagerBitboard Instance;
    [SerializeField] private int _h = 4;
    [SerializeField] private int _w = 4;
    [SerializeField] private int _playersTurn = 0;
    [SerializeField] private int _aIsTurn = 1;
    [SerializeField] private int _nextTurnIndex = 0;
    [SerializeField] private UnityEvent<Vector3> _boxCapturedEvent;
    private Bitboard _board;
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
        _board = new Bitboard(_h, _w);
        _numOfLinesTotal = _h * (_w - 1) + _w * (_h - 1);

    }

    // Update is called once per frame
    void Update()
    {
        if (_nextTurnIndex == _aIsTurn)
        {
            AIsMove();
        }

    }

    public void CaptureBox(Vector3 boxCoordAndCapturedBy)
    {
        _boxCapturedEvent.Invoke(boxCoordAndCapturedBy);
    }

    public void PlayersMove(Vector2 p1, Vector2 p2)
    {
        int boxIndex;
        int lineIndex;
        // If Vertical
        if (p1.x == p2.x)
        {
            boxIndex = p1.y > p2.y ?
                _board.GetBoxIndexFromVectors(p2, p1) : 
                _board.GetBoxIndexFromVectors(p1, p2);
            lineIndex = (int) BitIndex.Right;
        }
        else
        {
            boxIndex = p1.x > p2.x ?
                _board.GetBoxIndexFromVectors(p2, p1) :
                _board.GetBoxIndexFromVectors(p1, p2);
            lineIndex = (int)BitIndex.Top; ;
        }
        _nextTurnIndex = _board.MakeMove(boxIndex, lineIndex, _playersTurn, true);
    }

    public void AIsMove()
    {
        int choosenBox = -1;
        int choosenLine = -1;
        //if (_board.AvailableLines.Count >= (int)_numOfLinesTotal / 2)
        //{
        //    // Complete the box if any
        //    if (_board.LastLineForBoxesWithThreeConnections.Any())
        //    {
        //        chosenLine = _board.LastLineForBoxesWithThreeConnections.Dequeue();
        //    }
        //    else
        //    {
        //        // Otherwise randomly choose
        //        for (int i = 0; i < 10; i++)
        //        {
        //            var randomLine = _board.AvailableLines.ElementAt(
        //                _randomizer.Next(_board.AvailableLines.Count));

        //            // Check connections but don't connect the lines yet
        //            int[] numConnections = _board.CheckBothBoxConnections(
        //                false, _aIsTurn, randomLine, false);

        //            if (numConnections[0] < 2 && numConnections[1] < 2)
        //            {
        //                chosenLine = randomLine;
        //                break;
        //            }
        //        }
        //    }
        //}

        if (choosenBox == -1)
        {
            (_, choosenBox, choosenLine) = MiniMaxBitboard.GetScore(_board, 10, -100000, 100000, _aIsTurn);
        }
        _nextTurnIndex = _board.MakeMove(choosenBox, choosenLine, _aIsTurn, true);
        (Vector2 p1, Vector2 p2) = _board.GetVectorsFromBoxAndLineIndex(choosenBox, choosenLine);
        Debug.Log($"AI: {p1}, {p2}");
        LineController.Instance.MakeLine(p1, p2);

    }
}
