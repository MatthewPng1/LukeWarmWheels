using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderPersentage : MonoBehaviour
{
    [SerializeField] private float maxValue = 100f;
    [SerializeField] private float currentValue = 0f;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI percentageText;    void Start()
    {
        UpdateUI();
    }

    public void SetValue(float value)
    {
        currentValue = Mathf.Clamp(value, 0f, maxValue);
        UpdateUI();
    }

    public void SetMaxValue(float value)
    {
        maxValue = value;
        UpdateUI();
    }

    void UpdateUI()
    {
        // sanity: avoid division by zero
        if (maxValue <= 0f)
        {
            Debug.LogWarning($"[{name}] SliderPersentage: maxValue is <= 0 (={maxValue}). Set a positive maxValue.");
            return;
        }

        if (fillImage == null)
        {
            // try to auto-assign if user forgot
            fillImage = GetComponent<Image>();
            if (fillImage == null) fillImage = GetComponentInChildren<Image>();
        }

        if (fillImage != null)
        {
            if (fillImage.type != Image.Type.Filled)
            {
                Debug.LogWarning($"[{name}] SliderPersentage: assigned Image is not set to 'Filled' type. Current type: {fillImage.type}. Please set Image.type = Filled for fillAmount to work.");
            }
            fillImage.fillAmount = Mathf.Clamp01(currentValue / maxValue);
        }

        if (percentageText != null)
        {
            int percentage = Mathf.RoundToInt((currentValue / maxValue) * 100f);
            percentageText.text = percentage.ToString() + "%";
        }
    }

    // Smooth value setting
    public void SetValueSmooth(float value, float speed = 8f)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothTo(value, speed));
    }

    private System.Collections.IEnumerator SmoothTo(float target, float speed)
    {
        target = Mathf.Clamp(target, 0f, maxValue);
        while (Mathf.Abs(currentValue - target) > 0.01f)
        {
            currentValue = Mathf.Lerp(currentValue, target, Time.deltaTime * speed);
            UpdateUI();
            yield return null;
        }
        currentValue = target;
        UpdateUI();
    }

    // convenience: add an amount to the current value
    public void AddValue(float delta)
    {
        SetValue(currentValue + delta);
    }
}
