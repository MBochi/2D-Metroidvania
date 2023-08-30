using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update

    private Rigidbody2D rb;

    private float movementDirection_x = 1;
    private float movementDirection_y = 1;
    private int projectileDamage = 10;
    private float movementSpeed = 4f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(float x, float y, int damage, float speed)
    {
        movementDirection_x = x;
        movementDirection_y = y;
        projectileDamage = damage;
        movementSpeed = speed;
    }
    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(movementDirection_x,movementDirection_y).normalized * movementSpeed;
    }

        void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "Projectile" && collision.gameObject.tag != "GoblinKing")
        {
            if(collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerCombat>().TakeDamage(projectileDamage);
            }
            Destroy(this.gameObject);
        }
        
    }
}
