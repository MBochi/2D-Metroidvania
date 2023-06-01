using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTrigger : MonoBehaviour
{
    public Vector3 newCamPos, newPlayerPos;

    CamController camController;

    // Start is called before the first frame update
    void Start()
    {
        camController = Camera.main.GetComponent<CamController>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag == "Player")
        {
            camController.minPos += newCamPos;
            camController.maxPos += newCamPos;

            other.transform.position += newPlayerPos;
        }
    }
}
