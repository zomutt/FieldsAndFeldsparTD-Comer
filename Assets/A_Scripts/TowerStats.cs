using UnityEditor.ShaderGraph.Internal;
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
    [SerializeField] private float shooterDamage;
    public float ShooterDamage => shooterDamage;
    [SerializeField] private float shooterCD;
    public float ShooterCD => shooterCD;
    [SerializeField] private int shooterCost;
    public float ShooterCost => shooterCost;

    [Header("AOE Towers")]
    [SerializeField] private float aoeDamage;
    public float AoeDamage => aoeDamage;

    [SerializeField] private float aoeDuration;
    public float AoeDuration => aoeDuration;

    [SerializeField] private float aoeCD;
    public float AoeCD => aoeCD;

    [SerializeField] private int aoeCost;
    public int AoeCost => aoeCost;

    [Header("Gold Farm")]
    [SerializeField] private float goldPerSec;
    public float GoldPerSec => goldPerSec;

    [SerializeField] private int goldCost;
    public int GoldCost => goldCost;

    private float savedShooterDamage;
    private float savedShooterCD;
    private float savedAoeDamage;
    private float savedAoeCD;
    private float savedGoldPerSec;

    private void OnEnable()
    {
        Instance = this;
    }
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
    internal void IncreaseShooterCost(int cost)
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
    internal void IncreaseAoeCost(int cost)
    {
        aoeCost += cost;
    }
    internal void IncreaseGoldPerSec(float rate)
    {
        goldPerSec += rate;
        Debug.Log($"New GPS: {goldPerSec}/s");
    }
    internal void IncreaseGoldCost(int cost)
    {
        goldCost += cost;
    }
}
