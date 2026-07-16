using System.Runtime.CompilerServices;
using UnityEditor.AdaptivePerformance.Editor;
using UnityEditor.ShaderGraph.Internal;
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

    private int shooterUpgradeLevel;   
    public int ShooterUpgradeLevel => shooterUpgradeLevel;
    private int savedShooterUpgradeLevel;

    [Header("AOE")]
    [SerializeField] private int baseAoeUpgradeCost;
    private int aoeUpgradeCost;
    private int savedAoeUpgradeCost;
    public int AoeUpgradeCost => aoeUpgradeCost;

    private int aoeUpgradeLevel;
    public int AoeUpgradeLevel => aoeUpgradeLevel;
    private int savedAoeUpgradeLevel;

    [Header("Gold Mine")]
    [SerializeField] private int baseMineUpgradeCost;
    private int mineUpgradeCost;
    private int savedMineUpgradeCost;
    public int MineUpgradeCost => mineUpgradeCost;

    private int mineUpgradeLevel;
    public int MineUpgradeLevel => mineUpgradeLevel;
    private int savedMineUpgradeLevel;

    [Header("Singe-Target Slow Tower")]
    [SerializeField] private int baseStSlowUpgradeCost;
    private int stSlowUpgradeCost;
    private int savedStSlowUpgradeCost;
    public int StSlowUpgradeCost => stSlowUpgradeCost;
    private int stSlowUpgradeLevel;
    public int StSlowUpgradeLevel => stSlowUpgradeLevel;
    private int savedStSlowUpgradeLevel;

    [SerializeField] private int stSlowUpgradeAmt;  // Stays static


    // There is an upgrade cap so that the player cannot break or freeze the AI
    [SerializeField] private float stSlowUpgradeCap; 
    public float StSlowUpgradeCap => stSlowUpgradeCap;


    [Header("Costs")]
    [SerializeField] private float costMultiplier;
    [SerializeField] private float slowCostMultiplier;      // Meant to be more expensive than regular towers

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

        shooterUpgradeLevel = 1;
        aoeUpgradeLevel = 1;
        stSlowUpgradeLevel = 1;
        mineUpgradeLevel = 1;

        savedMineUpgradeLevel = 1;
        savedStSlowUpgradeLevel = 1;
        savedAoeUpgradeLevel = 1;
        savedMineUpgradeLevel = 1;
    }
    public void SaveData()
    {
        // Called at level victory
        savedShooterUpgradeCost = shooterUpgradeCost;

        savedAoeUpgradeCost = aoeUpgradeCost;

        savedMineUpgradeCost = mineUpgradeCost;

        savedStSlowUpgradeCost = stSlowUpgradeCost;


        savedMineUpgradeLevel = mineUpgradeLevel;
        savedStSlowUpgradeLevel = stSlowUpgradeLevel;
        savedAoeUpgradeLevel = aoeUpgradeLevel;
        savedShooterUpgradeLevel = shooterUpgradeLevel;
    }
    public void LoadData()
    {
        // Called when restarting a new level. The player is not to keep their upgrades if they lose, they are only rewarded for winning.
        // Damage values are restored by TowerStats.LoadStats() -- no need to touch them here.
        shooterUpgradeCost = savedShooterUpgradeCost;
        shooterUpgradeLevel= savedShooterUpgradeLevel;

        aoeUpgradeCost = savedAoeUpgradeCost;
        aoeUpgradeLevel = savedAoeUpgradeLevel;

        mineUpgradeCost = savedMineUpgradeCost;
        mineUpgradeLevel = savedMineUpgradeLevel;

        stSlowUpgradeCost = savedStSlowUpgradeCost;
        stSlowUpgradeLevel = savedStSlowUpgradeLevel;

        UIController.Instance.UpdateUI();
    }

    public void UpgradeShooter()
    {
        if (GoldManager.Instance.CurrentGold < shooterUpgradeCost)
        {
            Debug.Log("You need more gold.");
            return;
        }
        TowerStats.Instance.ChangeShooterDamage(shooterUpgradeLevel);
        GoldManager.Instance.DecreaseGold(shooterUpgradeCost);

        shooterUpgradeCost = Mathf.RoundToInt(shooterUpgradeCost * costMultiplier);
        shooterUpgradeLevel++;
        UIController.Instance.UpdateUI();
    }
    public void UpgradeAoe()
    {
        if (GoldManager.Instance.CurrentGold < aoeUpgradeCost)
        {
            Debug.Log("You need more gold.");
            return;
        }
        TowerStats.Instance.ChangeAoeDamage(aoeUpgradeLevel);
        GoldManager.Instance.DecreaseGold(aoeUpgradeCost);

        aoeUpgradeCost = Mathf.RoundToInt(aoeUpgradeCost * costMultiplier);
        aoeUpgradeLevel++;
        UIController.Instance.UpdateUI();
    }
    public void UpgradeMine()
    {
        if (GoldManager.Instance.CurrentGold < mineUpgradeCost)
        {
            Debug.Log("You need more gold.");
            return;
        }
        GoldManager.Instance.ChangeGoldYield(mineUpgradeLevel);
        GoldManager.Instance.DecreaseGold(mineUpgradeCost);

        mineUpgradeCost = Mathf.RoundToInt(mineUpgradeCost * costMultiplier);
        mineUpgradeLevel++;
        UIController.Instance.UpdateUI();
    }

    public bool IsStSlowMaxed()
    {
        return TowerStats.Instance.StSlowAmount >= StSlowUpgradeCap;
    }

    public void UpgradeStSlow()
    {
        if (GoldManager.Instance.CurrentGold < stSlowUpgradeCost)
        {
            Debug.Log("You need more gold.");
            return;
        }
        if (IsStSlowMaxed())
        {
            Debug.Log("Slow is already capped, nothing happens.");
            return;
        }

        TowerStats.Instance.ChangeStSlow(stSlowUpgradeAmt);
        GoldManager.Instance.DecreaseGold(stSlowUpgradeCost);

        if (TowerStats.Instance.StSlowAmount > StSlowUpgradeCap)
        {
            TowerStats.Instance.CapSlow();
        }

        stSlowUpgradeLevel++;
        stSlowUpgradeCost = Mathf.RoundToInt(stSlowUpgradeCost * costMultiplier);
        UIController.Instance.UpdateUI();
    }
}