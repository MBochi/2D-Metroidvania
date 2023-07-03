using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] protected int damage = 1;
    private SafeGroundSaver safeGroundSaver;
    private SafeGroundCheckpointSaver safeGroundCheckpointSaver;

    public void Start()
    {
        safeGroundSaver = GameObject.FindGameObjectWithTag("Player").GetComponent<SafeGroundSaver>();
        safeGroundCheckpointSaver = GameObject.FindGameObjectWithTag("Player").GetComponent<SafeGroundCheckpointSaver>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        // check if the collided object is in the "Player" Layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerCombat player = collision.GetComponent<PlayerCombat>();
            player.TakeDamage(damage);

            safeGroundSaver.WarpPlayerToSafeGround();
            safeGroundCheckpointSaver.WarpPlayerToSafeGround();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(damage);      
        }    
     }
}
