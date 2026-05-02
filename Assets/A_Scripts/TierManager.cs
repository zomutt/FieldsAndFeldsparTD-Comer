using System.Collections;
using UnityEngine;

public class TierManager : MonoBehaviour
{
    /// <summary>
    /// This initiates new waves and keeps track of how many mobs have been killed.
    /// Killed mobs are compared against expected mobs to track when a wave should end.
    /// After the wave ends, the player has a grace period to prepare for the next wave.
    /// 
    /// This also handles initiating the spawns, but this will be refactored. 
    /// I could not figure out another way that wasn't bugged, but I will reconsider my approach when I have more time to think on it. 
    /// </summary>
    public static TierManager Instance;

    // Starts on tier 1
    private int currentTier;
    public int CurrentTier => currentTier;

    // Grabbed from WaveSpawnPool.cs
    private int expectedSpawns;
    //public int ExpectedSpawns => expectedSpawns;
    private int currentSpawns;
    // Increased each time a mob is killed from mob script
    private int mobsKilled;

    [SerializeField] private float waveSpawnDelay;
    // The player should have a longer chance to prepare for the first wave since they won't already have towers. After the first wave, they don't get as long.
    [SerializeField] private float gracePeriod;   

    [SerializeField] private float timeBetweenSpawns = .2f;
    [SerializeField] private EnemySpawner[] spawners;

    private bool isFirstRound;   // Used so that the player has a longer delay if this is the beginning of the level so they have a chance to prepare

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void StartLevel()
    {
        isFirstRound = true;
        mobsKilled = 0;
        currentTier = 1;
        StartCoroutine(WaveSpawnDelay());
    }
    private IEnumerator WaveSpawnDelay()
    {
        // Displays to the player how long it will be until the next round starts, and then starts the next round after the delay
        if (isFirstRound)
        {
            // The player should have a longer chance to prepare for the first wave since they won't already have towers. After the first wave, they don't get as long.
            UIController.Instance.StartCoroutine(UIController.Instance.WaveCountdown(gracePeriod));
            isFirstRound = false;  // Resets the wave spawn delay back to normal for the next round
            yield return new WaitForSeconds(gracePeriod);
        }
        else
        {
            UIController.Instance.StartCoroutine(UIController.Instance.WaveCountdown(waveSpawnDelay));
            yield return new WaitForSeconds(waveSpawnDelay);
        }
        currentSpawns = 0;    // Resets the spawn count for the new wave
        mobsKilled = 0;     // Resets the kill count for the new wave
        StartCoroutine(SpawnEnemies());
    }
    private IEnumerator SpawnEnemies()
    {
        expectedSpawns = WaveSpawnPool.Instance.GetAmountToSpawn(currentTier);
        // Get all three spawners from the scene
        // Tracks how many spawns have occured vs. what the spawn pool actually is
        while (currentSpawns < expectedSpawns)
        {
            foreach (EnemySpawner spawner in spawners)
            {
                if (currentSpawns >= expectedSpawns)
                {
                    break;
                }
                spawner.SpawnEnemy(currentTier);
                currentSpawns++;
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }
        yield return null;
    }

    public void RecordKill()
    {
        // Called by enemy base script anytime a mob is destroyed
        mobsKilled++;
        if (mobsKilled >= expectedSpawns)
        {
            Debug.Log($"Wave {currentTier} cleared!");
            if (currentTier < 4)
            {
                currentTier++;
                StartCoroutine(WaveSpawnDelay());
                GoldManager.Instance.GiveGold(currentTier * 100);     // Rewards the player with scaling gold once they complete a round
            }
            else if (currentTier == 4)
            {
                Debug.Log("Boss defeated! You win!");
                GameManager.Instance.WinLevel();
            }
        }
    }
}
