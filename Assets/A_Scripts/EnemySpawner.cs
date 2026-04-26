using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    [SerializeField] private Transform firstWaypoint; // assign per lane

    private void Awake()
    {
        Instance = this;
    }
    public void SpawnEnemy(int tier)
    {
        GameObject enemy = WaveSpawnPool.Instance.GetEnemy((WaveSpawnPool.TierLevel)tier);
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        // Turns off agent so that it can properly spawn in correct spawner (it was really bugged without this)
        agent.enabled = false;

        // Sets enemy to the position of the spawner now that the agent is off. Otherwise, it just favors mid and teleports which is a headache.
        enemy.transform.position = transform.position;
        agent.enabled = true;

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        movement.SetFirstDestination(firstWaypoint);
    }
}
