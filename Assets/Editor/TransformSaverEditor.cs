using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TransformSaver))]
public class TransformSaverEditor : Editor
{
    private void OnEnable()
    {
        // Check if the object has a reference to the TransformSaver script and apply saved values
        TransformSaver transformSaver = (TransformSaver)target;

        // If we are in play mode, save the position and rotation before stopping play
        if (EditorApplication.isPlaying)
        {
            transformSaver.savedPosition = transformSaver.targetTransform.position;
            transformSaver.savedRotation = transformSaver.targetTransform.eulerAngles;
        }
        else
        {
            // When not in play mode, apply the saved position and rotation values
            if (transformSaver.targetTransform != null)
            {
                transformSaver.targetTransform.position = transformSaver.savedPosition;
                transformSaver.targetTransform.eulerAngles = transformSaver.savedRotation;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        TransformSaver transformSaver = (TransformSaver)target;

        // Draw default inspector
        DrawDefaultInspector();

        if (GUILayout.Button("Save Transform"))
        {
            if (transformSaver.targetTransform != null)
            {
                transformSaver.savedPosition = transformSaver.targetTransform.position;
                transformSaver.savedRotation = transformSaver.targetTransform.eulerAngles;
                Debug.Log("Transform Saved!");
            }
        }

        if (GUILayout.Button("Apply Saved Transform"))
        {
            transformSaver.ApplySavedTransform();
            Debug.Log("Saved Transform Applied!");
        }
    }
}
