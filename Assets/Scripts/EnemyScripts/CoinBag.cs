using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBag : MonoBehaviour
{
    // Start is called before the first frame update
    private bool m_FacingRight = false;

    private Animator animator;
    private Rigidbody2D rb;
    private PlayerCombat playerCombat;

    private int damage = 25;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerCombat = GameObject.FindWithTag("Player").GetComponent<PlayerCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.velocity.x != 0)
        {
            if(rb.velocity.x > 0 && !m_FacingRight)
            {
                Flip();
            }
            else if(rb.velocity.x < 0 && m_FacingRight)
            {
                Flip();
            }
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "Projectile" && collision.gameObject.tag != "GoblinKing")
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            animator.SetTrigger("Hit");
            StartCoroutine(RemoveDelay());

            if(collision.gameObject.tag == "Player" && checkIfPlayerBlocksAttack(collision))
            {
                collision.gameObject.GetComponent<PlayerCombat>().TakeDamage(damage);
            }
        }
        
    }

    private bool checkIfPlayerBlocksAttack(Collision2D collider)
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

    private bool checkifEnemyBehindPlayer(Collision2D collider)
    {
        if (collider.transform.rotation.y < 0 && this.transform.localScale.x > 0 || 
            collider.transform.rotation.y == 0 && this.transform.localScale.x < 0) return true;

        else return false;     
    }

    public void setDamage(int newDamage)
    {
        damage = newDamage;
    }

    private void Flip()
	{
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        m_FacingRight = !m_FacingRight;
	}

    private IEnumerator RemoveDelay(){
        yield return new WaitForSeconds(.2f);
        Destroy(this.gameObject);
    }
}
