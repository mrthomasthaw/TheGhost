using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MrThaw;
using MrThaw.Goap.AIMemory.AIInfo;
using System;

public class WeaponTargetingSMB : CustomSMB
{

    private WeaponPositionControl weaponPositionControl;
    private WeaponInventory weaponInventory;


    public override void SetUp(Animator animator, AIBlackBoard blackBoard)
    {
        base.SetUp(animator, blackBoard);
        weaponPositionControl = animator.GetComponent<WeaponPositionControl>();
        weaponInventory = animator.GetComponent<AIController>().WeaponInventory;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AIBBDSelectedPrimaryThreat threat = blackBoard.GetBBData<AIBBDSelectedPrimaryThreat>();

        if(threat != null)
        {
            weaponPositionControl.HandleWeaponAim(true);
            Debug.Log("Threat T : " + threat.ThreatT);
            weaponPositionControl.IKControl.SetLookObj(threat.ThreatT);
            weaponPositionControl.IKControl.SetAimTarget(threat.ThreatT);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        weaponPositionControl.HandleWeaponAim(false);
        weaponPositionControl.IKControl.SetLookObj(null);
    }

    public void FireWeapon()
    {
        weaponInventory.CurrentWeapon.Shoot(true);
    }
}
