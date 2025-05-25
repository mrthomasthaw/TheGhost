using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovementSettingData
{
    [SerializeField]
    public CharacterMovementState movementState;
    public float normalMoveSpeed;
    public bool sprinting;
    public float runSpeed;
    public float vertical;
    public float horizontal;
    public float moveSpeedWhileAiming;
    public float turnSpeed;
    public Vector3 moveDirection;
    public Vector3 turnDirection;
}
