using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SnowmanController : MonoBehaviour
{
    [Header("Health")]
    public float maxHealthStageOne = 20f;
    public float maxHealthStageTwo = 30f;

    public Image healthBarImage;
    public Image healthBarBorder;

    private float currentHealth;
    private bool isStageTwo = false;

    [Header("Movement")]
    public float speed = 5f;
    public float stopTime = 1f;
    public Transform point1, point2, point3;

    [Header("Shooting")]
    public GameObject snowballPrefab;
    public Transform firePoint;
    public float fireRate = 0.8f;
    public Transform snowballContainer;

    [Header("Audio")]
    [SerializeField] private AudioClip _hitSfx;
    [SerializeField] private AudioSource _musicSource;

    [Header("Stages Settings")]
    public GameObject stageOneObject;
    public GameObject stageTwoObject;

    public Sprite healthBarBorderStageTwo;
    public AudioClip _stageTwoMusic;

    public float transitionTime = 2f;

    private bool _isMoving = true;
    private bool _canShoot = true;

    private Transform _currentTarget;
    private Transform player;

    private Rigidbody2D _rigidbody;
    private AudioSource _audioSource;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentHealth = maxHealthStageOne;
        _currentTarget = point1;
        stageTwoObject.SetActive(false);

        UpdateHealthBar();

        StartCoroutine(StartDelayedShooting());
        StartCoroutine(MoveBetweenPoints());
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
        PlayOneShot(_hitSfx);

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, isStageTwo ? maxHealthStageTwo : maxHealthStageOne);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            if (!isStageTwo)
            {
                StartCoroutine(EnterStageTwo());
            }
            else
            {
                StartCoroutine(EndGame());
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = currentHealth / (isStageTwo ? maxHealthStageTwo : maxHealthStageOne);
        }
    }

    private void PlayOneShot(AudioClip audio)
    {
        if (audio != null && _audioSource != null)
            _audioSource.PlayOneShot(audio);
    }

    private IEnumerator StartDelayedShooting()
    {
        yield return new WaitForSeconds(2f);

        StartCoroutine(ShootAtPlayer());
    }

    private IEnumerator MoveBetweenPoints()
    {
        bool firstMove = true;

        while (true)
        {
            yield return StartCoroutine(MoveToPoint(point2));

            if (!firstMove)
            {
                yield return StartCoroutine(SpecialAttack());
            }

            firstMove = false;

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
                Shoot(player.position);
            }
        }
    }

    private IEnumerator SpecialAttack()
    {
        _canShoot = false;

        for (int i = 0; i < 3; i++)
        {
            ShootSpread();
            yield return new WaitForSeconds(0.3f);
        }

        _canShoot = true;
    }

    private void ShootSpread()
    {
        Vector3[] directions =
        {
            new Vector3(-1, -1, 0).normalized,
            new Vector3(0, -1, 0).normalized,
            new Vector3(1, -1, 0).normalized
        };

        foreach (Vector3 direction in directions)
        {
            Shoot(direction);
        }
    }

    private void Shoot(Vector3 direction)
    {
        GameObject snowball = Instantiate(snowballPrefab, firePoint.position, Quaternion.identity, snowballContainer);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        snowball.transform.rotation = Quaternion.Euler(0, 0, angle);

        Snowball snowballScript = snowball.GetComponent<Snowball>();

        if (snowballScript != null)
        {
            snowballScript.SetDirection(direction);
        }
    }

    private IEnumerator EndGame()
    {
        enabled = false;

        yield return new WaitForSeconds(1f);

        GameManager.instance.CheckGameOver(false);
    }

    private IEnumerator EnterStageTwo()
    {
        isStageTwo = true;
        _canShoot = false;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        float elapsedTime = 0f;

        while (elapsedTime < transitionTime)
        {
            stageOneObject.SetActive(!stageOneObject.activeSelf);
            yield return new WaitForSeconds(0.2f);
            elapsedTime += 0.2f;
        }

        stageOneObject.SetActive(false);
        stageTwoObject.SetActive(true);

        healthBarBorder.sprite = healthBarBorderStageTwo;
        currentHealth = maxHealthStageTwo;
        
        _musicSource.clip = _stageTwoMusic;
        _musicSource.Play();

        UpdateHealthBar();

        _canShoot = true;

        if (collider != null)
            collider.enabled = true;

        StopAllCoroutines();
        StartCoroutine(FollowPlayer());
        StartCoroutine(ShootAtPlayerStageTwo());
    }

    private IEnumerator FollowPlayer()
    {
        while (isStageTwo)
        {
            if (player != null)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                Vector2 newPosition = Vector2.MoveTowards(_rigidbody.position,
                                                          _rigidbody.position + direction,
                                                          speed * 0.5f * Time.deltaTime);

                _rigidbody.MovePosition(newPosition);
            }

            yield return null;
        }
    }

    private IEnumerator ShootAtPlayerStageTwo()
    {
        while (isStageTwo) 
        {
            yield return new WaitForSeconds(0.5f);

            if (_canShoot && player != null)
            {
                Shoot(player.position);
            }
        }
    }
}
