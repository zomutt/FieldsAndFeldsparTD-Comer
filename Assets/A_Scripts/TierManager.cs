using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TierManager : MonoBehaviour
{
    /// <summary>
    /// This initiates new waves and keeps track of how many mobs have been killed.
    /// Killed mobs are compared against expected mobs to track when a wave should end.
    /// After the wave ends, the player has a grace period to prepare for the next wave.
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
    [SerializeField] private float timeBetweenSpawns = .2f;
    [SerializeField] private EnemySpawner[] spawners;
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
        mobsKilled = 0;
        currentTier = 1;
        StartCoroutine(WaveSpawnDelay());
    }
    public void RecordKill()
    {
        // Called by enemy base script anytime a mob is destroyed
        mobsKilled++;
        if (mobsKilled >= expectedSpawns)
        {
            Debug.Log($"Wave {currentTier} cleared!");
            currentTier++;
            if (currentTier < 4)
            {
                StartCoroutine(WaveSpawnDelay());
            }
            else Debug.Log("TM: All waves complete. Boss logic next.");
        }
    }
    private IEnumerator WaveSpawnDelay()
    {
        // Displays to the player how long it will be until the next round starts, and then starts the next round after the delay
        UIController.Instance.StartCoroutine(UIController.Instance.WaveCountdown(waveSpawnDelay));
        yield return new WaitForSeconds(waveSpawnDelay);
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
}
