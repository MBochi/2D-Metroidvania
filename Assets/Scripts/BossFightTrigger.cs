using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightTrigger : MonoBehaviour
{
    [SerializeField] GoblinBossRaum BossRoom;
    private GoblinKingController goblinKingController;
    // Start is called before the first frame update
    void Start()
    {
        goblinKingController = GameObject.FindGameObjectWithTag("GoblinKing").GetComponent<GoblinKingController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Player")
        {
            goblinKingController.ResetBossPosition();
            BossRoom.StartBossFight();
        }
    }
}
