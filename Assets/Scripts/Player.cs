using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool isTurnActive = false;
    public Color myColor { get; set; }
    public int playerIndex { get; set; }

    public void BeginTurn()
    {
        Debug.Log("MyColor   " + myColor);
        isTurnActive = true;
        UIManager.Instance.IndicatorColorSwitch(myColor);
    }

    public void EndTurn()
    {
        isTurnActive = false;
    }
}
