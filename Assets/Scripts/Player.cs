using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool isTurnActive = false;
    public Color myColor = new Color(255, 0, 0);
    public int playerIndex { get; set; }

    public string playerName = "Player";

    public void BeginTurn()
    {
        isTurnActive = true;
        UIManager.Instance.IndicatorColorSwitch(myColor);
    }

    public void EndTurn()
    {
        isTurnActive = false;
    }
}
