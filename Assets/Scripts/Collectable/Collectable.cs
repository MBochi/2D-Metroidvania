using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{

    protected GameObject playerObj;
    protected float collectRange = 2f;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        playerObj = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    public virtual void Update()
    {
        float dist_to_player = Vector2.Distance(playerObj.transform.position, transform.position);
        if(dist_to_player < collectRange)
        {
            PlayerCollected();
        }
    }

    protected virtual void PlayerCollected()
    {
        Destroy(this.gameObject);
    }
}
