using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPutDownSMB : CustomStateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Weapon put down on state enter");
        animationFinishingInfo.HavePuttingDownWeaponFinished = true; 
    }
}
