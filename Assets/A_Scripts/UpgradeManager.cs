using System.Runtime.CompilerServices;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;

/// <summary>
/// This script handles all things upgrades. The methods are called from OnClick events in UIController.cs
/// Costs of upgrades increase each time you get them, amount that it upgrades do not. Cost increases to add increasing difficulty.
/// This will be revisited as the scope increases.
/// Ex: Shooter Upgrade 1: Costs 50g, +5 dmg. Shooter Upgrade 2: Costs 75g, +5dmg. Etc. etc. (example numbers only)
/// 
/// Tracking upgrades is a system that is fully intended to come back.
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

    [Header("Singe-Target Slow Tower")]
    [SerializeField] private int baseStSlowUpgradeCost;
    private int stSlowUpgradeCost;
    private int savedStSlowUpgradeCost;
    public int StSlowUpgradeCost => stSlowUpgradeCost;
    [SerializeField] private float stSlowUpgrade;
    public float StSlowUpgrade => stSlowUpgrade;

    // There is an upgrade cap so that the player cannot break or exploit things.
    [SerializeField] private float stSlowUpgradeCap; 
    public float StSlowUpgradeCap => stSlowUpgradeCap;


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

        stSlowUpgradeCost = baseStSlowUpgradeCost;
        savedStSlowUpgradeCost = baseStSlowUpgradeCost;
    }
    public void SaveData()
    {
        // Called at level victory
        savedShooterUpgradeCost = shooterUpgradeCost;

        savedAoeUpgradeCost = aoeUpgradeCost;

        savedMineUpgradeCost = mineUpgradeCost;

        savedStSlowUpgradeCost = stSlowUpgradeCost;
    }
    public void LoadData()
    {
        // Called when restarting a new level. The player is not to keep their upgrades if they lose, they are only rewarded for winning.
        // Damage values are restored by TowerStats.LoadStats() -- no need to touch them here.
        shooterUpgradeCost = savedShooterUpgradeCost;

        aoeUpgradeCost = savedAoeUpgradeCost;

        mineUpgradeCost = savedMineUpgradeCost;

        stSlowUpgradeCost = savedStSlowUpgradeCost;

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
        UIController.Instance.UpdateUI();
    }

    public void UpgradeStSlow()
    {
        if (GoldManager.Instance.CurrentGold < stSlowUpgradeCost)
        {
            Debug.Log("You need more gold.");
            return;
        }
        else if (TowerStats.Instance.StSlowAmount == StSlowUpgrade)
        {
            Debug.Log("Slow is already capped, nothing happens.");
            return;
        }
        else if ((TowerStats.Instance.StSlowAmount + stSlowUpgrade) > StSlowUpgradeCap)
        {
            TowerStats.Instance.CapSlow();
            Debug.Log("Slow cap reached!");
            return;
        }
        else
        {
            TowerStats.Instance.ChangeStSlow(stSlowUpgrade);
            GoldManager.Instance.DecreaseGold(StSlowUpgradeCost);

            stSlowUpgradeCost += costIncrease;
            UIController.Instance.UpdateUI();
        }
    }
}