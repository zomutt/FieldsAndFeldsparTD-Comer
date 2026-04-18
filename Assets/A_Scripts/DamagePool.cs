using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// This handles all of the spawning of all damage types when called by the tower scripts.
/// This pool has intentionally been designed to be able to dynamically grow later down the line as more towers are added.
/// </summary>
public class DamagePool : MonoBehaviour
{
    public static DamagePool SharedInstance;

    [Header("Shooters")]
    [SerializeField] private GameObject shooterPrefab;
    [SerializeField] private int amountToPoolShooter;

    [Header("AOE")]
    [SerializeField] private int amountToPoolAOE;
    [SerializeField] private GameObject aoePrefab;

    // These are separate because I am being cautious of how players may run different builds
    private List<GameObject> shooterPool;
    private List<GameObject> aoePool;

    // I opted to use an enum despite there only being two towers at the moment for scope scaling purposes
    public enum DamageType
    {
        Shooter = 0,
        AOE = 1
    }
    // Links the damage types to their pool limit
    private Dictionary<DamageType, List<GameObject>> projectilePools;

    private void Awake()
    {
        SharedInstance = this;
    }
    private void Start()
    {
        // All bullets are stored in a universal spawner that each tower grabs from
        shooterPool = new List<GameObject>();
        GameObject newShooter;
        for (int i= 0; i < amountToPoolShooter; i++)
        {
            newShooter = Instantiate(shooterPrefab);
            newShooter.SetActive(false);
            shooterPool.Add(newShooter);
        }

        aoePool = new List<GameObject>();
        GameObject newAOE;
        for (int i = 0; i < amountToPoolAOE; i++)
        {
            newAOE = Instantiate(aoePrefab);
            newAOE.SetActive(false);
            aoePool.Add(newAOE);
        }

        // Links the appropriate damage type with it's corresponding pool
        projectilePools = new Dictionary<DamageType, List<GameObject>>
        {
            { DamageType.Shooter, shooterPool },
            { DamageType.AOE, aoePool },
        };
    }
    public GameObject GetProjectile(DamageType type)
    {
        // Looks up the correct pool
        List<GameObject> pool = projectilePools[type];
        
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }
        return null;
    }
}
