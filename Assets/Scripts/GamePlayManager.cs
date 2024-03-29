using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    [SerializeField] PlayerData[] playerDatas;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] private GameObject playerContainer;
    [HideInInspector] public Player[] players;
    public int currentPlayerIndex { get; private set; } = 0;
    public static GamePlayManager Instance { get; private set; }
    private int playerCount = 5;

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
        playerCount = playerDatas.Length;
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
                players[i].GetComponent<Player>().myColor = playerDatas[i].myColor;
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
}
