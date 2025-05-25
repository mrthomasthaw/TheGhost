using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraSetting
{

    [SerializeField] public Vector3 pivotLeftDefault;
    [SerializeField] public Vector3 pivotRightDefault;
    [SerializeField] public float horizontalInput;
    [SerializeField] public float verticalInput;

}
