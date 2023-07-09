using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinBossRaum : MonoBehaviour
{

    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GoblinKingController goblinBoss;
    private float doorSpeed = 4f;
    private bool leftDoorIsClosing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("k")) // replace with player entering room
        {
            StartBossFight();
        }

        if(leftDoorIsClosing)
        {
            doorLeft.transform.position += Vector3.down * Time.deltaTime * doorSpeed;
        }
    }

    private void StartBossFight()
    {
        goblinBoss.StartBossFight();
        CloseLeftDoor();
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
}
