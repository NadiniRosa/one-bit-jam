using UnityEngine;

public class SnowpileController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerBasicMovement player = other.GetComponent<PlayerBasicMovement>();

            if (player != null)
            {
                player.AddAmmo(player.maxAmmo); 
                gameObject.SetActive(false);
            }
        }
    }
}
