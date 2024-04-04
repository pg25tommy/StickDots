using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Image uiFill;
    [SerializeField] public TextMeshProUGUI timerText;
    [SerializeField] public AudioClip beepSound;
    [SerializeField] public float timeRemaining;
    [SerializeField] private float originalTime;
    [SerializeField] private int TimeForEachTurn;
    [SerializeField] private bool isRunning = false;
    public static Timer Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    //private void Start()
    //{
    //    StartTimer();
    //}

    private void Update()
    {
        if (isRunning)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerDisplay();

            if (timeRemaining <= 0)
            {
                GamePlayManager.Instance.NextTurn();
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

    public void StopTimer()
    {
        isRunning = false;
    }
    public void StartTimer()
    {
        int seconds = TimeForEachTurn;
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
