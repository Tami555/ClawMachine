using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timerText;
    public GameObject messageTextObject;
    public TextMeshProUGUI messageText;

    [Header("Settings")]
    public float gameDuration = 20f;

    private float timeLeft = 0f;
    private bool isGamePrepared = false;
    private bool isGameRunning = false;
    private bool hasGrabbedThisGame = false;  // 1 попытка за игру

    void Start()
    {
        if (timerText != null) timerText.text = "0";
        if (messageTextObject != null) messageTextObject.SetActive(false);
    }

    void Update()
    {
        // Ждём нажатия ЛЮБОЙ клавиши, чтобы стартовать
        if (isGamePrepared && !isGameRunning)
        {
            if (Input.anyKeyDown)
            {
                StartGame();
            }
        }

        // Тикаем таймер
        if (isGameRunning)
        {
            timeLeft -= Time.deltaTime;
            if (timerText != null) timerText.text = Mathf.CeilToInt(Mathf.Max(0, timeLeft)).ToString();

            if (timeLeft <= 0f)
            {
                EndGame();
            }
        }
    }

    void StartGame()
    {
        isGameRunning = true;
        isGamePrepared = false;
        hasGrabbedThisGame = false;
        timeLeft = gameDuration;
        Debug.Log("Игра началась!");
    }

    public void EndGame()
    {
        isGameRunning = false;
        timeLeft = 0f;
        if (timerText != null) timerText.text = "0";
        Debug.Log("Игра окончена.");

        // TODO: заставить кран вернуться и перестать слушать
    }

    public void ShowMessage(string message, float duration = 3f)
    {
        if (messageText == null || messageTextObject == null) return;
        messageText.text = message;
        messageTextObject.SetActive(true);
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), duration);
    }

    void HideMessage()
    {
        if (messageTextObject != null) messageTextObject.SetActive(false);
    }

    public void PrepareGame()
    {
        if (isGameRunning) return;
        isGamePrepared = true;
        timeLeft = gameDuration;
        if (timerText != null) timerText.text = Mathf.CeilToInt(timeLeft).ToString();
        Debug.Log("Игра подготовлена. Нажми любую клавишу для старта.");
    }

    public bool CanPlay() { return isGameRunning; }
    public bool HasGrabbed() { return hasGrabbedThisGame; }
    public void MarkGrabbed() { hasGrabbedThisGame = true; }
}