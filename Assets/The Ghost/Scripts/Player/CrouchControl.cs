using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchControl : MonoBehaviour
{
    InputControl inputControl;
    AnimStates animStates;
    Animator animator;
    [SerializeField] private bool crouch;
    
    float inputMagnitude;
    float h, v;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        inputControl = GetComponent<InputControl>();
        animStates = GetComponent<AnimStates>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Crouch"))
        {
            crouch = !crouch;
            if (crouch)
            {
                animator.CrossFadeInFixedTime("Rifle_CrouchLoop", 0.4f);
            }
            else
            {
                animator.CrossFadeInFixedTime("Locomotion", 0.4f);
            }

        }

        if(crouch)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            inputMagnitude = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));
            animator.SetFloat("InputMagnitude", inputMagnitude, 0.1f, Time.deltaTime);
            animator.SetFloat("Vertical", v, 0.08f, Time.deltaTime);
            animator.SetFloat("Horizontal", h, 0.08f, Time.deltaTime);
        }

    }
}
