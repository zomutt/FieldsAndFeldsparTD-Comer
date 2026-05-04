using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int currentLevel;
    public int CurrentLevel => currentLevel;    // Used by EnemyBase.cs because maxHealth is calculated as (baseHealth * currentLevel)
    private int totalKills;

    // This is used to increase the cost of towers and gold farm each level, making the game more difficult as the player progresses. It is set in the inspector for easy tuning.
    [SerializeField] private int costIncreasePerLevel = 100;
    [SerializeField] private GameObject UIManagerObj;
    private bool isResetting;
    private bool isFullReset;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // Freezes time at the beginning of the game until the player hits the start button
        PauseGame();        
        UIManagerObj.SetActive(true);    // Set active because I often turn it off while dev'ing
    }
    public void StartNewGame()
    {
        // This method is only called for level one -- not on level restarts, not on new levels

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SceneManager.LoadScene(0);      // Player should already be here, but we need to make SURE they really are.
            return;
        }
        InitializeAllStats();   // Initializes all stats to their starting values at the beginning of the game
        ResumeGame();       // Unfreezes time in case the player is starting a new game after losing or winning
        totalKills = 0;

        GoldManager.Instance.StartGame();   // Gives player their starting gold and income
        UIController.Instance.UpdateUI();
        TowerStats.Instance.SetAllCosts();
        TierManager.Instance.StartLevel();  // Gets the first wave of enemies going after a short delay
        currentLevel = 1;
        Debug.Log("Game starting from GM.");
        Debug.Log("Current level: " + currentLevel);
    }
    public void StartNewLevel()
    {
        LoadAllStats();    // Loads stats so that the player can keep their upgrades from previous levels
        TowerStats.Instance.SetAllCosts();
        GoldManager.Instance.IncreaseGoldCost();
        UIController.Instance.UpdateUI();

        TierManager.Instance.StartLevel();  // Gets the first wave of enemies going after a short delay
    }
    public void TrackTotalKills()
    {
        // Total kills accumulate across the entire playthrough intentionally. The kill tracking does nothing gameplay-wise but it can be cool to see.
        // This is because honestly, it can feel nice for the player to see big number go big if they're getting demoralized from losing.
        totalKills++;
        UIController.Instance.UpdateUI();
    }
    public int TotalKills { get { return totalKills; } }
    public void CastleDestroyed()
    {
        PauseGame();
        WaveSpawnPool.Instance.StopAllCoroutines();
        UIController.Instance.LoseGame();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GoldManager.Instance.ZeroFarmCount();

        if (isResetting)
        {
            isResetting = false;
            TierManager.Instance.StartLevel();
        }
        else if (!UIController.Instance.PendingGameReset)
        {
            StartNewLevel();
        }
        ResumeGame();
    }
    public void AdvanceLevel()
    {
    // Called by UIController when player hits next level button.
    // Game state win determined by TierManager, which signals the UIController when the player has won the level and can advance.
        if (currentLevel < 3)
        {
            SaveAllStats();
            currentLevel++;
            SceneManager.sceneLoaded += OnSceneLoaded;  // subscribe first
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            PauseGame();
            UIController.Instance.WinGame();
        }
    }
    public void WinLevel()        
    {
        // Called by TierManager to signal when the player has eliminated all enemies and won the level

        // Freezes time so that at the end of the game, the player can accurately see how long it takes for them to finish the game *minus* time spent in menus
        PauseGame();
        SaveAllStats();    // Saves any upgrades the player may have obtained

        if (currentLevel < 3)
        {
            UIController.Instance.WinLevel();
        }
        else
        {
            UIController.Instance.WinGame();
        }
    }
    public void ResetLevel()
    {
        ResumeGame();
        LoadAllStats();
        GoldManager.Instance.ZeroFarmCount();
        WaveSpawnPool.Instance.ResetPools();

        isResetting = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void LoadAllStats()
    {
        // This method is called when the player loses a level and decides to try again. It resets any upgrades they may have obtained during the level they lost, but keeps the ones from previous levels if applicable.
        TowerStats.Instance.LoadStats();
        GoldManager.Instance.LoadStats();
        UpgradeManager.Instance.LoadData();
        UIController.Instance.UpdateUI();
    }
    private void SaveAllStats()
    {
        // Stats are saved at the end of each level so that the player may carry their upgrades with them through the game
        TowerStats.Instance.SaveStats();
        GoldManager.Instance.SaveStats();
        UpgradeManager.Instance.SaveData();
    }
    private void InitializeAllStats()
    {
        // This method exists so the player can cleanly start a new game from scratch after winning, losing, or quitting a game
        GoldManager.Instance.InitializeStats();
        TowerStats.Instance.InitializeStats();
        UpgradeManager.Instance.InitializeData();
        UIController.Instance.UpdateUI();
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;    
    }
    public void ResetWholeGame()
    {
        // Do not start game, only clear game. Must be called from any time in games lifecycle.
        // Only things that really need to be worried about are the non-scene specific scripts.
        // Everything else has built in mechanisms for this.
        ResumeGame();

        StopAllCoroutines();      
        InitializeAllStats();

        currentLevel = 1;
        totalKills = 0;

        WaveSpawnPool.Instance.StopAllCoroutines();
        WaveSpawnPool.Instance.ResetPools();
        TierManager.Instance.StopAllCoroutines();
        UIController.Instance.TriggerPendingReset();
        UIController.Instance.StartingUI();       // Resets the UI to pre-game 

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(0);

        PauseGame();
    }
}