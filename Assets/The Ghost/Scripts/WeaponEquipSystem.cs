using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquipSystem
{
    Animator animator;

    WeaponEquipSettingData weaponEquipSettingData;

    public WeaponEquipSystem(Animator animator, WeaponEquipSettingData weaponEquipSettingData)
    {
        this.animator = animator;
        this.weaponEquipSettingData = weaponEquipSettingData;
    }

    public void PlayWeaponChangeAnimation(string name)
    {
        if(weaponEquipSettingData.isMoving)
            animator.CrossFadeInFixedTime(name, 0.1f, 1);
        else
            animator.CrossFadeInFixedTime(name, 0.1f, 2);
    }


    public void EquipWeapon()
    {
        if (weaponEquipSettingData.weaponEquipState != WeaponEquipState.Disarmed)
            return;

        PlayWeaponChangeAnimation("Unholster");
    }

    public void Disarmed()
    {
        if (weaponEquipSettingData.weaponEquipState != WeaponEquipState.Armed)
            return;

        PlayWeaponChangeAnimation("Holster");
    }
}
