using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimateInputController : MonoBehaviour
{
    Animator animator;

    MovementAnimateSystem movementAnimateSystem;

    public MovementAnimatorDataSetting movementAnimatorDataSetting;

    private void Start()
    {
        animator = GetComponent<Animator>();
        movementAnimateSystem = new MovementAnimateSystem(animator, movementAnimatorDataSetting);
    }

    private void Update()
    {
        ReadInputs();
        movementAnimateSystem.Tick();
    }

    private void OnAnimatorMove()
    {
        movementAnimatorDataSetting.animatorDeltaPosition = animator.deltaPosition;
    }

    private void ReadInputs()
    {
        movementAnimatorDataSetting.verticalInput = Input.GetAxis("Vertical");
        movementAnimatorDataSetting.horizontalInput = Input.GetAxis("Horizontal");
    }

}
