using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gundyr_Weapon : MonoBehaviour
{
    public int attackDamage = 20;
    public Vector3 normalAttackOffset;
    public Vector3 closeAttackOffset;
    public Vector3 centerAttackOffset;
    private Vector3 attackOffset;

    
    public float overheadAttackWidth;
    public float overheadAttackHeight;
    private int overheadAttackDamage = 80;

    public float slashAttackWidth;
    public float slashAttackHeight;
    private int slashAttackDamage = 50;

    public float plungeAttackWidth;
    public float plungeAttackHeight;
    private int plungeAttackDamage = 80;

    public float closeSlashAttackWidth;
    public float closeSlashAttackHeight;
    private int closeSlashAttackDamage = 50;

    public float pokeAttackWidth;
    public float pokeAttackHeight;
    private int pokeAttackDamage = 60;

    public float meleeAttackWidth;
    public float meleeAttackHeight;
    private int meleeAttackDamage = 50;

    public float stompAttackWidth;
    public float stompAttackHeight;
    private int stompAttackDamage = 80;

    private float attackWidth = 1f;
    private float attackHeight = 0.5f; // Adjust the height as desired
    public LayerMask attackMask;

    private bool isVisualizingColliders = false;
    private float visualizationInterval = 0.9f; // Adjust as needed
    private float lastVisualizationTime = 0f;

    private void Update()
    {
        // Check if it's time to visualize colliders again
        if (Time.time - lastVisualizationTime >= visualizationInterval)
        {
            isVisualizingColliders = true;
            lastVisualizationTime = Time.time;
        }
        else
        {
            isVisualizingColliders = false;
        }
    }

    public void OverheadAttack(){
        attackWidth = overheadAttackWidth;
        attackHeight = overheadAttackHeight;
        attackDamage = overheadAttackDamage;
        attackOffset = normalAttackOffset;
        Attack();

    }
    public void SlashAttack(){
        attackWidth = slashAttackWidth;
        attackHeight = slashAttackHeight;
        attackDamage = slashAttackDamage;
        attackOffset = normalAttackOffset;
        Attack();

    }
    public void PlungeAttack(){
        attackWidth = plungeAttackWidth;
        attackHeight = plungeAttackHeight;
        attackDamage = plungeAttackDamage;
        attackOffset = closeAttackOffset;
        Attack();
    }
    public void closeSlashAttack(){
        attackWidth = closeSlashAttackWidth;
        attackHeight = closeSlashAttackHeight;
        attackDamage = closeSlashAttackDamage;
        attackOffset = closeAttackOffset;
        Attack();
    }
    public void pokeAttack(){
        attackWidth = pokeAttackWidth;
        attackHeight = pokeAttackHeight;
        attackDamage = pokeAttackDamage;
        attackOffset = normalAttackOffset;
        Attack();
    }

    public void meleeAttack(){
        attackWidth = meleeAttackWidth;
        attackHeight = meleeAttackHeight;
        attackDamage = meleeAttackDamage;
        attackOffset = closeAttackOffset;
        Attack();
    }

    public void stompAttack(){
        attackWidth = stompAttackWidth;
        attackHeight = stompAttackHeight;
        attackDamage = stompAttackDamage;
        attackOffset = centerAttackOffset;
        Attack();
    }

    void Attack()
    {
        Vector3 pos = transform.position;
        pos += transform.right * attackOffset.x;
        pos += transform.up * attackOffset.y;

        if (isVisualizingColliders)
        {
            // Simulate physics for one frame to visualize colliders
            Physics2D.Simulate(Time.fixedDeltaTime);
        }

        Collider2D[] hitObjects = Physics2D.OverlapCapsuleAll(pos, new Vector2(attackWidth, attackHeight), CapsuleDirection2D.Horizontal, 0f, attackMask);
        foreach (Collider2D col in hitObjects)
        {
            HeroKnight hero = col.GetComponent<HeroKnight>();
            if (hero != null)
            {
                hero.TakeDamage(attackDamage);
            }
        }
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Vector3 pos = transform.position;
    //     pos += transform.right * centerAttackOffset.x;
    //     pos += transform.up * centerAttackOffset.y;
    //     Gizmos.DrawWireCube(pos, new Vector3(stompAttackWidth, stompAttackHeight, 0f));
    // }
}
