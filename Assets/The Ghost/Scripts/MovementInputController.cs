using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementInputController : MonoBehaviour
{
    public CharacterMovementSystem movementSystem;
    private Rigidbody rigidbody;

    public ThirdPersonCharacter thirdPersonCharacter;
    public Transform cameraTransform;
    [SerializeField]
    public MovementSettingData movementSettingData;

    // Start is called before the first frame update
    void Start()
    {
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        rigidbody = GetComponent<Rigidbody>();
        movementSystem = new CharacterMovementSystem(transform, cameraTransform, rigidbody, movementSettingData);
    }

    // Update is called once per frame
    void Update()
    {
        ReadInputs();
    }


    void ReadInputs()
    {
        movementSettingData.vertical = Input.GetAxis("Vertical");
        movementSettingData.horizontal = Input.GetAxis("Horizontal");
    }

    public void FixedTick()
    {
        switch (movementSettingData.movementState)
        {
            case CharacterMovementState.FreeDirection:
                movementSystem.HandleFreeDirectionMovement();
                break;
            case CharacterMovementState.Strafe:
                movementSystem.HandleStrafeMovement();
                break;
        }
    }

    public void ReadAnimatorDeltaPosition(Vector3 newPosition)
    {
        switch (movementSettingData.movementState)
        {
            case CharacterMovementState.FreeDirection:
                movementSettingData.moveDirection = newPosition;
                break;
            case CharacterMovementState.Strafe:

                break;
        }
    }
}
