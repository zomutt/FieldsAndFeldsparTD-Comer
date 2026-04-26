using UnityEngine;

/// <summary>
/// This script lives on the Player and handles the creation of towers.
/// When adding towers, double check the TowerSpawners.cs index that corresponds to tower.
/// This will be refactored.
/// </summary>
public class TowerBuilding : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        // The player itself has a TINY collider to prevent overlap between tiles; it can only target what is directly below the center of the players GO
        if (other.gameObject.CompareTag("BuildArea"))
        {
            var towerS = other.gameObject;
            if (towerS == null) return;     // Fail-safe

            var towerSquare = other.gameObject.GetComponent<TowerSpawners>();

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // Shooter tower
                towerSquare.SpawnTower(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // AOE tower
                towerSquare.SpawnTower(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                // Gold farm tower
                towerSquare.SpawnTower(3);
            }

            // More towers can be added as long as Enum and prefabs in TowerSpawner.cs are updated too
        }
    }
}
