
using UnityEngine;

namespace MrThaw
{
    public class AIStateSystemAnimate : AIStateSystem
    {
        // Animator states
        public static readonly int h_IdleWithWeapon = Animator.StringToHash("IdleWithWeapon");

        public static readonly int h_AimingWithWeapon = Animator.StringToHash("AimingWithWeapon");
        public static readonly int ht_Locomotion = Animator.StringToHash("Locomotion");
        public static readonly int h_CoverLocomotion = Animator.StringToHash("CoverLocomotion");
        public static readonly int h_Empty = Animator.StringToHash("Empty");
        public static readonly int h_RifleLocomotionCrouch = Animator.StringToHash("RifleLocomotionCrouch");
        public static readonly int h_crouchRelaxedLocomotion = Animator.StringToHash("Crouch Relaxed Locomotion");
        public static readonly int h_standRelaxedLocomotion = Animator.StringToHash("Stand Relaxed Locomotion");
        public static readonly int h_rifleLocomotionStand = Animator.StringToHash("RifleLocomotionStand");
        public static readonly int ht_Peek = Animator.StringToHash("Peek");

        // Animator parameters
        public static readonly int cap_WeaponPull = Animator.StringToHash("Weapon Pull");

        public static readonly int cap_Aim = Animator.StringToHash("Aim");
        public static readonly int cap_Reload = Animator.StringToHash("Reload");
        public static readonly int cap_Cover = Animator.StringToHash("Cover");
        public static readonly int cap_EdgePeek = Animator.StringToHash("EdgePeek");
        public static readonly int cap_UpPeek = Animator.StringToHash("UpPeek");

        private Animator animator;
        private ShooterWeaponSMB weaponSmb;

        private AIAnimationDict.AnimationDict currentAnimation = AIAnimationDict.AnimationDict.NoAnimation;
        private bool isCurrentAnimationInterruptable = true;

        public AIStateSystemAnimate(Animator _animator, WeaponPositionControl _weaponPositionControl)
        {
            this.animator = _animator;
        }

        public override void OnStart(AIBlackBoard blackBoard)
        {
            weaponSmb = animator.GetBehaviour<ShooterWeaponSMB>();
#if UNITY_EDITOR
            if (!weaponSmb)
                Debug.Log("Needed reference not found. " + ToString());
#endif
        }

        public override void OnActionActivate(AIBlackBoard blackBoard)
        {
            var bbd = blackBoard.GetBBData<AIBBDAnimate>();
            if (bbd != null)
            {
                currentAnimation = bbd.Animation;
                Animate();
            }
            blackBoard.RemoveAll<AIBBDAnimate>();
        }
        
        public void Animate()
        {
            switch (currentAnimation)
            {
                case AIAnimationDict.AnimationDict.NoAnimation:
                    return;

                case AIAnimationDict.AnimationDict.EnterCover:
                    animator.SetInteger(cap_Cover, 1);
                    isCurrentAnimationInterruptable = false;
                    break;

                case AIAnimationDict.AnimationDict.ExitCover:
                    animator.SetInteger(cap_Cover, 0);
                    isCurrentAnimationInterruptable = false;
                    break;

                case AIAnimationDict.AnimationDict.PullOutWeapon:
                    animator.SetInteger(cap_WeaponPull, 1);
                    isCurrentAnimationInterruptable = false;
                    break;

                case AIAnimationDict.AnimationDict.HolsterWeapon:
                    animator.SetInteger(cap_WeaponPull, 0);
                    isCurrentAnimationInterruptable = false;
                    break;

                case AIAnimationDict.AnimationDict.MeleeWeapon:
                    break;

                case AIAnimationDict.AnimationDict.Fire:
                    //weaponSmb.OnFireActivated();
                    break;

                case AIAnimationDict.AnimationDict.FireFromCover:
                    //weaponSmb.OnFireActivated();
                    break;

                case AIAnimationDict.AnimationDict.FireMoving:
                    //weaponSmb.OnFireActivated();
                    break;

                case AIAnimationDict.AnimationDict.PeekFromCover:
                    //var bbDCover = ai.Blackboard.GetBBData<AIBBDSelectedCoverPosition>();
                    //bool isEdgeCover = bbDCover.CoverInfo.isEdgeCover;
                    //animator.SetBool(isEdgeCover ? cap_EdgePeek : cap_UpPeek, true);
                    isCurrentAnimationInterruptable = false;
                    break;

                case AIAnimationDict.AnimationDict.UnPeekFromCover:
                    animator.SetBool(cap_UpPeek, false);
                    animator.SetBool(cap_EdgePeek, false);
                    isCurrentAnimationInterruptable = false;
                    break;

                case AIAnimationDict.AnimationDict.ReloadWeapon:
                    animator.SetTrigger(cap_Reload);
                    isCurrentAnimationInterruptable = false;
                    break;

                case AIAnimationDict.AnimationDict.UnAim:
                    animator.SetBool(cap_Aim, false);
                    isCurrentAnimationInterruptable = false;
                    break;

                case AIAnimationDict.AnimationDict.Aim:
                    animator.SetBool(cap_Aim, true);
                    //weaponSmb.OnAimActivated(false);
                    isCurrentAnimationInterruptable = false;
                    break;

                case AIAnimationDict.AnimationDict.HipFireAim:
                    animator.SetBool(cap_Aim, true);
                    //weaponSmb.OnAimActivated(false);
                    isCurrentAnimationInterruptable = false;
                    break;

                case AIAnimationDict.AnimationDict.Crouch:
                    animator.SetBool("Crouch", true);
                    isCurrentAnimationInterruptable = false;
                    break;

                case AIAnimationDict.AnimationDict.Stand:
                    animator.SetBool("Crouch", false);
                    isCurrentAnimationInterruptable = false;
                    break;
            }
        }

