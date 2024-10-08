using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XProjectileEnemy : Enemy
{
    // Start is called before the first frame update

    private Rigidbody2D rb;
    [SerializeField][Range(-1,1)] private float movementDirection_x = -1f;
    [SerializeField][Range(-1,1)] private float movementDirection_y = -1f;
   //[SerializeField] protected LayerMask m_WhatIsGround;
    //[SerializeField] private float movementSpeed = 1f;
    private float colliderRadius = .4f;
    private float collisionDelayTime = .1f;
    private bool canCollide = true;
    private bool canMove = true;

    [SerializeField] private float AttackCooldownTime = 9f;
    private float AttackTime = 1f;
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] int projectileDamage = 10;
    [SerializeField] float projectileSpeed = 8f;
    [SerializeField] int contactDamage = 20;

    private Vector3 RightCollisionOffset = new Vector3(.5f,0,0);
    private Vector3 LeftCollisionOffset = new Vector3(-.5f,0,0);
    private Vector3 TopCollisionOffset = new Vector3(0,.5f,0);
    private Vector3 BottomCollisionOffset = new Vector3(0,-.5f,0);
    void Start()
    {
        base.Init();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(AttackCooldown());
        despawnTime = .75f;
    }

    // Update is called once per frame
    public override void Update()
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
        if (canMove)
        {
            rb.velocity = new Vector2(movementDirection_x,movementDirection_y).normalized * movementSpeed;
        }
        else
        {
            rb.velocity = new Vector2(0,0);
        }
    }

    private bool RightCollision()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position + RightCollisionOffset, colliderRadius, m_WhatIsGround);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject && colliders[i].gameObject.tag != "Projectile")
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
            if (colliders[i].gameObject != gameObject && colliders[i].gameObject.tag != "Projectile")
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
            if (colliders[i].gameObject != gameObject && colliders[i].gameObject.tag != "Projectile")
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
            if (colliders[i].gameObject != gameObject && colliders[i].gameObject.tag != "Projectile")
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator ShootXProjectiles()
    {
        Debug.Log("Attack");
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(.2f);
        GameObject bullet = Instantiate(BulletPrefab, this.transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Setup(1,1,projectileDamage,projectileSpeed);
        bullet = Instantiate(BulletPrefab, this.transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Setup(-1,1,projectileDamage,projectileSpeed);
        bullet = Instantiate(BulletPrefab, this.transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Setup(1,-1,projectileDamage,projectileSpeed);
        bullet = Instantiate(BulletPrefab, this.transform.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Setup(-1,-1,projectileDamage,projectileSpeed);
        StartCoroutine(AttackCooldown());

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerCombat>().TakeDamage(contactDamage);
        }
    }

    protected override IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(AttackCooldownTime);

        canMove = false;
        StartCoroutine(ShootXProjectiles());
        StartCoroutine(AttackTimer());
    }

     private IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(AttackTime);
        canMove = true;
    }


    private IEnumerator collisionDelay()
    {
        yield return new WaitForSeconds(collisionDelayTime);
        canCollide = true;
    }


    void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine( this.transform.position - new Vector3(movementDirection_x, movementDirection_y, 1) * -2, this.transform.position);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position + RightCollisionOffset, colliderRadius);
        Gizmos.DrawWireSphere(this.transform.position + LeftCollisionOffset, colliderRadius);
        Gizmos.DrawWireSphere(this.transform.position + TopCollisionOffset, colliderRadius);
        Gizmos.DrawWireSphere(this.transform.position + BottomCollisionOffset, colliderRadius);
    }
}


