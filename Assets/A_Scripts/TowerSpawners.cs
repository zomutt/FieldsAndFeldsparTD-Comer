using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tower Spawners are tiled in such a way so as to keep the player from double building, messing up their geometry, miscounting tiles they can build, etc.
/// The different types of towers live as deactivated prefabs on the square itself so they can be accessed via index.
/// </summary>
public class TowerSpawners : MonoBehaviour
{
    public enum TowerType
    {
        None = 0,
        Shooter = 1,
        AOE = 2,
        Gold = 3
    }
    [Header("Towers -- Double check index until I refactor this")]
    [SerializeField] private GameObject[] towerPrefabs;
    private bool isOccupied;

    // The player should be able to clearly see which tile they are considered to be on to prevent mistakes
    private Renderer towerSR;
    private Color startColor;

    private void Start()
    {
        isOccupied = false;
        towerSR = GetComponent<Renderer>();
        startColor = towerSR.material.color;
    }

    // This is called from PlayerController when the player is on top of a square
    public void SpawnTower(int type)
    {
        if (isOccupied) return;
        TowerType selectedTower = (TowerType)type;
        int index = (int)selectedTower;
        Instantiate(towerPrefabs[index], transform.position + new Vector3 (0f, 2f, 0f), transform.rotation);
        isOccupied = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Allows player to see what square they are on
        if (other.gameObject.CompareTag("Player"))
        {
            towerSR.material.color = Color.green;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            towerSR.material.color = startColor;
        }
    }
}
