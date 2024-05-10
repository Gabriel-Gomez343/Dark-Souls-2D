using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroKnightCombat : MonoBehaviour
{
    public int attackDamage = 100;
    public Vector3 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;
    public LayerMask enemyLayers;
    public HeroKnight heroKnight;

    public GameObject impactAnimationPrefab; // Reference to the impact animation prefab

    private Transform playerTransform;

    private void Start()
    {
        heroKnight = GetComponent<HeroKnight>();
        playerTransform = transform;
    }

    public void Attack()
    {
        Vector3 pos = transform.position;
        float flipMultiplier = heroKnight.GetFacingDirection() == 1 ? 1f : -1f; // Get the flip multiplier based on facing direction
        pos += transform.right * attackOffset.x * flipMultiplier;
        pos += transform.up * attackOffset.y;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(pos, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Iudex_Gundyr>().TakeDamage(attackDamage);
        }

        // Check if hitEnemies is not empty
        if (hitEnemies.Length > 0)
        {
            // Instantiate the impact animation prefab at the attack position
            Instantiate(impactAnimationPrefab, pos, Quaternion.identity);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;
        Gizmos.DrawWireSphere(pos, attackRange);
    }
}
