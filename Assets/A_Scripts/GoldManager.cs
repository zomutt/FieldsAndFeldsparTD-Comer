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
    private readonly float goldInterval = 1f; // Time in seconds between gold generation

    [Header("Base Stats")]
    [SerializeField] private int baseGoldFarmYield;
    [SerializeField] private int baseGold;
    [SerializeField] private int basePassiveIncome;
    public int BasePassiveIncome => basePassiveIncome;
    [SerializeField] private int baseFarmCost;

    [Header("Gameplay-Changable Stats")]
    [SerializeField] private int goldFarmYield;
    public int GoldFarmYield => goldFarmYield;
    [SerializeField] private int goldFarmCost;
    public int GoldFarmCost => goldFarmCost;
    

    private int currentGold;
    public int CurrentGold => currentGold;

    private int savedGold;    // If player loses, they start with whatever gold they had at the beginning of the level
    private int savedGoldFarmYield;


    private int totalGoldFarms;       // GoldFarms are separate from this script in order to avoid situations where like 50 mines are all running update.

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
    }
    public void StartGame()
    {
        // Called at the very beginning of the game so that the player has something to start with.
        savedGold = currentGold;
        savedGoldFarmYield = goldFarmYield;
    }
    public void InitializeStats()
    {
        // Called by GameManager.cs at the very start of the game in order to stash what the player should have at level 1 in case of a fresh game start. Ensures all saved data is cleared.
        goldFarmYield = baseGoldFarmYield;
        goldFarmCost = baseFarmCost;
        currentGold = baseGold;
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
        // Called by GameManager.cs when a level is won
        savedGold = currentGold;
        savedGoldFarmYield = goldFarmYield;
    }
    public void LoadStats()
    {
        // Called by GameManager.cs when a level is lost
        currentGold = savedGold;
        goldFarmYield = savedGoldFarmYield;
    }
    public int GoldPerSec()
    {
        return ((goldFarmYield * totalGoldFarms) + basePassiveIncome);
    }
    private void GenerateGold()
    {
        int goldToAdd = (goldFarmYield * totalGoldFarms) + basePassiveIncome;
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

    // v This feature MIGHT come back for portfolio. If we want to add a feature where the player can sell gold farms, then we would need this method to decrease the farm count and update the gold generation accordingly.
    // For now, since we do not have a way to remove gold farms, this method is not necessary and can be added back in later if I decide to implement that feature. 

    //public void DecreaseFarmCount()
    //{
    //    // Called by GoldFarms.cs when a gold farm is removed, so that the gold generation can be updated accordingly.
    //    totalGoldFarms--;
    //    UIController.Instance.UpdateUI();
    //    Debug.Log($"Gold farm removed. Total gold farms: {totalGoldFarms}");
    //}
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
    }
    public void DecreaseGold(int amount)
    {
        // Called when purchasing towers or upgrades
        currentGold -= amount;
        UIController.Instance.UpdateUI();
    }
    public void IncreaseGoldCost(int goldIncreasePerLevel)
    {
        goldFarmCost += goldIncreasePerLevel;
        UIController.Instance.UpdateUI();
        Debug.Log($"Gold farm cost increased. New cost: {goldFarmCost}");
    }
}