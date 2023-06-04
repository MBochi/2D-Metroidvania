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
    protected Transform cliffCheck;
    protected GameObject playerObj;

    [SerializeField] private LayerMask m_WhatIsGround;
    const float k_CliffRadius = .2f;
    [SerializeField] protected bool m_FacingRight = true;
    protected bool isIdle = false;

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
        cliffCheck = this.transform.GetChild(0);

        switch(movement_type)
        {
            case 0:
            default:
                isIdle = true;
                break;
            case 1:
                isIdle = false;
                break;
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
        dist_to_player = Vector2.Distance(playerObj.transform.position, transform.position);
        Movement();
        Flip();
    }

    public virtual void FixedUpdate()
    {
    
    }

    protected virtual void Movement()
    {
        
        switch (movement_type)
        {
            case 0:
            default:
                MovementTypeDefault();
                break;
            case 1:
                MovementType1();
                break;
        }
    }


    protected virtual void MovementTypeDefault()
    {
        if(dist_to_player < aggroRange)
        {
            x_direction_to_player = playerObj.transform.position.x - transform.position.x;
            isIdle = false;
            animator.SetInteger("AnimState", 2);
            if(x_direction_to_player < 0) // player is left of enemy 
            {
                m_Rigidbody2D.velocity = new Vector2(movementSpeed *-1f ,m_Rigidbody2D.velocity.y); 
            }
            else if(x_direction_to_player > 0) // player is right of enemy
            {
                m_Rigidbody2D.velocity = new Vector2(movementSpeed ,m_Rigidbody2D.velocity.y);
            }
        }
        else
        {
            m_Rigidbody2D.velocity = new Vector2(0f,0f);
            animator.SetInteger("AnimState", 1); // run animation
        }



        if(x_direction_to_player < 0 && m_FacingRight)
        {
            m_FacingRight = !m_FacingRight;
        }
        else if(x_direction_to_player > 0 && !m_FacingRight)
        {
            m_FacingRight = !m_FacingRight;
        }
    }


    protected virtual void MovementType1()
    {
        if(!isIdle)
        {
            int idle_random = Random.Range(1,5000);
            if (idle_random == 1)
            {
                isIdle = true;
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(cliffCheck.position, k_CliffRadius, m_WhatIsGround);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject) // Turn Enemy if in front of a cliff
                {
                    m_FacingRight = !m_FacingRight;
                }
            }

            if(m_FacingRight == true) 
            {
                m_Rigidbody2D.velocity = new Vector2(movementSpeed, m_Rigidbody2D.velocity.y); 
            }
            else if(m_FacingRight == false)
            {
                m_Rigidbody2D.velocity = new Vector2(movementSpeed * -1, m_Rigidbody2D.velocity.y);
            }
            animator.SetInteger("AnimState", 2); // run animation
        }
        else
        {
            animator.SetInteger("AnimState", 1); // idle animation
            m_Rigidbody2D.velocity = new Vector2(0f, m_Rigidbody2D.velocity.y); 
            StartCoroutine(idleCoolDown());
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
        if(!isIdle)
        {
            if(m_FacingRight)         
            {             
                Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
                transform.rotation = Quaternion.Euler(rotator); // Switch the way the player is labelled as facing.             
                m_FacingRight = !m_FacingRight;         
            }          
            else         
            {             
                Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);             
                transform.rotation = Quaternion.Euler(rotator); // Switch the way the player is labelled as facing.            
                m_FacingRight = !m_FacingRight;        
            }
        }
	}



    protected virtual IEnumerator idleCoolDown(){
        yield return new WaitForSeconds(3f);
        isIdle = false;
    }

    protected virtual IEnumerator dmgCooldown(){
        yield return new WaitForSeconds(.4f);
        m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
