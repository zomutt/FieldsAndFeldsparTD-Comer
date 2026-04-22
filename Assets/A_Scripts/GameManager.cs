using UnityEngine;

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
        currentLevel = 1;
    }
    private void StartGame()
    {
        TierManager.Instance.StartLevel();
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
}

