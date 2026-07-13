using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The UIController has been set to ONLY handle UI. It does not make any game decisions, it only displays information and has buttons that call methods in the GameManager.
/// Furthermore, with it being a Singleton, I ensured that all direct references are ONLY to UI objects. This is so that references never get lost.
/// The actual UI itself lives as a child of the Singleton UIController.cs GameObject.
/// </summary>
public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [Header("Menus")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject helpMenu;
    [SerializeField] private GameObject confirmQuitPanel;
    [SerializeField] private GameObject confirmMainMenuPanel;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI roundText;
    private int minutes;
    private int seconds;

    [Header("Level Stats")]      // Displays to the player where they are at in the game
    [SerializeField] private TextMeshProUGUI roundCountText;
    [SerializeField] private TextMeshProUGUI levelCountText;

    [Header("Castle Stats")]
    [SerializeField] private TextMeshProUGUI castleHealthText;
    [SerializeField] private TextMeshProUGUI totalGoldText;
    [SerializeField] private TextMeshProUGUI goldPerSecText;
    [SerializeField] private TextMeshProUGUI totalKillsText;

    [Header("In-Game Panels")]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject controlPanel;

    [Header("Pause")]
    [SerializeField] private GameObject pausePanel;     // Black overlay that displays when game is paused
    [SerializeField] private TextMeshProUGUI pauseText;
    private bool isPaused;

    [Header("Help")]
    [SerializeField] private GameObject helpPanel;
    private bool isHelpOpen;
    private bool pausedByHelp;         // This is to try to help the bug that is occuring where closing from help keeps the game paused when it should not.

    [Header("Win/Lose")]
    [SerializeField] private GameObject levelWinPanel;
    [SerializeField] private GameObject winGamePanel;
    [SerializeField] private GameObject loseGamePanel;
    [SerializeField] private TextMeshProUGUI totalTime;       // Displayed at the end of the game

    [Header("Towers")]

    [Header("Shooter")]
    // SHOOTER TOWER
    [SerializeField] private TextMeshProUGUI shooterDMG;
    [SerializeField] private TextMeshProUGUI shooterCost;
    [SerializeField] private TextMeshProUGUI shooterUpgradeCost;
    [SerializeField] private TextMeshProUGUI shooterUpgradeAmt;

    [Header("AOE")]
    // AOE TOWER
    [SerializeField] private TextMeshProUGUI aoeDMG;
    [SerializeField] private TextMeshProUGUI aoeCost;
    [SerializeField] private TextMeshProUGUI aoeUpgradeCost;
    [SerializeField] private TextMeshProUGUI aoeUpgradeAmt;

    [Header("Single-Target Slow")]
    // SINGLE-TARGET SLOW TOWER
    [SerializeField] private TextMeshProUGUI stSlowAmount;
    [SerializeField] private TextMeshProUGUI stSlowCost;
    [SerializeField] private TextMeshProUGUI stSlowUpgradeCost;
    [SerializeField] private TextMeshProUGUI stSlowUpgradeAmt;

    [Header("Gold Mine")]
    // GOLD MINE
    [SerializeField] private TextMeshProUGUI mineYield;
    [SerializeField] private TextMeshProUGUI mineCost;
    [SerializeField] private TextMeshProUGUI mineUpgradeCost;
    [SerializeField] private TextMeshProUGUI mineUpgradeAmt;

    // This is needed so that UI controller ONLY resets everything when the player begins a fresh game.
    private bool pendingGameReset = false;
    public bool PendingGameReset => pendingGameReset;


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
        if (pendingGameReset)
        {
            StartingUI();  // Handles everything for full reset
            pendingGameReset = false;
            return; 
        }

        // For level transitions and resets, just close win/lose panels
        if (levelWinPanel != null) levelWinPanel.SetActive(false);
        if (winGamePanel != null) winGamePanel.SetActive(false);
        if (loseGamePanel != null) loseGamePanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false); 
        controlPanel.SetActive(true);
    }
    private void Start()
    {
        StartingUI();
    }
    public void StartingUI()
    {
        // Called both in Start() and when the game is supposed to start over from scratch. This ensures that player may start again without having to close the game.
        controlPanel.SetActive(true);
        mainMenuPanel.SetActive(true);
        losePanel.SetActive(false);
        confirmQuitPanel.SetActive(false);
        winGamePanel.SetActive(false);
        levelWinPanel.SetActive(false);
        helpMenu.SetActive(false);
        confirmMainMenuPanel.SetActive(false);

        pausePanel.SetActive(false);
        pauseText.text = "Pause";
        isPaused = false;
        isHelpOpen = false;
        pausedByHelp = false;

        roundText.text = null;

        minutes = 0;
        seconds = 0;
    }
    private void Update()
    {
        // If time is frozen, don't update the timer.
        if (Time.timeScale == 0f) return;

        // Timer is used to give player feedback on how long or short they took to complete the game. This gives the player incentive to try again if they don't like their time.
        minutes = Mathf.FloorToInt(Time.time/60f);
        seconds = Mathf.FloorToInt(Time.time % 60f); // Seconds = remainder after dividing by 60

        timerText.text = $"Time Elapsed: {minutes:00}:{seconds:00}";
    }
    public void UpdateUI()
    {
        HelpUI.Instance.UpdateTowerHelpUI();      // Catch-all
        if (CastleStats.Instance != null)
        {
            castleHealthText.text = $"Castle HP: {CastleStats.Instance.CurrentHealth}/{CastleStats.Instance.MaxHealth}";
        }

        levelCountText.text = $"Level: {GameManager.Instance.CurrentLevel}/3";
        roundCountText.text = $"Round: {TierManager.Instance.CurrentTier}/4";

        totalGoldText.text = $"Gold: {GoldManager.Instance.CurrentGold}";
        goldPerSecText.text = $"Gold/Sec: {GoldManager.Instance.GoldPerSec()}";

        // SHOOTER
        shooterDMG.text = $"Damage: {TowerStats.Instance.ShooterDamage}";
        shooterCost.text = $"Build Cost: {TowerStats.Instance.ShooterCost}";
        shooterUpgradeCost.text = $"Upgrade Cost: {UpgradeManager.Instance.ShooterUpgradeCost}";
        shooterUpgradeAmt.text = $"Dmg Upgrade: +{UpgradeManager.Instance.ShooterUpgradeLevel}";

        // AOE
        aoeDMG.text = $"Damage/Sec: {TowerStats.Instance.AoeDamage}";
        aoeCost.text = $"Build Cost: {TowerStats.Instance.AoeCost}";
        aoeUpgradeCost.text = $"Upgrade Cost: {UpgradeManager.Instance.AoeUpgradeCost}";
        aoeUpgradeAmt.text = $"Dmg Upgrade: +{UpgradeManager.Instance.AoeUpgradeLevel}";

        // MINE
        mineYield.text = $"Gold Yield: {GoldManager.Instance.GoldFarmYield}";
        mineCost.text = $"Build Cost: {GoldManager.Instance.GoldFarmCost}";
        mineUpgradeCost.text = $"Upgrade Cost: {UpgradeManager.Instance.MineUpgradeCost}";
        mineUpgradeAmt.text = $"Yield Upgrade: +{UpgradeManager.Instance.MineUpgradeLevel}";

        // SINGLE TARGET SLOW
        stSlowAmount.text = $"Slow Power: {TowerStats.Instance.StSlowAmount}";
        stSlowCost.text = $"Build Cost: {TowerStats.Instance.StSlowCost}";

        if (UpgradeManager.Instance.StSlowUpgradeCap > TowerStats.Instance.StSlowAmount)
        {
            stSlowUpgradeCost.text = $"Upgrade Cost: {UpgradeManager.Instance.StSlowUpgradeCost}";
            stSlowUpgradeAmt.text = $"Slow Upgrade: {UpgradeManager.Instance.StSlowUpgradeLevel}";
        }
        else if (UpgradeManager.Instance.StSlowUpgradeCap <= TowerStats.Instance.StSlowAmount)
        {
            stSlowUpgradeCost.text = "Upgrade maxed!";
            stSlowUpgradeAmt.text = "";
        }
        totalKillsText.text = $"Total Kills: {GameManager.Instance.TotalKills}";
    }
    public void ResetTimer()
    {
        // Called by GameManager.cs when the game is completely reset.
        minutes = 0;
        seconds = 0;
    }
    public IEnumerator WaveCountdown(float time, int currentTier)
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
    public void LoseGame()
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
    public void TriggerPendingReset()
    {
        pendingGameReset = true;
    }




    // PAUSE/HELP: 
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


    // QUITTING:
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

    // UPGRADES:
    public void OnClickShooterUpgrade()
    {
        UpgradeManager.Instance.UpgradeShooter();
    }
    public void OnClickAoeUpgrade()
    {
        UpgradeManager.Instance.UpgradeAoe();
    }

    public void OnClickStSlowUpgrade()
    {
        UpgradeManager.Instance.UpgradeStSlow();
    }
    public void OnClickMineUpgrade()
    {
        UpgradeManager.Instance.UpgradeMine();
    }

    // MENU:
    public void OnClickMainMenu()
    {
        confirmMainMenuPanel.SetActive(true);
    }
    public void OnClickDeclineMainMenu()
    {
        confirmMainMenuPanel.SetActive(false);
    }

    // LEVELS/RESTARTING/ETC: 
    public void OnClickRestartGame()
    {
        Debug.Log("UIC: Reset Game called");
        GameManager.Instance.ResetWholeGame();
    }

    public void OnClickStartGame()
    {
        Debug.Log("Starting game");
        GameManager.Instance.StartNewGame();
        mainMenuPanel.SetActive(false);
    }
    public void OnClickTryAgain()
    {
        loseGamePanel.SetActive(false);
        GameManager.Instance.ResetLevel();
    }
    public void OnClickOnward()
    {
        GameManager.Instance.AdvanceLevel();
    }
}
