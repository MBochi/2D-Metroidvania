using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireCheckPointSaver : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsCheckpoint;

    public Vector2 SavedBonfireLocation {get; private set; } = Vector2.zero;
    public Vector2 NewBonfireLocation {get; private set; } = Vector2.zero;
    [SerializeField] private bool isPlayerInBonfireRange;

    private PlayerCombat playerCombat;

    private void Start() 
    {
        SavedBonfireLocation = transform.position;
        isPlayerInBonfireRange = false;
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Return) && isPlayerInBonfireRange)
        {
            SavedBonfireLocation = NewBonfireLocation;
            Debug.Log("BonfireCheckPoint set at " + SavedBonfireLocation);
        }
    }

    private void OnTriggerEnter2D(Collider2D collison) 
    {
        if((whatIsCheckpoint.value & (1 << collison.gameObject.layer)) > 0)
            {
                isPlayerInBonfireRange = true;
                NewBonfireLocation = new Vector2(collison.bounds.center.x, collison.bounds.min.y);
            }
    }

    private void OnTriggerExit2D(Collider2D collison) 
    {
        isPlayerInBonfireRange = false;
    }

    public void WarpPlayerToLastBonfire()
    {
        transform.position = SavedBonfireLocation;
    }
}
