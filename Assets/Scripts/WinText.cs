using UnityEngine;
using TMPro;

public class WinText : MonoBehaviour
{
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.1f;

    private TextMeshProUGUI text;
    private float baseScale;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        baseScale = 1f;
    }

    void Update()
    {
        float scale = baseScale + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = Vector3.one * scale;
    }
}
