using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;              // CameraTarget (центр автомата)

    [Header("Distance")]
    public float distance = 50f;          // текуща€ дистанци€
    public float minDistance = 5f;        // минимальный зум
    public float maxDistance = 50f;       // максимальный зум
    public float zoomSpeed = 5f;          // скорость зума

    [Header("Rotation")]
    public float rotationSpeed = 3f;      // скорость вращени€
    public float minYAngle = 10f;         // ограничение по вертикали (не под пол)
    public float maxYAngle = 80f;         // ограничение по вертикали (не над крышей)

    private float currentX = 180f;          // текущий угол по горизонтали
    private float currentY = 20f;         // текущий угол по вертикали

    void Start()
    {
        // —разу выставл€ем камеру в стартовую позицию
        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        // ¬ращение Ч только когда зажата ѕ ћ
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
        }

        // «ум колЄсиком
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        if (target == null) return;

        // —читаем позицию камеры вокруг цели
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = new Vector3(0, 0, -distance);
        Vector3 position = target.position + rotation * direction;

        transform.position = position;
        transform.LookAt(target.position);
    }
}