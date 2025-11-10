using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int currentScore = 0;
    private int finalScore = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Increment the current score by 1
    /// </summary>
    public void IncrementScore()
    {
        currentScore++;
    }

    /// <summary>
    /// Store the final score for this run. No persistence required.
    /// </summary>
    public void SetFinalScore(int score)
    {
        finalScore = score;
    }

    /// <summary>
    /// Return the final score set at the end of the typing scene.
    /// </summary>
    public int GetFinalScore()
    {
        return finalScore;
    }

    /// <summary>
    /// Get the current score during gameplay
    /// </summary>
    public int GetCurrentScore()
    {
        return currentScore;
    }

    /// <summary>
    /// Reset both current and final scores
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        finalScore = 0;
    }
}
