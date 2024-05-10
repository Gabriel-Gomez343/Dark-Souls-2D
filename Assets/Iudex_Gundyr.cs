using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class Iudex_Gundyr : MonoBehaviour
{
    public Animator animator;
    public int maxHealth = 2000;
    public int currentHealth;
    public Transform player;
    Iudex_Gundyr iudex_gundyr;
    private Vector3 originalPosition;
    public HealthBar healthBar; 

    public float longAttackRange;
    public float midAttackRange;
    public float closeAttackRange;
    private float minAttackDelay = 1.0f;
    private float maxAttackDelay = 4.0f;
    
    private Coroutine attackCoroutine;
    private string[] attackTriggersMid = { "PokeAttack", "slash", "stomp"}; 
    private string[] attackTriggersLong = { "JumpAttack", "combo" };
    private string[] attackTriggersShort = {"slash", "punch", "stomp" };

    private bool isFlipped = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private float timeDelay = 0.0f;
    private bool lookAtPlayer = true;
    private bool isFirstAttack = true;

    public Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        iudex_gundyr = animator.GetComponent<Iudex_Gundyr>();
        originalPosition = transform.position;
        healthBar.SetMaxHealth(maxHealth);


    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (isDead){
            return;
        }
      
        if (lookAtPlayer){
            iudex_gundyr.LookAtPlayer();
        }

        if (distance < longAttackRange){
            animator.SetBool("isWalking", false);

            if (!isAttacking){

                if (distance < closeAttackRange){
                    Debug.Log("Player is in close attack range.");
                    StartAttackCoroutine(attackTriggersShort);
                }
                else if (distance < midAttackRange){
                    Debug.Log("Player is in mid attack range.");
                    StartAttackCoroutine(attackTriggersMid);
                }
                else {
                    Debug.Log("Player is in long attack range.");
                    StartAttackCoroutine(attackTriggersLong);
                }
                
            }
            
        }     
        else{    
            isFirstAttack = true;
            animator.SetBool("isWalking", true);
        }
        
        
    }
      private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Draw the long attack range
        Gizmos.DrawWireSphere(transform.position, longAttackRange);

        Gizmos.color = Color.yellow;

        // Draw the mid attack range
        Gizmos.DrawWireSphere(transform.position, midAttackRange);

        Gizmos.color = Color.green;

        // Draw the close attack range
        Gizmos.DrawWireSphere(transform.position, closeAttackRange);
    }


    public void TakeDamage(int damage)
    {
        if (!isDead){
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);


        if (currentHealth <= 0)
        {
            isDead = true;
            animator.SetBool("isDead", true);
            animator.Play("Death",0,0);
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

void StartAttackCoroutine(string[] triggers)
{
     Debug.Log("Starting Attack Coroutine");
    if (attackCoroutine != null)
        StopCoroutine(attackCoroutine);
    attackCoroutine = StartCoroutine(AttackRoutine(triggers));
}

IEnumerator AttackRoutine(string[] attackTriggers)
{
    //yield return new WaitForSeconds(0.1f);
    if (!isAttacking){
        isAttacking = true;

        // Wait for a random delay before attacking
        
        timeDelay = Random.Range(minAttackDelay, maxAttackDelay);
        if(isFirstAttack){
            timeDelay = 0.0f;
        }
        yield return new WaitForSeconds(timeDelay);
        //Debug.Log("Delay Length: " + timeDelay);
        

        
        // Randomly select an attack trigger
        int randomIndex = Random.Range(0, attackTriggers.Length);
        string randomTrigger = attackTriggers[randomIndex];

        // Trigger the selected attack
        animator.SetTrigger(randomTrigger);


        // Wait for a short duration to ensure the animation has started playing
        yield return new WaitForSeconds(0.1f);

        // Get the duration of the current animation clip
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        // Wait for the duration of the animation clip
        yield return new WaitForSeconds(animationLength);

        // Set isAttacking back to false after the attack animation finishes
        isAttacking = false;
        isFirstAttack = false;
    }
            
    
}

    public void MoveForwardBurst(float burstForce)
    {
        // Apply a force in the forward direction
        
        Vector3 direction = -transform.right;
        rb.AddForce(direction * burstForce, ForceMode2D.Impulse);
    }

    public void Jump(float jumpForce)
{
    // Check if the Rigidbody2D component is attached
    if (rb != null)
    {
         // Calculate the forward direction based on the enemy's facing direction
    Vector2 forwardDirection = transform.right;
    float forwardVelocity;

    if (jumpForce > 5f){

    // Apply an impulse force in the upward direction to simulate jumping
    rb.AddForce(Vector2.up * (jumpForce + 1.4f), ForceMode2D.Impulse);

    // Calculate the forward velocity based on the jumpForce
    forwardVelocity = -(jumpForce - 1.65f);
    }

    else {
        rb.AddForce(Vector2.up * (jumpForce), ForceMode2D.Impulse);

    // Calculate the forward velocity based on the jumpForce
    forwardVelocity = -(jumpForce);
    }

    // Get the current vertical velocity
    float verticalVelocity = rb.velocity.y;

    // Move the enemy forward while jumping in the direction it is facing
    rb.velocity = forwardDirection * forwardVelocity + Vector2.up * verticalVelocity;
    }
    else
    {
        Debug.LogWarning("Rigidbody2D component not found. Jump action cannot be performed.");
    }
}
public void LockLookAtPlayerToggle(){
    lookAtPlayer = false;
    Debug.Log("lookAtPlayer: " + lookAtPlayer);
}
public void UnlockLookAtPlayer(){
    lookAtPlayer = true;
    Debug.Log("lookAtPlayer: " + lookAtPlayer);
}
    
}
