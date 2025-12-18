using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

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
}
