using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Board board;
    public int h = 4;
    public int w = 4;
    public const int playersTurn = 0;
    public const int AIsTurn = 1;
    public int nextTurnIndex = 1;
    public int numOfLinesTotal;
    public System.Random randomizer = new System.Random();

    // Start is called before the first frame update
    void Start()
    {
        board = new Board(h, w);
        numOfLinesTotal = h * (w - 1) + w * (h - 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (nextTurnIndex == playersTurn)
        {
            PlayersMove();
        }
        else
        {
            AIsMove();
        }

        if (board.availableLines.Count > 0)
        {
            // GAME OVER
        }
    }

    public void PlayersMove()
    {
        Console.WriteLine("YOUR TURN");
        string input = Console.ReadLine();
        var items = input.Split(',');
        var numbers = new List<int>();
        foreach (string item in items)
            if (int.TryParse(item, out var number))
                numbers.Add(number);
        Vector2 p1 = new Vector2(numbers[0], numbers[1]);
        Vector2 p2 = new Vector2(numbers[2], numbers[3]);
        var lineToConnect = Tuple.Create(p1, p2);
        nextTurnIndex = board.MakeMove(lineToConnect, playersTurn);
    }

    public void AIsMove()
    {
        Console.WriteLine("AI's TURN");

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
                        false, AIsTurn, randomLine);

                    if (numConnections[0] < 2 && numConnections[1] < 2)
                    {
                        chosenLine = randomLine;
                    }
                }
            }
        }

        if (chosenLine == null)
        {
            (_, chosenLine) = MinMax.getScore(board, 10, -100000, 100000, AIsTurn);
        }
        nextTurnIndex = board.MakeMove(chosenLine, AIsTurn);
        Console.WriteLine(
            chosenLine.Item1.x.ToString() + ", " +
            chosenLine.Item1.y.ToString() + ", " +
            chosenLine.Item2.x.ToString() + ", " +
            chosenLine.Item2.y.ToString());
    }
}
