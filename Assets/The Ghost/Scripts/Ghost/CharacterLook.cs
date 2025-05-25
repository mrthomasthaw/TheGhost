using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLook : MonoBehaviour
{
    public CameraInputController cameraInputController;

    void Start()
    {
        cameraInputController = FindObjectOfType<CameraInputController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = cameraInputController.CamerHolderTransform.forward * 5f;
        dir.Normalize();
        dir.y = 0;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 6f);
    }
}
