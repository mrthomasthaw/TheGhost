using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterWeaponSMB : CustomSMB
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (stateInfo.shortNameHash == h_Empty)
        //{
        //    weaponLayerTarget = 0;
        //    rightHandTarget = 0;
        //    leftHandTarget = 0;
        //}
        //else if (stateInfo.shortNameHash == h_PullOutWeapon)
        //{
        //    weaponLayerTarget = 1;

        //    animator.SetFloat(cap_WeaponStyle, CGunAtt.gunStyle);
        //}
        //else if (stateInfo.shortNameHash == h_IdleWithWeapon)
        //{
        //    shooter.SsLookAt.OnActionExit(shooter.ai); // Disable look at manually
        //    rightHandTarget = 0;
        //    leftHandTarget = 1;
        //}
        //else if (stateInfo.shortNameHash == h_AimingWithWeapon)
        //{
        //    rightHandTarget = 1;
        //    leftHandTarget = 1;
        //}
        //else if (stateInfo.shortNameHash == h_HolsterWeapon)
        //{
        //}
        //else if (stateInfo.shortNameHash == h_ReloadWeapon)
        //{
        //    shooter.SsLookAt.OnActionExit(shooter.ai); // Disable look at manually
        //    rightHandTarget = 0;

        //    leftHandTarget = 0;

        //    if (CGunAtt != null && CGunAtt.curClipObject && CGunAtt.curClipPrefab)
        //    {
        //        CGunAtt.curClipObject.SetParent(null);
        //        if (CGunAtt.curClipObject.GetComponent<Rigidbody>())
        //        {
        //            CGunAtt.curClipObject.GetComponent<Rigidbody>().AddForce(transform.forward * 1f);
        //            CGunAtt.curClipObject.GetComponent<Rigidbody>().isKinematic = false;
        //        }
        //        if (CGunAtt.curClipObject.GetComponent<Collider>())
        //        {
        //            CGunAtt.curClipObject.GetComponent<Collider>().enabled = true;
        //            CGunAtt.curClipObject.GetComponent<Collider>().isTrigger = false;
        //        }
        //        if (CGunAtt.curClipObject.GetComponent<Destroy>())
        //            CGunAtt.curClipObject.GetComponent<Destroy>().enabled = true;
        //    }
        //}
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (stateInfo.shortNameHash == h_PullOutWeapon)
        //{
        //    UpdateWeights();
        //}
        //else if (stateInfo.shortNameHash == h_IdleWithWeapon)
        //{
        //    UpdateWeights();
        //    HandleWeaponHandsUpdate();
        //}
        //else if (stateInfo.shortNameHash == h_AimingWithWeapon)
        //{
        //    UpdateWeights();
        //    HandleWeaponHandsUpdate();
        //    if (animator.GetNextAnimatorStateInfo(1).shortNameHash != h_IdleWithWeapon && !animator.IsInTransition(1))
        //    {
        //        SetLookAtAndTurn(false);
        //        FireWithTimer();
        //    }
        //}
        //else if (stateInfo.shortNameHash == h_HolsterWeapon)
        //{
        //    UpdateWeights();
        //}
        //else if (stateInfo.shortNameHash == h_ReloadWeapon)
        //{
        //    UpdateWeights();
        //}
    }

    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (stateInfo.shortNameHash == h_IdleWithWeapon)
        //{
        //    HandleWeaponHandsOnAnimatorIK();
        //}
        //else if (stateInfo.shortNameHash == h_AimingWithWeapon)
        //{
        //    HandleWeaponHandsOnAnimatorIK();
        //}
        //else if (stateInfo.shortNameHash == h_HolsterWeapon)
        //{
        //}
        //else if (stateInfo.shortNameHash == h_ReloadWeapon)
        //{
        //    HandleWeaponHandsOnAnimatorIK();
        //}
    }
}
