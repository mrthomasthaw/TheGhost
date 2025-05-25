using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityHandler : MonoBehaviour
{
    Vector3 groundStandingPoint;

    Rigidbody body;

    public MovementSettingData movementSettingData;

    [SerializeField] private float groundCheckOffset;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float inAirTimer;
    [SerializeField] private float leapingForce;
    [SerializeField] private float gravity = -9.81f;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleGroundCheck();
    }

    private void FixedUpdate()
    {
        HandleFallingAndLanding();
    }

    private void HandleGroundCheck()
    {
        groundStandingPoint = transform.position;

        Vector3 origin = transform.position;
        origin.y += groundCheckOffset;
        Vector3 direction = -Vector3.up;
        RaycastHit hit;

        Debug.DrawRay(origin, direction, Color.red);
        if (Physics.SphereCast(origin, 0.2f, direction, out hit, 100f, groundLayer))
        {
            //Debug.Log("Grounded");
            groundStandingPoint.y = hit.point.y;
            isGrounded = true;
        }
        else
        {
            //Debug.Log("Not Grounded");

            isGrounded = false;
        }
    }

    private void HandleFallingAndLanding()
    {
        if (isGrounded)
        {
            inAirTimer = 0;
            if (CommonUtil.CalculateInputAmount(movementSettingData.vertical, movementSettingData.horizontal) > 0)
                transform.position = Vector3.Lerp(transform.position, groundStandingPoint, Time.deltaTime / 0.1f);
            else
                transform.position = groundStandingPoint;
        }
        else
        {
            inAirTimer += Time.deltaTime;
            body.AddForce(transform.forward * leapingForce);
            body.AddForce(-Vector3.up * gravity * inAirTimer);
        }
    }
}
