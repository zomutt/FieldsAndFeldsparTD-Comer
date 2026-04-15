using System.Collections.Generic;
using UnityEngine;

public class TowerBase : MonoBehaviour
{
    protected List<EnemyBase> targetsInRange = new();
    protected EnemyBase currentTarget;

    protected virtual void Update()
    {
        GetCurrentTarget();
        // Insert shoot logic here
    }
    public void AddTargetToInRangeList(EnemyBase target)
    {
        if (!targetsInRange.Contains(target))
        {
            targetsInRange.Add(target);
        }
    }
    public void RemoveTargetFromInRangeList(EnemyBase target)
    {
        targetsInRange.Remove(target);
    }
    private void GetCurrentTarget()
    {
        if (targetsInRange.Count <= 0)
        {
            currentTarget = null;
            return;
        }
    }
}
