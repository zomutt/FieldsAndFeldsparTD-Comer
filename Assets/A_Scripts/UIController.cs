using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI roundText;

    [Header("Castle Stats")]
    [SerializeField] private TextMeshProUGUI castleHealthText;
    [SerializeField] private TextMeshProUGUI castleArmorText;
    [SerializeField] private TextMeshProUGUI totalGoldText;
    [SerializeField] private TextMeshProUGUI goldPerSecText;

    [Header("Panels")]
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject castleStatsPanel;

    [Header("Towers")]
    [SerializeField] private TextMeshProUGUI shooterDMG;
    [SerializeField] private TextMeshProUGUI shooterCost;

    [SerializeField] private TextMeshProUGUI aoeDMG;
    [SerializeField] private TextMeshProUGUI aoeCost;

    [SerializeField] private TextMeshProUGUI goldYield;
    [SerializeField] private TextMeshProUGUI goldCost;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        timerPanel.SetActive(true);
        castleStatsPanel.SetActive(true);
        losePanel.SetActive(false);
        roundText.text = null;
    }
    private void Update()
    {
        // Convert elapsed time into MM:SS format
        int minutes = Mathf.FloorToInt(Time.time/60f);
        int seconds = Mathf.FloorToInt(Time.time % 60f); // Seconds = remainder after dividing by 60

        timerText.text = $"Time Elapsed: {minutes:00}:{seconds:00}"; // The 00 ensures that 2 digits will always be shown
    }
    internal void UpdateUI()
    {
        // This rounds up or down because while there's percentage based damage reduction that gives messy numbers, the players do not need to see this on the front end.
        int displayHP = Mathf.RoundToInt(CastleStats.Instance.CurrentHealth);

        castleHealthText.text = $"Castle HP: {displayHP}/{CastleStats.Instance.MaxHealth}";
        castleArmorText.text = $"Castle Armor: {CastleStats.Instance.Armor}";
        totalGoldText.text = $"Gold: {GoldManager.Instance.CurrentGold}";
        goldPerSecText.text = $"Gold/Sec: {GoldManager.Instance.GoldPerSec()}";

        shooterDMG.text = $"Damage: {TowerStats.Instance.ShooterDamage}";
        shooterCost.text = $"Cost: {TowerStats.Instance.ShooterCost}";

        aoeDMG.text = $"Damage/Sec: {TowerStats.Instance.AoeDamage}";
        aoeCost.text = $"Cost: {TowerStats.Instance.AoeCost}";

        goldYield.text = $"Gold Yield: {TowerStats.Instance.GoldPerSec}";
        goldCost.text = $"Cost: {TowerStats.Instance.GoldCost}";
    }
    internal IEnumerator WaveCountdown(float time)
    {
        while (time > 0)
        {
            roundText.text = $"Round {TierManager.Instance.CurrentTier} starting in: {time:0}";
            yield return new WaitForSeconds(1f);
            time--;
        }
        roundText.text = null;
    }
    internal void LoseGame()
    {
        timerPanel.SetActive(false);
        castleStatsPanel.SetActive(false);
        losePanel.SetActive(true);
    }
    internal void OnClickReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        castleStatsPanel.SetActive(true);
        timerPanel.SetActive(true);

        // Resets stats to what was saved.
        CastleStats.Instance.LoadStats();
    }
}
