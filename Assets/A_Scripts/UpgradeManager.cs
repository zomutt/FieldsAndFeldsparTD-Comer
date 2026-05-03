using UnityEngine;

/// <summary>
/// This script handles all things upgrades. The methods are called from OnClick events in UIController.cs
/// Costs of upgrades increase each time you get them, amount that it upgrades do not. Cost increases to add increasing difficulty.
/// This will be revisited as the scope increases.
/// Ex: Shooter Upgrade 1: Costs 50g, +5 dmg. Shooter Upgrade 2: Costs 75g, +5dmg. Etc. etc. (example numbers only)
/// </summary>
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("Shooter")]
    [SerializeField] private int baseShooterUpgradeCost;
    private int shooterUpgradeCost;
    private int savedShooterUpgradeCost;
    public int ShooterUpgradeCost => shooterUpgradeCost;

    [SerializeField] private int shooterDMGUpgrade;     // Stays static. This will be revistited once there is a higher scope to the game.
    public int ShooterDMGUpgrade => shooterDMGUpgrade;

    [Header("AOE")]
    [SerializeField] private int baseAoeUpgradeCost;
    private int aoeUpgradeCost;
    private int savedAoeUpgradeCost;
    public int AoeUpgradeCost => aoeUpgradeCost;

    [SerializeField] private int aoeDMGUpgrade;
    public int AoeDMGUpgrade => aoeDMGUpgrade;

    [Header("Gold Mine")]
    [SerializeField] private int baseMineUpgradeCost;
    private int mineUpgradeCost;
    private int savedMineUpgradeCost;
    public int MineUpgradeCost => mineUpgradeCost;

    [SerializeField] private int mineYieldUpgrade;
    public int MineYieldUpgrade => mineYieldUpgrade;

    [Header("General")]
    [SerializeField] private int costIncrease;    // The price increase each category has per upgrade. Ex: 100 for 1st upgrade, 150 for next, 200 for next, etc. etc.

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void InitializeData()
    {
        // Called at the very beginning of the game by GameManager.cs
        // This allows for clean resets
        shooterUpgradeCost = baseShooterUpgradeCost;
        aoeUpgradeCost = baseAoeUpgradeCost;
        mineUpgradeCost = baseMineUpgradeCost;
        savedShooterUpgradeCost = baseShooterUpgradeCost;
        savedAoeUpgradeCost = baseAoeUpgradeCost;
        savedMineUpgradeCost = baseMineUpgradeCost;
    }
    public void SaveData()
    {
        savedShooterUpgradeCost = shooterUpgradeCost;
        savedAoeUpgradeCost = aoeUpgradeCost;
        savedMineUpgradeCost = mineUpgradeCost;
    }
    public void LoadData()
    {
        shooterUpgradeCost = savedShooterUpgradeCost;
        aoeUpgradeCost = savedAoeUpgradeCost;
        mineUpgradeCost = savedMineUpgradeCost;
    }
    public void UpgradeShooter()
    {
        if (GoldManager.Instance.CurrentGold < shooterUpgradeCost)
        {
            Debug.Log("You need more gold.");
            return;
        }
        TowerStats.Instance.UpgradeShooterDamage(shooterDMGUpgrade);
        GoldManager.Instance.DecreaseGold(shooterUpgradeCost);
        shooterUpgradeCost += costIncrease;
        UIController.Instance.UpdateUI();
    }
    public void UpgradeAoe()
    {
        if (GoldManager.Instance.CurrentGold < aoeUpgradeCost)
        {
            Debug.Log("You need more gold.");
            return;
        }
        TowerStats.Instance.UpgradeAoeDamage(aoeDMGUpgrade);
        GoldManager.Instance.DecreaseGold(aoeUpgradeCost);
        aoeUpgradeCost += costIncrease;
        UIController.Instance.UpdateUI();
    }
    public void UpgradeMine()
    {
        if (GoldManager.Instance.CurrentGold < mineUpgradeCost)
        {
            Debug.Log("You need more gold.");
            return;
        }
        GoldManager.Instance.IncreaseGoldYield(mineYieldUpgrade);
        GoldManager.Instance.DecreaseGold(mineUpgradeCost);
        mineUpgradeCost += costIncrease;
        UIController.Instance.UpdateUI();
    }
}
