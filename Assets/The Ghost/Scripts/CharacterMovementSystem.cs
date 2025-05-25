using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementSystem 
{
    private Transform transform;
    private Transform cameraTransform;
    private Rigidbody rigidbody;
    private MovementSettingData movementData;


    public CharacterMovementSystem(Transform transform, Transform cameraTransform, Rigidbody rigidbody, MovementSettingData movementData)
    {
        this.transform = transform;
        this.cameraTransform = cameraTransform;
        this.rigidbody = rigidbody;
        this.movementData = movementData;
    }


    public void HandleFreeDirectionMovement()
    {
        float moveAmount = CommonUtil.CalculateInputAmount(movementData.vertical, movementData.horizontal);
        Vector3 moveDirection = movementData.moveDirection * moveAmount;
        moveDirection.y = 0;
        rigidbody.velocity = moveDirection * movementData.normalMoveSpeed;

        if(moveAmount != 0)
        {
            movementData.turnDirection = cameraTransform.forward * movementData.vertical + cameraTransform.right * movementData.horizontal;
            movementData.turnDirection.Normalize();
            movementData.turnDirection.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(movementData.turnDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, movementData.turnSpeed * Time.deltaTime);
        }
    }

    public void HandleStrafeMovement()
    {
        Vector3 moveDirection;
        moveDirection = cameraTransform.forward * movementData.vertical;
        moveDirection += cameraTransform.right * movementData.horizontal;
        moveDirection.y = 0;
        moveDirection.Normalize();
        rigidbody.velocity = moveDirection * movementData.normalMoveSpeed * Time.deltaTime;

        float moveAmount = CommonUtil.CalculateInputAmount(movementData.vertical, movementData.horizontal);
        if (moveAmount != 0)
        {
            movementData.turnDirection = cameraTransform.forward;
            movementData.turnDirection.Normalize();
            movementData.turnDirection.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(movementData.turnDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, movementData.turnSpeed * Time.deltaTime);
        }
    }

    public float CaculateArcTangent()
    {
        Vector3 relativeDirection = transform.InverseTransformDirection(movementData.turnDirection);
        return Mathf.Atan2(relativeDirection.x, relativeDirection.z);
    }

}
