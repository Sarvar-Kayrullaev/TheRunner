using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandActionController : MonoBehaviour
{
    Animator animator;
    [HideInInspector] public WeaponHolster holster;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("ClimbEnd"))
            {
                Invoke("ResetHolster",0.1f);
                
            }
        }
    }
    void ResetHolster()
    {
        holster.RedrawWeapon();
        Destroy(gameObject, 0.3f);
    }
}
