using UnityEngine;
using System.Collections;

public class ClawController : MonoBehaviour
{
    [Header("Horizontal Movement")]
    public float moveSpeed = 3f;
    public float leftLimit = -5f;
    public float rightLimit = 8f;

    [Header("Depth Limits (Z)")]
    public float backLimit = -5f;    // задн€€ граница
    public float frontLimit = 5f;    // передн€€ граница

    [Header("Vertical Movement")]
    public float topY = 35f;
    public float bottomY = 25f;
    public float verticalSpeed = 2f;
    public float waitAtBottom = 2f;

    [Header("Grab Settings")]
    public Transform grabPoint;
    public float grabRadius = 0.5f;
    public LayerMask toyLayer;

    [Header("Basket Settings")]
    public Transform basketPoint;
    public float basketSpeed = 3f;

    private enum ClawState { Idle, MovingDown, Grabbing, MovingUp, MovingToBasket, Dropping, Returning }
    private ClawState state = ClawState.Idle;

    private float waitTimer = 0f;
    private GameObject grabbedToy = null;
    private Vector3 startPosition;
    private static int wonToysCount = 0;
    private bool isWinAnimating = false;
    private GameTimer gameTimer;

    void Start()
    {
        startPosition = transform.position;
        gameTimer = FindObjectOfType<GameTimer>();
    }

    void Update()
    {
        // ≈сли врем€ вышло Ч надо вернуть кран, Ќќ только если он не несЄт игрушку
        if (gameTimer != null && !gameTimer.CanPlay())
        {
            bool isCarryingToy = grabbedToy != null;

            // ≈сли кран несЄт игрушку Ч даЄм ему доехать до корзины и сбросить
            if (isCarryingToy)
            {
                // ≈сли он ещЄ не в MovingToBasket Ч отправл€ем туда
                if (state != ClawState.MovingToBasket
                    && state != ClawState.Dropping
                    && state != ClawState.MovingUp)  // MovingUp тоже нужен Ч чтобы подн€ть игрушку наверх
                {
                    state = ClawState.MovingUp;
                }
            }
            else
            {
                // »грушки нет Ч можно сразу домой
                if (state != ClawState.Idle && state != ClawState.Returning)
                {
                    state = ClawState.Returning;
                }
            }
        }

        switch (state)
        {
            case ClawState.Idle: HandleIdle(); break;
            case ClawState.MovingDown: HandleMovingDown(); break;
            case ClawState.Grabbing: HandleGrabbing(); break;
            case ClawState.MovingUp: HandleMovingUp(); break;
            case ClawState.MovingToBasket: HandleMovingToBasket(); break;
            case ClawState.Dropping: HandleDropping(); break;
            case ClawState.Returning: HandleReturning(); break;
        }
    }

    void HandleIdle()
    {
        if (gameTimer == null || !gameTimer.CanPlay()) return;  // игра не активна Ч кран стоит

        float horizontal = -Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += horizontal * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, leftLimit, rightLimit);
        pos.z += vertical * moveSpeed * Time.deltaTime;
        pos.z = Mathf.Clamp(pos.z, backLimit, frontLimit);
        transform.position = pos;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // ѕроверка на доллар
            HandItems hand = FindObjectOfType<HandItems>();
            if (hand != null && hand.IsDollarInHand()) return;

            // ѕроверка: одна попытка за игру
            if (gameTimer.HasGrabbed()) return;

