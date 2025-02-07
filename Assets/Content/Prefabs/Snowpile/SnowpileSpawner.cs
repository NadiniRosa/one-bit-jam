using System.Collections;
using UnityEngine;

public class SnowpileSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject snowPilePrefab;
    public float respawnTime = 3f;

    private void Start()
    {
        StartCoroutine(CheckAndRespawn());
    }

    private IEnumerator CheckAndRespawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnTime);

            if (!snowPilePrefab.activeInHierarchy)
            {
                Respawn();
            }
        }
    }

    private void Respawn()
    {
        if (spawnPoints.Length == 0)
        {
            return;
        }

        Transform newSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        snowPilePrefab.transform.position = newSpawnPoint.position;
        snowPilePrefab.SetActive(true);
    }
}
