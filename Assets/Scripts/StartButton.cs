using UnityEngine;

public class StartButton : MonoBehaviour
{
    [Header("References")]
    public MoneyManager moneyManager;
    public GameTimer gameTimer;

    [Header("Visual")]
    public Color normalColor = Color.red;
    public Color pressedColor = new Color(0.6f, 0f, 0f);

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null) rend.material.color = normalColor;
    }

    void OnMouseDown()
    {
        Debug.Log("Кнопка Start нажата!");

        if (moneyManager == null || gameTimer == null) return;

        // 1. Проверяем, что в руке нет доллара
        HandItems hand = FindObjectOfType<HandItems>();
        if (hand != null && hand.IsDollarInHand())
        {
            //gameTimer.ShowMessage("Сначала уберите доллар (Tab), потом начинайте игру!");
            return;
        }

        // 2. Проверяем баланс
        if (moneyManager.GetBalance() < moneyManager.GetGamePrice())
        {
            gameTimer.ShowMessage("Недостаточно средств!\nПополните баланс до 10$");
            return;
        }

        // 3. Проверяем, что игра ещё не идёт (защита от повторного запуска)
        if (gameTimer.CanPlay())
        {
            return;  // игра уже запущена — ничего не делаем
        }

        // 4. Списываем деньги и готовим игру
        moneyManager.SpendMoney(moneyManager.GetGamePrice());
        gameTimer.PrepareGame();
    }

    void OnMouseEnter()
    {
        if (rend != null) rend.material.color = pressedColor;
    }

    void OnMouseExit()
    {
        if (rend != null) rend.material.color = normalColor;
    }
}