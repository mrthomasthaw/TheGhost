using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimateSystem
{
    Animator animator;
    MovementAnimatorDataSetting movementAnimatorDataSetting;

    public MovementAnimateSystem(Animator animator, MovementAnimatorDataSetting movementAnimatorDataSetting)
    {
        this.animator = animator;
        this.movementAnimatorDataSetting = movementAnimatorDataSetting;
    }

    public void HandleFreeDirectionMovement()
    {
        animator.SetFloat("Input Angle", movementAnimatorDataSetting.inputAngle, 0.08f, Time.deltaTime);
        animator.SetFloat("Input Magnitude", CommonUtil.CalculateInputAmount(movementAnimatorDataSetting.verticalInput, movementAnimatorDataSetting.horizontalInput), 0.08f, Time.deltaTime);
    }

    public void HandleStrafeMovement()
    {
        animator.SetFloat("Vertical", movementAnimatorDataSetting.verticalInput, 0.08f, Time.deltaTime);
        animator.SetFloat("Horizontal", movementAnimatorDataSetting.horizontalInput, 0.08f, Time.deltaTime);
    }

    public void Tick()
    {
        switch (movementAnimatorDataSetting.characterMovementState)
        {
            case CharacterMovementState.FreeDirection:
                HandleFreeDirectionMovement();
                break;
            case CharacterMovementState.Strafe:
                HandleStrafeMovement();
                break;
        }
    }
}
