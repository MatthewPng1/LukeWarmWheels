using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CoinUi : MonoBehaviour
{
    public TMP_Text coinText; // assign in Inspector

    void OnEnable()
    {
        if (coinText == null)
            coinText = GetComponent<TMP_Text>();

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.OnCoinsChanged += UpdateText;
            // initialize
            UpdateText(CoinManager.Instance.Coins);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        if (CoinManager.Instance != null)
            CoinManager.Instance.OnCoinsChanged -= UpdateText;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // In case a new scene has its own UI, reassign the text if necessary
        if (coinText == null)
            coinText = GetComponent<TMP_Text>();
        UpdateText(CoinManager.Instance != null ? CoinManager.Instance.Coins : 0);
    }

    void UpdateText(int amount)
    {
        if (coinText != null)
            coinText.text = amount.ToString();
    }
}