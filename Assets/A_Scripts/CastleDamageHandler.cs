using UnityEngine;
using System.Collections;

public class CastleDamageHandler : MonoBehaviour
{
    /// <summary>
    /// This is a helper script so that the CastleStats can cleanly call Coroutines since it is a non-MonoBehaviour class.
    /// </summary>
    public static CastleDamageHandler Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void BeginIframe()
    {
        StartCoroutine(Iframe());
    }
    private IEnumerator Iframe()
    {
        Debug.Log("Castle iframe called");
        CastleStats.Instance.SetInvincible(true);
        yield return new WaitForSeconds(0.2f);
        CastleStats.Instance.SetInvincible(false);
    }

    // Called when wave is cleared or castle is destroyed.
    public void ClearAllDamageCoroutines()
    {
        StopAllCoroutines();
        CastleStats.Instance.SetInvincible(false);
    }
}