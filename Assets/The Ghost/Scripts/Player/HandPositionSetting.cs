using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandPositionSetting
{
    [Header("-Rifle-")]
    public Vector3 rifleIdlePos;
    public Vector3 rifleIdleEulerAngle;
    public Vector3 rifleAimPos;
    public Vector3 rifleAimEulerAngle;

    [Header("-Pistol-")]
    public Vector3 pistolIdlePos;
    public Vector3 pistolIdleEulerAngle;
    public Vector3 pistolAimPos;
    public Vector3 pistolAimEulerAngle;
}
