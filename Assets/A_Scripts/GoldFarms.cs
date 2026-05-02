using UnityEngine;

/// <summary>
/// Very tiny helper script that helps the GoldManager.cs script keep track of how many gold farms there are.
/// This prevents the script from having to do a FindObjectsOfType every time it needs to calculate gold generation, which is a very expensive operation.
/// Also prevents having like 50 gold farms all running update at once. A lot of other process are expensive so I want to save where I can.
/// Now, the GoldManager.cs script can just keep track of farm amount number as gold farms are added or removed.
/// </summary>
public class GoldFarms : MonoBehaviour
{
    private void OnEnable()
    {
        // Tells the GoldGeneration script that a new gold farm has been added, so that it can update the gold generation accordingly.
        GoldManager.Instance.IncreaseFarmCount();
    }
    //private void OnDisable()
    //{
    //    GoldManager.Instance.DecreaseFarmCount();
    //}
}
