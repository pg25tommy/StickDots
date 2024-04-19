using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject createRoomPage;
    [SerializeField] private GameObject joinRoomPage;
    [SerializeField] private UnityEvent<Vector2> gameStart;

    private string roomType;
    private void Awake()
    {
        roomType = PlayerPrefs.GetString("RoomType");
        if(roomType == "Create")
        {
            createRoomPage.SetActive(true);
        }
        if (roomType == "Join")
        {
            joinRoomPage.SetActive(true);
        }
    }

    public void GameStart()
    {
        // Slider slider = createRoomPage.GetComponentInChildren<Slider>();

        int x = int.Parse(createRoomPage.transform.GetChild(1).GetComponentInChildren<TMP_InputField>().text);
        int y = int.Parse(createRoomPage.transform.GetChild(2).GetComponentInChildren<TMP_InputField>().text);

        Debug.Log(x + " " + y);

        // int boardSize = (int) slider.value;
        // TODO: 

        //return;

        gameStart.Invoke(new Vector2(x, y));
    }
}
