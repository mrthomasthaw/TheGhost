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
    private AIBBDSelectedPrimaryThreat threat;
    private AIBBDSMBFireWeapon bbBFireWeapon;
    private float fireTimer = 0;
    private float pauseTimer = 0;
    private float fireTimerMax;
    private float pauseTimerMax;


    public override void SetUp(Animator animator, AIBlackBoard blackBoard)
    {
        base.SetUp(animator, blackBoard);
        weaponPositionControl = animator.GetComponent<WeaponPositionControl>();
        weaponInventory = animator.GetComponent<AIController>().WeaponInventory;

        fireTimerMax = UnityEngine.Random.Range(1.6f, 4f);
        pauseTimerMax = UnityEngine.Random.Range(0.6f, 2f);
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        threat = blackBoard.GetOneBBData<AIBBDSelectedPrimaryThreat>(EnumType.AIBlackBoardKey.SelectedPrimaryThreat);

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
        blackBoard.RemoveBBData(bbBFireWeapon);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(threat != null)
        {
            if (bbBFireWeapon != null && bbBFireWeapon.FireWeapon)
            {
                if (fireTimer < fireTimerMax)
                {
                    fireTimer += Time.deltaTime;
                    FireWeapon(true);
                }
                else if (pauseTimer < pauseTimerMax)
                {
                    pauseTimer += Time.deltaTime;
                    FireWeapon(false);
                }
                else
                {
                    fireTimer = 0;
                    pauseTimer = 0;

                    fireTimerMax = UnityEngine.Random.Range(1.6f, 4f);
                    pauseTimerMax = UnityEngine.Random.Range(0.6f, 2f);
                }
            }
            else
            {
                bbBFireWeapon = blackBoard.GetOneBBData<AIBBDSMBFireWeapon>(EnumType.AIBlackBoardKey.FireWeapon);
                FireWeapon(false);
            }
        }
        else
        {
            FireWeapon(false);
        }
    }

    public void FireWeapon(bool shoot)
    {
        weaponInventory.CurrentWeapon.Shoot(shoot);
    }
}
