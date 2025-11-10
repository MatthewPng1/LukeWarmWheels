using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Goal")]
    public int targetCollectibles = 10;

    [Header("Timer")]
    public float timeLimit = 60f;
    public Image timeFill;          // UI Image (Filled)
    public TMP_Text timeText;       // optional

    [Header("UI")]
    public TMP_Text countText;      // e.g., "3/10"
    public GameObject endPanel;
    public TMP_Text endMessage;
    public Button tryAgainButton;

    float timeLeft;
    int collected;
    bool ended;

    void Start()
    {
        Time.timeScale = 1f;  // Ensure normal time scale
        ended = false;
        collected = 0;
        timeLeft = timeLimit;
        
        UpdateCountUI();
        UpdateTimeUI();

        if (endPanel) endPanel.SetActive(false);
        if (tryAgainButton)
            tryAgainButton.onClick.AddListener(RestartScene);
            
        Debug.Log($"GameController: Starting game. Target collectibles: {targetCollectibles}");
    }

    void Update()
    {
        if (ended) return;

        // Countdown only if game hasn't been won
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndGame(false);  // Lost due to time
        }

        UpdateTimeUI();
    }

    // Helper method to check if all items are collected
    public bool IsGameComplete()
    {
        return collected >= targetCollectibles;
    }

    void UpdateTimeUI()
    {
        if (timeFill)
            timeFill.fillAmount = Mathf.Clamp01(timeLeft / timeLimit);

        if (timeText)
            timeText.text = $"{Mathf.CeilToInt(timeLeft)}s";
    }

    void UpdateCountUI()
    {
        if (countText)
            countText.text = $"{collected}/{targetCollectibles}";
    }

    public void AddCollect(int amount = 1)
    {
        if (ended) return;

        collected += amount;
        UpdateCountUI();

        Debug.Log($"GameController: Collected {collected}/{targetCollectibles}");

        // End game immediately if all items are collected
        if (collected >= targetCollectibles)
        {
            Debug.Log("GameController: Congrats on getting ready on time!");
            EndGame(true);  // Won by collecting all items
        }
    }

    public void EndGame(bool win)
    {
        if (ended) return;  // prevent multiple end-game calls
        
        ended = true;
        Time.timeScale = 0f; // Stop gameplay and timer

        if (endPanel) 
        {
            endPanel.SetActive(true);
            if (endMessage)
            {
                if (win)
                {
                    endMessage.text = "Congrats on getting ready on time!";
                }
                else
                {
                    endMessage.text = "Out of time!";
                }

                // Center the text if it's a TMP_Text
                if (endMessage is TMP_Text tmpText)
                {
                    tmpText.alignment = TextAlignmentOptions.Center;
                }
            }
        }

        Debug.Log($"Game ended - Win: {win}, Time left: {timeLeft:F1}s, Items: {collected}/{targetCollectibles}");
    }

    void RestartScene()
    {
        Time.timeScale = 1f;
        var idx = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(idx);
    }
}
