using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Rigidbody2D rb;
    public Animator animator;
    public float runSpeed = 40f;
    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if(Input.GetButtonDown("Jump") && !crouch)
        {
            jump = true;
        }

        if (Input.GetButtonDown("Crouch") && controller.m_Grounded)
            {
                gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                crouch = true;
            }

        if(Input.GetButtonUp("Crouch"))
            {
                gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
                crouch = false;
            }

        MovementAnimation();
    }

    private void FixedUpdate() 
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
        animator.SetBool("Jump", false);
    }

    private void MovementAnimation(){
        if(Input.GetAxisRaw("Horizontal") != 0){
            animator.SetInteger("AnimState", 1);
        } 
        else{
            animator.SetInteger("AnimState", 0);
        }

        if(rb.velocity.y < 0){
            animator.SetFloat("AirSpeedY", -1);
        } 
        else{
            animator.SetFloat("AirSpeedY", 0);
        }

        if(jump && !crouch)
        {
            animator.SetBool("Jump", true);
        }

        if(crouch)
        {
            animator.SetBool("Crouch", true);
        }

        if(!crouch)
        {
            animator.SetBool("Crouch", false);
        }
    }
}
