using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        
    }
    public void SaveRoomType(string roomType)
    {
        PlayerPrefs.SetString("RoomType", roomType);
        PlayerPrefs.Save();
    }
}
