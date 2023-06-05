using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] protected int damage = 1;
    
    public void Start(){}

    private void OnTriggerStay2D(Collider2D collision) {

        // check if the collided object is in the "Player" Layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerCombat player = collision.GetComponent<PlayerCombat>();
            player.TakeDamage(damage);
        }
        else
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(damage);
        }
        
     }
}
