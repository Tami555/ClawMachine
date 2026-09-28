using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinAnimation : MonoBehaviour
{
    [Header("UI")]
    public GameObject winTextObject;
    public TextMeshProUGUI winText;

    [Header("Settings")]
    public float flightDuration = 1f;        // сколько летит в центр
    public float displayDuration = 3f;       // сколько крутится
    public float spinSpeed = 200f;           // скорость вращения
    public float scaleMultiplier = 2f;       // во сколько раз увеличить
    public float distanceFromCamera = 5f;    // на каком расстоянии от камеры
    public Color glowColor = new Color(1f, 0.8431373f, 0f, 1f);  // жёлтое свечение

    [Header("Names Dictionary")]
    public ToyNameEntry[] toyNames;          // список имён (заполним в Inspector)

    [System.Serializable]
    public class ToyNameEntry
    {
        public string objectName;            // имя объекта в Unity (Toy1, Toy2...)
        public string displayName;           // красивое имя для игрока
    }

    // Публичный метод — вызывается из ClawController при победе
    public IEnumerator PlayWinAnimation(GameObject toy, System.Action onFinish)
    {
        SoundManager.Instance.PlayWin();
        // 1. Запоминаем оригинальные параметры игрушки
        Vector3 originalScale = toy.transform.localScale;
        Quaternion originalRotation = toy.transform.rotation;

        // 2. Отключаем физику, чтобы не мешала
        Rigidbody rb = toy.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // 3. Определяем красивую надпись
        string displayName = GetDisplayName(toy.name);
        if (winText != null)
        {
            winText.text = "ВЫ ВЫИГРАЛИ:\n" + displayName + "!";
        }
        if (winTextObject != null) winTextObject.SetActive(true);

        // 4. Летим в точку перед камерой
        Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward * distanceFromCamera;
        Vector3 startPos = toy.transform.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / flightDuration;
            toy.transform.position = Vector3.Lerp(startPos, targetPos, t);
            // Игрушка поворачивается к камере
            toy.transform.rotation = Quaternion.Slerp(originalRotation, Quaternion.LookRotation(toy.transform.position - Camera.main.transform.position), t);
            yield return null;
        }

        // 5. Крутим, светим, увеличиваем
        float elapsed = 0f;
        while (elapsed < displayDuration)
        {
            elapsed += Time.deltaTime;
            // Вращение
            toy.transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
            // Пульсация масштаба
            float pulse = 1f + Mathf.Sin(elapsed * 5f) * 0.1f;
            toy.transform.localScale = originalScale * scaleMultiplier * pulse;
            // Свечение через изменение цвета материала
            Renderer rend = toy.GetComponent<Renderer>();
            if (rend != null)
            {
                float glow = Mathf.PingPong(elapsed * 3f, 1f);
                rend.material.color = Color.Lerp(Color.white, glowColor, glow);
                rend.material.EnableKeyword("_EMISSION");
                rend.material.SetColor("_EmissionColor", glowColor * glow);
            }
            yield return null;
        }

        // 6. Возвращаем цвет
        Renderer rend2 = toy.GetComponent<Renderer>();
        if (rend2 != null)
        {
            rend2.material.color = Color.white;
            rend2.material.SetColor("_EmissionColor", Color.black);
        }

        // 7. Скрываем надпись
        if (winTextObject != null) winTextObject.SetActive(false);

        // 8. Сообщаем, что анимация закончена
        onFinish?.Invoke();
    }

    string GetDisplayName(string objectName)
    {
        if (toyNames == null) return objectName;

        foreach (var entry in toyNames)
        {
            if (objectName.Contains(entry.objectName))
                return entry.displayName;
        }
        return objectName;
    }
}