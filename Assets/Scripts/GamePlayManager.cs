using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GamePlayManager : MonoBehaviour
{
    [SerializeField] private int _h = 4;
    [SerializeField] private int _w = 4;
    [SerializeField] public PlayerColor[] playerColor;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private GameObject playerContainer;
    public Player[] players;
    public int currentPlayerIndex { get; private set; } = 0;
    public static GamePlayManager Instance { get; private set; }
    private int playerCount;
    private Board _board;
    [SerializeField] private UnityEvent<Vector3> _boxCapturedEvent;

    public int PlayersCount => playerCount;
    public int H => _h;
    public int W => _w;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        InitailizePlayers();
        StartTurn();
        _board = new Board(_h, _w);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            UIManager.Instance.IndicatorColorSwitch(players[currentPlayerIndex].myColor);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }
    }
    void InitailizePlayers()
    {
        Debug.Log("InitailizePlayers");
        playerCount = playerColor.Length;
        players = new Player[playerCount];
        if (players.Length == 0) { return; }
        for (int i = 0; i < playerCount; i++)
        {
            if (playerPrefab != null)
            {
                GameObject playerObject = Instantiate(playerPrefab);
                playerObject.transform.parent = playerContainer.transform;
                playerObject.name = $"player {i +1}";
                playerObject.GetComponentInChildren<TextMeshProUGUI>().text = playerObject.name;
                players[i] = playerObject.AddComponent<Player>();
                players[i].GetComponent<Player>().playerIndex = i;
                players[i].GetComponent<Player>().myColor = playerColor[i].myColor;
                Debug.Log(players[i].GetComponent<Player>().myColor);
            }
        }

        playerContainer.GetComponent<PlayerContainer>().InitAvatorList(playerCount);
    }
    void StartTurn()
    {
        players[currentPlayerIndex].BeginTurn();
        Timer.Instance.StartTimer();
    }

    public void EndTurn()
    {
        Debug.Log("EndTurn " + currentPlayerIndex);

        players[currentPlayerIndex].GetComponent<Player>().EndTurn();

        NextTurn();
    }
    public void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % playerCount;
        
        playerContainer.GetComponent<PlayerContainer>().rotateAvator();
        StartTurn();
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
        int nextTurnIndex = _board.MakeMove(lineToConnect, currentPlayerIndex, true);
        if (nextTurnIndex != currentPlayerIndex)
        {
            NextTurn();
        }
        else
        {
            StartTurn();
        }
    }

    public void CaptureBox(Vector3 boxCoordAndCapturedBy)
    {
        _boxCapturedEvent.Invoke(boxCoordAndCapturedBy);
    }
}
