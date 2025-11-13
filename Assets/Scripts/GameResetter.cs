using UnityEngine;

/// <summary>
/// Central helper for resetting persistent managers when returning to main menu.
/// Call GameResetter.ResetAll() before loading the menu to clear runtime state.
/// </summary>
public static class GameResetter
{
    public static void ResetAll()
    {
        // Reset play time
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.ResetTimer();
            TimeManager.Instance.SetPaused(false);
        }

        // Reset coins (collected only) and optionally clear registrations if needed
        if (CoinManager.Instance != null)
        {
            // Clear both collected coins and registered level totals for a true hard-reset
            CoinManager.Instance.ResetAllProgress();
        }

        // Reset score manager final score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetFinalScore(0);
        }

        // Optionally clear PlayerPrefs keys that are used by managers
        PlayerPrefs.DeleteKey("Coins");
        PlayerPrefs.DeleteKey("TypingHighScore");
    }
}