using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public abstract class BaseBossController : MonoBehaviour
{
    [Header("Basic Attributes")]
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float contactDamage = 15f;
    protected float currentHealth;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 7f;
    public float fireRate = 1f;
    public float attackRange = 8f;
    public float chaseStopDistance = 2f;
    protected float nextFireTime = 0f;

    [Header("Behavior")]
    protected Transform playerTransform;

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

    protected bool isDead = false;
    protected Rigidbody2D rb;

    protected virtual void Start()
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
            Debug.LogError($"[{this.GetType().Name}] Player object not found! Ensure player has 'PlayerCharacter_Boss' tag.");
            enabled = false;
        }
    }

    protected virtual void Update()
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

    protected virtual void MoveTowards(Vector2 direction, float speed)
    {
        Vector3 newPosition = transform.position + (Vector3)direction * speed * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        transform.position = newPosition;
    }

    protected virtual void AttemptToShoot()
    {
        if (projectilePrefab == null) return;
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    // This method is marked as 'virtual' so derived classes can override it with their own logic.
    protected virtual void Shoot()
    {
        // This is the default, single-shot behavior.
        // It will be implemented in the regular BossController.
        // MidAutumnBossController will provide a completely different implementation.
        Debug.LogWarning($"[{this.GetType().Name}] Shoot() method not implemented in derived class. Using default behavior is not intended.");
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

    protected virtual void Die()
    {
        isDead = true;
        Debug.Log($"[{this.GetType().Name}] Boss has been defeated!");
        if (rb != null) rb.velocity = Vector2.zero;

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.SetProgressionState(stateToSetOnDefeat);
            PlayerDataManager.Instance.SaveDataToFile();
            Debug.Log($"[{this.GetType().Name}] Game progression set to '{stateToSetOnDefeat}' and data saved.");
        }
        else
        {
            Debug.LogError($"[{this.GetType().Name}] PlayerDataManager.Instance not found! Cannot update progression or save data.");
        }

        StartCoroutine(ReturnToSceneAfterDelay());
    }

    protected IEnumerator ReturnToSceneAfterDelay()
    {
        Debug.Log($"[{this.GetType().Name}] Returning to scene '{sceneToReturnTo}' in {delayBeforeReturn} seconds.");
        yield return new WaitForSeconds(delayBeforeReturn);
        if (!string.IsNullOrEmpty(sceneToReturnTo))
        {
            SceneManager.LoadScene(sceneToReturnTo);
        }
        else
        {
            Debug.LogError($"[{this.GetType().Name}] sceneToReturnTo is not set!");
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("PlayerCharacter_Boss"))
        {
            PlayerController_BossBattle player = collision.gameObject.GetComponent<PlayerController_BossBattle>();
            if (player != null)
            {
                // player.TakeDamage(contactDamage);
            }
        }
    }
}
