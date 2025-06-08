using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    [Header("Basic Attributes")]
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float contactDamage = 15f;
    private float currentHealth;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 7f;
    public float fireRate = 1f;
    public float attackRange = 8f;
    public float chaseStopDistance = 2f;
    private float nextFireTime = 0f;

    [Header("Behavior")]
    private Transform playerTransform;

    [Header("Movement Boundaries")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    [Header("Scene Transition")]
    public string sceneToReturnTo = "_SceneLong";
    public float delayBeforeReturn = 2f;

    [Header("Game Progression")]
    [Tooltip("The game state to set when this boss is defeated.")]
    public GameProgressionState stateToSetOnDefeat;

    private bool isDead = false;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("PlayerCharacter_Boss");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("[BossController] Player object not found! Ensure player has 'PlayerCharacter_Boss' tag.");
            enabled = false;
        }

        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    void Update()
    {
        if (isDead || playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        if (distanceToPlayer <= attackRange)
        {
            AttemptToShoot();
            if (distanceToPlayer > chaseStopDistance)
            {
                MoveTowards(directionToPlayer, moveSpeed * 0.5f);
            }
        }
        else
        {
            MoveTowards(directionToPlayer, moveSpeed);
        }
    }

    void MoveTowards(Vector2 direction, float speed)
    {
        Vector3 newPosition = transform.position + (Vector3)direction * speed * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        transform.position = newPosition;
    }

    void AttemptToShoot()
    {
        if (projectilePrefab == null) return;
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void Shoot()
    {
        if (playerTransform == null) return;
        Vector2 directionToShoot = (playerTransform.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        BossProjectile bossProjectileScript = projectile.GetComponent<BossProjectile>();
        if (bossProjectileScript != null)
        {
            bossProjectileScript.Initialize(directionToShoot, projectileSpeed);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead || amount <= 0) return;
        currentHealth -= amount;
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("[BossController] Boss has been defeated!");
        if (rb != null) rb.velocity = Vector2.zero;

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.SetProgressionState(stateToSetOnDefeat);
            PlayerDataManager.Instance.SaveDataToFile();
            Debug.Log($"[BossController] Game progression set to '{stateToSetOnDefeat}' and data saved.");
        }
        else
        {
            Debug.LogError("[BossController] PlayerDataManager.Instance not found! Cannot update progression or save data.");
        }

        StartCoroutine(ReturnToSceneAfterDelay());
    }

    IEnumerator ReturnToSceneAfterDelay()
    {
        Debug.Log($"[BossController] Returning to scene '{sceneToReturnTo}' in {delayBeforeReturn} seconds.");
        yield return new WaitForSeconds(delayBeforeReturn);
        if (!string.IsNullOrEmpty(sceneToReturnTo))
        {
            SceneManager.LoadScene(sceneToReturnTo);
        }
        else
        {
            Debug.LogError("[BossController] sceneToReturnTo is not set!");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("PlayerCharacter_Boss"))
        {
            PlayerController_BossBattle player = collision.gameObject.GetComponent<PlayerController_BossBattle>();
            if (player != null)
            {
                // Assuming PlayerController has a TakeDamage method.
                // player.TakeDamage(contactDamage);
            }
        }
    }
}
