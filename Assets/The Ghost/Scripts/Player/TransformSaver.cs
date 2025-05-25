using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformSaver : MonoBehaviour
{
    public Transform targetTransform; // The Transform reference to copy
    public Vector3 savedPosition;
    public Vector3 savedRotation;

    // Call this method to apply saved position and rotation
    public void ApplySavedTransform()
    {
        if (targetTransform != null)
        {
            targetTransform.position = savedPosition;
            targetTransform.eulerAngles = savedRotation;
        }
    }
}
