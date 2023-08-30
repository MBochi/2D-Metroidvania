using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XProjectileEnemy : MonoBehaviour
{
    // Start is called before the first frame update

    private Rigidbody2D rb;
    private float movementDirection_x = -1f;
    private float movementDirection_y = -1f;
    [SerializeField] protected LayerMask m_WhatIsGround;
    [SerializeField] private float movementSpeed = 1f;
    private float colliderRadius = .4f;
    private float collisionDelayTime = .1f;
    private bool canCollide = true;

    private Vector3 RightCollisionOffset = new Vector3(.5f,0,0);
    private Vector3 LeftCollisionOffset = new Vector3(-.5f,0,0);
    private Vector3 TopCollisionOffset = new Vector3(0,.5f,0);
    private Vector3 BottomCollisionOffset = new Vector3(0,-.5f,0);
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate((movementDirection.transform.position - this.transform.position).normalized * movementSpeed * Time.deltaTime);
        if(canCollide)
        {
            if(RightCollision() || LeftCollision())
            {
                movementDirection_x *= -1;
                canCollide = false;
                StartCoroutine(collisionDelay());
            }
            else if(TopCollision() || BottomCollision())
            {
                movementDirection_y *= -1;
                canCollide = false;
                StartCoroutine(collisionDelay());
            }
        }
        rb.velocity = new Vector2(movementDirection_x,movementDirection_y).normalized * movementSpeed;
    }

    private bool RightCollision()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position + RightCollisionOffset, colliderRadius, m_WhatIsGround);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }

    private bool LeftCollision()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position + LeftCollisionOffset, colliderRadius, m_WhatIsGround);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }

    private bool BottomCollision()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position + BottomCollisionOffset, colliderRadius, m_WhatIsGround);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }

    private bool TopCollision()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position + TopCollisionOffset, colliderRadius, m_WhatIsGround);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }






    private IEnumerator collisionDelay()
    {
        yield return new WaitForSeconds(collisionDelayTime);
        canCollide = true;
    }
        

    void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine( this.transform.position - new Vector3(movementDirection_x, movementDirection_y, 1) * -1, this.transform.position);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position + RightCollisionOffset, colliderRadius);
        Gizmos.DrawWireSphere(this.transform.position + LeftCollisionOffset, colliderRadius);
        Gizmos.DrawWireSphere(this.transform.position + TopCollisionOffset, colliderRadius);
        Gizmos.DrawWireSphere(this.transform.position + BottomCollisionOffset, colliderRadius);
    }
}


