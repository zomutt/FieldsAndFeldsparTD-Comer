using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This is a simple script that controls the AI movement of the enemies. 
/// Enemies spawn -> Enemies follow nav mesh -> Enemies hit castle (handled separately) -> Castle loses hp -> GG.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    // The castle, aka the enemies objective
    private Transform castleTransform;
    NavMeshAgent agent;
    [SerializeField] private float speed;
    internal float Speed => speed;          // Needed so that enemy can never out run projectiles


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        GameObject castleGO = GameObject.FindGameObjectWithTag("Castle");
        castleTransform = castleGO.transform;
        agent.SetDestination(castleTransform.position);
        agent.speed = speed;

        // This makes it less likely for enemies to stack on top of each other
        agent.avoidancePriority = Random.Range(30, 70);
    }
}
