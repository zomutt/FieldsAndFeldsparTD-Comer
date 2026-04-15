using UnityEngine;

/// <summary>
/// Tower Spawners are tiled in such a way so as to keep the player from double building, messing up their geometry, miscounting tiles they can build, etc.
/// The different types of towers live as deactivated prefabs on the square itself so they can be accessed via index.
/// </summary>
public class TowerSpawners : MonoBehaviour
{
    [SerializeField] private GameObject[] tower;
    private bool isOccupied;

    private void Start()
    {
        isOccupied = false;
    }

    public void SpawnTower(int index)
    {
        if (isOccupied) return;
        tower[index].SetActive(true);
        isOccupied = true;     // Prevents double building
    }
}
