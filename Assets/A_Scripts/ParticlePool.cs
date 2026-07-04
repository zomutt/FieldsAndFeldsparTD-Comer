using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance;

    [Header("Death Visuals")]
    [SerializeField] private ParticleSystem enemyDeathParticles;
    private ObjectPool<ParticleSystem> deathPool;

    [Header("Slow Visuals")]
    [SerializeField] private ParticleSystem enemySlowParticles;
    private ObjectPool<ParticleSystem> slowPool;


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

        deathPool = new ObjectPool<ParticleSystem>(
            createFunc: () => Instantiate(enemyDeathParticles),
            actionOnGet: (ps) => ps.gameObject.SetActive(true),
            actionOnRelease: (ps) =>
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.gameObject.SetActive(false);
            },
            defaultCapacity: 30, maxSize: 90
            );

        slowPool = new ObjectPool<ParticleSystem>(
            createFunc: () => Instantiate(enemySlowParticles),
            actionOnGet: (ps) => ps.gameObject.SetActive(true),
            actionOnRelease: (ps) =>
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.gameObject.SetActive(false);
            },
            defaultCapacity: 30, maxSize: 90
            );
    }
    public void SpawnDeathEffect(Vector3 position)
    {
        // Gets the location where the enemy died and places the particle fx there
        var ps = deathPool.Get();
        ps.transform.position = position;
        ps.transform.localPosition = Vector3.zero;
        ps.Play();
        StartCoroutine(ReturnToDeathPool(ps));
    }

    public void SpawnSlowEffect(Transform enemy, float duration)
    {
        // Called by EnemyMovement.cs when slowed
        var ps = slowPool.Get();
        ps.transform.SetParent(enemy);
        ps.transform.localPosition = Vector3.zero;
        ps.Play();
        StartCoroutine(ReturnToSlowPool(ps, duration));
    }

    // Reads the particle systems configured lifetime regardless of any changes
    // If it is read wrong, issues and inconsistencies may occur
    private float GetParticleLifetime(ParticleSystem ps)
    {
        var main = ps.main;
        switch (main.startLifetime.mode)
        {
            case ParticleSystemCurveMode.Constant:
                return main.startLifetime.constant;
            case ParticleSystemCurveMode.TwoConstants:
                return main.startLifetime.constantMax;
            case ParticleSystemCurveMode.Curve:
            case ParticleSystemCurveMode.TwoCurves:
                return main.startLifetime.curveMultiplier;
            default:
                return 1f;      // Fallback, but this should never be reached
        }
    }
    private IEnumerator ReturnToDeathPool(ParticleSystem ps)
    {
        // Gets the maximum lifespan of the particle effect
        float lifetime = GetParticleLifetime(ps);
        yield return new WaitForSeconds(lifetime);
        deathPool.Release(ps);
    }

    private IEnumerator ReturnToSlowPool(ParticleSystem ps, float lifetime)
    {
        // This should only be active for as long as the slow is active
        yield return new WaitForSeconds(lifetime);
        transform.SetParent(null);
        slowPool.Release(ps);
    }
}
