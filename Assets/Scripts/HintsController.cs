using UnityEngine;
using TMPro;

public class HintsController : MonoBehaviour
{
    [System.Serializable]
    public class HintCard
    {
        public GameObject cardObject;      // сам объект Card
        public string title;               // заголовок
        [TextArea(3, 10)]
        public string body;                // текст карточки
    }

    [Header("Cards")]
    public HintCard[] cards;

    [Header("UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public GameObject hintPanel;           // общий Canvas с фоном и текстом
    public GameObject dimBackground;

    [Header("Settings")]
    //public float autoHideDelay = 8f;       // через сколько секунд спр€тать
    public bool showOnStart = true;        // показывать ли при старте

    private int currentCardIndex = -1;     // -1 = ничего не показано
    private bool isVisible = false;

    void Start()
    {
        // —крываем всЄ при старте
        HideAll();

        if (showOnStart)
        {
            ShowCard(0);
        }
    }

    void Update()
    {
        // F1 Ч следующа€ карточка
        if (Input.GetKeyDown(KeyCode.F1))
        {
            NextCard();
        }

        // F2 Ч закрыть всЄ
        if (Input.GetKeyDown(KeyCode.F2))
        {
            HideAll();
        }
    }

    void NextCard()
    {
        if (!isVisible)
        {
            // ≈сли ничего не показано Ч открываем первую
            ShowCard(0);
            return;
        }

        // »наче Ч следующа€
        int next = currentCardIndex + 1;
        if (next >= cards.Length)
        {
            //  арточки кончились Ч закрываем всЄ
            HideAll();
        }
        else
        {
            ShowCard(next);
        }
    }

    void ShowCard(int index)
    {
        if (cards == null || index < 0 || index >= cards.Length) return;

        currentCardIndex = index;
        isVisible = true;

        if (hintPanel != null) hintPanel.SetActive(true);
        if (dimBackground != null) dimBackground.SetActive(true);

        if (titleText != null) titleText.text = cards[index].title;
        if (bodyText != null) bodyText.text = cards[index].body;

        // јвтоскрытие
        //CancelInvoke(nameof(HideAll));
        //Invoke(nameof(HideAll), autoHideDelay);
    }

    void HideAll()
    {
        isVisible = false;
        currentCardIndex = -1;
        CancelInvoke(nameof(HideAll));

        if (hintPanel != null) hintPanel.SetActive(false);
        if (dimBackground != null) dimBackground.SetActive(false);
    }
}