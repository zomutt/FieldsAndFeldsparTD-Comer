using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
public class CastleStats : MonoBehaviour
{
    /// <summary>
    /// The Castle in this game acts as the stationary player, and thus, takes on a lot of the same attributes as a PlayerStats.cs script would.
    /// This includes health, upgrades, and currency.
    /// Of course, our player is also the builder (invulerable, does not fight), but that serves more as a simple tool to construct towers rather than the the thing to protect.
    /// Systems such as shielding and repair will be implemented for portfolio, but passive heal is meant to make up for removing this mechanic.
    /// </summary>
    public static CastleStats Instance;

    [SerializeField] private int currentHealth;         // Serialized for testing purposes
    public int CurrentHealth => currentHealth;

    [SerializeField] private int maxHealth;
    public int MaxHealth => maxHealth;

    // The player may repair their castle and purchase upgrades that increase amount repaired.
    // TEMP DISABLED. MAY COME BACK FOR PORTFOLIO.
    // Passive heal is replacing shield/repair mechanics for fairness.

    //[SerializeField] private int repairAmount;
    //public int RepairAmount => repairAmount;


    // The player may purchase a shield in an emergency to grant temporary invulnerability. TEMP DISABLED. MAY COME BACK FOR PORTFOLIO.
    //private bool hasShield;
    //public bool HasShield => hasShield;

    [SerializeField] private int passiveHealAmount;

    // This works for both the shields and the iframe coroutine.

    private bool isInvincible;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        currentHealth = maxHealth;
        isInvincible = false;
        StartCoroutine(PassiveHeal());
    }

    //public void Repair()
    //{
    //    currentHealth += repairAmount;
    //    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    //    Debug.Log($"Castle repaired! New castle HP: {currentHealth}");
    //}
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        StartCoroutine(Iframe());
        UIController.Instance.UpdateUI();

        if (currentHealth <= 0)
        {
            Debug.Log("CastleStats: Castle destroyed! Game over.");
            GameManager.Instance.CastleDestroyed();
        }
    }
    private IEnumerator PassiveHeal()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (currentHealth < maxHealth)
            {
                currentHealth += passiveHealAmount;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                UIController.Instance.UpdateUI();
                Debug.Log($"Castle passively healed! New castle HP: {currentHealth}");
            }
        }
    }
    private IEnumerator Iframe()
    {
        isInvincible = true;
        yield return new WaitForSeconds(.4f);
        isInvincible = false;
    }
}
