using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw {
    public class AIBBDSMBFireWeapon : AIBlackBoardData
    {
        public bool FireWeapon { get; private set; }

        public AIBBDSMBFireWeapon() 
        { 

        }

        public AIBBDSMBFireWeapon(bool fireWeapon)
        {
            this.FireWeapon = fireWeapon;
        }
    }
}
