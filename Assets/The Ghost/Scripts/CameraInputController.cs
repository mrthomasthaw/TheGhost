using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInputController : MonoBehaviour
{
    Transform targetTransform;
    Transform pivotTransform;
    Transform cameraHolderTransform;
    Camera camera;


    public Transform CamerHolderTransform
    {
        get
        {
            return cameraHolderTransform;
        }
    }

    public Transform CameraFollowPoint
    {
        get
        {
            return _cameraFollowPoint;
        }
    }

    [SerializeField] private Transform _cameraFollowPoint;
    [SerializeField] public CameraSetting cameraSetting;
    [SerializeField] Transform[] rayOrigins;
    [SerializeField] MrThaw.GlobalVars globalVars;


    public ShoulderSetting shoulder;

    public bool debugAim;
    public float aimOffset;
    public float chestLookOffset;
    public float followSpeed;
    public float LookAngle { get; set; }
    public float PivotAngle { get; set; }
    public Transform TargetTransform { get => TargetTransform1; set => TargetTransform1 = value; }
    public Transform TargetTransform1 { get => TargetTransform2; set => TargetTransform2 = value; }
    public Transform TargetTransform2 { get => targetTransform; set => targetTransform = value; }

    public float mouseLookSpeed;
    public float gamePadLookSpeed;
    public float collisionRadius;
    public float collisionOffset;
    public float minimumCollisionOffset;
    public LayerMask CollisionLayer;
    public Vector3 aimPos;
    public Vector3 chestLookPos;
    public Vector3 chestLookDir;

    public float defaultOffset;
    public float rotationSpeed;
    public bool lockCursor;

    public float mouseSensitivityX = 2.0f;
    public float mouseSensitivityY = 2.0f;
    public float smoothingFactor = 0.1f;

    public bool isInRecoilEffect;

    [SerializeField] private float defaultFieldOfView = 60f;
    [SerializeField] private float zoomFieldOfView = 35f;

    float currentVelocityX, currentVelocityY;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private Vector2 currentMouseLook;
    private Vector2 smoothV;

    private bool precisionAim;
    private bool crouch;

    private void Awake()
    {
        pivotTransform = transform.GetChild(0);


        TargetTransform = _cameraFollowPoint;
        camera = Camera.main;
        cameraHolderTransform = camera.transform.parent;

        globalVars = FindObjectOfType<MrThaw.GlobalVars>();
        //pivotTransform.localPosition = cameraSetting.pivotDefault;
    }


    //Note call this method in LateUpdate when using CharacterController.Move() in Update()
    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.Lerp(transform.position, TargetTransform.position, followSpeed * Time.deltaTime);
        transform.position = targetPosition;
    }


    //private void RotateCamera()
    //{
    //    float inputX = cameraSetting.horizontalInput * Mathf.Abs(cameraSetting.horizontalInput);
    //    float inputY = cameraSetting.verticalInput * Mathf.Abs(cameraSetting.verticalInput);

    //    float smoothX = Mathf.Clamp(inputX, -1.5f, 1.5f);
    //    float smoothY = Mathf.Clamp(inputY, -1.5f, 1.5f);


    //    if (debugAim)
    //    {
    //        transform.rotation = Quaternion.Euler(Vector3.forward);
    //        pivotTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
    //        return;
    //    }

    //    //PivotAngle -= smoothY * rotationSpeed * Time.deltaTime;
    //    PivotAngle = Mathf.Clamp(PivotAngle, -50, 45);

    //    Quaternion targetRotationY = Quaternion.Euler(PivotAngle, 0, 0);
    //    pivotTransform.localRotation = targetRotationY;

    //    LookAngle += smoothX * rotationSpeed * Time.deltaTime;
    //    Quaternion targetRotationX = Quaternion.Euler(0, LookAngle, 0);
    //    transform.rotation = targetRotationX;
    //}


    private void RotateCamera()
    {
        if(isInRecoilEffect && (cameraSetting.verticalInput == 0 || cameraSetting.horizontalInput == 0))
        {
            Debug.Log("Recoil In effect");
            PivotAngle += 2f * Time.deltaTime;
            PivotAngle = Mathf.Clamp(PivotAngle, -40, 45);
            //LookAngle += cameraSetting.horizontalInput * mouseSensitivityX * Time.deltaTime;

            smoothV.x = Mathf.SmoothDamp(smoothV.x, PivotAngle, ref currentVelocityX, smoothingFactor + 0.002f);
            smoothV.y = Mathf.SmoothDamp(smoothV.y, LookAngle, ref currentVelocityY, smoothingFactor + 0.002f);

        }
        else
        {
            PivotAngle -= cameraSetting.verticalInput * mouseSensitivityY * Time.deltaTime;
            PivotAngle = Mathf.Clamp(PivotAngle, -40, 45);
            LookAngle += cameraSetting.horizontalInput * mouseSensitivityX * Time.deltaTime;

            smoothV.x = Mathf.SmoothDamp(smoothV.x, PivotAngle, ref currentVelocityX, smoothingFactor);
            smoothV.y = Mathf.SmoothDamp(smoothV.y, LookAngle, ref currentVelocityY, smoothingFactor);
        }



        if (debugAim)
        {
            transform.rotation = Quaternion.Euler(Vector3.forward);
            pivotTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            return;
        }



        Quaternion targetRotationY = Quaternion.Euler(smoothV.x, 0, 0);
        pivotTransform.localRotation = targetRotationY;


        Quaternion targetRotationX = Quaternion.Euler(0, smoothV.y, 0);
        transform.rotation = targetRotationX;
    }




    private void CollisionControl()
    {
        float targetOffset = defaultOffset;
        Vector3 direction = cameraHolderTransform.position - pivotTransform.position;

        direction.Normalize();

        RaycastHit hit;

        bool rayHit = false;

        for(int x = 0; x < rayOrigins.Length; x++) // ray origin should be 2
        {
            Vector3 dir = -rayOrigins[x].forward;
            float dist = Vector3.Distance(rayOrigins[x].position, cameraHolderTransform.position);
            rayHit = Physics.Raycast(rayOrigins[x].position, dir, out hit, dist, CollisionLayer);
            if (rayHit)
            {
                //Debug.Log(hit.collider.name);
                float distance = Vector3.Distance(pivotTransform.position, hit.point);
                targetOffset = -(distance - collisionOffset); // set collisionOffset to 0 for smoother experience
                break;
            }
            Debug.DrawRay(rayOrigins[x].position, dir * dist);
        }


        //if(Physics.SphereCast(pivotTransform.position, collisionRadius, direction, out hit, Mathf.Abs(targetOffset), CollisionLayer))
        //{ 
        //    Debug.Log(hit.collider.name);
        //    float distance = Vector3.Distance(pivotTransform.position, hit.point);
        //    targetOffset = -(distance - collisionOffset);
        //}

        if(Mathf.Abs(targetOffset) < minimumCollisionOffset)
        {
            targetOffset -= minimumCollisionOffset;
        }

        if(rayHit)
        {
            float newOffsetZ = Mathf.Lerp(cameraHolderTransform.localPosition.z, targetOffset, 0.2f);
            cameraHolderTransform.localPosition = new Vector3(0, 0, newOffsetZ);
        }

    }

    private void LookTarget(Vector3 origin, Vector3 direction, ref Vector3 targetPos, float offset, Color color)
    {
        Vector3 dir = direction - origin;
        dir.Normalize();
        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(origin, direction * offset, color);
        targetPos = ray.GetPoint(offset);
    }


    private void LockCursor()
    {
        if (lockCursor)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
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

        float currentFieldOfView = camera.fieldOfView;
        if (precisionAim)
        {
            camera.fieldOfView = Mathf.Lerp(currentFieldOfView, zoomFieldOfView, Time.deltaTime * 8f);
        }
        else
        {
            camera.fieldOfView = Mathf.Lerp(currentFieldOfView, defaultFieldOfView, Time.deltaTime * 8f);
        }
    }

    public void HandleAllCameraMovement()
    {
        LockCursor();
        //RotateCamera();
        CollisionControl();
        LookTarget(cameraHolderTransform.position, cameraHolderTransform.forward, ref aimPos, aimOffset, Color.red);
        LookTarget(cameraHolderTransform.position, cameraHolderTransform.forward + chestLookDir, ref chestLookPos, chestLookOffset, Color.blue);
        UpdatePivotShoulderPosition();
        HandleCameraHeight();
    }

    private void HandleInputs()
    {
        cameraSetting.horizontalInput = Input.GetAxis("Mouse X");
        cameraSetting.verticalInput = Input.GetAxis("Mouse Y");

        if(Input.GetButtonDown("ShoulderChange"))
        {
            shoulder = shoulder == ShoulderSetting.Right ? ShoulderSetting.Left : ShoulderSetting.Right;
        }
    }

    void UpdatePivotShoulderPosition()
    {
        switch(shoulder)
        {
            case ShoulderSetting.Right:
                Vector3 rightPos = cameraSetting.pivotRightDefault;
                pivotTransform.localPosition = Vector3.Lerp(pivotTransform.localPosition, rightPos, Time.deltaTime * 4f);
                break;
            case ShoulderSetting.Left:
                Vector3 leftPos = cameraSetting.pivotLeftDefault;
                pivotTransform.localPosition = Vector3.Lerp(pivotTransform.localPosition, leftPos, Time.deltaTime * 4f);
                break;
        }
    }

    void HandleCameraHeight()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            crouch = !crouch;
        }

        Vector3 newPos = pivotTransform.localPosition;
        float currentCameraHeight = newPos.y;

        if (crouch)
        {
            currentCameraHeight -= 0.6f;
            newPos.y = currentCameraHeight;
        }


        pivotTransform.localPosition = Vector3.Lerp(pivotTransform.localPosition, newPos, Time.deltaTime * 4f);
    }


    private void Update()
    {
        HandleInputs();
        RotateCamera();
        HandlePrecisionAim();
    }


    private void LateUpdate()
    {
        HandleAllCameraMovement();
        FollowTarget();
    }

}
