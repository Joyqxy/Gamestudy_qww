using UnityEngine;

public class PlayerController_BossBattle : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseMoveSpeed = 5f;
    public float baseFireRate = 2f;    // Shots per second
    public int baseProjectileDamage = 10;

    [Header("Dependencies & Config")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public BossSceneManager sceneManager;

    private float nextFireTime = 0f;

    // Melee attack fields are kept in case you want to re-enable them later
    [Header("Melee Attack (Currently Inactive)")]
    public Transform meleeAttackPoint;
    public float meleeAttackRange = 0.5f;
    public LayerMask enemyLayers;
    public float meleeAttackRate = 1.5f;
    public int meleeDamage = 15;
    private float nextMeleeTime = 0f;

    void Update()
    {
        HandleMovement();
        HandleAttack();
    }

    void HandleMovement()
    {
        float currentMoveSpeed = baseMoveSpeed;

        // Apply Speed Upgrade
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.HasSpeedUpgrade)
        {
            currentMoveSpeed *= 1.5f;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        transform.Translate(moveDirection * currentMoveSpeed * Time.deltaTime);
    }

    void HandleAttack()
    {
        // Per your request, bullets are infinite, so we always attempt to shoot.
        if (Input.GetButton("Fire1"))
        {
            float currentFireRate = baseFireRate;

            // Apply Fire Rate Upgrade
            if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.HasFireRateUpgrade)
            {
                currentFireRate *= 1.5f;
            }

            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / currentFireRate;
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;

        int currentDamage = baseProjectileDamage;

        // Apply Damage Upgrade
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.HasDamageUpgrade)
        {
            currentDamage = Mathf.RoundToInt(baseProjectileDamage * 1.5f);
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Apply Scale Upgrade
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.HasDamageUpgrade)
        {
            projectile.transform.localScale *= 1.5f;
        }

        Projectile_BossBattle projectileScript = projectile.GetComponent<Projectile_BossBattle>();
        if (projectileScript != null)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0;
            Vector2 direction = (mouseWorldPosition - firePoint.position).normalized;

            projectileScript.Initialize(direction, projectileSpeed, currentDamage);
        }
    }

    public void TakeDamage(int damage)
    {
        if (sceneManager != null)
        {
            sceneManager.PlayerTakesDamage(damage);
        }
        else
        {
            Debug.LogError($"[{this.GetType().Name}] SceneManager reference is not set! Player cannot take damage.");
        }
    }
}