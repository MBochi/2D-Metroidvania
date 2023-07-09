using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireCheckPointSaver : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsCheckpoint;

    public Vector2 BonfireLocation {get; private set; } = Vector2.zero;

    private PlayerCombat playerCombat;

    private void Start() 
    {
        BonfireLocation = transform.position;
    }

    private void OnTriggerStay2D(Collider2D collison) 
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if((whatIsCheckpoint.value & (1 << collison.gameObject.layer)) > 0)
            {
                BonfireLocation = new Vector2(collison.bounds.center.x, collison.bounds.min.y);
                Debug.Log("BonfireCheckPoint set at " + BonfireLocation);
            }
        }
    }

    public void WarpPlayerToLastBonfire()
    {
        transform.position = BonfireLocation;
    }
}
