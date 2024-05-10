using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem; 

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] GameObject m_slideDust;
    [SerializeField] float idleBlockSpeed = 0.5f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    private bool                m_isWallSliding = false;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_timeSinceStaminaUsed = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;
    private int                 staminaUsage = 0;
    private int                 staminaRecovery = 70;
    private bool                iFrame = false;
    public Text                 estusCounterText;
    private bool                isBlocking = false;

    private bool isMenuVisible = false;
    private bool isPaused = false;
    public int maxHealth = 100;
    public int currentHealth;
    public int maxStamina = 1000;
    public int currentStamina;
    public bool playerDead = false;
    public int healthFlask;

    public HealthBar healthBar;
    public StaminaBar staminaBar;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public Image menuImage;
    //public int attackDamage = 100;
    
    // Use this for initialization
    void Start ()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
        estusCounterText.text = healthFlask.ToString();

        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();

        menuImage.gameObject.SetActive(false);

        //healAction = InputSystem.OnHeal();

    }

    // Update is called once per frame
    void Update ()
    {
        estusCounterText.text = healthFlask.ToString();
        if (m_animator.GetBool("isDead")){
            return;
        }
        
        // Increase timer that controls attack combo

        // STAMINA RECOVERY
        m_timeSinceAttack += Time.deltaTime;
        m_timeSinceStaminaUsed += Time.deltaTime;

        if (m_timeSinceStaminaUsed > 0.75f){
            if (m_animator.GetBool("IdleBlock")){
                if(currentStamina <= maxStamina - (staminaRecovery/2))
            currentStamina += (staminaRecovery/2);

            else 
            currentStamina = maxStamina;
            }
            else {
            if(currentStamina <= maxStamina - staminaRecovery)
            currentStamina += staminaRecovery;

            else 
            currentStamina = maxStamina;
            }

            staminaBar.SetStamina(currentStamina);
            m_timeSinceStaminaUsed = 0.0f;
        }
                

        // Increase timer that checks roll duration
        if (m_rolling)
    {
        m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
        {
            m_rolling = false;
            m_rollCurrentTime = 0.0f; // Reset roll timer
        }
    }

        // Disable rolling if timer extends duration
        if(m_rollCurrentTime > m_rollDuration) 
            m_rolling = false;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");
        float currentSpeed = m_rolling ? m_rollForce : m_speed;
        
        if (m_animator.GetBool("IdleBlock"))
{
    currentSpeed = idleBlockSpeed;
}
        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;        
        }
            
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
            
            
        }

        // Move
        if (!m_rolling )
            m_body2d.velocity = new Vector2(inputX * currentSpeed, m_body2d.velocity.y);
        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);



        //Run
        if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }
    }

    
    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }

    public void TakeDamage(int damage){
        int staminaModifier = 8;
        if (m_animator.GetBool("IdleBlock") && currentStamina > damage*staminaModifier)
        {
            UseStamina(damage*staminaModifier);
        }
       
       else if (!iFrame){ 
        if (!m_animator.GetBool("isDead")){
            if (m_animator.GetBool("IdleBlock")){
                damage = Mathf.RoundToInt(damage * 0.65f);
            }
            currentHealth -= damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            m_animator.SetTrigger("Death");
            m_animator.SetBool("isDead", true);
            playerDead = true;
        
        }
        else {
            
            m_animator.SetTrigger("Hurt");
        }
       }
            
        
        healthBar.SetHealth(currentHealth);
        }

    }
    public void Toggle_iFrame(){
        iFrame = !iFrame;
    }

    void UseStamina(int stamina){
        currentStamina -= stamina;
        if (currentStamina < 0){
            currentStamina = 0;
        }
        staminaBar.SetStamina(currentStamina);
        m_timeSinceAttack = 0.0f;
    }

    void RestoreHealth(int recovery){
        if (maxHealth >= currentHealth + recovery){
        currentHealth += recovery;
        }

        else{
        currentHealth = maxHealth;
        }

        healthBar.SetHealth(currentHealth);
    }
    public int GetFacingDirection(){
        return m_facingDirection;
    }
    public bool isPlayerDead(){
        return playerDead;
    }
    void OnDrawGizmosSelected(){
        if (attackPoint == null){
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    public void OnHeal(){
        if (healthFlask > 0 && !isMenuVisible)
        {
            RestoreHealth(40);
            healthFlask -= 1;
        }
    }
    public void OnJump(){
        if (m_grounded && !m_rolling && !isMenuVisible)
        {
            staminaUsage = 130;

            if (staminaUsage <= currentStamina){
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);

            UseStamina(staminaUsage);
            }

           
        }
    }
    public void OnRoll(){
        
        if (!m_rolling && !m_isWallSliding && m_grounded && !isMenuVisible)
        {
            staminaUsage = 100;
            if (staminaUsage <= currentStamina){
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);

            UseStamina(staminaUsage);
            }

           
        }
    }
    public void OnFire(){
        if(m_timeSinceAttack > 0.25f && !m_rolling && !isMenuVisible)
        {
            staminaUsage = 150;
            if (currentStamina > 0){
            m_currentAttack++;


            //Atack Animation
            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            m_timeSinceAttack = 0.0f;

            UseStamina(staminaUsage);
            }

            
        }
    }

    public void OnBlock(){
        if (!m_rolling)
        {
        if (!isBlocking)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
            isBlocking = true;
        }
        else
        {
            // If the block button is released, cancel the block
            m_animator.SetBool("IdleBlock", false);
            isBlocking = false;
        }
        }

    }

    public void OnMenu(){
        // Toggle the visibility of the menu image and its children
    if (!isMenuVisible)
    {
        menuImage.gameObject.SetActive(true);
        isMenuVisible = true;
    }
    else
    {
        menuImage.gameObject.SetActive(false);
        isMenuVisible = false;
    }

    // Toggle pause state
    TogglePause();
    }

    public void TogglePause()
    {
         isPaused = !isPaused;
    Time.timeScale = isPaused ? 0 : 1;

    // Enable or disable the text and button based on pause state
    Text[] textComponents = menuImage.GetComponentsInChildren<Text>(true);
    foreach (Text textComponent in textComponents)
    {
        textComponent.gameObject.SetActive(isMenuVisible && isPaused);
    }

    Button[] buttonComponents = menuImage.GetComponentsInChildren<Button>(true);
    foreach (Button buttonComponent in buttonComponents)
    {
        buttonComponent.gameObject.SetActive(isMenuVisible && isPaused);
    }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
