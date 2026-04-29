using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int currentLevel;
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
        // Only here for testing purposes
        StartGame();
    }
    private void StartGame()
    {
        // This method is only called for level one -- not on level restarts, not on new levels
        LoadAllStats();
        GoldManager.Instance.StartGame();
        UIController.Instance.UpdateUI();
        TierManager.Instance.StartLevel();
        currentLevel = 1;
        Debug.Log("Game starting from GM.");
    }
    public void CastleDestroyed()
    {
        UIController.Instance.LoseGame();
    }
    private void AdvanceLevel()
    {
        if (currentLevel < 4)
        {
            currentLevel++;
        }
        
        else
        {
            WinGame();
        }
    }
    private void WinGame()
    {

    }
    private void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        LoadAllStats();
        TierManager.Instance.StartLevel();
    }
    private void LoadAllStats()
    {
        GoldManager.Instance.LoadStats();
        CastleStats.Instance.LoadStats();
        UIController.Instance.UpdateUI();
    }
}

