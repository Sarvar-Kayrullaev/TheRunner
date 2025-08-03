using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimator : StateMachineBehaviour
{
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Weapon weapon = animator.gameObject.GetComponent<Weapon>();
        if (stateInfo.IsName("ReloadEmpty"))
        {
            weapon.isReloading = true;
        }
        else if (stateInfo.IsName("Reload"))
        {
            weapon.isReloading = true;
        }
        else
        {
            weapon.isReloading = false;
        }
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("ReloadEmpty") || stateInfo.IsName("Reload"))
        {
            Weapon weapon = animator.gameObject.GetComponent<Weapon>();
            for (int i = 0; i < weapon.magazineSize; i++)
            {
                if (weapon.GetAllAmmo() <= 0) break;
                if (weapon.currentAmmo >= weapon.magazineSize) break;
                weapon.currentAmmo++;
                weapon.SetAllAmmo(weapon.GetAllAmmo()-1);

                weapon.holster.ammoBagText.text = "" + weapon.GetAllAmmo();
                weapon.holster.RebuildBullet(weapon.currentAmmo, weapon.magazineSize);
            }
            weapon.isReloading = false;
        }
    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    //override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    //override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    //{
    //    
    //}
}
