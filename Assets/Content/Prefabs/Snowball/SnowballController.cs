using UnityEngine;

public class Snowball : MonoBehaviour
{
    [Header("Snowball Settings")]
    public float speed = 10f;
    public float maxDistance = 30f;

    private Vector3 startPosition;
    private Vector3 direction;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }
}
