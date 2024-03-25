using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Image uiFill;
    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] public Button[] timerButtons;
    [SerializeField] public AudioClip beepSound;
    [SerializeField] public float timeRemaining;
    [SerializeField] private float originalTime;
    [SerializeField] private bool isRunning = false;

    private void Start()
    {
        foreach (Button button in timerButtons)
        {
            button.onClick.AddListener(delegate { StartTimer(button); });
        }
        Begin();
    }

    private void Begin()
    {
        originalTime = timeRemaining;
    }

    private void Update()
    {
        if (isRunning)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();

            if (timeRemaining <= 0)
            {
                Debug.Log("Time's up!");
                isRunning = false;
            }
            else
            {
                if (timeRemaining <= 15 && timeRemaining > 5)
                {
                    if (beepSound != null)
                        AudioSource.PlayClipAtPoint(beepSound, transform.position);
                }
                else if (timeRemaining <= 5)
                {
                    if (beepSound != null)
                        AudioSource.PlayClipAtPoint(beepSound, transform.position);
                }
            }
        }
    }

    private void StartTimer(Button button)
    {
        int seconds = 0;
        if (button.name == "30SecButton")
            seconds = 30;
        else if (button.name == "60SecButton")
            seconds = 60;
        else if (button.name == "90SecButton")
            seconds = 90;

        timeRemaining = seconds;
        originalTime = timeRemaining;
        isRunning = true;
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        uiFill.fillAmount = Mathf.InverseLerp(0, originalTime, timeRemaining);
    }
}
