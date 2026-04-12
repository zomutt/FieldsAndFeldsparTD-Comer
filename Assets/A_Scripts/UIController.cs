using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [Header("Timer")]
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Castle Stats")]
    [SerializeField] private GameObject castleStatsPanel;
    [SerializeField] private TextMeshProUGUI castleHealthText;
    [SerializeField] private TextMeshProUGUI castleArmorText;
    [SerializeField] private TextMeshProUGUI totalGoldText;

    [Header("Panels")]
    [SerializeField] private GameObject losePanel;
    

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
        totalGoldText.text = $"Gold: {CastleStats.Instance.CurrentGold}";
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
        CastleStats.Instance.StartLevelOver();
    }
}
