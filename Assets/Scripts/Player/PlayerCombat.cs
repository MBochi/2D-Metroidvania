using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;
    public HealthBar healthBar;

    public LayerMask enemyLayers;
    public Transform attackPoint;

    public int attackDamage = 40;
    public float attackRange = 1.1f;

    private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Attack();
                nextAttackTime = Time.time + 1f/attackRate;
            }
        }
    }

    private void Attack()
    {
        // Play an attack animation
        int attackAnimationNumber = UnityEngine.Random.Range(1,4);
        animator.SetBool("Attack" + attackAnimationNumber, true);

        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage the enemies
        foreach (Collider2D enemy in hitEnemies)
        {
            if(enemy.gameObject.tag == "Enemy")
            {
                enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
            }
            else if(enemy.gameObject.tag == "GoblinKing")
            {
                enemy.GetComponent<GoblinKingController>().TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if(currentHealth < 0) return; // temporary edit to disable player inputs
        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        healthBar.SetHealth(currentHealth);
        Debug.Log("Player took Damage: " + damage);
        if(currentHealth <= 0){
            Die();
            return;
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        healthBar.SetHealth(currentHealth);
        Debug.Log("Player healed:" + amount);
    }

    protected void Die()
    {
        animator.SetTrigger("Death");

        GetComponent<Collider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        this.enabled = false;
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
