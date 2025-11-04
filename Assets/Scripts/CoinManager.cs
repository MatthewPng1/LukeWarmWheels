using UnityEngine;
using System;
using System.Collections.Generic;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    // Public read-only access to coins
    public int Coins { get; private set; }

    // Optional event so UI or other systems can update when coins change
    public event Action<int> OnCoinsChanged;

    private const string CoinsPrefKey = "Coins";

    // Track per-level collectible counts so we can show "collected across game / total available"
    private Dictionary<string, int> levelCoinTotals = new Dictionary<string, int>();

    // Total number of collectible coins available across registered levels
    public int TotalAvailableCoins { get; private set; }

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ResetCoins(); // Reset coins when game starts instead of loading saved coins
        }
        else
        {
            Destroy(gameObject); // only one instance allowed
        }
    }

    // Adds coins and notifies listeners
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        Coins += amount;
        SaveCoins();
        OnCoinsChanged?.Invoke(Coins);
    }

    // Attempt to spend coins, return true when successful
    public bool SpendCoins(int amount)
    {
        if (amount <= 0) return false;
        if (Coins >= amount)
        {
            Coins -= amount;
            SaveCoins();
            OnCoinsChanged?.Invoke(Coins);
            return true;
        }
        return false;
    }

    public void SetCoins(int amount)
    {
        Coins = Mathf.Max(0, amount);
        SaveCoins();
        OnCoinsChanged?.Invoke(Coins);
    }

    public void ResetCoins()
    {
        Coins = 0;
        PlayerPrefs.SetInt(CoinsPrefKey, 0);
        PlayerPrefs.Save();
        // Clear any per-level registered totals as well for a fresh playthrough
        levelCoinTotals.Clear();
        TotalAvailableCoins = 0;
        OnCoinsChanged?.Invoke(Coins);
    }

    /// <summary>
    /// Register the total number of collectible coins present in a level/scene.
    /// This will be counted only once per scene name to avoid double-counting when
    /// reloading the same scene.
    /// </summary>
    public void RegisterLevelTotal(string sceneName, int count)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        if (count <= 0) return;
        if (levelCoinTotals.ContainsKey(sceneName)) return; // already registered

        levelCoinTotals[sceneName] = count;
        TotalAvailableCoins += count;
    }

    // Save to PlayerPrefs so value persists between runs (optional)
    public void SaveCoins()
    {
        PlayerPrefs.SetInt(CoinsPrefKey, Coins);
        PlayerPrefs.Save();
    }

    // Load from PlayerPrefs
    public void LoadCoins()
    {
        Coins = PlayerPrefs.GetInt(CoinsPrefKey, 0);
        OnCoinsChanged?.Invoke(Coins);
    }
}