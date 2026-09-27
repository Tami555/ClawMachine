using UnityEngine;

public class HandItems : MonoBehaviour
{
    [Header("Items")]
    public GameObject wallet;      // объект кошелька
    public GameObject dollar;      // объект доллара

    [Header("Follow Settings")]
    public float distanceFromCamera = 1.5f;   // на каком расстоянии от камеры
    public float offsetX = 0.6f;              // сдвиг вправо от центра
    public float offsetY = -0.4f;             // сдвиг вниз

    // Состояния: 0 = ничего, 1 = кошелёк, 2 = доллар
    private int currentState = 0;

    void Start()
    {
        if (wallet != null) wallet.SetActive(false);
        if (dollar != null) dollar.SetActive(false);
    }

    void Update()
    {
        // Tab — переключение
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentState = (currentState + 1) % 3;  // 0 -> 1 -> 2 -> 0
            ApplyState();
        }

        // Следуем за камерой (плавно)
        FollowCamera();
    }

    void ApplyState()
    {
        if (wallet != null) wallet.SetActive(currentState == 1);
        if (dollar != null) dollar.SetActive(currentState == 2);
        Debug.Log("Состояние руки: " + currentState);
    }

    void FollowCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // Позиция перед камерой
        Vector3 targetPos = cam.transform.position
                          + cam.transform.forward * distanceFromCamera
                          + cam.transform.right * offsetX
                          + cam.transform.up * offsetY;

        // Вращение: смотрим на камеру (чтобы объект был к нам «лицом»)
        Quaternion targetRot = Quaternion.LookRotation(
            transform.position - cam.transform.position
        );

        // Применяем к родителю (объекты — его дети)
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 10f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
    }

    public int GetState() { return currentState; }
    public bool IsDollarInHand() { return currentState == 2; }
}