using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeGroundCheckpointSaver : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsCheckpoint;

    public Vector2 SafeGroundLocation {get; private set; } = Vector2.zero;


    private void Start() 
    {
        SafeGroundLocation = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collison) 
    {
        if((whatIsCheckpoint.value & (1 << collison.gameObject.layer)) > 0)
        {
            SafeGroundLocation = new Vector2(collison.bounds.center.x, collison.bounds.min.y);
        }
    }

    public void WarpPlayerToSafeGround()
    {
        transform.position = SafeGroundLocation;
    }
}
