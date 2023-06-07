using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinKingController : MonoBehaviour
{

    private bool m_FacingRight = false;
    private Animator animator;
    private Rigidbody2D rb;
    [SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;
    const float k_GroundedRadius = .5f;

    [SerializeField] private GameObject coinBagPrefab;
    [SerializeField] private GameObject coinBagThrowPoint;

    private SpriteRenderer sprite;

    private BoxCollider2D boxCollider;
    private Material mat;

    private bool canJump = true;
    private float jumpCooldownTime = 2f;

    private bool canThrow = true;
    private float throwCooldownTime = 2f;

    private bool canEat = true;
    private float eatCooldownTime = 2f;

    private bool canCommand = true;
    private float commandCooldownTime = 2f;

    private bool canPanic = true;
    private float panicCooldownTime = 2f;

    private float m_JumpForce = 300f;

    private int maxHealth = 500;
    private int currentHealth;
    private int attackDamage = 25;
    private float movementSpeed = 5f;
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

        currentHealth = maxHealth;
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
        if (Input.GetKeyDown("u"))
        {
            MoveRight();
        }
        if (Input.GetKeyUp("z") || Input.GetKeyUp("u"))
        {
            StopMovement();
        }
        


        

        
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


    private void GroundCheck()
    {
        animator.SetBool("Grounded", false);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				animator.SetBool("Grounded", true);
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
        if(currentHealth > 0)
        {
            Hurt(); // hurt animation
            currentHealth -= damageAmount;
            if(currentHealth < 0)
            {
                currentHealth = 0;
            }
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
            canCommand = false;
            animator.SetTrigger("Command");
            StartCoroutine(CommandCooldown());
        }

    }

    private void Hurt()
    {
        animator.SetTrigger("Hurt");
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
        }
        
    }

    private void Throw()
    {
        if(canThrow)
        {
            canThrow = false;
            animator.SetTrigger("Throw");
            StartCoroutine(ThrowDelay());
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

    private IEnumerator JumpDelay(){
        yield return new WaitForSeconds(.2f);
        rb.AddForce(new Vector2(0f, m_JumpForce));
        //boxCollider.isTrigger = true;
    }

    private IEnumerator ThrowDelay(){
        yield return new WaitForSeconds(1.25f);
        GameObject coinBag = Instantiate(coinBagPrefab, coinBagThrowPoint.transform.position, Quaternion.identity);
        coinBag.GetComponent<CoinBag>().setDamage(attackDamage);
        if(m_FacingRight)
        {
            coinBag.GetComponent<Rigidbody2D>().AddForce(new Vector2(400f,300f));
        }
        else
        {
            coinBag.GetComponent<Rigidbody2D>().AddForce(new Vector2(-400f,300f));
        }
    }

    private IEnumerator JumpCooldown(){
        yield return new WaitForSeconds(jumpCooldownTime);
        canJump = true;
    }

    private IEnumerator CommandCooldown(){
        yield return new WaitForSeconds(commandCooldownTime);
        canCommand = true;
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
