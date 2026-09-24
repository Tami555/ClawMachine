using UnityEngine;

public class ClawController : MonoBehaviour
{
    [Header("Horizontal Movement")]
    public float moveSpeed = 3f;
    public float leftLimit = -5f;
    public float rightLimit = 8f;

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

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
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
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 pos = transform.position;
        pos.x += horizontal * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, leftLimit, rightLimit);
        transform.position = pos;

        if (Input.GetKeyDown(KeyCode.Space))
        {
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
            Debug.Log("Поймали игрушку: " + grabbedToy.name);
        }
        else
        {
            Debug.Log("Не поймали — пусто.");
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
            grabbedToy.transform.SetParent(null);
            Rigidbody rb = grabbedToy.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
            grabbedToy = null;
            Debug.Log("Игрушка сброшена в корзину!");
        }

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
        }
    }

    // Рисуем радиус захвата в редакторе (только для отладки)
    void OnDrawGizmosSelected()
    {
        if (grabPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(grabPoint.position, grabRadius);
        }
    }
}