using UnityEngine;

public class MidAutumnBossController : BaseBossController // Also inherits from the base class
{
    [Header("Mid-Autumn Boss Settings")]
    [Tooltip("The fire point on the left side.")]
    public Transform firePointLeft;
    [Tooltip("The fire point on the right side.")]
    public Transform firePointRight;

    protected override void Start()
    {
        base.Start(); // Executes the Start logic from BaseBossController
        if (firePointLeft == null || firePointRight == null)
        {
            Debug.LogError($"[{this.GetType().Name}] One or both fire points have not been assigned in the Inspector!");
            enabled = false;
        }
    }

    // Override the Shoot method to provide the specific dual-shot implementation
    protected override void Shoot()
    {
        if (playerTransform == null || projectilePrefab == null) return;

        // --- Fire from Left Point ---
        Vector2 directionLeft = (playerTransform.position - firePointLeft.position).normalized;
        GameObject projectileLeft = Instantiate(projectilePrefab, firePointLeft.position, Quaternion.identity);
        BossProjectile projectileScriptLeft = projectileLeft.GetComponent<BossProjectile>();
        if (projectileScriptLeft != null)
        {
            projectileScriptLeft.Initialize(directionLeft, projectileSpeed);
        }

        // --- Fire from Right Point ---
        Vector2 directionRight = (playerTransform.position - firePointRight.position).normalized;
        GameObject projectileRight = Instantiate(projectilePrefab, firePointRight.position, Quaternion.identity);
        BossProjectile projectileScriptRight = projectileRight.GetComponent<BossProjectile>();
        if (projectileScriptRight != null)
        {
            projectileScriptRight.Initialize(directionRight, projectileSpeed);
        }
    }
}