using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This range detection works by finding and keeping track of what enemies enter the radius. 
/// The GO itself is a child of the tower and is a flattened cylinder with a trigger collider.
/// </summary>

public class EnemyDetection : MonoBehaviour
{
    private TowerBase tower;
    private void Start()
    {
        // Automatically gets reference to shooter tower if not assigned
        tower = GetComponentInParent<TowerBase>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Adds enemy to in range target list
            // Choosing EnemyBase specifically instead of other scripts gaurantees this method will work on all enemies
            tower.AddTargetToInRangeList(other.GetComponent<EnemyBase>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            tower.RemoveTargetFromInRangeList(other.GetComponent<EnemyBase>());
        }
    }
}
