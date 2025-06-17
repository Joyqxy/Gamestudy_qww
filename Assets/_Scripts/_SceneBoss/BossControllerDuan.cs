using UnityEngine;
using System.Collections;

// This class inherits all the basic functionalities from BaseBossController.
public class BossControllerDuan : BaseBossController
{
    [Header("Duanwu Boss Special Attack")]
    [Tooltip("The central point from which the fan-shaped attack originates.")]
    public Transform specialAttackFirePoint;

    [Tooltip("The number of projectile lines in the fan.")]
    public int numberOfLines = 3;

    [Tooltip("The number of projectiles per line.")]
    public int projectilesPerLine = 5;

    [Tooltip("The total angle spread of the entire fan attack (e.g., 45 degrees).")]
    public float totalFanAngle = 45f;

    [Tooltip("The angle spread for projectiles within a single line.")]
    public float spreadWithinLine = 10f;

    // We will use the inherited 'fireRate' to control the special attack interval.
    // To attack every 5 seconds, set 'Fire Rate' in the Inspector to 0.2 (1 attack / 5 seconds = 0.2).

    protected override void Start()
    {
        // Execute the Start logic from the base class (finds player, etc.)
        base.Start();

        // Validate the specific fire point for this boss.
        if (specialAttackFirePoint == null)
        {
            Debug.LogError($"[{this.GetType().Name}] The Special Attack Fire Point has not been assigned in the Inspector! Using the main transform as a fallback.");
            specialAttackFirePoint = transform;
        }
    }

    // We override the Shoot method to replace the default single-shot behavior
    // with our unique fan-shaped barrage.
    protected override void Shoot()
    {
        if (playerTransform == null || projectilePrefab == null) return;

        Debug.Log($"[{this.GetType().Name}] Executing Fan Shot Special Attack!");

        // 1. Determine the central direction towards the player.
        Vector2 centerDirection = (playerTransform.position - specialAttackFirePoint.position).normalized;
        float centerAngle = Mathf.Atan2(centerDirection.y, centerDirection.x) * Mathf.Rad2Deg;

        // 2. Calculate the angle between the main lines of the fan.
        float angleBetweenLines = 0;
        if (numberOfLines > 1)
        {
            angleBetweenLines = totalFanAngle / (numberOfLines - 1);
        }
        float startAngle = centerAngle - totalFanAngle / 2f;

        // 3. Loop through each line of projectiles.
        for (int i = 0; i < numberOfLines; i++)
        {
            float currentLineAngle = startAngle + i * angleBetweenLines;

            // 4. For each line, create a spread of projectiles.
            float angleBetweenProjectiles = 0;
            if (projectilesPerLine > 1)
            {
                angleBetweenProjectiles = spreadWithinLine / (projectilesPerLine - 1);
            }
            float lineStartAngle = currentLineAngle - spreadWithinLine / 2f;

            // 5. Loop through each projectile within the line.
            for (int j = 0; j < projectilesPerLine; j++)
            {
                float projectileAngle = lineStartAngle + j * angleBetweenProjectiles;

                // 6. Calculate the final direction vector.
                Vector2 projectileDirection = new Vector2(
                    Mathf.Cos(projectileAngle * Mathf.Deg2Rad),
                    Mathf.Sin(projectileAngle * Mathf.Deg2Rad)
                );

                // 7. Instantiate and initialize the projectile.
                GameObject projectileObj = Instantiate(projectilePrefab, specialAttackFirePoint.position, Quaternion.identity);
                BossProjectile bossProjectileScript = projectileObj.GetComponent<BossProjectile>();
                if (bossProjectileScript != null)
                {
                    bossProjectileScript.Initialize(projectileDirection, projectileSpeed);
                }
            }
        }
    }
}
