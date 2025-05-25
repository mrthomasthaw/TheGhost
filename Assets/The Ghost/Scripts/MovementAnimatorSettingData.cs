using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovementAnimatorDataSetting
{
    public float horizontalInput;
    public float verticalInput;
    public float inputAmount;
    public float inputAngle;
    public Vector3 animatorDeltaPosition;

    [SerializeField]
    public CharacterMovementState characterMovementState;
}
