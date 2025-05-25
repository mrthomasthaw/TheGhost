using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControl : MonoBehaviour
{
    Rigidbody rigidbody;
    Animator animator;

    [SerializeField]
    float moveSpeed;

    float horizontal;
    float vertical;

    Vector3 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        animator.applyRootMotion = false;
        animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
        rigidbody.isKinematic = false;
        rigidbody.mass = 1;
    }

    // Update is called once per frame
    void Update()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = moveDirection * moveSpeed;
    }

    private void OnAnimatorMove()
    {
        moveDirection = animator.deltaPosition;
    }
}
