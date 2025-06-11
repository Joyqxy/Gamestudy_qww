using UnityEngine;

public class BossController : BaseBossController // Inherits from the base class
{
    [Header("Generic Boss Settings")]
    [Tooltip("The single point from which projectiles are fired.")]
    public Transform firePoint;

    protected override void Start()
    {
        base.Start(); // Executes the Start logic from BaseBossController
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    // Override the Shoot method to provide the specific single-shot implementation
    protected override void Shoot()
    {
        if (playerTransform == null || projectilePrefab == null) return;

        Vector2 directionToShoot = (playerTransform.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        BossProjectile bossProjectileScript = projectile.GetComponent<BossProjectile>();
        if (bossProjectileScript != null)
        {
            bossProjectileScript.Initialize(directionToShoot, projectileSpeed);
        }
    }
}
