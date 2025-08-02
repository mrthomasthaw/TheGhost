using MrThaw;
using MrThaw.AIMapData;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace MrThaw.AIMapData
{

    [CustomEditor(typeof(AIMapDataGenerator))]
    public class AIMapDataGeneratorEditor : Editor
    {
        private AIMapDataGenerator _generator;

        public MapPositionPointContainer mapPositionContainer;

        private void OnEnable()
        {
            _generator = target as AIMapDataGenerator;
        }

        public override void OnInspectorGUI()
        {
            mapPositionContainer = (MapPositionPointContainer)EditorGUILayout.ObjectField("Map Position Container :", mapPositionContainer, typeof(MapPositionPointContainer), true);

            DrawDefaultInspector();

            if (GUILayout.Button("Create Map Points of Rays"))
            {
                GameObject goPar = new GameObject(); Transform parent = goPar.transform;
                parent.name = "Map Position Container";
                mapPositionContainer = goPar.AddComponent<MapPositionPointContainer>();

                int counter = 0;
                foreach (var point in GetPointsOnNavMesh())
                {
                    GameObject newGo;
                    int id = counter;

                    if (!_generator.pointPrefab) 
                        newGo = new GameObject();
                    else
                        newGo = Instantiate(_generator.pointPrefab) as GameObject;
                    newGo.transform.parent = parent;
                    newGo.transform.position = point;
                    newGo.layer = LayerMask.NameToLayer("TacticalPoint");
                    newGo.transform.name = "Tactical Position Point " + counter++;

                    var tacticalPositionPoint = newGo.AddComponent<TacticalPositionPoint>();
                    tacticalPositionPoint.SetData(id, point);


                    if (mapPositionContainer.TacticalPositionPoints == null)
                        mapPositionContainer.TacticalPositionPoints = new List<TacticalPositionPoint>();
                    mapPositionContainer.TacticalPositionPoints.Add(tacticalPositionPoint);
                    BoxCollider bc = newGo.AddComponent<BoxCollider>();
                    bc.size = new Vector3(.01f, .01f, .01f);
                    bc.isTrigger = true;
                }
            }
        }

        private List<Vector3> GetPointsOnNavMesh()
        {
            List<Vector3> points = new List<Vector3>();
            NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();

            float x = 0, z = 0;
            while (x < _generator.maxDistanceX)
            {
                z = 0;
                while (z < _generator.maxDistanceZ)
                {
                    for (int i = 0; i < triangles.indices.Length; i += 3)
                    {
                        Vector3 v0 = triangles.vertices[triangles.indices[i]];
                        Vector3 v1 = triangles.vertices[triangles.indices[i + 1]];
                        Vector3 v2 = triangles.vertices[triangles.indices[i + 2]];
                        Ray ray = new Ray(_generator.checkStartPoint + Vector3.right * x + Vector3.forward * z, Vector3.down * _generator.rayLength);
                        if (Intersect(v0, v1, v2, ray
                            ))
                        {
                            Vector3 normal = Vector3.Cross((v0 - v1), (v2 - v0)).normalized;
                            float d = Vector3.Dot(normal, ((v0 + v1 + v2) / 3) - ray.origin) / Vector3.Dot(normal, ray.direction);
                            Vector3 iPoint = ray.origin + ray.direction * d;
                            points.Add(iPoint);
                        }
                    }
                    z += _generator.checkSpace;
                }
                x += _generator.checkSpace;
            }
            return points;
        }

        public static bool Intersect(Vector3 p1, Vector3 p2, Vector3 p3, Ray ray)
        {
            // Vectors from p1 to p2/p3 (edges)
            Vector3 e1, e2;

            Vector3 p, q, t;
            float det, invDet, u, v;

            //Find vectors for two edges sharing vertex/point p1
            e1 = p2 - p1;
            e2 = p3 - p1;

            // calculating determinant
            p = Vector3.Cross(ray.direction, e2);

            //Calculate determinat
            det = Vector3.Dot(e1, p);

            //if determinant is near zero, ray lies in plane of triangle otherwise not
            if (det > -Mathf.Epsilon && det < Mathf.Epsilon) { return false; }
            invDet = 1.0f / det;

            //calculate distance from p1 to ray origin
            t = ray.origin - p1;

            //Calculate u parameter
            u = Vector3.Dot(t, p) * invDet;

            //Check for ray hit
            if (u < 0 || u > 1) { return false; }

            //Prepare to test v parameter
            q = Vector3.Cross(t, e1);

            //Calculate v parameter
            v = Vector3.Dot(ray.direction, q) * invDet;

            //Check for ray hit
            if (v < 0 || u + v > 1) { return false; }

            if ((Vector3.Dot(e2, q) * invDet) > Mathf.Epsilon)
            {
                //ray does intersect
                return true;
            }

            // No hit at all
            return false;
        }

        public void OnSceneGUI()
        {
            Handles.color = Color.white;
            _generator.maxDistanceX = Handles.ScaleSlider(_generator.maxDistanceX, _generator.checkStartPoint, Vector3.right, Quaternion.LookRotation(Vector3.up), .5f, 1);
            _generator.maxDistanceX = _generator.maxDistanceX < 0 ? 0 : _generator.maxDistanceX;

            _generator.maxDistanceZ = Handles.ScaleSlider(_generator.maxDistanceZ, _generator.checkStartPoint, Vector3.forward, Quaternion.LookRotation(Vector3.up), .5f, 1);
            _generator.maxDistanceZ = _generator.maxDistanceZ < 0 ? 0 : _generator.maxDistanceZ;

            _generator.checkSpace = Handles.ScaleValueHandle(_generator.checkSpace, _generator.checkStartPoint, Quaternion.LookRotation(Vector3.up), 1.5f, Handles.CircleCap, 1f);
            _generator.checkSpace = _generator.checkSpace <= 0 ? 0.05f : _generator.checkSpace;

            Handles.color = Color.red;

            EditorGUI.BeginChangeCheck();
            Vector3 sP = Handles.PositionHandle(_generator.checkStartPoint, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "");
                _generator.checkStartPoint = sP;
            }

            NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
            Mesh mesh = new Mesh();
            mesh.vertices = triangles.vertices;
            mesh.triangles = triangles.indices;

            float x = 0, z = 0;
            while (x < _generator.maxDistanceX)
            {
                z = 0;
                while (z < _generator.maxDistanceZ)
                {
                    Vector3 startPoint = _generator.checkStartPoint + Vector3.right * x + Vector3.forward * z;
                    Handles.color = Color.white;
                    Handles.DrawLine(startPoint, startPoint + Vector3.down * _generator.rayLength
                        );
                    z += _generator.checkSpace;
                }
                x += _generator.checkSpace;
            }
        }
    }

}
