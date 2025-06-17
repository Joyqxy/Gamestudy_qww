using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public abstract class BaseBossController : MonoBehaviour
{
    public event System.Action<float, float> OnHealthChanged;

    [Header("Basic Attributes")]
    public float maxHealth = 100f;
    protected float currentHealth;
    public float moveSpeed = 2f;
    public float contactDamage = 15f;

    // ... other variables ...
    protected Transform playerTransform;
    protected bool isDead = false;
    protected Rigidbody2D rb;
    public GameObject projectilePrefab;
    public float projectileSpeed = 7f;
    public float fireRate = 1f;
    public float attackRange = 8f;
    public float chaseStopDistance = 2f;
    protected float nextFireTime = 0f;
    public float minX = -10f, maxX = 10f, minY = -5f, maxY = 5f;
    public string sceneToReturnTo = "_SceneLong";
    public float delayBeforeReturn = 2f;


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
            Debug.LogError($"[{this.GetType().Name}] Player object not found!");
            enabled = false;
        }

        Debug.Log($"[{this.GetType().Name}] Start: Invoking OnHealthChanged with initial health.");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead || amount <= 0) return;
        currentHealth -= amount;
        Debug.Log($"[BaseBossController] Boss health is now {currentHealth}/{maxHealth}.");

        // Trigger health update event
        Debug.Log($"[BaseBossController] Invoking OnHealthChanged event for listeners.");
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    // All other methods (Update, MoveTowards, Die, etc.) remain unchanged and complete.
    protected virtual void Update() { if (isDead || playerTransform == null) return; float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position); Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized; if (distanceToPlayer <= attackRange) { AttemptToShoot(); if (distanceToPlayer > chaseStopDistance) { MoveTowards(directionToPlayer, moveSpeed * 0.5f); } } else { MoveTowards(directionToPlayer, moveSpeed); } }
    protected virtual void MoveTowards(Vector2 direction, float speed) { Vector3 newPosition = transform.position + (Vector3)direction * speed * Time.deltaTime; newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX); newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY); transform.position = newPosition; }
    protected virtual void AttemptToShoot() { if (projectilePrefab == null) return; if (Time.time >= nextFireTime) { Shoot(); nextFireTime = Time.time + 1f / fireRate; } }
    protected abstract void Shoot();
    protected virtual void Die() { isDead = true; Debug.Log($"[{this.GetType().Name}] Boss defeated!"); if (rb != null) rb.velocity = Vector2.zero; if (PlayerDataManager.Instance != null) { PlayerDataManager.Instance.SaveDataToFile(); } StartCoroutine(ReturnToSceneAfterDelay()); }
    protected IEnumerator ReturnToSceneAfterDelay() { Debug.Log($"[{this.GetType().Name}] Returning to '{sceneToReturnTo}' in {delayBeforeReturn}s."); yield return new WaitForSeconds(delayBeforeReturn); if (!string.IsNullOrEmpty(sceneToReturnTo)) { SceneManager.LoadScene(sceneToReturnTo); } else { Debug.LogError($"[{this.GetType().Name}] sceneToReturnTo is not set!"); } }
    protected virtual void OnCollisionEnter2D(Collision2D collision) { if (isDead) return; if (collision.gameObject.CompareTag("PlayerCharacter_Boss")) { PlayerController_BossBattle player = collision.gameObject.GetComponent<PlayerController_BossBattle>(); if (player != null) { player.TakeDamage((int)contactDamage); } } }
}