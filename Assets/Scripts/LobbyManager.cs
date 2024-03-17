using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject createRoomPage;
    [SerializeField] private GameObject joinRoomPage;
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
}
