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
    public void SetSpeed(float newSpeed)
    {
        // This is here for when I wish to add mechanics that increase movement speed
        agent.speed = newSpeed;
    }
}
