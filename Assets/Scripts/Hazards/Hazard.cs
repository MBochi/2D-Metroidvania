using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] protected int damage = 1;
    private WarpToSafeGround warpToSafeGround;

    public void Start()
    {
        warpToSafeGround = GameObject.FindGameObjectWithTag("Player").GetComponent<WarpToSafeGround>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        // check if the collided object is in the "Player" Layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerCombat player = collision.GetComponent<PlayerCombat>();
            player.TakeDamage(damage);
            warpToSafeGround.WarpPlayerToSafeGround();
        }
        else
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(damage);      
        }    
     }
}
