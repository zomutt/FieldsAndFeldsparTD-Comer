using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    void Update()
    {
        // These are backwards because of how the level was set up vs. how the camera was set up
        float x = -Input.GetAxisRaw("Vertical");
        float z = Input.GetAxisRaw("Horizontal");

        Vector3 dir = new Vector3(x, 0f, z).normalized;

        transform.Translate(moveSpeed * Time.deltaTime * dir, Space.World);
    }
}
