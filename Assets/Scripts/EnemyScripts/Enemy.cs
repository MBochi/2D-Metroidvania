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
    

    protected Vector3 currentTarget;
    protected Animator animator;
    protected Rigidbody2D m_Rigidbody2D;
    [SerializeField] protected Transform cliffCheck;
    [SerializeField] protected Transform wallCheck;
    protected GameObject playerObj;

    [SerializeField] private LayerMask m_WhatIsGround;
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
        animator = GetComponentInChildren<Animator>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        playerObj = GameObject.FindWithTag("Player");
        currentHealth = maxHealth;
    }

    public virtual void Update()
    {
        dist_to_player = Vector2.Distance(playerObj.transform.position, transform.position);
        x_direction_to_player = playerObj.transform.position.x - transform.position.x;

        if(dist_to_player < aggroRange && x_direction_to_player < aggroRange) // aggro mode
        {
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
        m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
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
        m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
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
        m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
