using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] int movement_type; // 1: Move left to right; 0/default: Idle until player is near
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected int maxHealth = 100;
    protected int currentHealth;
    protected float aggroRange = 5f;
    protected float attackRadius = .5f;
    protected float attackRange = 1.5f;
    [SerializeField] protected int attackDamage = 20;
    

    
    protected float attackRate = 1f;
    protected float attackDelay = .5f;
    protected float nextAttackTime = 0f;
    protected bool isAttacking = false;
    protected bool isDead = false;
    protected bool canStillDamagePlayer = true;

    protected float despawnTime = 3f;


    protected Animator animator;
    protected Rigidbody2D m_Rigidbody2D;
    protected PlayerCombat playerCombat;

    [SerializeField] protected Transform cliffCheck;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected Transform LootSpawnPoint;
    
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected LayerMask m_WhatIsGround;

    [SerializeField] protected GameObject coinPrefab;
    [SerializeField] protected GameObject heartPrefab;
    [SerializeField] protected GameObject playerObj;

    
    const float k_CliffRadius = .2f;
    [SerializeField] protected bool m_FacingRight = false;

    protected float dist_to_player;
    protected float x_direction_to_player;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        animator = GetComponent<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        playerCombat = GameObject.FindWithTag("Player").GetComponent<PlayerCombat>();
        playerObj = GameObject.FindWithTag("Player");
        currentHealth = maxHealth;
        startPosition = this.transform.position;

    }

    public virtual void Update()
    {

        if (Input.GetKeyDown("j"))
        {
            Respawn();
        }
        dist_to_player = Vector2.Distance(playerObj.transform.position, transform.position);
        x_direction_to_player = playerObj.transform.position.x - transform.position.x;

        if(dist_to_player < aggroRange && x_direction_to_player < aggroRange) // aggro mode
        {
            if(dist_to_player < attackRange)
            {
                if (!isAttacking)
                {
                    Attack();
                    return;
                }
            }
            AggroMovement();
        }
        else // normal movement depending on settings
        {
            switch(movement_type)
            {
                default:
                case 0:
                    m_Rigidbody2D.velocity = new Vector2(0f, m_Rigidbody2D.velocity.y);
                    animator.SetInteger("AnimState", 1); // idle animation
                    break;
                case 1:
                    PatrolingMovement();
                    break;
            }
        }
    }

    protected virtual void Attack()
    {  
        isAttacking = true;
        canStillDamagePlayer = true;
        StartCoroutine(AttackCooldown());
        animator.SetBool("Attack", true);
        if (!isDead) m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        StartCoroutine(AttackDelay());
    
    }
    
    protected virtual void PatrolingMovement()
    {
        animator.SetInteger("AnimState", 2); // run animation
        if(m_FacingRight)
        {
            m_Rigidbody2D.velocity = new Vector2(movementSpeed, m_Rigidbody2D.velocity.y);
        }
        else
        {
            m_Rigidbody2D.velocity = new Vector2(movementSpeed * -1, m_Rigidbody2D.velocity.y);
        }

        if(!HasFloorInfront() || HasWallInfront())
        {
            Flip();
        }
    }

    protected virtual bool HasFloorInfront()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(cliffCheck.position, k_CliffRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject) // Turn Enemy if in front of a wall
            {
                return true;
            }
        }
        return false;
    }

    protected virtual bool HasWallInfront()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(wallCheck.position, k_CliffRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject) // Turn Enemy if in front of a wall
            {   
                return true; 
            }
        }
        return false;
    }

    protected virtual void AggroMovement()
    {
        if(x_direction_to_player > 0 && m_FacingRight) // Facing left and Player is left
        {
            m_Rigidbody2D.velocity = new Vector2(movementSpeed, m_Rigidbody2D.velocity.y);
            animator.SetInteger("AnimState", 2); // run animation
        }
        else if(x_direction_to_player < 0 && !m_FacingRight) // facing right and Player is right
        {
            m_Rigidbody2D.velocity = new Vector2(movementSpeed * -1, m_Rigidbody2D.velocity.y);
            animator.SetInteger("AnimState", 2); // run animation
        }
        else if ((x_direction_to_player > 0 && !m_FacingRight) || (x_direction_to_player < 0 && m_FacingRight)) // Player is in aggro range, but facing wrong direction
        {
            Flip();
        }
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        canStillDamagePlayer = false;
        Debug.Log("Enemy took Damage: " + damage);
        if (!isDead) m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        if(currentHealth <= 0){
            Die();
            return;
        }
        StartCoroutine(dmgCooldown());
    }

    protected virtual void Die()
    {
        Debug.Log("Enemy died!");
        animator.SetTrigger("Death");

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        isDead = true;
        m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;

        SpawnLoot();
        StartCoroutine(despawnCooldown());
    }

    protected virtual void SpawnLoot()
    {

        // Coins
        int coinAmount = Random.Range(0,10); // spawn 0-9 coins on death 
        Debug.Log("Spawning " + coinAmount + " Coins");

    

        for(int i = 0; i < coinAmount; i++)
        {
            float xoffset = Random.Range(-1f,1f);
            float yoffset = Random.Range(0f,1f);
            GameObject coin = Instantiate(coinPrefab, new Vector3(LootSpawnPoint.transform.position.x + xoffset, LootSpawnPoint.transform.position.y + yoffset, LootSpawnPoint.transform.position.y), Quaternion.identity);
            int xDirection = Random.Range(-3,4);
            int yDirection = Random.Range(1,4);
            coin.GetComponent<Rigidbody2D>().AddForce(new Vector2(xDirection,yDirection),ForceMode2D.Impulse);
            
        }


        int heartAmount = Random.Range(0,2); // spawn 0-9 coins on death 
        Debug.Log("Spawning " + coinAmount + " hearts");

    

        for(int i = 0; i < heartAmount; i++)
        {
            float xoffset = Random.Range(-1f,1f);
            float yoffset = Random.Range(0f,1f);
            GameObject heart = Instantiate(heartPrefab, new Vector3(LootSpawnPoint.transform.position.x + xoffset, LootSpawnPoint.transform.position.y + yoffset, LootSpawnPoint.transform.position.y), Quaternion.identity);
            int xDirection = Random.Range(-3,4);
            int yDirection = Random.Range(1,4);
            heart.GetComponent<Rigidbody2D>().AddForce(new Vector2(xDirection,yDirection),ForceMode2D.Impulse);
            
        }
    }

    protected virtual void Flip()
	{
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        m_FacingRight = !m_FacingRight;
	}

    protected virtual IEnumerator dmgCooldown(){
        yield return new WaitForSeconds(.4f);
        if (!isDead) m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    protected virtual IEnumerator despawnCooldown(){
        yield return new WaitForSeconds(despawnTime);
        Destroy(this.gameObject);
    }

    protected virtual IEnumerator AttackCooldown(){
        yield return new WaitForSeconds(attackRate);
        if (!isDead) m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        isAttacking = false;
    }

    protected virtual IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        
        if(canStillDamagePlayer)
        {
            // Detect if player is in range of attack
            Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
            bool playerWasHit = false;
            // Damage the player
            foreach (Collider2D player in hitPlayer)
            {
                if (checkIfPlayerBlocksAttack(player))
                {
                    playerWasHit = true;
                }
                
            }
            if(playerWasHit)
            {
                playerObj.GetComponent<PlayerCombat>().TakeDamage(attackDamage);
            }
        }
        
    }

    private bool checkIfPlayerBlocksAttack(Collider2D collider)
    {
        // true if either the player blocks and the attack comes from the other side or player is not blocking
        if (playerCombat.isBlocking && checkifEnemyBehindPlayer(collider) || !playerCombat.isBlocking)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    private bool checkifEnemyBehindPlayer(Collider2D collider)
    {
        if (collider.transform.rotation.y < 0 && this.transform.localScale.x > 0 || 
            collider.transform.rotation.y == 0 && this.transform.localScale.x < 0) return true;

        else return false;     
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
