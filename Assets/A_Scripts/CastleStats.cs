using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
public class CastleStats
{
    /// <summary>
    /// The Castle in this game acts as the stationary player, and thus, takes on a lot of the same attributes as a PlayerStats.cs script would.
    /// This includes health, upgrades, and currency.
    /// Of course, our player is also the builder (invulerable, does not fight), but that serves more as a simple tool to construct towers rather than the the thing to protect.
    /// Think of the castle as the main hub/econ center. I like to think of it as the main area where commerce and such happens. May be refactored later.
    /// </summary>
    public static CastleStats Instance { get; private set; } = new CastleStats();

    private float currentHealth;
    public float CurrentHealth => currentHealth;

    private float maxHealth;
    public float MaxHealth => maxHealth;

    // The player may repair their castle and purchase upgrades that increase amount repaired.
    private int repairAmount;
    public int RepairAmount => repairAmount;

    // Armor reduces incoming damage based off percentage. There is only damage type in game, so only one armor type is needed.
    // Done as decimals to correspond with percentage i.e. 1 - .5 = 95% incoming damage.
    private float armor;
    public float Armor => armor;

    // The castle is intended to be able to defend itself.
    private float damage;
    public float Damage => damage;

    // The player may purchase a shield in an emergency to grant temporary invulnerability.
    private bool hasShield;
    public bool HasShield => hasShield;

    // This works for both the shields and the iframe coroutine.

    private bool isInvincible;
    public bool IsInvincible => isInvincible;
    public void SetInvincible(bool value)
    {
        isInvincible = value;
    }

    // If the player loses during a level, they do not restart from the beginning of the game.
    // They start at the beginning of the level and get to keep whatever upgrades they had initially.
    // Ex: If during level one they get +5 maxHP, they go into level two with that +5 maxHP
    // If they die during level 2, they do not keep whatever upgrades they got during that level, but they still have the +5 maxHP
    private float savedMaxHealth;
    private float savedArmor;

    private CastleStats()
    {
        // ONLY called at the beginning of the game
        SetStatsInitial();
        SaveStats();
    }
    public void SaveStats()
    {
        savedMaxHealth = maxHealth;
        savedArmor = armor;
    }
    public void LoadStats()
    {
        // This is called when the player loses the round; the player respawns in the same level essentially from right where they left off.
        maxHealth = savedMaxHealth;
        armor = savedArmor;
        isInvincible = false;
    }
    public void SetStatsInitial()      // Only called at the beginning of the game, not on level restarts
    {
        maxHealth = 100;
        currentHealth = maxHealth;

        hasShield = false;
        isInvincible = false;

        savedArmor = .1f;
    }

    public void Repair()
    {
        currentHealth += repairAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Castle repaired! New castle HP: {currentHealth}");
    }
    public void TakeDamage(float rawDamage)
    {
        if (isInvincible) return;

        float armorReduction = (1 - armor);      // Ex: 1.00 - .1 = 90% damage taken
        float incomingDamage = rawDamage * armorReduction;

        currentHealth -= incomingDamage;

        CastleDamageHandler.Instance.BeginIframe();
        UIController.Instance.UpdateUI();

        if (currentHealth <= 0)
        {
            CastleDamageHandler.Instance.ClearAllDamageCoroutines();
            Debug.Log("CastleStats: Castle destroyed! Game over.");
            GameManager.Instance.CastleDestroyed();
            WaveSpawnPool.Instance.StopAllCoroutines();
        }
    }
}
