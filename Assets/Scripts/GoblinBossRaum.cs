using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBossRaum : MonoBehaviour
{

    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GoblinKingController goblinBoss;
    private float doorSpeed = 4f;
    private bool leftDoorIsClosing = false;
    private bool bossFightStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(leftDoorIsClosing)
        {
            doorLeft.transform.position += Vector3.down * Time.deltaTime * doorSpeed;
        }
    }

    public void StartBossFight()
    {
        if(!bossFightStarted)
        {
            bossFightStarted = true;
            CloseLeftDoor();
            StartCoroutine(BossFightStartingCoolDown());
        }
        
    }

    private void CloseLeftDoor()
    {
        leftDoorIsClosing = true;
        StartCoroutine(LeftDoorMovingCoolDown());
    }

    private IEnumerator LeftDoorMovingCoolDown(){
        yield return new WaitForSeconds(1f);
        leftDoorIsClosing = false;
    }

    private IEnumerator BossFightStartingCoolDown(){
        yield return new WaitForSeconds(2.5f);
        goblinBoss.StartBossFight();
    }
}
