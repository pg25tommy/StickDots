using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Board board;
    public int h = 4;
    public int w = 4;
    public const int playersTurn = 0;
    public const int AIsTurn = 1;
    public int nextTurnIndex = 0;
    public int numOfLinesTotal;
    public System.Random randomizer = new System.Random();
    public UnityEvent<Vector3> BoxCapturedEvent;

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
        board = new Board(h, w);
        numOfLinesTotal = h * (w - 1) + w * (h - 1);

    }

    // Update is called once per frame
    void Update()
    {
        if (nextTurnIndex == AIsTurn)
        {
            AIsMove();
        }

        if (board.availableLines.Count > 0)
        {
            // GAME OVER
        }
    }

    public void CaptureBox(Vector3 boxCoordAndCapturedBy)
    {
        BoxCapturedEvent.Invoke(boxCoordAndCapturedBy);
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
        nextTurnIndex = board.MakeMove(lineToConnect, playersTurn, true);
    }

    public void AIsMove()
    {
        Tuple<Vector2, Vector2> chosenLine = null;
        if (board.availableLines.Count >= (int)numOfLinesTotal / 2)
        {
            // Complete the box if any
            if (board.lastLineForBoxesWithThreeConnections.Any())
            {
                chosenLine = board.lastLineForBoxesWithThreeConnections.Dequeue();
            }
            else
            {
                // Otherwise randomly choose
                for (int i = 0; i < 10; i++)
                {
                    var randomLine = board.availableLines.ElementAt(
                        randomizer.Next(board.availableLines.Count));

                    // Check connections but don't connect the lines yet
                    int[] numConnections = board.CheckBothBoxConnections(
                        false, AIsTurn, randomLine, false);

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
            (_, chosenLine) = MinMax.getScore(board, 10, -100000, 100000, AIsTurn);
        }
        nextTurnIndex = board.MakeMove(chosenLine, AIsTurn, true);
        Debug.Log($"AI: {chosenLine}");
        LineController.Instance.MakeLine(chosenLine.Item1, chosenLine.Item2);

    }
}
