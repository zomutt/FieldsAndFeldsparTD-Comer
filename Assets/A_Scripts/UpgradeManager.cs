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
    private int shooterUpgrades;    // Tracks how many upgrades the player has purchased
    private int savedShooterUpgrades;

    [Header("AOE")]
    [SerializeField] private int baseAoeUpgradeCost;
    private int aoeUpgradeCost;
    private int savedAoeUpgradeCost;
    public int AoeUpgradeCost => aoeUpgradeCost;

    [SerializeField] private int aoeDMGUpgrade;
    public int AoeDMGUpgrade => aoeDMGUpgrade;
    private int aoeUpgrades;
    private int savedAoeUpgrades;

    [Header("Gold Mine")]
    [SerializeField] private int baseMineUpgradeCost;
    private int mineUpgradeCost;
    private int savedMineUpgradeCost;
    public int MineUpgradeCost => mineUpgradeCost;

    [SerializeField] private int mineYieldUpgrade;
    public int MineYieldUpgrade => mineYieldUpgrade;
    private int mineUpgrades;
    private int savedMineUpgrades;

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

        savedShooterUpgradeCost = baseShooterUpgradeCost;
        savedAoeUpgradeCost = baseAoeUpgradeCost;

        mineUpgradeCost = baseMineUpgradeCost;
        savedMineUpgradeCost = baseMineUpgradeCost;

        shooterUpgrades = 0;
        savedShooterUpgrades = 0;

        aoeUpgrades = 0;
        savedAoeUpgrades = 0;

        mineUpgrades = 0;
        savedMineUpgrades = 0;
    }
    public void SaveData()
    {
        // Called at level victory
        savedShooterUpgradeCost = shooterUpgradeCost;
        savedShooterUpgrades = shooterUpgrades;

        savedAoeUpgradeCost = aoeUpgradeCost;
        savedAoeUpgrades = aoeUpgrades;

        savedMineUpgrades = mineUpgrades;
        savedMineUpgradeCost = mineUpgradeCost;

    }
    public void LoadData()
    {
        // Called when restarting a new level. The player is not to keep their upgrades if they lose, they are only rewarded for winning.
        // Base gold granted at the level start makes up for if the player put themselves in a bad spot if they can figure out how to recover.
        shooterUpgradeCost = savedShooterUpgradeCost;
        TowerStats.Instance.ChangeShooterDamage(-shooterDMGUpgrade * (shooterUpgrades - savedShooterUpgrades));
        shooterUpgrades = savedShooterUpgrades;

        aoeUpgradeCost = savedAoeUpgradeCost;
        TowerStats.Instance.ChangeAoeDamage(-aoeDMGUpgrade * (aoeUpgrades - savedAoeUpgrades));
        aoeUpgrades = savedAoeUpgrades;

        mineUpgradeCost = savedMineUpgradeCost;
        GoldManager.Instance.ChangeGoldYield(-mineYieldUpgrade * (mineUpgrades - savedMineUpgrades));
        mineUpgrades = savedMineUpgrades;

        UIController.Instance.UpdateUI();
    }
    public void UpgradeShooter()
    {
        if (GoldManager.Instance.CurrentGold < shooterUpgradeCost)
        {
            Debug.Log("You need more gold.");
            return;
        }
        TowerStats.Instance.ChangeShooterDamage(shooterDMGUpgrade);
        GoldManager.Instance.DecreaseGold(shooterUpgradeCost);

        shooterUpgradeCost += costIncrease;
        shooterUpgrades++;
        UIController.Instance.UpdateUI();
    }
    public void UpgradeAoe()
    {
        if (GoldManager.Instance.CurrentGold < aoeUpgradeCost)
        {
            Debug.Log("You need more gold.");
            return;
        }
        TowerStats.Instance.ChangeAoeDamage(aoeDMGUpgrade);
        GoldManager.Instance.DecreaseGold(aoeUpgradeCost);

        aoeUpgradeCost += costIncrease;
        aoeUpgrades++;
        UIController.Instance.UpdateUI();
    }
    public void UpgradeMine()
    {
        if (GoldManager.Instance.CurrentGold < mineUpgradeCost)
        {
            Debug.Log("You need more gold.");
            return;
        }
        GoldManager.Instance.ChangeGoldYield(mineYieldUpgrade);
        GoldManager.Instance.DecreaseGold(mineUpgradeCost);

        mineUpgradeCost += costIncrease;
        mineUpgrades++;
        UIController.Instance.UpdateUI();
    }
}
