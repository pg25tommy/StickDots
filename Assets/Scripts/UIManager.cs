using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image indicator;
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void IndicatorColorSwitch(Color newColor)
    {
        indicator.color = newColor;

        // change the alpha to 1 (default is 0)
        Color color = indicator.color;
        color.a = 1f;
        indicator.color = color;
    }
}
