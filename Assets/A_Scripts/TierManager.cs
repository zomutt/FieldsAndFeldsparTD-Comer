    using System.Collections;
    using UnityEngine;
public class TierManager : MonoBehaviour
{
    /// <summary>
    /// This initiates new waves and keeps track of how many mobs have been killed.
    /// Killed mobs are compared against expected mobs to track when a wave should end.
    /// After the wave ends, the player has a grace period to prepare for the next wave.
    /// </summary>
    public static TierManager Instance { get; private set; }

    // Starts on tier 1
    private int currentTier;
    public int CurrentTier => currentTier;

    // Grabbed from WaveSpawnPool.cs
    private int expectedSpawns;
    private int currentSpawns;
    // Increased each time a mob is killed from mob script
    private int mobsKilled;
    private Transform castleTransform;

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
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
        
    public void StartLevel()
    {
        spawners = null;     // Makes sure we're getting the proper references on each scene
        FindSpawners();
        castleTransform = GameObject.FindGameObjectWithTag("Castle").transform;
        isFirstRound = true;
        mobsKilled = 0;
        currentTier = 1;
        StartCoroutine(WaveSpawnDelay());
    }
    private void FindSpawners()
    {
        GameObject[] spawnerObjects = GameObject.FindGameObjectsWithTag("Spawner");
        spawners = new EnemySpawner[spawnerObjects.Length];
        for (int i = 0; i < spawnerObjects.Length; i++)
        {
            spawners[i] = spawnerObjects[i].GetComponent<EnemySpawner>();
        }
    }
    private IEnumerator WaveSpawnDelay()
    {
        // Displays to the player how long it will be until the next round starts, and then starts the next round after the delay
        if (isFirstRound)
        {
            // The player should have a longer chance to prepare for the first wave since they won't already have towers. After the first wave, they don't get as long.
            UIController.Instance.StartCoroutine(UIController.Instance.WaveCountdown(gracePeriod, currentTier));
            isFirstRound = false;  // Resets the wave spawn delay back to normal for the next round
            yield return new WaitForSeconds(gracePeriod);
        }
        else
        {
            UIController.Instance.StartCoroutine(UIController.Instance.WaveCountdown(waveSpawnDelay, currentTier));
            yield return new WaitForSeconds(waveSpawnDelay);
        }
        currentSpawns = 0;  
        mobsKilled = 0;     
        StartCoroutine(SpawnEnemies());
    }
    private IEnumerator SpawnEnemies()
    {
        if (spawners == null || spawners.Length == 0)
        {
            Debug.LogError("No spawners found, aborting spawn.");
            yield break;  // All my homies hate infinite loop crashes
        }
        expectedSpawns = WaveSpawnPool.Instance.GetAmountToSpawn(currentTier);
        // Get all three spawners from the scene
        // Tracks how many spawns have occured vs. what the spawn pool actually is
        while (currentSpawns < expectedSpawns)
        {
            foreach (EnemySpawner spawner in spawners)
            {
                if (spawner == null)
                {
                    continue;
                }
                if (currentSpawns >= expectedSpawns)
                {
                    break;
                }
                spawner.SpawnEnemy(currentTier, castleTransform);
                currentSpawns++;
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }
        yield return null;
    }
    public void RecordKill()
    {
        // Called by EnemyBase.cs anytime a mob is destroyed
        // This tracks how many kills there are and compares them against the number in the pool. This works because I only pool the exact number that I need.
        mobsKilled++;
        if (mobsKilled < expectedSpawns)
        {
            return;  // Wave not over yet, so don't do anything else.
        }
        Debug.Log($"Wave {currentTier} cleared!");

        GoldManager.Instance.GiveGold(currentTier * 100);  // Reward scales with tier

        if (currentTier < 4)
        {
            currentTier++;
            StartCoroutine(WaveSpawnDelay());
        }
        else
        {
            GameManager.Instance.WinLevel();
        }
    }
}
