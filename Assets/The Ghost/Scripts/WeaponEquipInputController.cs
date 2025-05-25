using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquipInputController : MonoBehaviour
{
    Animator animator;

    public WeaponEquipSettingData weaponEquipSettingData;

    WeaponEquipSystem weaponEquipSystem;

    private void Start()
    {
        animator = GetComponent<Animator>();
        weaponEquipSystem = new WeaponEquipSystem(animator, weaponEquipSettingData);
    }

    private void Update()
    {
        ReadInputs();

        switch(weaponEquipSettingData.weaponEquipState)
        {
            case WeaponEquipState.Disarmed:
                HandleDisarmedState();
                break;
            case WeaponEquipState.Armed:
                HandleArmedState();
                break;
        }
    }



    private void ReadInputs()
    {
        weaponEquipSettingData.verticalInput = animator.GetFloat("Vertical");
        weaponEquipSettingData.horizontalInput = animator.GetFloat("Horizontal");
        weaponEquipSettingData.isMoving = 
            CommonUtil.CalculateInputAmount(weaponEquipSettingData.verticalInput, weaponEquipSettingData.horizontalInput) > 0;
    }

    private void HandleDisarmedState()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            weaponEquipSystem.EquipWeapon();
        }

    }

    private void HandleArmedState()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            weaponEquipSystem.Disarmed();
        }
    }
}
