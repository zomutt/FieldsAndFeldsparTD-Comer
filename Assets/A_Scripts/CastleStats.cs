using System.Collections;
using UnityEngine;
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

    [SerializeField] private int repairAmount;
    public int RepairAmount => repairAmount;

    [SerializeField] private int baseRepairCost;
    private int currentRepairCost;
    public int CurrentRepairCost => currentRepairCost;
    [SerializeField] private int repairCostIncreasePerUse;
    private int savedRepairCost;
    


    // The player may purchase a shield in an emergency to grant temporary invulnerability. TEMP DISABLED. MAY COME BACK FOR PORTFOLIO.
    //private bool hasShield;
    //public bool HasShield => hasShield;

    [SerializeField] private int passiveHealAmount;

    // This works for both the shields and the iframe coroutine.

    private bool isInvincible;
    private bool isDevModeOn;

    [SerializeField] private GameObject shieldVisual;
    [SerializeField] private GameObject[] flames = new GameObject[7];

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        currentHealth = maxHealth;
        isInvincible = false;
        StartCoroutine(PassiveHeal());
        shieldVisual.SetActive(false);
    }
    public void InitializeStats()
    {
        savedRepairCost = baseRepairCost;
        DisableFlames();
    }
    public void LoadStats()
    {
        currentRepairCost = savedRepairCost;
    }
    public void SaveStats()
    {
        savedRepairCost = currentRepairCost;
    }
    public void Repair()
    {
        if (GoldManager.Instance.CurrentGold < currentRepairCost || currentHealth >= maxHealth)
        {
            return;
        }
        int workingHealth = currentHealth;     // For debugging purposes
        currentHealth += repairAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        GoldManager.Instance.DecreaseGold(currentRepairCost);
        Debug.Log($"Castle repaired for {currentHealth - workingHealth} ! New castle HP: {currentHealth}");

        currentRepairCost += repairCostIncreasePerUse;
        UIController.Instance.UpdateUI();
    }
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        if (isDevModeOn) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        StartCoroutine(Iframe());
        UIController.Instance.UpdateUI();

        if (currentHealth <= 0)
        {
            Debug.Log("CastleStats: Castle destroyed! Game over.");
            GameManager.Instance.CastleDestroyed();
        }
        FlameDeterminator();
    }
    private IEnumerator PassiveHeal()
    {
        // Passively heals the castle so that the player has a chance of recovery after a bad moment
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (currentHealth < maxHealth)
            {
                currentHealth += passiveHealAmount;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                UIController.Instance.UpdateUI();
                FlameDeterminator();
            }
        }
    }
    private void DisableFlames()
    {
        for (int i = 0; i < flames.Length; i++)
        {
            flames[i].SetActive(false);
        }
    }
    private void FlameDeterminator()
    { 
        // This method handles the display of status effect flames on the castle.
        // There is a better way to do this that I will get back around to after I take math.

        int flameCount = 0;

        float healthPercent = (currentHealth / maxHealth) * 100f;

        if (healthPercent <= 100 && healthPercent > 85) flameCount = 0;
        else if (healthPercent <= 85 && healthPercent > 70) flameCount = 1;
        else if (healthPercent <= 70 && healthPercent > 55) flameCount = 2;
        else if (healthPercent <= 55 && healthPercent > 40) flameCount = 3;
        else if (healthPercent <= 40 && healthPercent > 25) flameCount = 4;
        else if (healthPercent <= 25 && healthPercent > 10) flameCount = 5;
        else if (healthPercent <= 10) flameCount = flames.Length;
        else Debug.Log("Flames error.");

        for (int i = 0; i < flames.Length; i++)
        {
            flames[i].SetActive(i < flameCount);
        }
    }
    private IEnumerator Iframe()
    {
        isInvincible = true;
        yield return new WaitForSeconds(.4f);
        isInvincible = false;
    }

    public void ToggleDevInvincible()
    {
        if (!isDevModeOn)
        {
            isInvincible = true;
            isDevModeOn = true;
            shieldVisual.SetActive(true);
            Debug.Log("Castle is invulnerable");
        }
        else
        {
            isInvincible = false;
            isDevModeOn = false;
            shieldVisual.SetActive(false);
            Debug.Log("Castle is now vulnerable.");
        }
    }
}
