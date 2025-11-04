using UnityEngine;
using TMPro;

/// <summary>
/// Simple UI helper for an end-game or summary screen.
/// Attach to a GameObject in your end-game/menu scene and assign the
/// summaryText field to a TextMeshProUGUI (or TMP_Text) to display
/// "COLLECTED: X / Y" where X is coins collected and Y is total available.
/// </summary>
public class EndGameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text summaryText;

    private void OnEnable()
    {
        // Update when the UI becomes visible
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (summaryText == null) return;

        if (CoinManager.Instance == null)
        {
            summaryText.text = "TOTAL: 0 / 0";
            return;
        }

        int collected = CoinManager.Instance.Coins;
        int available = CoinManager.Instance.TotalAvailableCoins;

        summaryText.text = $"TOTAL COINS: {collected} / {available}";
    }
}
