using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CameraWeaponRecoilHandler : MonoBehaviour
{
    CameraInputController cameraInputController;
    MrThaw.GlobalVars globalVars;

    [SerializeField] Transform targetCharacterRootT;

    [SerializeField] private float currentRecoilXPos, currentRecoilYPos;
    [SerializeField] private float maxRecoilAmountX, maxRecoilAmountY;

    [SerializeField] private float recoilX, recoilY;

    [SerializeField] private bool precisionAim;

    [SerializeField] AnimationCurve recoilCurve;

    [SerializeField] Transform recoilTransform;

    [SerializeField] bool recoilFlag;

    [SerializeField] float recoilResetSpeed = 3;
    [SerializeField] float recoilMultiplier = 1;




    [SerializeField]float curveT;
    [SerializeField] float recoilTotal;
    [SerializeField] float recoilValue; 
    [SerializeField] float lerpSpeed;
    [SerializeField] float recoilSpeed;
    [SerializeField] float curveSpeed;

    [SerializeField] float recoilEffectTimer;
    [SerializeField] float recoilEffectResetSpeed;

    [SerializeField] bool crouch;

    private void Start()
    {
        cameraInputController = GetComponent<CameraInputController>();
        targetCharacterRootT = CommonUtil.FindRootTransform(cameraInputController.CameraFollowPoint);
        globalVars = targetCharacterRootT.GetComponent<MrThaw.GlobalVars>();
    }

    private void Update()
    {
        HandlePrecisionAim();

        AdjustRecoilXYValues();

        UpdateRecoilEffectTimer();


        //HandleRecoil();
    }

    private void HandlePrecisionAim()
    {
        if (Input.GetButton("PreciseAim"))
        {
            precisionAim = true;
        }
        else
        {
            precisionAim = false;
        }
    }

    private void UpdateRecoilEffectTimer()
    {
        if (recoilEffectTimer > 0)
        {
            if (cameraInputController.cameraSetting.verticalInput != 0 || cameraInputController.cameraSetting.horizontalInput != 0)
            {
                recoilEffectTimer = 0;
            }

            recoilEffectTimer -= Time.deltaTime * recoilEffectResetSpeed;
            cameraInputController.isInRecoilEffect = true;
        }
        else
        {
            cameraInputController.isInRecoilEffect = false;
            recoilEffectTimer = 0;
        }
    }

    private void AdjustRecoilXYValues()
    {
        recoilX = maxRecoilAmountX;
        recoilY = maxRecoilAmountY;

        if (precisionAim)
        {
            recoilX = maxRecoilAmountX * 0.55f;
            recoilY = maxRecoilAmountY * 0.55f;
        }
        else if (Mathf.Abs(globalVars.CharacterHorizontalSpeed) >= 1.2f || Mathf.Abs(globalVars.CharacterVerticalSpeed) >= 1.2f)
        {
            recoilX = maxRecoilAmountX * 1.4f;
            recoilY = maxRecoilAmountY * 1.4f;
        }


        if (Input.GetButtonDown("Crouch"))
        {
            HandleCrouch();
        }

        if (crouch)
        {
            recoilX = recoilX - (maxRecoilAmountX * 0.8f);
            recoilY = recoilY - (maxRecoilAmountY * 0.8f);
        }

    }

    void HandleCrouch()
    {
        crouch = !crouch;
    }

    public void CalculateRecoil()
    {
        recoilEffectTimer += 0.35f;

        currentRecoilXPos = ((UnityEngine.Random.value - 0.5f) / 2) * recoilX;
        currentRecoilYPos = ((UnityEngine.Random.value - 0.5f) / 2) * recoilY;
        cameraInputController.PivotAngle -= Mathf.Abs(currentRecoilYPos);
        cameraInputController.LookAngle -= currentRecoilXPos;

        Debug.Log("Recoil");
    }

    public void StartRecoil()
    {
        recoilEffectTimer += 0.3f;
        recoilFlag = true;
    }


    void HandleRecoil()
    {
        float delta = Time.deltaTime;
        Quaternion targetRot = Quaternion.identity;
        float resetSpeed = delta * recoilResetSpeed;


        Vector3 e = Vector3.zero;


        float kickAmount = 0;
        float hRecoil = recoilCurve.Evaluate(curveT) * recoilMultiplier;
        e.y = hRecoil;

        if (recoilFlag)
        {


            lerpSpeed = delta / recoilSpeed;
            curveT += delta / curveSpeed;

            if (curveT > 1)
            {
                curveT = 0;
            }

            recoilTotal += recoilValue;
            kickAmount = recoilTotal;

            e.x = -kickAmount;
            recoilFlag = false;

        }
        else
        {
            if (curveT > 1)
            {
                curveT = 1;
            }

            recoilTotal = recoilValue;
            //recoilTotal = 0;

            //recoilTotal = Mathf.Clamp(recoilTotal, 0f, recoilTotal);
            if (curveT > 0)
            {
                curveT -= delta / recoilResetSpeed;
            }

            hRecoil = 0;
        }


        targetRot = Quaternion.Euler(e);


        recoilTransform.localRotation = Quaternion.Slerp(recoilTransform.localRotation,
                            targetRot, lerpSpeed);

       
    } 

    /*
    void HandleRecoil()
    {
        float delta = Time.deltaTime;
        Quaternion targetRot = Quaternion.identity;
        float resetSpeed = delta * recoilResetSpeed;

        float hRecoil = recoilCurve.Evaluate(curveT) * recoilMultiplier;
        if (recoilFlag)
        {
            Vector3 e = Vector3.zero;
            e.x = -recoilValueSum;

            e.y = hRecoil;
            targetRot = Quaternion.Euler(e);

            lerpSpeed = delta / recoilSpeed;
            curveT += delta * curveSpeed;

            if (curveT > 1)
            {
                curveT = 0;
            }

            recoilValueSum += recoilValue;
            recoilFlag = false;

        }
        else
        {
            if (curveT > 1)
            {
                curveT = 1;
            }

            recoilValueSum = recoilValue;
            if (curveT > 0)
            {
                curveT -= delta / recoilResetSpeed;
            }

            hRecoil = 0;
        }


        recoilTransform.localRotation = Quaternion.Slerp(recoilTransform.localRotation,
            targetRot, lerpSpeed);

    } */

}
