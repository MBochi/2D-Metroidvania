using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{

    protected GameObject playerObj;
    protected float collectRange = 1f;
    protected bool canBeCollected = false;
    protected float pickupCooldown = 3f;
    protected float despawnTime = 20f;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        playerObj = GameObject.FindWithTag("Player");
        StartCoroutine(collectionCooldown());
    }

    // Update is called once per frame
    public virtual void Update()
    {
        float dist_to_player = Vector2.Distance(playerObj.transform.position, transform.position);
        if((dist_to_player < collectRange) && canBeCollected)
        {
            PlayerCollected();
        }
    }

    protected virtual void PlayerCollected()
    {
        Destroy(this.gameObject);
    }

    protected virtual IEnumerator collectionCooldown(){
        yield return new WaitForSeconds(pickupCooldown);
        canBeCollected = true;
        StartCoroutine(despawnTimer());
    }

    protected virtual IEnumerator despawnTimer(){
        yield return new WaitForSeconds(despawnTime);
        Destroy(this.gameObject);
    }
}
