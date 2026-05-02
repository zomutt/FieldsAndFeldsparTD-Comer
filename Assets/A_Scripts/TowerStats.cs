using UnityEngine;


[CreateAssetMenu(fileName = "TowerStats", menuName = "ScriptableObjects/TowerStats", order = 1)]
public class TowerStats : ScriptableObject
{
    /// <summary>
    /// The player may purchase upgrades for their towers and gold farm, and this is what stores and saves them
    /// Upgrades persist through levels, but not through level losses.
    /// Using a ScriptableObject so tower stats live in one place and can be tuned in the Inspector.
    /// This keeps the data out of scene objects and avoids hardcoding values in scripts.
    /// 
    /// THIS IS NOT FULLY IMPLEMENTED YET. I have the structure in place, but I have not yet implemented the actual upgrade system in the game.
    /// </summary>

    public static TowerStats Instance { get; private set; }

    [Header("Shooter Towers")]
    [SerializeField] private int baseShooterDamage;      // This stays the same, it is the value needed at the beginning of the game. No upgrades. 
    [SerializeField] private float shooterDamage;
    public float ShooterDamage => shooterDamage;

    [SerializeField] private float shooterCD;      // Attack speed never changes, so no need to initialize it
    public float ShooterCD => shooterCD;

    [SerializeField] private int baseShooterCost;        // This stays the same
    [SerializeField] private int shooterCost;
    public float ShooterCost => shooterCost;

    [Header("AOE Towers")]
    [SerializeField] private int baseAoeDamage;      // This stays the same
    [SerializeField] private float aoeDamage;
    public float AoeDamage => aoeDamage;

    [SerializeField] private float aoeDuration;
    public float AoeDuration => aoeDuration;

    [SerializeField] private float aoeCD;
    public float AoeCD => aoeCD;

    [SerializeField] private int baseAoeCost;        
    [SerializeField] private int aoeCost;       
    public int AoeCost => aoeCost;

    /// SAVED STATS ///
    private float savedShooterDamage;
    private float savedAoeDamage;

    private void OnEnable()
    {
        Instance = this;
    }
    public void InitializeStats()
    {
        // Called at the beginning of the game to set what the base is meant to be. This is important for if the player starts from scratch after playing a game i.e. winning, losing, quitting
        shooterDamage = baseShooterDamage;
        shooterCost = baseShooterCost;

        aoeDamage = baseAoeDamage;
        aoeCost = baseAoeCost;
    }
    public void SaveStats()
    {
        // Called when level is won
        savedShooterDamage = shooterDamage;

        savedAoeDamage = aoeDamage;
    }
    public void LoadStats()
    {
        // Called when level is lost
        shooterDamage = savedShooterDamage;

        aoeDamage = savedAoeDamage;
    }
    public void UpgradeShooterDamage(float damage)
    {
        shooterDamage += damage;
        Debug.Log($"New shooter tower damage: {shooterDamage}");
    }
    public void IncreaseShooterCost(int cost)
    {
        // The cost of each tower increases per level. This is ONLY called when advancing to the next level.
        shooterCost += cost;
    }
    public void UpgradeAoeDamage(float damage)
    {
        aoeDamage += damage;
        Debug.Log($"New aoe damage: {aoeDamage}");
    }
    public void IncreaseAllCosts(int costIncreasePerLevel)
    { 
        // Called from GameManager when advancing to next level
        aoeCost += costIncreasePerLevel;
        shooterCost += costIncreasePerLevel;
    }
}
