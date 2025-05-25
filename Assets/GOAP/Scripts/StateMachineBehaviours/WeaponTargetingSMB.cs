using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MrThaw;
using MrThaw.Goap.AIMemory.AIInfo;

public class WeaponTargetingSMB : CustomSMB
{

    private WeaponPositionControl weaponPositionControl;

    public override void SetUp(Animator animator, AIBlackBoard blackBoard)
    {
        base.SetUp(animator, blackBoard);
        weaponPositionControl = animator.GetComponent<WeaponPositionControl>();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AIBBDSelectedPrimaryThreat threat = blackBoard.GetBBData<AIBBDSelectedPrimaryThreat>();

        if(threat != null)
        {
            weaponPositionControl.HandleWeaponAim(true);
            weaponPositionControl.IKControl.SetLookObj(threat.ThreatT);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        weaponPositionControl.HandleWeaponAim(false);
        weaponPositionControl.IKControl.SetLookObj(null);
    }
}
