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


    private float m_JumpForce = 300f;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        GroundCheck();  
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("j"))
        {
            Jump();
        }
        if (Input.GetKeyDown("k"))
        {
            Throw();
        }
        
        if(rb.velocity.y < 0)
        {
            animator.SetFloat("AirSpeedY", -1);
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

    private void Throw()
    {
        animator.SetTrigger("Throw");
        StartCoroutine(ThrowDelay());
    }

    private void Slam()
    {
         rb.AddForce(new Vector2(0f, -(m_JumpForce)/64));
    }

    private void Jump()
    {
        animator.SetTrigger("Jump");
        StartCoroutine(JumpDelay());
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
    }

    private IEnumerator ThrowDelay(){
        yield return new WaitForSeconds(1.25f);
        GameObject coinBag = Instantiate(coinBagPrefab, coinBagThrowPoint.transform.position, Quaternion.identity);
        if(m_FacingRight)
        {
            coinBag.GetComponent<Rigidbody2D>().AddForce(new Vector2(400f,100f));
        }
        else
        {
            coinBag.GetComponent<Rigidbody2D>().AddForce(new Vector2(-400f,100f));
        }
    }




    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(m_GroundCheck.position, k_GroundedRadius);
    }
}
