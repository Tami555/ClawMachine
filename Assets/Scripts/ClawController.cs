using UnityEngine;

public class ClawController : MonoBehaviour
{
    [Header("Horizontal Movement")]
    public float moveSpeed = 3f;
    public float leftLimit = -5f;
    public float rightLimit = 8f;

    [Header("Vertical Movement")]
    public float topY = 10f;           // верхняя позиция (старт)
    public float bottomY = 2f;        // нижняя позиция (у игрушек)
    public float verticalSpeed = 2f;    // скорость опускания
    public float waitAtBottom = 2f;     // сколько ждать внизу перед автоподъёмом

    // Состояния крана
    private enum ClawState { Idle, MovingDown, Grabbing, MovingUp }
    private ClawState state = ClawState.Idle;

    private float waitTimer = 0f;

    void Update()
    {
        switch (state)
        {
            case ClawState.Idle:
                HandleIdle();
                break;
            case ClawState.MovingDown:
                HandleMovingDown();
                break;
            case ClawState.Grabbing:
                HandleGrabbing();
                break;
            case ClawState.MovingUp:
                HandleMovingUp();
                break;
        }
    }

    // ============ СОСТОЯНИЯ ============

    void HandleIdle()
    {
        // Движение влево-вправо (только когда кран наверху)
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 pos = transform.position;
        pos.x += horizontal * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, leftLimit, rightLimit);
        transform.position = pos;

        // Пробел — начать опускание
        if (Input.GetKeyDown(KeyCode.Space))
        {
            state = ClawState.MovingDown;
            waitTimer = 0f;
            Debug.Log("Кран опускается...");
        }
    }

    void HandleMovingDown()
    {
        Vector3 pos = transform.position;
        pos.y -= verticalSpeed * Time.deltaTime;

        // Пробел #2 — прервать опускание и сделать захват
        if (Input.GetKeyDown(KeyCode.Space))
        {
            state = ClawState.Grabbing;
            waitTimer = 0f;
            Debug.Log("Захват! (пока заглушка)");
            // Здесь на этапе 3 будет проверка: поймали игрушку или нет
            return;
        }

        // Дошли до низа?
        if (pos.y <= bottomY)
        {
            pos.y = bottomY;
            transform.position = pos;
            state = ClawState.Grabbing;
            waitTimer = 0f;
            Debug.Log("Кран на дне, ждём 2 секунды...");
            return;
        }

        transform.position = pos;
    }

    void HandleGrabbing()
    {
        // Просто ждём 2 секунды, потом поднимаемся
        waitTimer += Time.deltaTime;
        if (waitTimer >= waitAtBottom)
        {
            state = ClawState.MovingUp;
            Debug.Log("Поднимаемся наверх...");
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
            state = ClawState.Idle;
            Debug.Log("Кран вернулся наверх. Можно снова двигать.");
            return;
        }

        transform.position = pos;
    }
}