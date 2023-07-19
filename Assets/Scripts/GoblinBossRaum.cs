using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBossRaum : MonoBehaviour
{

    
    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GoblinKingController goblinBoss;
    private Vector3 startingPos;
    private float doorSpeed = 4f;
    private bool leftDoorIsOpening = false;
    private bool leftDoorIsClosing = false;
    private bool bossFightStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = doorLeft.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(leftDoorIsClosing)
        {
            doorLeft.transform.position += Vector3.down * Time.deltaTime * doorSpeed;
        }

        if(leftDoorIsOpening)
        {
            doorLeft.transform.position = Vector3.MoveTowards(doorLeft.transform.position,startingPos,doorSpeed * Time.deltaTime);
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

    public void OpenLeftDoor()
    {
        leftDoorIsOpening = !leftDoorIsOpening;
        bossFightStarted = false;
        StartCoroutine(LeftDoorOpeningCoolDown());
    }

    private IEnumerator LeftDoorOpeningCoolDown()
    {
        yield return new WaitForSeconds(1f);
        leftDoorIsOpening = !leftDoorIsOpening;
    }

    private void CloseLeftDoor()
    {
        leftDoorIsClosing = !leftDoorIsClosing;
        StartCoroutine(LeftDoorClosingCoolDown());
    }

    private IEnumerator LeftDoorClosingCoolDown(){
        yield return new WaitForSeconds(1f);
        leftDoorIsClosing = !leftDoorIsClosing;
    }

    private IEnumerator BossFightStartingCoolDown(){
        yield return new WaitForSeconds(2.5f);
        goblinBoss.StartBossFight();
    }
}
