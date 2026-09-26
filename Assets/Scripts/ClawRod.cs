using UnityEngine;

public class ClawRod : MonoBehaviour
{
    [Header("References")]
    public Transform claw;              // ссылка на клешню (Claw)
    public Transform rodPivot;          // верхняя точка штанги (ClawRodPivot)
    public Transform rodCylinder;       // сам цилиндр (ClawRod)

    [Header("Settings")]
    public float rodTopY = 33f;         // Y-координата «потолка», откуда свисает штанга
    public float minRodLength = 0.1f;   // минимальная длина штанги
    public float thickness = 0.15f;     // толщина штанги (X и Z)
    public bool attachToClawZ = true;   // штанга следует за клешнёй по X и Z

    void LateUpdate()
    {
        if (claw == null || rodPivot == null || rodCylinder == null) return;

        // 1. Верх штанги всегда на потолке, но X и Z — как у клешни
        Vector3 pivotPos = rodPivot.position;
        pivotPos.x = claw.position.x;
        pivotPos.y = rodTopY;
        if (attachToClawZ) pivotPos.z = claw.position.z;
        rodPivot.position = pivotPos;

        // 2. Считаем длину штанги = расстояние от потолка до клешни
        float length = rodTopY - claw.position.y;
        if (length < minRodLength) length = minRodLength;

        // 3. Масштабируем цилиндр по Y
        Vector3 scale = rodCylinder.localScale;
        scale.x = thickness;
        scale.y = length / 2f;  // делим на 2, потому что у Unity-цилиндра высота = 2 * scale.y
        scale.z = thickness;
        rodCylinder.localScale = scale;
    }
}