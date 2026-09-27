using UnityEngine;
using TMPro;
using System.Collections;

public class MoneyManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI balanceText;       // текст баланса

    [Header("Money Settings")]
    public int startBalance = 0;
    public int dollarValue = 5;               // сколько добавляет один доллар
    public int gamePrice = 10;                // стоимость игры

    [Header("Dollar Hide")]
    public GameObject dollarObject;           // объект доллара в руке
    public float hideDuration = 2.5f;         // на сколько секунд прячется

    private int balance;
    private bool isHidingDollar = false;

    void Start()
    {
        balance = startBalance;
        UpdateBalanceUI();
    }

    void Update()
    {
        // Пробел — кладём доллар (только если он в руке)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandItems hand = FindObjectOfType<HandItems>();
            if (hand != null && hand.IsDollarInHand() && !isHidingDollar)
            {
                StartCoroutine(InsertDollar());
            }
        }
    }

    IEnumerator InsertDollar()
    {
        isHidingDollar = true;

        // Прячем доллар
        if (dollarObject != null) dollarObject.SetActive(false);

        // Добавляем деньги
        balance += dollarValue;
        UpdateBalanceUI();

        Debug.Log("Вставили доллар. Баланс: " + balance);

        // Ждём
        yield return new WaitForSeconds(hideDuration);

        // Показываем снова (если рука всё ещё в состоянии доллара)
        HandItems hand = FindObjectOfType<HandItems>();
        if (hand != null && hand.IsDollarInHand() && dollarObject != null)
        {
            dollarObject.SetActive(true);
        }

        isHidingDollar = false;
    }

    void UpdateBalanceUI()
    {
        if (balanceText != null)
            balanceText.text = "Баланс: " + balance + " $";
    }

    public int GetBalance() { return balance; }
    public int GetGamePrice() { return gamePrice; }
    public void SpendMoney(int amount) { balance -= amount; UpdateBalanceUI(); }
}