using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SnowmanController : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 20f;
    public Image healthBarImage;

    private float currentHealth;

    [Header("Movement")]
    public float speed = 5f;
    public float stopTime = 1f;
    public Transform point1, point2, point3;

    [Header("Shooting")]
    public GameObject snowballPrefab;
    public Transform firePoint; 
    public float fireRate = 0.5f;
    public Transform snowballContainer;

    [Header("Audio")]
    [SerializeField] private AudioClip _hitSfx;

    private Transform _currentTarget;
    private bool _isMoving = true;
    private bool _canShoot = true;
    private Transform player;

    private Rigidbody2D _rigidbody;
    private AudioSource _audioSource;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentHealth = maxHealth;
        _currentTarget = point1;

        UpdateHealthBar();
        StartCoroutine(MoveBetweenPoints());
        StartCoroutine(ShootAtPlayer());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SnowBall"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }

    private void TakeDamage(float damage)
    {
        PlayeOneShot(_hitSfx);

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = currentHealth / maxHealth;
        }
    }

    private void PlayeOneShot(AudioClip audio)
    {
        if (audio != null && _audioSource != null)
            _audioSource.PlayOneShot(audio);
    }

    private IEnumerator MoveBetweenPoints()
    {
        while (true)
        {
            yield return StartCoroutine(MoveToPoint(point2));

            float waitTime = Random.Range(0f, stopTime);
            yield return new WaitForSeconds(waitTime);

            _currentTarget = (Random.value < 0.5f) ? point1 : point3;
            yield return StartCoroutine(MoveToPoint(_currentTarget));
        }
    }

    private IEnumerator MoveToPoint(Transform target)
    {
        _isMoving = true;

        while (Mathf.Abs(transform.position.x - target.position.x) > 0.1f)
        {
            Vector2 newPosition = Vector2.MoveTowards(_rigidbody.position,
                new Vector2(target.position.x, _rigidbody.position.y), speed * Time.deltaTime);

            _rigidbody.MovePosition(newPosition);
            yield return null;
        }

        _isMoving = false;
    }

    private IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);

            if (_isMoving && _canShoot && player != null)
            {
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        Vector3 direction = (player.position - firePoint.position).normalized;
        GameObject snowball = Instantiate(snowballPrefab, firePoint.position, Quaternion.identity, snowballContainer);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        snowball.transform.rotation = Quaternion.Euler(0, 0, angle);

        Snowball snowballScript = snowball.GetComponent<Snowball>();

        if (snowballScript != null)
        {
            snowballScript.SetDirection(direction);
        }
    }

    private void Die()
    {
        GameManager.instance.CheckGameOver(false);
        Destroy(gameObject);
    }
}
