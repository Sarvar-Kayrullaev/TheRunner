using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerRoot;

public class Hand : MonoBehaviour
{
    Animator animator;
    public Player player;
    bool grounded = true;
    bool flyStart = false;
    bool isFlying = false;
    float doItTime;
    bool climbed = false;

    AnimationClip previousClip;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        bool isGrounded = Utility.IsGrounded(player);

        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetInteger("Movement", 3);
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                animator.SetInteger("Movement", 1);
            }
            else
            {
                animator.SetInteger("Movement", 2);
            }
        }
        if (isGrounded)
        {
            isFlying = false;
            flyStart = false;
        }else
        {   //flying
            if(isFlying == false)
            {
                flyStart = true;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Space) || flyStart && !player.movement.isClimbing)
        {
            if (flyStart)
            {
                isFlying = true;
                animator.Play("JumpUp");
                grounded = false;
                animator.SetBool("isGrounded", false);
            }
            else
            {
                animator.Play("JumpUp");
                grounded = false;
                animator.SetBool("isGrounded", false);
                doItTime = Time.time;
            }
        }
        else
        {
            float doIt = Time.time - doItTime;
            if (doIt >= 0.2f && isGrounded && !player.movement.isClimbing)
            {
                if (!grounded )
                {
                    JumpDownSound();
                    AnimatorClipInfo[] currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
                    
                    grounded = true;
                    animator.SetBool("isGrounded", true);
                }
            }
        }
        if(player.movement.isClimbing )
        {
            if (!climbed)
            {
                player.audio.PlayOneShot(player.climbingSound[Random.Range(0, player.climbingSound.Length - 1)]);
                animator.Play("Climb2_Start");
                climbed = true;
            }
        }
        else
        {
            climbed = false;
        }
    }

    void JumpDownSound()
    {
        if(Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit, 3))
        {
            switch (hit.collider.tag)
            {
                case "Footsteps/Ground":
                    player.audio.PlayOneShot(player.jumpDownSound[Random.Range(0, player.jumpDownSound.Length - 1)]);
                    break;
                case "Footsteps/Wood":
                    player.audio.PlayOneShot(player.woodJumpDownSound[Random.Range(0, player.woodJumpDownSound.Length - 1)]);
                    break;
            }
        }
    }
}
