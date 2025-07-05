
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MrThaw;

[CustomEditor(typeof(MapData))]
public class MapDataEditor : Editor
{
    private MapData mapData;

    private List<PointData> loadedPoints = new List<PointData>();

    private void OnEnable()
    {
        mapData = target as MapData;
        loadedPoints = new List<PointData>();

        loadedPoints.Clear();

        for (int i = mapData.debugIndexStartFrom; i < mapData.allPointDataMonos.Count && i < mapData.debugIndexStartFrom + mapData.pointDebugCount; i++)
        {
            PointData point = mapData.allPointDataMonos[i].pointData;
            if (point != null)
                loadedPoints.Add(point);
        }
    }

    private void OnDisable()
    {
        loadedPoints.Clear();
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        int dbgStartIndexFrom = EditorGUILayout.IntField("Debug Index starts from", mapData.debugIndexStartFrom);
        int pointDbgCount = EditorGUILayout.IntField("Debug Point Count", mapData.pointDebugCount);

        bool showCover = EditorGUILayout.Toggle("Show Cover Positions", mapData.debugCoverNormals, EditorStyles.radioButton);
        bool showCoverCrouch = EditorGUILayout.Toggle("Show Cover Crouch Amount", mapData.debugCoverCrouchVal, EditorStyles.radioButton);
        bool showCoverEdge = EditorGUILayout.Toggle("Show Cover Edges", mapData.debugCoverEdges, EditorStyles.radioButton);

        EditorGUILayout.LabelField("Loaded " + loadedPoints.Count + " points");

        if (EditorGUI.EndChangeCheck())
        {
            mapData.pointDebugCount = pointDbgCount;
            mapData.debugIndexStartFrom = dbgStartIndexFrom;
            mapData.debugIndexStartFrom = mapData.debugIndexStartFrom < 0 ? 0 : mapData.debugIndexStartFrom;
            mapData.debugCoverNormals = showCover;
            mapData.debugCoverCrouchVal = showCoverCrouch;
            mapData.debugCoverEdges = showCoverEdge;

            for (int i = 0; i < loadedPoints.Count; i++)
            {
                loadedPoints[i] = null;
            }
            loadedPoints.Clear();

            for (int i = mapData.debugIndexStartFrom; i < mapData.allPointDataMonos.Count && i < mapData.debugIndexStartFrom + mapData.pointDebugCount; i++)
            {
                PointData point = mapData.allPointDataMonos[i].pointData;
                if (point != null)
                    loadedPoints.Add(point);
            }

            SceneView.RepaintAll();
        }
    }

    public void OnSceneGUI()
    {
        foreach (var point in loadedPoints)
        {
            Handles.color = new Color(1, 1, 1, .2f);
            Handles.color = new Color(0, 0, 0, .2f);

            Handles.DrawSolidArc(point.Position, Vector3.up, Vector3.right, 360, .1f);

            if (point.HaveCover)
            {
                if (mapData.debugCoverNormals)
                {
                    foreach (var coverR in point.possibleCovers)
                    {
                        Handles.color = Color.white;
                        Handles.DrawLine(coverR.CoverPosition + Vector3.up * .5f, coverR.CoverPosition + Vector3.up * .5f + coverR.CoverNormal * .3f);
                        Handles.color = Color.gray;
                        Handles.DrawLine(point.Position, coverR.CoverPosition + Vector3.up * .5f);
                    }
                }

                if (mapData.debugCoverCrouchVal)
                {
                    foreach (var coverR in point.possibleCovers)
                    {
                        GUIContent content = new GUIContent(coverR.coverHeight + "");
                        GUIStyle gs = new GUIStyle();
                        gs.normal.textColor = Color.green;
                        Handles.Label(coverR.CoverPosition + Vector3.up * .7f, content, gs);
                    }
                }

                if (mapData.debugCoverEdges)
                {
                    foreach (var coverR in point.possibleCovers)
                    {
                        if (coverR.isEdge)
                        {
                            GUIContent content = new GUIContent((coverR.isLeftEdge ? "L" : "R") + " E");
                            GUIStyle gs = new GUIStyle();
                            gs.normal.textColor = Color.red;
                            Handles.Label(coverR.CoverPosition + Vector3.up * .55f, content, gs);
                        }
                    }
                }
            }
        }
    }
}