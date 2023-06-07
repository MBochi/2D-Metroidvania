using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBag : MonoBehaviour
{
    // Start is called before the first frame update
    private bool m_FacingRight = false;

    private Animator animator;
    private Rigidbody2D rb;

    private int damage = 25;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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
        if (collision.gameObject.tag != "Enemy")
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            animator.SetTrigger("Hit");
            StartCoroutine(RemoveDelay());

            if(collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerCombat>().TakeDamage(damage);
            }
        }
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
