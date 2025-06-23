using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MrThaw;

public class AnimateMovement : MonoBehaviour
{
    [SerializeField] ShoulderSetting shoulder;
    [SerializeField] CharacterAimState aimState;
    [SerializeField] private CrouchEventSO crouchEvent;
    [SerializeField] private DeathEventSO deathEvent;
    Animator animator;
    AimInputHelper aimInputHelper;

    public float H { get; private set; }
    public float V { get; private set; }
    private float speedMultiplier;
    private bool precisionAim;
    private bool inOtherState;
    
    [SerializeField]private bool death = false;

    private void OnEnable() 
    { 
        crouchEvent.OnEventRaised += OnOtherAction;
        deathEvent.OnEventRaised += OnDeath;
    }
    private void OnDisable() 
    { 
        crouchEvent.OnEventRaised -= OnOtherAction;
        deathEvent.OnEventRaised -= OnDeath;
    }

    // Start is called before the first frame update
    void Start()
    {

        animator = GetComponent<Animator>();
        aimInputHelper = aimInputHelper == null ? new AimInputHelper() : aimInputHelper; // The start method will called repeatedly due to disable/ enable script by other

        aimInputHelper.onAim += OnAim;
        aimInputHelper.onPutDownWeapon += OnPutDownWeapon;
    }

    // Update is called once per frame
    void Update()
    {
        if(death) return;

        aimInputHelper.HandleHipFireAndAimInputs(ref precisionAim, ref aimState, Time.deltaTime);

        AdaptMoveSpeedOnAimState();

        HandleMovementState();

        HandleShoulderSwitch();
    }

    private void HandleShoulderSwitch()
    {
        if (Input.GetButtonDown("ShoulderChange"))
        {
            shoulder = shoulder == ShoulderSetting.Right ? ShoulderSetting.Left : ShoulderSetting.Right;

            if (inOtherState)
                return;

            if (aimState == CharacterAimState.Idle)
            {
                string locomotionStateName = shoulder == ShoulderSetting.Right ? "Default Locomotion Right Shoulder" : "Default Locomotion Left Shoulder";
                animator.CrossFade(locomotionStateName, 0.5f);
            }
            else if (aimState == CharacterAimState.HipFire || aimState == CharacterAimState.PreciseAim)
            {
                string locomotionStateName = shoulder == ShoulderSetting.Right ? "Weapon Locomotion Right Shoulder" : "Weapon Locomotion Left Shoulder";
                animator.CrossFade(locomotionStateName, 0.5f);
            }
        }
    }

    private void HandleMovementState()
    {
        V = Input.GetAxis("Vertical") * speedMultiplier;
        H = Input.GetAxis("Horizontal") * speedMultiplier;
        animator.SetFloat("Vertical", V, 0.1f, Time.deltaTime);
        animator.SetFloat("Horizontal", H, 0.1f, Time.deltaTime);
    }

    private void AdaptMoveSpeedOnAimState()
    {
        switch(aimState)
        {
            case CharacterAimState.Idle:
                speedMultiplier = 2f;
                break;
            case CharacterAimState.HipFire:
                speedMultiplier = 2f;
                break;
            case CharacterAimState.PreciseAim:
                speedMultiplier = 1f;
                break;
        }
    }

    public void OnAim()
    {
        if (inOtherState) return;
        string locomotionStateName = shoulder == ShoulderSetting.Right ? "Weapon Locomotion Right Shoulder" : "Weapon Locomotion Left Shoulder";
        animator.CrossFade(locomotionStateName, 0.1f);
    }

    public void OnPutDownWeapon()
    {
        if (inOtherState) return;
        string locomotionStateName = shoulder == ShoulderSetting.Right ? "Default Locomotion Right Shoulder" : "Default Locomotion Left Shoulder";
        animator.CrossFade(locomotionStateName, 0.1f);
    }

    public void OnOtherAction(GameObject sender, bool state)
    {
        if (sender == this.gameObject)
            inOtherState = state;
    }

    public void OnDeath(GameObject sender)
    {
        if(sender == this.gameObject)
            death = true;
    }
}
