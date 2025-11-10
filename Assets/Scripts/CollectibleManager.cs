using UnityEngine;
using TMPro;

// Manages all collectibles in the scene and updates a progress UI (SliderPersentage)
public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    [Tooltip("Optional reference to the progress UI that will fill as collectibles are collected")]
    public SliderPersentage progressBar;

    [Tooltip("Optional text (TMP) that will show collected/total e.g. 3/10")]
    public TextMeshProUGUI countText;

    [Tooltip("Reference to the GameController to notify of completion")]
    public GameController gameController;

    int totalCollectibles = 0;
    int collected = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        // Count all active collectibles in the scene
        var items = FindObjectsOfType<collectibles>();
        totalCollectibles = items.Length;
        collected = 0;

        // Sync with game controller
        if (gameController != null)
        {
            gameController.targetCollectibles = totalCollectibles;
        }

        if (progressBar != null)
        {
            progressBar.SetMaxValue(Mathf.Max(1, totalCollectibles));
            progressBar.SetValue(0f);
        }

        Debug.Log($"CollectibleManager: found {totalCollectibles} collectibles");
    }

    // Re-count collectibles (useful if the manager was created at runtime)
    public void RecountCollectibles()
    {
        var items = FindObjectsOfType<collectibles>();
        totalCollectibles = items.Length;
        collected = 0;
        if (progressBar != null)
        {
            progressBar.SetMaxValue(Mathf.Max(1, totalCollectibles));
            progressBar.SetValue(0f);
        }

        if (countText != null)
            countText.text = $"{collected}/{totalCollectibles}";
    }

    // Call this when an item is collected
    public void RegisterCollect()
    {
        collected = Mathf.Min(collected + 1, totalCollectibles);
        if (progressBar != null)
        {
            // set the progress as the number of collected items
            progressBar.SetValue(collected);
        }

        if (countText != null)
            countText.text = $"{collected}/{totalCollectibles}";

        // Update game controller
        if (gameController != null)
        {
            gameController.AddCollect(1); // This will handle checking for game completion
        }

        Debug.Log($"CollectibleManager: collected {collected}/{totalCollectibles}");
    }
}
