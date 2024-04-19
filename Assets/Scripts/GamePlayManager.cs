using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayManager : MonoBehaviour
{
    [SerializeField] private int _h = 4;
    [SerializeField] private int _w = 4;
    [SerializeField] public PlayerColor[] playerColor;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private GameObject playerContainer;
    //TODO: WHEN INTEGRATING COLOR PICKER
    //[SerializeField] private TextMeshProUGUI currentPlayerName;
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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        if (scene.name == "04_Local_Multiplayer")
            CreateBoardOfSize();
    }


    void Start()
    {
        
        
        
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

    public void SetBoardSize(Vector2 value)
    {
        
        _h = (int)value.x;
        _w = (int)value.y;
    }
    
    public void CreateBoardOfSize()
    {
        Transform a = FindFirstObjectByType<UIManager>().transform;
        playerContainer = a.GetChild(2).gameObject;
        // if (H == 1)
        // {
        //     _h = 4; 
        //     _w = 4;
        // }
        // else if (size == 2)
        // {
        //     _h = 6;
        //     _w = 6;
        // }
        // else if (size == 3)
        // {
        //     _h = 8;
        //     _w = 8;
        // }
        InitailizePlayers();
        //TODO: WHEN INTEGRATING COLOR PICKER
        //ChangePlayerInfo();
        StartTurn();
        _board = new Board(_h, _w);
        GridGenerator.Instance.CreateBoard();
        LineController.Instance.CreateLineDrawing();
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
        //TODO: WHEN INTEGRATING COLOR PICKER
        //currentPlayerName.text = players[currentPlayerIndex].playerName.ToString();
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
