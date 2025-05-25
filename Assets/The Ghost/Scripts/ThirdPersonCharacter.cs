using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacter : MonoBehaviour
{
    public MovementInputController movementInputController;
    public MovementAnimateInputController movementAnimateInputController;

    private WeaponEquipInputController weaponEquipInputController;
    private WeaponChangeAnimationEvent weaponChangeAnimationEvent;
    private MovementSettingData movementSettingData;
    private MovementAnimatorDataSetting movementAnimatorDataSetting;
    private GravityHandler gravityHandler;

    // Start is called before the first frame update
    void Start()
    {
        movementInputController = GetComponent<MovementInputController>();
        movementAnimateInputController = GetComponent<MovementAnimateInputController>();
        gravityHandler = GetComponent<GravityHandler>();
        weaponEquipInputController = GetComponent<WeaponEquipInputController>();
        weaponChangeAnimationEvent = GetComponent<WeaponChangeAnimationEvent>();
        movementSettingData = movementInputController.movementSettingData;
        movementAnimatorDataSetting = movementAnimateInputController.movementAnimatorDataSetting;
        gravityHandler.movementSettingData = movementSettingData;
        weaponChangeAnimationEvent.weaponEquipSettingData = weaponEquipInputController.weaponEquipSettingData;
    }

    private void Update()
    {
        movementAnimatorDataSetting.inputAngle = movementInputController.movementSystem.CaculateArcTangent();
    }

    private void FixedUpdate()
    {
        movementInputController.ReadAnimatorDeltaPosition(movementAnimatorDataSetting.animatorDeltaPosition);
        movementInputController.FixedTick();
    }

    private void LateUpdate()
    {
        //movementInputController.ReadAnimatorDeltaPosition(movementAnimatorDataSetting.animatorDeltaPosition);
    }

}
