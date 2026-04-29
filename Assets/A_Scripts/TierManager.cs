using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TierManager : MonoBehaviour
{
    /// <summary>
    /// This initiates new waves and keeps track of how many mobs have been killed.
    /// Killed mobs are compared against expected mobs to track when a wave should end.
    /// After the wave ends, the player has a grace period to prepare for the next wave.
    /// This also spawns the final boss of the level. Since there is only one per level, I don't see much reason to include it in the pool.
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

    [Header("Boss")]
    [SerializeField] private Transform midSpawner;   // Where the boss spawns from
    [SerializeField] private GameObject levelBoss;
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

        if (currentTier < 4)
        {
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
        else if (currentTier == 4)
        {
            Instantiate(levelBoss, midSpawner.position, midSpawner.rotation);
            expectedSpawns = 1;
        }
    }
    public void RecordKill()
    {
        // Called by enemy base script anytime a mob is destroyed
        mobsKilled++;
        if (mobsKilled >= expectedSpawns)
        {
            Debug.Log($"Wave {currentTier} cleared!");
            currentTier++;
            if (currentTier <= 4)
            {
                StartCoroutine(WaveSpawnDelay());
                GoldManager.Instance.GiveGold(currentTier * 100);     // Rewards the player with scaling gold once they complete a round
            }
            else Debug.Log("TM: All waves complete. Boss logic next.");
        }
    }
}
