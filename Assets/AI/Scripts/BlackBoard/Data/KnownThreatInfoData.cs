using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnownThreatInfoData : BlackBoardData
{
    public override BlackBoardKey BlackBoardKey => BlackBoardKey.KnownThreatInfo;
    public Transform ThreatTransform {  get; set; }
    public HealthControl HealthControl {  get; set; }

    public override string ToString()
    {
        return $"{{ BlackBoardKey = {BlackBoardKey}, ThreatTransform = {ThreatTransform}, HealthControl = {HealthControl} }}";
    }


}
