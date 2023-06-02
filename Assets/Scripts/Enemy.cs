using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public Animator animator;
    protected int maxHealth = 100;
    protected float aggroRange = 7f;
    protected float movementSpeed = 2f;
    protected float knockbackSpeed = 5000f;
    [SerializeField] int currentHealth;
    protected Rigidbody2D m_Rigidbody2D;

    protected bool m_FacingRight = true;

    protected float dist_to_player;
    protected float x_direction_to_player;

    protected GameObject playerObj;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        playerObj = GameObject.FindWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        if(currentHealth <= 0){
            Die();
        }
        Knockback();
    }

    private void Die()
    {

        Debug.Log("Enemy died!");
        animator.SetTrigger("Death");

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezePositionX;

    }

    public void Flip()
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

    public void Knockback()
    {
        Vector2 direction = Vector2.zero;
        if(x_direction_to_player < 0) // player is left of enemy 
        {
            direction = Vector2.right;
        }
        else if(x_direction_to_player > 0) // player is right of enemy
        {
            direction = Vector2.left;
        }
        m_Rigidbody2D.AddForce(direction * knockbackSpeed);
    }

    public abstract void Move();

}