            gameTimer.MarkGrabbed();  // помечаем, что попытка использована
            state = ClawState.MovingDown;
            waitTimer = 0f;
        }
    }

    void HandleMovingDown()
    {
        Vector3 pos = transform.position;
        pos.y -= verticalSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryGrab();
            state = ClawState.MovingUp;
            return;
        }

        if (pos.y <= bottomY)
        {
            pos.y = bottomY;
            transform.position = pos;
            state = ClawState.Grabbing;
            waitTimer = 0f;
            return;
        }

        transform.position = pos;
    }

    void HandleGrabbing()
    {
        waitTimer += Time.deltaTime;
        if (waitTimer >= waitAtBottom)
        {
            TryGrab();
            state = ClawState.MovingUp;
        }
    }

    void TryGrab()
    {
        Collider[] hits = Physics.OverlapSphere(grabPoint.position, grabRadius, toyLayer);
        if (hits.Length > 0)
        {
            grabbedToy = hits[0].gameObject;
            grabbedToy.transform.SetParent(grabPoint);
            Rigidbody rb = grabbedToy.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
            Debug.Log("ѕоймали игрушку: " + grabbedToy.name);
        }
        else
        {
            Debug.Log("Ќе поймали Ч пусто.");
        }
    }

    void HandleMovingUp()
    {
        Vector3 pos = transform.position;
        pos.y += verticalSpeed * Time.deltaTime;
        if (pos.y >= topY)
        {
            pos.y = topY;
            transform.position = pos;
            state = grabbedToy != null ? ClawState.MovingToBasket : ClawState.Returning;
            return;
        }
        transform.position = pos;
    }

    void HandleMovingToBasket()
    {
        if (basketPoint == null)
        {
            Debug.LogWarning("Basket Point не назначен!");
            state = ClawState.Returning;
            return;
        }

        Vector3 target = new Vector3(basketPoint.position.x, transform.position.y, basketPoint.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, basketSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            state = ClawState.Dropping;
            waitTimer = 0f;
        }
    }

    void HandleDropping()
    {
        if (grabbedToy != null)
        {
            // ќтпускаем игрушку Ч она физически падает в корзину
            grabbedToy.transform.SetParent(null);
            Rigidbody rb = grabbedToy.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;

            grabbedToy = null;
            waitTimer = 0f;
        }

        // ∆дЄм немного, потом возвращаемс€ (сама анимаци€ запуститс€ по триггеру)
        waitTimer += Time.deltaTime;
        if (waitTimer >= 0.5f)
        {
            state = ClawState.Returning;
        }
    }

    void HandleReturning()
    {
        Vector3 target = new Vector3(startPosition.x, topY, startPosition.z);
        transform.position = Vector3.MoveTowards(transform.position, target, basketSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            state = ClawState.Idle;

            // ≈сли игра идЄт и попытка уже потрачена Ч завершаем игру досрочно
            if (gameTimer != null && gameTimer.CanPlay() && gameTimer.HasGrabbed())
            {
                gameTimer.EndGame();  // нужно сделать публичным в GameTimer
            }
        }
    }

    // –исуем радиус захвата в редакторе (только дл€ отладки)
    void OnDrawGizmosSelected()
    {
        if (grabPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(grabPoint.position, grabRadius);
        }
    }

    void DropToyToFloor(GameObject toy)
    {
        if (toy == null) return;  // защита от null

        // —тавим игрушки в р€д справа от автомата
        Vector3 dropPos = new Vector3(5f + wonToysCount * 0.8f, 1f, -20f);
        dropPos.x = Random.Range(15f, 30f);
        //dropPos.z += Random.Range(-0.2f, 0.2f);

        toy.transform.position = dropPos;
        toy.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        //toy.transform.localScale = Vector3.one;

        Rigidbody rb = toy.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        wonToysCount++;
    }

    public void OnToyLandedInBasket(GameObject toy)
    {
        if (isWinAnimating) return;  // защита от повторов
        isWinAnimating = true;

        Vector3 originalScale = toy.transform.localScale;

        WinAnimation winAnim = FindObjectOfType<WinAnimation>();
        if (winAnim != null)
        {
            StartCoroutine(winAnim.PlayWinAnimation(toy, () =>
            {
                toy.transform.localScale = originalScale;
                DropToyToFloor(toy);
                isWinAnimating = false;
            }));
        }
        else
        {
            DropToyToFloor(toy);
            isWinAnimating = false;
        }
    }
}