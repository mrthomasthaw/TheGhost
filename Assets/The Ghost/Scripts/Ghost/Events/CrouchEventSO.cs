using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObjects/CrouchEventSO", menuName = "Events/Crouch Event")]
public class CrouchEventSO : ScriptableObject
{
    public event Action<GameObject, bool> OnEventRaised;


    public void RaiseEvent(GameObject sender, bool state) => OnEventRaised?.Invoke(sender, state);
}
