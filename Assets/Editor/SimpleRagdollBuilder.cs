using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimpleRagdollBuilder : EditorWindow
{
    private GameObject targetCharacter;

    [MenuItem("Tools/Simple Ragdoll Builder")]
    static void Init()
    {
        SimpleRagdollBuilder window = (SimpleRagdollBuilder)EditorWindow.GetWindow(typeof(SimpleRagdollBuilder));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Ragdoll Builder", EditorStyles.boldLabel);

        targetCharacter = (GameObject)EditorGUILayout.ObjectField("Target Character", targetCharacter, typeof(GameObject), true);

        if (targetCharacter != null && GUILayout.Button("Create Ragdoll"))
        {
            CreateRagdoll(targetCharacter);
        }
    }

    void CreateRagdoll(GameObject target)
    {
        Animator animator = target.GetComponent<Animator>();

        if (animator == null || !animator.isHuman)
        {
            Debug.LogError("The selected character must have a Humanoid Animator.");
            return;
        }

        // Common human bone mapping
        Dictionary<HumanBodyBones, string> boneNames = new Dictionary<HumanBodyBones, string>()
        {
            { HumanBodyBones.Hips, "Hips" },
            { HumanBodyBones.Spine, "Spine" },
            { HumanBodyBones.LeftUpperLeg, "LeftUpperLeg" },
            { HumanBodyBones.LeftLowerLeg, "LeftLowerLeg" },
            { HumanBodyBones.RightUpperLeg, "RightUpperLeg" },
            { HumanBodyBones.RightLowerLeg, "RightLowerLeg" },
            { HumanBodyBones.LeftUpperArm, "LeftUpperArm" },
            { HumanBodyBones.LeftLowerArm, "LeftLowerArm" },
            { HumanBodyBones.RightUpperArm, "RightUpperArm" },
            { HumanBodyBones.RightLowerArm, "RightLowerArm" },
            { HumanBodyBones.Head, "Head" }
        };

        foreach (var pair in boneNames)
        {
            Transform bone = animator.GetBoneTransform(pair.Key);
            if (bone != null)
            {
                AddRagdollComponents(bone.gameObject);
            }
        }

        // Add joints
        ConnectJoints(animator);

        Debug.Log("Ragdoll created successfully.");
    }

    void AddRagdollComponents(GameObject bone)
    {
        if (!bone.GetComponent<Rigidbody>())
            bone.AddComponent<Rigidbody>();

        if (!bone.GetComponent<Collider>())
        {
            CapsuleCollider col = bone.AddComponent<CapsuleCollider>();
            col.direction = 1; // Y-axis
            col.height = 0.2f;
            col.radius = 0.05f;
        }
    }

    void ConnectJoints(Animator animator)
    {
        // Example: Connect limbs to the body using CharacterJoint
        Connect(animator, HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftUpperArm);
        Connect(animator, HumanBodyBones.RightLowerArm, HumanBodyBones.RightUpperArm);
        Connect(animator, HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftUpperLeg);
        Connect(animator, HumanBodyBones.RightLowerLeg, HumanBodyBones.RightUpperLeg);
        Connect(animator, HumanBodyBones.LeftUpperArm, HumanBodyBones.Spine);
        Connect(animator, HumanBodyBones.RightUpperArm, HumanBodyBones.Spine);
        Connect(animator, HumanBodyBones.LeftUpperLeg, HumanBodyBones.Hips);
        Connect(animator, HumanBodyBones.RightUpperLeg, HumanBodyBones.Hips);
        Connect(animator, HumanBodyBones.Head, HumanBodyBones.Spine);
    }

    void Connect(Animator animator, HumanBodyBones childBone, HumanBodyBones parentBone)
    {
        Transform child = animator.GetBoneTransform(childBone);
        Transform parent = animator.GetBoneTransform(parentBone);

        if (child != null && parent != null)
        {
            CharacterJoint joint = child.gameObject.GetComponent<CharacterJoint>();
            if (!joint)
                joint = child.gameObject.AddComponent<CharacterJoint>();

            joint.connectedBody = parent.GetComponent<Rigidbody>();
        }
    }
}
