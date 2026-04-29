using System.Collections;
using UnityEngine;

/// <summary>
/// This is a small script that serves to handle all things gold generation and gold book keeping.
/// Responsible for generating gold per second, tracking how much gold the player has, and updating the UI accordingly.
/// </summary>
public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    private float goldTimer;
    private float goldInterval = 1f; // Time in seconds between gold generation

    [SerializeField] private int goldFarmYield;
    public int GoldFarmYield => goldFarmYield;

    private int totalGoldFarms;       // GoldFarms are separate from this script in order to avoid situations where like 50 mines are all running update.

    private int currentGold;
    public int CurrentGold => currentGold;

    private int savedGold;    // If player loses, they start with whatever gold they had at the beginning of the level
    private int savedGoldFarmYield;

    [SerializeField] private int passiveIncome;      // Base income that is independent from the mines

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        goldTimer = 0f;
        goldFarmYield = 5;
        savedGoldFarmYield = 5;
    }
    public void StartGame()
    {
        // Called at the very beginning of the game so that the player has something to start with.
        currentGold = 5000;
        savedGold = currentGold;

        savedGoldFarmYield = goldFarmYield;
    }
    private void Update()
    {
        goldTimer += Time.deltaTime;
        if (goldTimer >= goldInterval)
        {
            GenerateGold();
            goldTimer = 0f; // Reset the timer
        }
    }
    public void SaveStats()
    {
        savedGold = currentGold;
        savedGoldFarmYield = goldFarmYield;
    }
    public void LoadStats()
    {
        currentGold = savedGold;
        goldFarmYield = savedGoldFarmYield;
    }
    public int GoldPerSec()
    {
        return (goldFarmYield * totalGoldFarms);
    }
    private void GenerateGold()
    {
        int goldToAdd = (goldFarmYield * totalGoldFarms) + passiveIncome;
        //Debug.Log($"Generating gold: {goldToAdd} (Yield: {goldFarmYield} x Farms: {totalGoldFarms}) + Passive income: {passiveIncome}");
        currentGold += goldToAdd;
        UIController.Instance.UpdateUI();
    }
    public void IncreaseFarmCount()
    {
        // Caled by GoldFarms.cs when a new gold farm is added, so that the gold generation can be updated accordingly.
        totalGoldFarms++;
        UIController.Instance.UpdateUI();
        Debug.Log($"Gold farm added. Total gold farms: {totalGoldFarms}");
    }
    public void DecreaseFarmCount()
    {
        // Called by GoldFarms.cs when a gold farm is removed, so that the gold generation can be updated accordingly.
        totalGoldFarms--;
        UIController.Instance.UpdateUI();
        Debug.Log($"Gold farm removed. Total gold farms: {totalGoldFarms}");
    }
    public void IncreaseGoldYield(int yieldIncrease)
    {
        // Called by the upgrade system when the player purchases a gold farm yield upgrade, so that the gold generation can be updated accordingly.
        goldFarmYield += yieldIncrease;
        UIController.Instance.UpdateUI();
        Debug.Log($"Gold farm yield increased. New yield: {goldFarmYield}");
    }
    public void GiveGold(int amount)
    {
        // Called when an enemy is killed since enemies drop gold as reward
        currentGold += amount;
        UIController.Instance.UpdateUI();
        Debug.Log($"Enemy killed! Gold increased by {amount}. Current gold: {currentGold}");
    }
    public void DecreaseGold(int amount)
    {
        // Called when purchasing towers or upgrades
        currentGold -= amount;
        UIController.Instance.UpdateUI();
        Debug.Log($"Gold taken: {amount}, New Gold {currentGold}");
    }
}