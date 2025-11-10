using UnityEngine;
using TMPro;

/// <summary>
/// Displays the final score using the ScoreManager. The ScoreManager holds the
/// final score in memory (no persistence required).
/// </summary>
public class ScoreUI : MonoBehaviour
{
    [Header("Score Display")]
    [SerializeField] private TMP_Text currentScoreText;

    void OnEnable()
    {
        UpdateScoreDisplay();
    }

    public void UpdateScoreDisplay()
    {
        if (currentScoreText == null) return;

        int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.GetFinalScore() : 0;
        currentScoreText.text = $"Test Score:    {finalScore}";
    }
}