using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinKingController : MonoBehaviour
{

    private bool m_FacingRight = true;
    private Animator animator;
    public HealthBar healthBar;
    private Rigidbody2D rb;
    [SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;
    const float k_GroundedRadius = .5f;

    private GameObject playerObj;
    public GameObject bossHealthbarObj;
    private float x_direction_to_player;

    [SerializeField] private GameObject coinBagPrefab;
    [SerializeField] private GameObject coinBagThrowPoint;
    [SerializeField] private GameObject shockWavePrefab;
    [SerializeField] private GameObject PlatformLeft;
    [SerializeField] private GameObject PlatformRight;
    private int lastPlatformChoice = -1;

    private SpriteRenderer sprite;

    private BoxCollider2D boxCollider;
    private Material mat;

    private bool bossFightStarted = false;
    private bool canDoMove = true;
    private bool canTakeDamage = false;

    private bool canJump = true;
    private float jumpCooldownTime = 2f;

    private bool canThrow = true;
    private float throwCooldownTime = 2f;

    private bool canEat = true;
    private float eatCooldownTime = 2f;
    private bool hasHealed = false;

    private bool canCommand = true;
    private float commandCooldownTime = .5f;

    private bool canPanic = true;
    private float panicCooldownTime = 1.5f;

    private float m_JumpForce = 500f;

    private int maxHealth = 500;
    private int currentHealth;
    private int coinBagDamage = 25;
    private int contactDamage = 50;
    private float movementSpeed = 5f;

    private Vector3 startingPos;
    // Start is called before the first frame update
    void Start()
    {
        Init();
        //Time.timeScale = 0.15f; // make unity run in slowmo
    }

    private void Init()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        ToggleMovementFreeze(true);

        playerObj = GameObject.FindWithTag("Player");

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        startingPos = transform.position;
    }

    private void FixedUpdate()
    {
        if(currentHealth > 0)
        {
            GroundCheck();
        }
          
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth == 0)
        {
            Die();
        }

        if (currentHealth < 0)
        {
            return;
        }

        if(bossFightStarted)
        {
            x_direction_to_player = playerObj.transform.position.x - transform.position.x;

            if(canDoMove && canPanic)
            {
                canDoMove = false;
                int randomMove = Random.Range(1,6); // random move between 1 and 5
                
                if (currentHealth <= maxHealth/3 && !hasHealed) // single time heal when low HP
                {
                    randomMove = 6;
                    hasHealed = true;
                }

                if(hasHealed) // small chance to heal again
                {
                    int randomHealChance = Random.Range(0,20);
                    if(randomHealChance == 5)
                    {
                        randomMove = 6;
                    }
                }

                switch (randomMove)
                {
                    case 1: // 40% chance for normal Jump
                    case 2:
                        Jump();
                        StartCoroutine(CanDoMoveCooldown(2.5f));
                        break;

                    case 3: // 40% chance for normal Throw
                    case 4:
                        Throw();
                        StartCoroutine(CanDoMoveCooldown(1.5f));
                        break;

                    case 5: // 20% chance to jump to a platform
                        JumpToPlatform();
                        StartCoroutine(CanDoMoveCooldown(6f));
                        break;

                    case 6:
                        Command();
                        StartCoroutine(CanDoMoveCooldown(3f));
                        break;
                    
                }
            }
            
            /*  MANUAL CONTROLLS
            if (Input.GetKeyDown("j"))
            {
                Jump();
            }
            if (Input.GetKeyDown("k"))
            {
                Throw();
            }
            if (Input.GetKeyDown("l"))
            {
                Eat(5);
            }
            if (Input.GetKeyDown("p"))
            {
                Command();
            }
            if (Input.GetKeyDown("o"))
            {
                Die();
                currentHealth = 0;
            }
            if (Input.GetKeyDown("h"))
            {
                Panic();
            }
            if (Input.GetKeyDown("z"))
            {
                MoveLeft();
            }
            if (Input.GetKeyDown("t"))
            {
                JumpToPlatform();
            }
            if (Input.GetKeyDown("u"))
            {
                MoveRight();
            }
            if (Input.GetKeyUp("z") || Input.GetKeyUp("u"))
            {
                StopMovement();
            }

            */
        }
        


        
        // LR Movement + Fall animations
        
        if(rb.velocity.y < 0)
        {
            animator.SetFloat("AirSpeedY", -1);
            //boxCollider.isTrigger = false;
            Slam();
        } 
        else
        {
            animator.SetFloat("AirSpeedY", 0);
        }

        if(rb.velocity.x != 0)
        {
            animator.SetInteger("AnimState", 1);
            if(rb.velocity.x > 0 && !m_FacingRight)
            {
                Flip();
            }
            else if(rb.velocity.x < 0 && m_FacingRight)
            {
                Flip();
            }
        }
        else
        {
            animator.SetInteger("AnimState", 0);
        }
    }

    public void StartBossFight()
    {
        if(!bossFightStarted)
        {
            Debug.Log("Start Bossfight");
            bossHealthbarObj.SetActive(true);
            canDoMove = true;
            canPanic = true;
            Flip();
            bossFightStarted = true;
            Panic();
            canTakeDamage = true;
        }
    }

    public void StopBossFight()
    {
        if (bossFightStarted)
        {
            Debug.Log("Stop Bossfight");
            bossHealthbarObj.SetActive(false);
            bossFightStarted = !bossFightStarted;
            canTakeDamage = !canTakeDamage;
            currentHealth = maxHealth;
        }
    }

    public void ResetBossPosition()
    {
        transform.position = startingPos;
    }


    private void GroundCheck()
    {
        animator.SetBool("Grounded", false);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				animator.SetBool("Grounded", true);
                ToggleMovementFreeze(true);
			}
		}
    }

    private void ToggleMovementFreeze(bool freezeX)
    {
        if(freezeX)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        }
        else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }


    }

    public void TakeDamage(int damageAmount)
    {  
        if(currentHealth > 0 && canTakeDamage)
        {
            Hurt(); // hurt animation
            currentHealth -= damageAmount;
            if(currentHealth < 0)
            {
                currentHealth = 0;  
            }
            healthBar.SetHealth(currentHealth);
        }
    } 

    private void MoveLeft()
    {
        ToggleMovementFreeze(false);
        rb.velocity = new Vector2(-movementSpeed, rb.velocity.y);
    }

    private void MoveRight()
    {
        ToggleMovementFreeze(false);
        rb.velocity = new Vector2(movementSpeed, rb.velocity.y);
    }

    private void StopMovement()
    {
        ToggleMovementFreeze(true);
    }

    private void JumpToPlatform()
    {
        int platformChoice = Random.Range(0,2);
        if (platformChoice == lastPlatformChoice)
        {
            if (platformChoice == 0) platformChoice = 1;
            if (platformChoice == 1) platformChoice = 0;
        }

        if(platformChoice == 1) // left
        {
            transform.position = PlatformLeft.transform.position;
            if(!m_FacingRight) Flip();
        }
        else if(platformChoice == 0) // right
        {
            transform.position = PlatformRight.transform.position;
            if(m_FacingRight) Flip();
        }
        StartCoroutine(JumpToPlatformCooldown());
        lastPlatformChoice = platformChoice;
    }

    private void Die()
    {
        currentHealth = -1;
        animator.SetTrigger("Death");
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        boxCollider.enabled = false;
    }

    private void Panic()
    {
        if(canPanic)
        {
            canPanic = false;
            animator.SetTrigger("Panic");
            StartCoroutine(PanicCooldown());
        }
    }

    private void Command()
    {
        if(canCommand)
        {
            canTakeDamage = false;
            StartCoroutine(CanTakeDamageCooldown(3f));
            canCommand = false;
            animator.SetTrigger("Command");
            StartCoroutine(CommandCooldown());

            GameObject shockWave = Instantiate(shockWavePrefab, this.transform.position, Quaternion.identity);
        }

    }

    private void Hurt()
    {
        string animation_name = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (animation_name == "Goblin King Idle Animation" || animation_name == "Goblin King Run Animation")
        {
            animator.SetTrigger("Hurt");
        }
        StartCoroutine(FlashRed());
    }

    private void Eat(int healAmount)
    {
        if(canEat)
        {
            canEat = false;
            animator.SetTrigger("Eat");
            StartCoroutine(EatCooldown());

            currentHealth += healAmount;
            if(currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            healthBar.SetHealth(currentHealth);
        }
        
    }

    private void Throw()
    {
        if(canThrow)
        {
            canThrow = false;
            if(x_direction_to_player > 0 && !m_FacingRight) Flip();
            else if(x_direction_to_player < 0 && m_FacingRight) Flip();
            animator.SetTrigger("Throw");
            StartCoroutine(ThrowDelay());
            StartCoroutine(ThrowCooldown());
        }
    }

    private void ThrowMultipleCoinBags()
    {
        if(canThrow)
        {
            canThrow = false;
            animator.SetTrigger("Throw");
            StartCoroutine(ThrowDelayMultiple());
            StartCoroutine(ThrowCooldown());
        }
    }

    private void Slam()
    {
         rb.AddForce(new Vector2(0f, -(m_JumpForce)/64));
    }

    private void Jump()
    {
        if(canJump)
        {
            canJump = false;
            animator.SetTrigger("Jump");
            StartCoroutine(JumpDelay());
            StartCoroutine(JumpCooldown());
        }
    }

    private void Flip()
	{
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        m_FacingRight = !m_FacingRight;
	}


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerCombat>().TakeDamage(contactDamage);
        }
    }

    private IEnumerator JumpDelay(){
        yield return new WaitForSeconds(.2f);
        float xJumpDirection = x_direction_to_player * 40f;
        ToggleMovementFreeze(false);
        rb.AddForce(new Vector2(xJumpDirection, m_JumpForce));
        //boxCollider.isTrigger = true;
    }

    private IEnumerator ThrowDelay(){
        yield return new WaitForSeconds(1.25f);
        GameObject coinBag = Instantiate(coinBagPrefab, coinBagThrowPoint.transform.position, Quaternion.identity);
        coinBag.GetComponent<CoinBag>().setDamage(coinBagDamage);
        if(m_FacingRight)
        {
            coinBag.GetComponent<Rigidbody2D>().AddForce(new Vector2(400f,300f));
        }
        else
        {
            coinBag.GetComponent<Rigidbody2D>().AddForce(new Vector2(-400f,300f));
        }
    }

    private IEnumerator ThrowDelayMultiple(){
        yield return new WaitForSeconds(1.25f);

        for(int i = 1; i < 5; i++)
        {
            GameObject coinBag = Instantiate(coinBagPrefab, coinBagThrowPoint.transform.position, Quaternion.identity);
            coinBag.GetComponent<CoinBag>().setDamage(coinBagDamage);
            if(m_FacingRight)
            {
                coinBag.GetComponent<Rigidbody2D>().AddForce(new Vector2(100f * i,300f));
            }
            else
            {
                coinBag.GetComponent<Rigidbody2D>().AddForce(new Vector2(-100f * i,300f));
            }
        }
    }

    private IEnumerator JumpCooldown(){
        yield return new WaitForSeconds(jumpCooldownTime);
        canJump = true;
    }

    private IEnumerator JumpToPlatformCooldown()
    {
        yield return new WaitForSeconds(1f);
        ThrowMultipleCoinBags();
        StartCoroutine(JumpFromPlatform());
    }

    private IEnumerator JumpFromPlatform()
    {
        yield return new WaitForSeconds(3f);
        Jump();
    }

    private IEnumerator CommandCooldown(){
        yield return new WaitForSeconds(commandCooldownTime);
        canCommand = true;
        Eat(maxHealth / 2);
    }

    private IEnumerator CanDoMoveCooldown(float time){
        yield return new WaitForSeconds(time);
        canDoMove = true;
    }

    private IEnumerator ThrowCooldown(){
        yield return new WaitForSeconds(throwCooldownTime);
        canThrow = true;
    }

    private IEnumerator EatCooldown(){
        yield return new WaitForSeconds(eatCooldownTime);
        canEat = true;
    }

    private IEnumerator PanicCooldown(){
        yield return new WaitForSeconds(panicCooldownTime);
        canPanic = true;
    }

    private IEnumerator CanTakeDamageCooldown(float time){
        yield return new WaitForSeconds(time);
        canTakeDamage = true;
    }

    private IEnumerator FlashRed()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }


    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(m_GroundCheck.position, k_GroundedRadius);
    }
}
