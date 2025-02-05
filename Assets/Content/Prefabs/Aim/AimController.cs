using UnityEngine;

public class AimController : MonoBehaviour
{
    public Transform player;
    public float offsetRotation = 0f;

    void Update()
    {
        if (player == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector3 direction = mousePosition - player.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle + offsetRotation);
        transform.position = mousePosition;
    }
}
