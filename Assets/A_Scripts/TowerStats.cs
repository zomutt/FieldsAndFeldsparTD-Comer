using UnityEngine;


[CreateAssetMenu(fileName = "TowerStats", menuName = "ScriptableObjects/TowerStats", order = 1)]
public class TowerStats : ScriptableObject
{
    /// <summary>
    /// The player may purchase upgrades for their towers and gold farm, and this is what stores and saves them
    /// Upgrades persist through levels, but not through level losses.
    /// Using a ScriptableObject so tower stats live in one place and can be tuned in the Inspector.
    /// This keeps the data out of scene objects and avoids hardcoding values in scripts.
    /// </summary>

    public static TowerStats Instance { get; private set; }

    [Header("All")]
    [SerializeField] private int costIncreasePerLevel;

    [Header("Shooter Towers")]
    [SerializeField] private int baseShooterDamage;      // This stays the same, it is the value needed at the beginning of the game. No upgrades. 
    private int shooterDamage;
    public int ShooterDamage => shooterDamage;

    [SerializeField] private float shooterCD;      // Attack speed never changes, so no need to initialize it
    public float ShooterCD => shooterCD;

    [SerializeField] private int baseShooterCost;        // This stays the same
    private int shooterCost;
    public int ShooterCost => shooterCost;

    [Header("AOE Towers")]
    [SerializeField] private int baseAoeDamage;      // This stays the same
    private int aoeDamage;
    public int AoeDamage => aoeDamage;

    [SerializeField] private float aoeDuration;
    public float AoeDuration => aoeDuration;

    [SerializeField] private float aoeCD;
    public float AoeCD => aoeCD;

    [SerializeField] private int baseAoeCost;        
    private int aoeCost;       
    public int AoeCost => aoeCost;

    [Header("Single Target Slow Towers")]
    [Header("Slow amt acts as %")]
    [SerializeField] private float baseStSlowAmount;       // Calculated as a %, use 1-100 (do not exceed 65 unless for testing)
    private float stSlowAmount;              
    public float StSlowAmount => stSlowAmount;

    [SerializeField] private float stSlowTime;
    public float StSlowTime => stSlowTime;

    [SerializeField] private int baseStSlowCost;
    private int stSlowCost;
    public int StSlowCost => stSlowCost;
    [SerializeField] private float stSlowCD;
    public float StSlowCD => stSlowCD;

    /// SAVED STATS ///
    private int savedShooterDamage;

    private int savedAoeDamage;

    private float savedStSlowAmount;

    private void OnEnable()
    {
        Instance = this;
    }
    private void OnValidate()
    {
        float stBefore = stSlowAmount;
        stSlowAmount = Mathf.Clamp(stSlowAmount, 1f, 100f);
        if (stBefore != stSlowAmount)
        {
            Debug.Log($"Single target slow clamped! {stBefore} -> {stSlowAmount}. Make sure you are entering in a value between 1-100; it operates as a percentage.");
        }
        if (stSlowAmount > 65)
        {
            Debug.Log($"Slow tower value exceeds recommended cap of slow cap ({UpgradeManager.Instance.StSlowUpgradeCap}). Your value: ({stSlowAmount}). Is this for testing?");
        }
    }
    public void InitializeStats()
    {
        // Called at the beginning of the game to set what the base is meant to be. This is important for if the player starts from scratch after playing a game i.e. winning, losing, quitting.

        // Shooter towers
        shooterDamage = baseShooterDamage;
        savedShooterDamage = baseShooterDamage;
        shooterCost = baseShooterCost;

        // AOE towers
        aoeDamage = baseAoeDamage;
        savedAoeDamage = baseAoeDamage;
        aoeCost = baseAoeCost;

        // Single-target slow towers
        stSlowAmount = baseStSlowAmount;
        savedStSlowAmount = baseStSlowAmount;
        stSlowCost = baseStSlowCost;
    }
    public void SaveStats()
    {
        // Called when level is won
        savedShooterDamage = shooterDamage;

        savedAoeDamage = aoeDamage;

        savedStSlowAmount = stSlowAmount;
    }
    public void LoadStats()
    {
        // Called when level is lost
        shooterDamage = savedShooterDamage;

        aoeDamage = savedAoeDamage;

        stSlowAmount = savedStSlowAmount;
    }
    public void ChangeShooterDamage(int damage)
    {
        // Upgrades and resets 
        shooterDamage += damage;
        Debug.Log($"New shooter tower damage: {shooterDamage}");
    }
    public void ChangeAoeDamage(int damage)
    {
        // Upgrades and resets
        aoeDamage += damage;
        Debug.Log($"New aoe damage: {aoeDamage}");
    }
    public void ChangeStSlow(float slow)
    {
        stSlowAmount += slow;
        Debug.Log($"New single-target slow amount: {stSlowAmount}");
    }
    public void CapSlow()      // Needed for if the upgrade is higher than the cap, we just essentially clamp it. This should not ever occur, but this catches it if it does.
    {
        stSlowAmount = UpgradeManager.Instance.StSlowUpgradeCap;
        Debug.Log($"Single target slow tower is set to cap. New slow amount: {stSlowAmount}. Fix upgrade amounts to be even.");
    }
    public void IncreaseAllCosts()
    {
        // Called from GameManager ONLY when advancing to next level.
        aoeCost += costIncreasePerLevel;
        shooterCost += costIncreasePerLevel;
        stSlowCost += costIncreasePerLevel;

        UIController.Instance.UpdateUI();
    }
}
