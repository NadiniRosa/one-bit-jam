using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerBasicMovement : MonoBehaviour
{
    [Header("Player Settings")]
    public float speed = 5f;
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public Animator animator;

    [Header("Snowball Fire")]
    public GameObject snowballPrefab;
    public Transform firePoint;
    public float fireCooldown = 0.2f;
    public Transform snowballContainer;
    private bool canShoot = true;

    [Header("Audio")]
    [SerializeField] private AudioClip shootSfx;
    [SerializeField] private AudioClip dashSfx;

    [Header("Ammo")]
    public int maxAmmo = 10;
    [SerializeField] private TMP_Text ammoText;
    private int currentAmmo;

    private Vector3 lastMovementDirection = Vector3.zero;
    private bool isDashing = false;
    private bool canDash = true;

    private Rigidbody2D rb;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        currentAmmo = maxAmmo;
        UpdateAmmoText();
    }

    private void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    private void HandleMovement()
    {
        if (isDashing) return;
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f).normalized;

        rb.MovePosition(rb.position + (Vector2)movement * speed * Time.deltaTime);

        if (movement != Vector3.zero)
            lastMovementDirection = movement;

        if (Input.GetKeyDown(KeyCode.Space) && canDash && lastMovementDirection != Vector3.zero)
            StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            rb.MovePosition(rb.position + (Vector2)lastMovementDirection * dashSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        PlayOneShot(dashSfx);
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) && canShoot && currentAmmo > 0)
            StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        canShoot = false;
        currentAmmo--;
        UpdateAmmoText();

        animator.SetTrigger("Shoot");
        PlayOneShot(shootSfx);

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector3 shootDirection = (mousePosition - firePoint.position).normalized;

        GameObject snowball = Instantiate(snowballPrefab, firePoint.position, Quaternion.identity, snowballContainer);

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        snowball.transform.rotation = Quaternion.Euler(0, 0, angle);

        snowball.GetComponent<Snowball>()?.SetDirection(shootDirection);

        yield return new WaitForSeconds(fireCooldown);
        canShoot = true;
    }

    private void PlayOneShot(AudioClip audio)
    {
        if (audio != null && audioSource != null)
            audioSource.PlayOneShot(audio);
    }

    private void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            ammoText.text = $"X {currentAmmo}/{maxAmmo}";
        }
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        UpdateAmmoText();
    }
}
