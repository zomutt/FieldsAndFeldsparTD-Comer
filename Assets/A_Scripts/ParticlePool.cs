
/// THIS WILL COME BACK FOR PORTFOLIO. SAVING IT HERE FOR NOW. ///





//using System.Collections;
//using UnityEngine;
//using UnityEngine.Pool;

//public class ParticlePool : MonoBehaviour
//{
//    public static ParticlePool Instance;
//    [Header("Death Visuals")]
//    [SerializeField] private ParticleSystem enemyDeathParticles;
//    private ObjectPool<ParticleSystem> deathPool;


//    private void Awake()
//    {
//        Instance = this;

//        deathPool = new ObjectPool<ParticleSystem>(
//            createFunc: () => Instantiate(enemyDeathParticles),
//            actionOnGet: (ps) => ps.gameObject.SetActive(true),
//            actionOnRelease: (ps) => ps.gameObject.SetActive(false),
//            defaultCapacity: 20, maxSize: 90
//            );
//    }
//    public void SpawnDeathEffect(Vector3 position)
//    {
//        // Gets the location where the enemy died and places the particle fx there
//        var ps = deathPool.Get();
//        ps.transform.position = position;
//        ps.Play();
//        StartCoroutine(ReturnToDeathPool(ps));
//    }
//    private IEnumerator ReturnToDeathPool(ParticleSystem ps)
//    {
//        // Gets the maximum lifespan of the particle effect
//        float lifetime = ps.main.startLifetime.constantMax;
//        yield return new WaitForSeconds(lifetime);

//        deathPool.Release(ps);
//    }
//}
