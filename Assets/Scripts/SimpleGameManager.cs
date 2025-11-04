using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class SimpleGameManager : MonoBehaviour
{
    public static SimpleGameManager instance;

    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private TMP_Text endGameTitle;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text levelCompleteCoins;

    private int coinCount = 0;
    private int totalCoins = 0;
    private int savedCoinCount = 0;

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        SimpleUIManager.instance.fadeFromBlack = true;
        savedCoinCount = 0; // Reset saved count at level start
        UpdateGUI();
        FindTotalCoins();
    }
    
  //  private bool isRestarting = false;
    
//    public void RestartLevel()
  //  {
 //       isRestarting = true;
   //     StartCoroutine(RestartLevelCoroutine());
  //  }

    public void IncrementCoinCount()
    {
        coinCount++;
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddCoins(1);
        }
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        if (coinText != null)
        {
            coinText.text = coinCount.ToString();
        }
    }

    private void FindTotalCoins()
    {
        // Find all objects with Coin tag in the scene
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        totalCoins = coins.Length;

        // Register this level's total with the global CoinManager so we can compute
        // the total available coins across the whole playthrough.
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.RegisterLevelTotal(SceneManager.GetActiveScene().name, totalCoins);
        }
    }

    public void LevelComplete()
    {
        blackScreen.SetActive(true);
        StartCoroutine(LevelCompleteCoroutine());
    }
   
    private IEnumerator LevelCompleteCoroutine()
    {
        yield return new WaitForSeconds(2f);
        endGamePanel.SetActive(true);
        endGameTitle.text = "LEVEL COMPLETE";
        levelCompleteCoins.text = "COINS COLLECTED: " + coinCount.ToString() + " / " + totalCoins.ToString();
    }

    public void RestartLevel()
    {
        StartCoroutine(RestartLevelCoroutine());
    }

    private IEnumerator RestartLevelCoroutine()
    {
        blackScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        
        // Reset CoinManager to 0 before reloading scene
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.ResetCoins();
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}