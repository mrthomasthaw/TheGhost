using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class RagdollRemover
{
    [MenuItem("Tools/Remove Ragdoll Components")]
    public static void RemoveRagdoll()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogError("Please select a GameObject with a Humanoid Animator.");
            return;
        }

        Animator animator = selected.GetComponent<Animator>();
        if (animator == null || !animator.isHuman)
        {
            Debug.LogError("Selected GameObject must have a Humanoid Animator.");
            return;
        }

        HumanBodyBones[] bones = new HumanBodyBones[]
        {
            HumanBodyBones.Hips,
            HumanBodyBones.Spine,
            HumanBodyBones.Chest,
            HumanBodyBones.Head,
            HumanBodyBones.LeftUpperArm,
            HumanBodyBones.LeftLowerArm,
            HumanBodyBones.RightUpperArm,
            HumanBodyBones.RightLowerArm,
            HumanBodyBones.LeftUpperLeg,
            HumanBodyBones.LeftLowerLeg,
            HumanBodyBones.RightUpperLeg,
            HumanBodyBones.RightLowerLeg
        };

        int count = 0;
        foreach (HumanBodyBones bone in bones)
        {
            Transform boneTransform = animator.GetBoneTransform(bone);
            if (boneTransform == null) continue;

            Object.DestroyImmediate(boneTransform.GetComponent<Rigidbody>());
            Object.DestroyImmediate(boneTransform.GetComponent<Collider>());
            Object.DestroyImmediate(boneTransform.GetComponent<CharacterJoint>());
            count++;
        }

        Debug.Log($"Removed ragdoll components from {count} bones.");
    }
}