        public override bool IsInterruptable(AIBlackBoard blackBoard)
        {
            return isCurrentAnimationInterruptable;
        }

        public bool HasFinished()
        {
            if (currentAnimation == AIAnimationDict.AnimationDict.NoAnimation)
                return true;

            int L0_ShortNameHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            int L1_ShortNameHash = animator.GetCurrentAnimatorStateInfo(1).shortNameHash;

            int L0_TagHash = animator.GetCurrentAnimatorStateInfo(0).tagHash;
            //int L1_TagHash = animator.GetCurrentAnimatorStateInfo(1).tagHash;

            bool L0_InTransition = animator.IsInTransition(0);
            bool L1_InTransition = animator.IsInTransition(1);

            switch (currentAnimation)
            {
                case AIAnimationDict.AnimationDict.EnterCover:
                    return L0_ShortNameHash == h_CoverLocomotion && !L0_InTransition;

                case AIAnimationDict.AnimationDict.ExitCover:
                    return L0_TagHash == ht_Locomotion && !L0_InTransition;

                case AIAnimationDict.AnimationDict.PullOutWeapon:
                    return L1_ShortNameHash == h_IdleWithWeapon && !L1_InTransition;

                case AIAnimationDict.AnimationDict.HolsterWeapon:
                    return L1_ShortNameHash == h_Empty && !L1_InTransition;

                case AIAnimationDict.AnimationDict.MeleeWeapon:
                    return L0_TagHash == ht_Locomotion && !L0_InTransition;

                case AIAnimationDict.AnimationDict.Fire:
                    return true;

                case AIAnimationDict.AnimationDict.FireFromCover:
                    return true;

                case AIAnimationDict.AnimationDict.FireMoving:
                    return true;

                case AIAnimationDict.AnimationDict.PeekFromCover:
                    return L0_TagHash == ht_Peek;

                case AIAnimationDict.AnimationDict.UnPeekFromCover:
                    return L0_ShortNameHash == h_CoverLocomotion && !L0_InTransition;

                case AIAnimationDict.AnimationDict.ReloadWeapon:
                    return ((!animator.GetBool(cap_Aim) && L1_ShortNameHash == h_IdleWithWeapon || animator.GetBool(cap_Aim) && L1_ShortNameHash == h_AimingWithWeapon)) && !L1_InTransition;

                case AIAnimationDict.AnimationDict.UnAim:
                    return L1_ShortNameHash == h_IdleWithWeapon && !L1_InTransition;

                case AIAnimationDict.AnimationDict.Aim:
                    //return L1_ShortNameHash == h_AimingWithWeapon && !L1_InTransition && weaponSmb.RightHandAim > .7f;
                    return L1_ShortNameHash == h_AimingWithWeapon && !L1_InTransition;
                case AIAnimationDict.AnimationDict.HipFireAim:
                    //return L1_ShortNameHash == h_AimingWithWeapon && !L1_InTransition && weaponSmb.RightHandAim > .7f;
                    return true;
                case AIAnimationDict.AnimationDict.Crouch:
                    return !L0_InTransition && L0_ShortNameHash == h_RifleLocomotionCrouch || L0_ShortNameHash == h_crouchRelaxedLocomotion;

                case AIAnimationDict.AnimationDict.Stand:
                    return !L0_InTransition && L0_ShortNameHash == h_standRelaxedLocomotion || L0_ShortNameHash == h_rifleLocomotionStand;
            }
            return true;
        }

        public override void OnActionExit(AIBlackBoard blackBoard)
        {
            // Reset
            if (currentAnimation == AIAnimationDict.AnimationDict.Fire ||
                currentAnimation == AIAnimationDict.AnimationDict.FireFromCover ||
                currentAnimation == AIAnimationDict.AnimationDict.FireMoving
                )
                //weaponSmb.OnFireDeActivated();

            isCurrentAnimationInterruptable = true;
            currentAnimation = AIAnimationDict.AnimationDict.NoAnimation;
        }

        public override bool HasStateFinished()
        {
            if (currentAnimation == AIAnimationDict.AnimationDict.NoAnimation)
            {
                return true;
            }
            else
            {
                return HasFinished();
            }
        }
    }
}