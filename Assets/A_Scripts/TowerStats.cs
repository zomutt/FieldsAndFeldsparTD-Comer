using UnityEngine;

public class TowerStats
{
    /// <summary>
    /// The player may purchase upgrades for their towers and gold farm, and this is what stores and saves them
    /// Upgrades persist through levels, but not through level losses.
    /// </summary>
    public static TowerStats Instance {  get; private set; } = new TowerStats();

    // SHOOTER TOWERS //
    private float shooterDamage;
    public float ShooterDamage => shooterDamage;

    private float savedShooterDamage;          // Upgrades are saved and stored when advancing level. This tells the tower what to revert to if the player loses a round after advancing.

    private float shooterRange;
    public float ShooterRange => shooterRange;

    private float savedShooterRange;

    private float shooterCost;
    public float ShooterCost => shooterCost;

    // AOE TOWERS //
    private float aoeDamage;
    public float AoeDamage => aoeDamage;

    private float savedAoeDamage;

    private float aoeRange;
    public float AoeRange => aoeRange;

    private float savedAoeRange;

    private float aoeCost;
    public float AoeCost => aoeCost;

    // GOLD FARM //
    private float goldPerSec;
    public float GoldPerSec => goldPerSec;
    private float savedGoldPerSec;

    private float goldCost;
    public float GoldCost => goldCost;

    internal void SetSavedStats()
    {
        // Called when level is won
        savedShooterDamage = shooterDamage;
        savedShooterRange = shooterRange;

        savedAoeDamage = aoeDamage;
        savedAoeRange = aoeRange;

        savedGoldPerSec = goldPerSec;
    }
    internal void StartLevelOver()
    {
        // Called when level is lost
        shooterDamage = savedShooterDamage;
        shooterRange = savedShooterRange;

        aoeDamage = savedAoeDamage;
        aoeRange = savedAoeRange;

        goldPerSec = savedGoldPerSec;
    }
    internal void UpgradeShooterDamage(float damage)
    {
        shooterDamage += damage;
        Debug.Log($"New shooter tower damage: {shooterDamage}");
    }
    internal void UpgradeShooterRange(float range)
    { 
        shooterRange += range;
        Debug.Log($"New shooter range: {shooterRange}");
    }
    internal void IncreaseShooterCost(float cost)
    {
        // The cost of each tower increases per level. This is ONLY called when advancing to the next level.
        shooterCost += cost;
    }
    internal void UpgradeAoeDamage(float damage)
    {
        aoeDamage += damage;
        Debug.Log($"New aoe damage: {aoeDamage}");
    }
    internal void UpgradeAoeRange(float range)
    {
        aoeRange += range;
        Debug.Log($"New aoe range: {aoeRange}");
    }
    internal void IncreaseAoeCost(float cost)
    {
        aoeCost += cost;
    }
    internal void IncreaseGoldPerSec(float rate)
    {
        goldPerSec += rate;
        Debug.Log($"New GPS: {goldPerSec}/s");
    }
    internal void IncreaseGoldCost(float cost)
    {
        goldCost += cost;
    }
}
