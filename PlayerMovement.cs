using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController;
    private float horizontalMovement = 0f;
    private bool jump = false;
    private bool crouch = false;
    private bool dash = false;

    // Get dynamic parameters
    private void Update() 
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        
        if(Input.GetButtonDown("Jump")) jump = true;
        if(Input.GetKeyDown(KeyCode.LeftShift) && !dash) dash = true;
        if(Input.GetKeyDown(KeyCode.J)) characterController.Shoot();

        if(Input.GetButton("Crouch") && !jump) crouch = true;
        if(Input.GetButtonUp("Crouch")) crouch = false;
    }

    public void SetJump(bool jump)
    {
        this.jump = jump;
    }

    public void SetDash(bool dash)
    {
        this.dash = dash;
    }

    //Player Movement
    private void FixedUpdate() {
        characterController.Move(horizontalMovement * Time.fixedDeltaTime, jump, crouch, dash);
    }
}
