using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBanditEnemy : Enemy
{

    void Update()
    {
        dist_to_player = Vector2.Distance(playerObj.transform.position, transform.position);
        Move();
    }

    void FixedUpdate()
    {
        if(x_direction_to_player > 0 && m_FacingRight)
        {
            Flip();
        }
        else if(x_direction_to_player < 0 && !m_FacingRight)
        {
            Flip();
        }
        
    }

    public override void Move() // Move towards player if player is close eough
    {
        if(dist_to_player < aggroRange)
        {
            x_direction_to_player = playerObj.transform.position.x - transform.position.x;
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
            animator.SetInteger("AnimState", 1);
        }
    }
}