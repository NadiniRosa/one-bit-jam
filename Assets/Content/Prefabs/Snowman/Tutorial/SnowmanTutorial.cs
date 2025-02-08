using UnityEngine;

public class SnowmanTutorial : MonoBehaviour
{
    public GameObject pressF;
    public GameObject cutscenes;
    public GameObject player;

    private bool isInTrigger = false;

    private void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.F))
        {
            if (cutscenes != null)
            {
                cutscenes.SetActive(true);
            }

            pressF.SetActive(false);
            player.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = true;
            pressF.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = false;
            pressF.SetActive(false);
            other.gameObject.SetActive(false);
        }
    }
}
