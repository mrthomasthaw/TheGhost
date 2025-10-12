using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlackBoardData
{
    public abstract BlackBoardKey BlackBoardKey { get; }

    public bool IsStillValid { get; set; }

    public float Score { get; set; }

    public override string ToString()
    {
        return $"{{ BlackBoardKey = {BlackBoardKey}, IsStillValid = {IsStillValid}, Score = {Score} }}";
    }


}

public enum BlackBoardKey
{
    Animate,
    SelectedPrimaryThreat,
    KnownThreatInfo,
    FireWeapon,
    MoveTo,
    TurnTo,
    LookAt,
    PatrolRoute,
}
