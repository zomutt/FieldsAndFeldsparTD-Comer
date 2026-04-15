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

    private float shooterCD;
    public float ShooterCD => shooterCD;

    private float savedShooterCD;

    private float shooterCost;
    public float ShooterCost => shooterCost;

    // AOE TOWERS //
    private float aoeDamage;
    public float AoeDamage => aoeDamage;

    private float savedAoeDamage;

    private float aoeCD;
    public float AoeCD => aoeCD;

    private float savedAoeCD;

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
        savedShooterCD = shooterCD;

        savedAoeDamage = aoeDamage;
        savedAoeCD = aoeCD;

        savedGoldPerSec = goldPerSec;
    }
    internal void StartLevelOver()
    {
        // Called when level is lost
        shooterDamage = savedShooterDamage;
        shooterCD = savedShooterCD;

        aoeDamage = savedAoeDamage;
        aoeCD = savedAoeCD;

        goldPerSec = savedGoldPerSec;
    }
    internal void UpgradeShooterDamage(float damage)
    {
        shooterDamage += damage;
        Debug.Log($"New shooter tower damage: {shooterDamage}");
    }
    internal void UpgradeShooterCD(float cd)
    { 
        shooterCD -= cd;
        Debug.Log($"New shooter range: {shooterCD}");
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
    internal void UpgradeAoeCD(float range)
    {
        aoeCD += range;
        Debug.Log($"New aoe range: {aoeCD}");
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
