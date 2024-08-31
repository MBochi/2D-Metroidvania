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

    [SerializeField] public int attackDamage = 40;
    [SerializeField] public float attackRange = 1.1f;
    [SerializeField] public float attackRate = 2f;
    [SerializeField] private float nextAttackTime = 0f;

    [SerializeField] private int maxHealth = 100;
    [SerializeField] public int currentHealth;
    [SerializeField] public bool isBlocking = false;

    private BonfireCheckPointSaver bonfireCheckPointSaver;
    private CameraControlTrigger cameraControlTrigger;
    private GoblinBossRaum goblinBossRaum;
    private GoblinKingController goblinKingController;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        bonfireCheckPointSaver = GameObject.FindGameObjectWithTag("Player").GetComponent<BonfireCheckPointSaver>();
        cameraControlTrigger = GameObject.Find("BossDoorCAM").GetComponent<CameraControlTrigger>();
        goblinBossRaum = GameObject.Find("GoblinBossRaum").GetComponent<GoblinBossRaum>();
        goblinKingController = GameObject.FindGameObjectWithTag("GoblinKing").GetComponent<GoblinKingController>();
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

        Block();
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

    public void Block()
    {

        if (Input.GetButtonDown("Block") || (animator.GetBool("IdleBlock") && animator.GetBool("Block")))
            {
                gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                animator.SetBool("Block", true);
                animator.SetBool("IdleBlock", true);
                isBlocking = true;
            }

        if(Input.GetButtonUp("Block"))
            {
                gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
                animator.SetBool("Block", false);
                animator.SetBool("IdleBlock", false);
                isBlocking = false;
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
        Debug.Log("Player healed:" + amount);

        if(currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        healthBar.SetHealth(currentHealth);
    }
    
    protected void Die()
    {
        animator.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        this.enabled = false;
        gameObject.GetComponent<PlayerMovement>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        StartCoroutine(DeathScreen());
    }

    IEnumerator DeathScreen()
    {
        yield return new WaitForSeconds(5);
        goblinBossRaum.OpenLeftDoor();
        goblinKingController.StopBossFight();
        ResetPlayer();
    }

    private void ResetPlayer()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        bonfireCheckPointSaver.WarpPlayerToLastBonfire();

        GetComponent<Collider2D>().enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
        this.enabled = true;
        gameObject.GetComponent<PlayerMovement>().enabled = true;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        animator.SetTrigger("IdleActive");
        cameraControlTrigger.SwapCameraOnDeath();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
