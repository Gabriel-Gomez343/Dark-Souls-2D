using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public int maxHealth = 200;
    int currentHealth;
    public Transform player;
    Enemy enemy;
    private Vector3 originalPosition;

    public float returnDistance = 6f;
    public float detectionRange = 5f;
    public float attackRange = 2f;
    private float transitionDelay = 1f; // Adjust this value to set the desired delay time
    //private float attackDelay = 2f;

    private bool isTransitioning = false;
    private bool isFlipped = false;
    private bool isAttacking = false;
    //private bool isOutOfDistance = true;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = animator.GetComponent<Enemy>();
        originalPosition = transform.position;
    }

    // Update is called once per frame
     void Update ()
    {
     float distance = Vector2.Distance(transform.position, player.position);
      
        if (distance < detectionRange)
            enemy.LookAtPlayer();

        if (isAttacking)
            return;
        else if (distance < attackRange && !isAttacking)
        {
            animator.SetInteger("AnimState", 1);
            StartCoroutine(Attack());
        }
        else if (distance < detectionRange && distance > attackRange)
        {
            if (!isTransitioning && !isAttacking)
            {
                StartCoroutine(TransitionToRun());
            }
        }
        else if (distance >= detectionRange)
        {
            enemy.LookAtPlayer();
            animator.SetInteger("AnimState", 3);
        } 

    }

    public void TakeDamage(int damage)
    {
        if (!animator.GetBool("IsDead")){
        currentHealth -= damage;
        Debug.Log(currentHealth);

        animator.SetTrigger("Hurt");

        //play hurt animation
        if (currentHealth <= 0)
        {
            Die();
        }
        }
    }

    void LookAtPlayer(){
        Vector3 flipped = transform.localScale;

        flipped.z *= -1f;

        if (transform.position.x > player.position.x && isFlipped){
            transform.localScale = flipped;
            transform.Rotate(0f,180f,0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped){
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }
    void Die()
    {
        //Debug.Log("Enemy died!");

        animator.SetBool("IsDead", true);

        //GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

    }
     private IEnumerator TransitionToRun()
    {
        isTransitioning = true;
        
        yield return new WaitForSeconds(transitionDelay);
        animator.SetInteger("AnimState", 2);
    
        isTransitioning = false;
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        // Wait for the attack animation to finish
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isAttacking = false;
        animator.SetInteger("AnimState", 1); // Transition back to combat idle
        animator.ResetTrigger("Attack");
    }
    
   
}
