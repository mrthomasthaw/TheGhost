using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AttachScriptToBodyPart : EditorWindow
{
    private GameObject targetObject;

    [MenuItem("Tools/Add HitableBodyPart To Children")]
    public static void ShowWindow()
    {
        GetWindow<AttachScriptToBodyPart>("Add HitableBodyPart");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select a GameObject to add HitableBodyPart script to all children (if they have Collider or Rigidbody)", EditorStyles.wordWrappedLabel);
        targetObject = (GameObject)EditorGUILayout.ObjectField("Target GameObject", targetObject, typeof(GameObject), true);

        if (targetObject != null && GUILayout.Button("Add HitableBodyPart to Children"))
        {
            int count = AddScriptToChildren(targetObject.transform);
            Debug.Log($"Added HitableBodyPart to {count} children of {targetObject.name}");
        }
    }

    private int AddScriptToChildren(Transform parent)
    {
        int addedCount = 0;

        foreach (Transform child in parent)
        {
            GameObject go = child.gameObject;

            bool hasCollider = go.GetComponent<Collider>() != null;
            bool hasRigidbody = go.GetComponent<Rigidbody>() != null;
            bool hasScript = go.GetComponent<TargetableBodyPart>() != null;

            if ((hasCollider || hasRigidbody) && !hasScript)
            {
                Undo.AddComponent<TargetableBodyPart>(go);
                Debug.Log($"Added HitableBodyPart to '{go.name}'");
                addedCount++;
            }

            // Recursive call to children
            addedCount += AddScriptToChildren(child);
        }

        return addedCount;
    }
}
