using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The UIController has been set to ONLY handle UI. It does not make any game decisions, it only displays information and has buttons that call methods in the GameManager.
/// Furthermore, with it being a Singleton, I ensured that all references are ONLY to UI objects. This is so that references never get lost.
/// The actual UI itself lives as a child of the Singleton UIController.cs GameObject.
/// </summary>
public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [Header("Menus")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject helpMenu;
    [SerializeField] private GameObject confirmQuitPanel;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI roundText;
    private int minutes;
    private int seconds;

    [Header("Castle Stats")]
    [SerializeField] private TextMeshProUGUI castleHealthText;
    [SerializeField] private TextMeshProUGUI totalGoldText;
    [SerializeField] private TextMeshProUGUI goldPerSecText;
    [SerializeField] private TextMeshProUGUI totalKillsText;

    [Header("In-Game Panels")]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject controlPanel;

    [Header("Pause/Help")]   // Honestly these two really just go hand in hand
    [SerializeField] private GameObject pausePanel;     // Black overlay that displays when game is paused
    [SerializeField] private TextMeshProUGUI pauseText;
    private bool isPaused;
    [SerializeField] private GameObject helpPanel;
    private bool isHelpOpen;
    private bool pausedByHelp;         // This is to try to help the bug that is occuring where closing from help keeps the game paused when it should not.

    [Header("Win/Lose")]
    [SerializeField] private GameObject levelWinPanel;
    [SerializeField] private GameObject winGamePanel;
    [SerializeField] private GameObject loseGamePanel;
    [SerializeField] private TextMeshProUGUI totalTime;       // Displayed at the end of the game

    [Header("Towers")]
    [SerializeField] private TextMeshProUGUI shooterDMG;
    [SerializeField] private TextMeshProUGUI shooterCost;
    [SerializeField] private TextMeshProUGUI shooterUpgradeCost;
    [SerializeField] private TextMeshProUGUI shooterUpgradeAmt;

    [SerializeField] private TextMeshProUGUI aoeDMG;
    [SerializeField] private TextMeshProUGUI aoeCost;
    [SerializeField] private TextMeshProUGUI aoeUpgradeCost;
    [SerializeField] private TextMeshProUGUI aoeUpgradeAmt;

    [SerializeField] private TextMeshProUGUI mineYield;
    [SerializeField] private TextMeshProUGUI mineCost;
    [SerializeField] private TextMeshProUGUI mineUpgradeCost;
    [SerializeField] private TextMeshProUGUI mineUpgradeAmt;


    // This is needed so that UI controller ONLY resets everything when the player begins a fresh game.
    private bool pendingGameReset = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (levelWinPanel != null) levelWinPanel.SetActive(false);
        if (winGamePanel != null) winGamePanel.SetActive(false);
        if (loseGamePanel != null) loseGamePanel.SetActive(false);

        if (pendingGameReset)
        {
            StartGame();
            pendingGameReset = false;
        }
    }
    private void Start()
    {
        StartGame();
    }
    private void StartGame()
    {
        // Called both in Start() and when the game is supposed to start over from scratch. This ensures that player may start again without having to close the game.
        controlPanel.SetActive(true);
        mainMenuPanel.SetActive(true);
        losePanel.SetActive(false);
        confirmQuitPanel.SetActive(false);
        winGamePanel.SetActive(false);
        levelWinPanel.SetActive(false);
        helpMenu.SetActive(false);

        pausePanel.SetActive(false);
        pauseText.text = "Pause";
        isPaused = false;
        isHelpOpen = false;

        roundText.text = null;

        minutes = 0;
        seconds = 0;
    }
    public void ResetTimer()
    {
        // Called by GameManager.cs when the game is completely reset.
        minutes = 0;
        seconds = 0;
    }
    private void Update()
    {
        // If time is frozen, don't update the timer.
        if (Time.timeScale == 0f) return;


        // Timer is used to give player feedback on how long or short they took to complete the game. This gives the player incentive to try again if they don't like their time.

        // Convert elapsed time into MM:SS format
        minutes = Mathf.FloorToInt(Time.time/60f);
        seconds = Mathf.FloorToInt(Time.time % 60f); // Seconds = remainder after dividing by 60

        timerText.text = $"Time Elapsed: {minutes:00}:{seconds:00}"; // The 00 ensures that 2 digits will always be shown
    }
    internal void UpdateUI()
    {
        if (CastleStats.Instance.CurrentHealth < 34)      // Makes sure the color of the castle HP stands out if the player is in immediate danger of losing.
        { 
            castleHealthText.color = Color.red;
        }
        else
        {
            castleHealthText.color = Color.black;
        }
        castleHealthText.text = $"Castle HP: {CastleStats.Instance.CurrentHealth}/{CastleStats.Instance.MaxHealth}";
        totalGoldText.text = $"Gold: {GoldManager.Instance.CurrentGold}";
        goldPerSecText.text = $"Gold/Sec: {GoldManager.Instance.GoldPerSec()}";

        shooterDMG.text = $"Damage: {TowerStats.Instance.ShooterDamage}";
        shooterCost.text = $"Cost: {TowerStats.Instance.ShooterCost}";
        shooterUpgradeCost.text = $"Upgrade Cost: {UpgradeManager.Instance.ShooterUpgradeCost}";
        shooterUpgradeAmt.text = $"Dmg Upgrade: +{UpgradeManager.Instance.ShooterDMGUpgrade}";

        aoeDMG.text = $"Damage/Sec: {TowerStats.Instance.AoeDamage}";
        aoeCost.text = $"Cost: {TowerStats.Instance.AoeCost}";
        aoeUpgradeCost.text = $"Upgrade Cost: {UpgradeManager.Instance.AoeUpgradeCost}";
        aoeUpgradeAmt.text = $"Dmg Upgrade: +{UpgradeManager.Instance.AoeDMGUpgrade}";

        mineYield.text = $"Gold Yield: {GoldManager.Instance.GoldFarmYield}";
        mineCost.text = $"Cost: {GoldManager.Instance.GoldFarmCost}";
        mineUpgradeCost.text = $"Upgrade Cost: {UpgradeManager.Instance.MineUpgradeCost}";
        mineUpgradeAmt.text = $"Yield Upgrade: +{UpgradeManager.Instance.MineYieldUpgrade}";

        totalKillsText.text = $"Total Kills: {GameManager.Instance.TotalKills}";
    }
    internal IEnumerator WaveCountdown(float time, int currentTier)
    {
        while (time > 0)
        {
            roundText.text = $"Round {currentTier} starting in: {time:0}";
            yield return new WaitForSeconds(1f);
            time--;
        }
        roundText.text = null;
    }
    public void TrackTotalKills(int totalKills)
    {
        totalKillsText.text = ($"Total Kills: {totalKills}");
    }
    internal void LoseGame()
    {
        losePanel.SetActive(true);
        controlPanel.SetActive(false);
    }
    public void WinLevel()
    {
        levelWinPanel.SetActive(true);
        controlPanel.SetActive(false);
    }
    public void WinGame()
    { 
        winGamePanel.SetActive(true);
        controlPanel.SetActive(false);

        // Displays to the player how long it took for them to finish the game
        totalTime.text = timerText.text;    
    }
    public void OnClickPauseGame()
    {
        if (!isPaused)
        {
            GameManager.Instance.PauseGame();
            isPaused = true;
            pausePanel.SetActive(true);
            pauseText.text = "Play";       // Switches the text to actually make sense
        }
        else
        {
            GameManager.Instance.ResumeGame();
            isPaused = false;
            pausePanel.SetActive(false);
            pauseText.text = "Pause";
        }
    }
    public void OnClickToggleHelp()
    {
        if (!isHelpOpen)
        {
            if (!mainMenuPanel.activeSelf && !isPaused)
            {
                GameManager.Instance.PauseGame();
                isPaused = true;
                pausePanel.SetActive(true);
                pausedByHelp = true;  
            }
            helpMenu.SetActive(true);
            isHelpOpen = true;
        }
        else
        {
            helpMenu.SetActive(false);
            isHelpOpen = false;
            if (pausedByHelp)
            {
                GameManager.Instance.ResumeGame();
                isPaused = false;
                pausePanel.SetActive(false);
                pausedByHelp = false;
            }
        }
    }
    public void OnClickStartGame()
    {
        GameManager.Instance.StartNewGame();
        mainMenuPanel.SetActive(false);
    }
    public void OnClickTryAgain()
    {
        GameManager.Instance.ResetLevel();
    }
    public void OnClickOnward()
    {         
        GameManager.Instance.AdvanceLevel();
    }
    public void OnClickQuit()
    {
        confirmQuitPanel.SetActive(true);
    }
    public void OnClickConfirmQuit()
    {
        Application.Quit();
    }
    public void OnClickCancelQuit()
    {
        confirmQuitPanel.SetActive(false);
    }
    public void OnClickShooterUpgrade()
    {
        UpgradeManager.Instance.UpgradeShooter();
    }
    public void OnClickAoeUpgrade()
    {
        UpgradeManager.Instance.UpgradeAoe();
    }
    public void OnClickMineUpgrade()
    {
        UpgradeManager.Instance.UpgradeMine();
    }
    //public void OnClickMainMenu()
    //{
    //    GameManager.Instance.ResetEntireGame();
    //}
}
