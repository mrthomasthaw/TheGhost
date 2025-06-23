using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLook : MonoBehaviour
{
    public CameraInputController cameraInputController;


    private bool death = false;

    [SerializeField]
    private DeathEventSO deathEventSO;

    private void OnEnable()
    {
        deathEventSO.OnEventRaised += OnDeath;
    }

    private void OnDisable()
    {
        deathEventSO.OnEventRaised += OnDeath;
    }

    void Start()
    {
        cameraInputController = FindObjectOfType<CameraInputController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (death) return;

        Vector3 dir = cameraInputController.CamerHolderTransform.forward * 5f;
        dir.Normalize();
        dir.y = 0;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 6f);
    }


    public void OnDeath(GameObject sender)
    {
        if (sender == this.gameObject)
            death = true;
    }
}
