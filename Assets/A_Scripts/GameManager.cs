using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This handles all game state functions, yet allocates behaviour to other scripts as needed.
/// Things such as removing Singletons to restart the game, advancing levels, and keeping track of what level the player is on are all handled here.
/// This also handles calling the save and load functions for stats at the appropriate times, such as when the player wins or loses a level, or starts a new game.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int currentLevel;

    // This is used to increase the cost of towers and gold farm each level, making the game more difficult as the player progresses. It is set in the inspector for easy tuning.
    [SerializeField] private int costIncreasePerLevel = 50;
    [SerializeField] private GameObject UIManagerObj;
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
        Time.timeScale = 0f;        
        UIManagerObj.SetActive(true);    // Set active because I often turn it off while dev'ing
    }
    public void StartGame()
    {
        // This method is only called for level one -- not on level restarts, not on new levels
        InitializeAllStats();   // Initializes all stats to their starting values at the beginning of the game
        Time.timeScale = 1f;        // Unfreezes time in case the player is starting a new game after losing or winning

        GoldManager.Instance.StartGame();   // Gives player their starting gold and income
        UIController.Instance.UpdateUI();

        TierManager.Instance.StartLevel();  // Gets the first wave of enemies going after a short delay
        currentLevel = 1;
        Debug.Log("Game starting from GM.");
    }
    public void ResetEntireGame()
    {
        Time.timeScale = 1f;

        // Destroy persistent singletons so they reinitialize
        Destroy(UIController.Instance.gameObject);
        Destroy(GoldManager.Instance.gameObject);
        Destroy(TierManager.Instance.gameObject);

        // Load the first level scene
        SceneManager.LoadScene("LevelOne");
        currentLevel = 1;
        Time.timeScale = 0f;    // Paused until the player hits the start button in the main menu, this keeps the timer intact
    }

    public void CastleDestroyed()
    {
        UIController.Instance.LoseGame();
    }
    private void Update()
    {
        // Cheat for testing purposes
        if (Input.GetKeyDown(KeyCode.F1))
        {
            AdvanceLevel();
        }
    }
    public void AdvanceLevel()       
    {
        // Called by UIController when player hits next level button.
        // Game state win determined by TierManager, which signals the UIController when the player has won the level and can advance.
        if (currentLevel < 4)
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            SaveAllStats();
            SceneManager.LoadScene(currentIndex + 1);
            currentLevel++;
            TowerStats.Instance.IncreaseAllCosts(costIncreasePerLevel);
            LoadAllStats();
            UIController.Instance.UpdateUI();
            StartGame();
            Time.timeScale = 1f;        // Unfreezes time after the player hits next level button
        }
        
        else
        {
            Time.timeScale = 0f;

            // Displays the UI win screen
            UIController.Instance.WinGame();
        }
    }
    public void WinLevel()        
    {
        // Called by TierManager to signal when the player has eliminated all enemies and won the level

        // Freezes time so that at the end of the game, the player can accurately see how long it takes for them to finish the game *minus* time spent in menus
        Time.timeScale = 0f;
        SaveAllStats();    // Saves any upgrades the player may have obtained
        UIController.Instance.WinLevel();
    }
    public void ResetLevel()
    {
        // Called by UI controller when player loses the level but decides to try again
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        LoadAllStats();

        TierManager.Instance.StartLevel();
        Time.timeScale = 1f;       
    }
    private void LoadAllStats()
    {
        // This method is called when the player loses a level and decides to try again. It resets any upgrades they may have obtained during the level they lost, but keeps the ones from previous levels if applicable.
        TowerStats.Instance.LoadStats();
        GoldManager.Instance.LoadStats();
        CastleStats.Instance.LoadStats();
        UIController.Instance.UpdateUI();
    }
    private void SaveAllStats()
    {
        // Stats are saved at the end of each level so that the player may carry their upgrades with them through the game
        TowerStats.Instance.SaveStats();
        GoldManager.Instance.SaveStats();
        CastleStats.Instance.SaveStats();
    }
    private void InitializeAllStats()
    {
        // This method exists so the player can cleanly start a new game from scratch after winning, losing, or quitting a game
        CastleStats.Instance.SetStatsInitial();
        GoldManager.Instance.InitializeStats();
        TowerStats.Instance.InitializeStats();
        UIController.Instance.UpdateUI();
    }
}