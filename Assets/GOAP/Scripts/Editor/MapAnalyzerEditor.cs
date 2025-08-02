using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using MrThaw;
using System.Linq;

[CustomEditor(typeof(MapAnalyzer))]
public class MapAnalyzerEditor : Editor
{
    private MapAnalyzer analyzer;
    public MapData mapData;

    private PointDataMono[] dataMonos;
    private void OnEnable()
    {
        analyzer = target as MapAnalyzer;
    }

    public override void OnInspectorGUI()
    {
        mapData = (MapData)EditorGUILayout.ObjectField("Map data:", mapData, typeof(MapData), true);

        DrawDefaultInspector();

        if (GUILayout.Button("Create Map Points of Rays"))
        {
            GameObject goPar = new GameObject(); Transform parent = goPar.transform;
            parent.name = "Map Data";
            mapData = goPar.AddComponent<MapData>();

            int counter = 0;
            foreach (var point in GetPointsOnNavMesh())
            {
                GameObject newGo;
                if (!analyzer.pointPrefab)
                    newGo = new GameObject();
                else
                    newGo = Instantiate(analyzer.pointPrefab) as GameObject;
                newGo.transform.parent = parent;
                newGo.transform.position = point;
                newGo.transform.name = "DataPoint " + counter++;

                var pointDataMono = newGo.AddComponent<PointDataMono>();
                if (mapData.allPointDataMonos == null)
                    mapData.allPointDataMonos = new List<PointDataMono>();
                mapData.allPointDataMonos.Add(pointDataMono);
                BoxCollider bc = newGo.AddComponent<BoxCollider>();
                bc.size = new Vector3(.01f, .01f, .01f);
                bc.isTrigger = true;
            }
        }

        
        if (mapData != null)
        {
            if (GUILayout.Button("Generate Map Data"))
            {
                dataMonos = mapData.GetComponentsInChildren<PointDataMono>();
                foreach (var dMono in dataMonos)
                {
                    if (mapData.allPointDataMonos.Contains(dMono))
                        continue;
                    mapData.allPointDataMonos.Add(dMono);
                }

                Debug.Log("AFter adding monoPoint ");
                List<PointData> dataPoints = new List<PointData>();

                // Create a gameobject for every child gameobject of "Map Data"
                //var monos = mapData.transform.GetComponentsInChildren<PointDataMono>();
                var monos = dataMonos;
                for (int i = 0; i < monos.Length; i++)
                {
                    dataPoints.Add(new PointData(i, monos[i].transform.position, monos[i].gameObject));
                    monos[i].pointDataNo = i;
                }

                Debug.Log("AFter adding another monoPoint ");

                List<PointCoverData> allPossibleCoverPoints = new List<PointCoverData>();

                bool debugCover = CoverTargetLogic.debugCoverCheck;
                CoverTargetLogic.debugCoverCheck = false;
                for (int i = 0; i < dataPoints.Count; i++)
                {
                    var point = dataPoints[i];

                    #region Cover Check

                    Vector3 coverPosition = Vector3.zero; Vector3 coverNormal = Vector3.zero;
                    bool canCover = CoverTargetLogic.CheckCoverAroundPosition(analyzer.coverCheckParams, point.Position, ref coverNormal, ref coverPosition,
                        point.Position);
                    PointCoverData coverPointData = null;
                    if (canCover)
                    {
                        coverPointData = new PointCoverData(coverNormal, coverPosition, CoverTargetLogic.RequestCrouch(analyzer.coverCrouchCheckParams, analyzer.coverCheckParams.rayMask, coverPosition, coverNormal));
                        allPossibleCoverPoints.Add(coverPointData);
                    }

                    #endregion Cover Check
                }

                Debug.Log("After cover check");
                if (CoverTargetLogic.transform_static)
                    DestroyImmediate(CoverTargetLogic.transform_static.gameObject, false);
                CoverTargetLogic.debugCoverCheck = debugCover;

                // NavmeshPath check - not used
                //foreach (var dPoint in dataPoints)
                //    dPoint.distanceDataList = new List<DistanceData>();
                //for (int i = 0; i < dataPoints.Count; i++)
                //{
                //    var point_i = dataPoints[i];

                //    for (int j = 0; j < dataPoints.Count; j++)
                //    {
                //        var point_j = dataPoints[j];
                //        if (j == i)
                //            continue;
                //        if (point_j.distanceDataList.FirstOrDefault(x => x.pointDataIndexNo == point_i.pointDataNo) != null)
                //            continue;
                //        if (Vector3.Distance(point_i.Position, point_j.Position) > analyzer.distanceDataCollectPerPointOverlapRadius * 2f)
                //            continue;

                //        var path = new NavMeshPath();
                //        NavMesh.CalculatePath(point_i.Position, point_j.Position, NavMesh.AllAreas, path);
                //        if (path.status == NavMeshPathStatus.PathComplete)
                //        {
                //            point_i.distanceDataList.Add(new DistanceData(dataPoints[j].pointDataNo, CalculatePathLength(path, point_i.Position, point_j.Position)));
                //            //point.distanceDataList.Add(new DistanceData(j, path.corners.Length));
                //        }
                //    }
                //}

                Debug.Log("Cover edge check");
                //Cover edge checks
                List<PointCoverData> newPoints = new List<PointCoverData>();
                foreach (var coverPointData in allPossibleCoverPoints)
                {
                    var newCD = CheckForSide(new PointCoverData(coverPointData), true);
                    if (newCD != null && !newCD.IsEqual(coverPointData))
                        newPoints.Add(newCD);
                    newCD = CheckForSide(new PointCoverData(coverPointData), false);
                    if (newCD != null && !newCD.IsEqual(coverPointData))
                        newPoints.Add(newCD);
                }
                foreach (var cpd in newPoints)
                    allPossibleCoverPoints.Add(cpd);

                //Remove same cover points (except edges)
                //for (int i = 0; i < allPossibleCoverPoints.Count; i++)
                //{
                //    if (allPossibleCoverPoints[i].isEdge)
                //        continue;
                //    var cCoverPoint_i = allPossibleCoverPoints[i];
                //    for (int j = i + 1; j < allPossibleCoverPoints.Count; j++)
                //    {
                //        if (!allPossibleCoverPoints[j].isEdge && Vector3.Distance(cCoverPoint_i.CoverPosition, allPossibleCoverPoints[j].CoverPosition) < analyzer.closeCoverRemovalDist)
                //        {
                //            allPossibleCoverPoints.Remove(allPossibleCoverPoints[j]);
                //        }
                //    }
                //}

                Debug.Log("Before removing same cover point ");

                List<PointCoverData> toRemove = new List<PointCoverData>();

                for (int i = 0; i < allPossibleCoverPoints.Count; i++)
                {
                    if (allPossibleCoverPoints[i].isEdge)
                        continue;

                    var cCoverPoint_i = allPossibleCoverPoints[i];
                    for (int j = i + 1; j < allPossibleCoverPoints.Count; j++)
                    {
                        if (!allPossibleCoverPoints[j].isEdge && Vector3.Distance(cCoverPoint_i.CoverPosition, allPossibleCoverPoints[j].CoverPosition) < analyzer.closeCoverRemovalDist)
                        {
                            toRemove.Add(allPossibleCoverPoints[j]);
                        }
                    }
                }

                foreach (var r in toRemove)
                    allPossibleCoverPoints.Remove(r);


                // Remove similar edge cover points
                //for (int i = 0; i < allPossibleCoverPoints.Count; i++)
                //{
                //    if (!allPossibleCoverPoints[i].isEdge)
                //        continue;
                //    var cCoverPoint_i = allPossibleCoverPoints[i];
                //    for (int j = i + 1; j < allPossibleCoverPoints.Count; j++)
                //    {
                //        if (allPossibleCoverPoints[j].isEdge && Vector3.Distance(cCoverPoint_i.CoverPosition, allPossibleCoverPoints[j].CoverPosition) < analyzer.closeCoverEdgeRemovalDist)
                //        {
                //            allPossibleCoverPoints.Remove(allPossibleCoverPoints[j]);
                //        }
                //    }
                //}

                Debug.Log("Before removing edge  ");
                List<PointCoverData> toRemoveEdges = new List<PointCoverData>();

                for (int i = 0; i < allPossibleCoverPoints.Count; i++)
                {
                    if (!allPossibleCoverPoints[i].isEdge)
                        continue;

                    var cCoverPoint_i = allPossibleCoverPoints[i];
                    for (int j = i + 1; j < allPossibleCoverPoints.Count; j++)
                    {
                        if (allPossibleCoverPoints[j].isEdge && Vector3.Distance(cCoverPoint_i.CoverPosition, allPossibleCoverPoints[j].CoverPosition) < analyzer.closeCoverEdgeRemovalDist)
                        {
                            toRemoveEdges.Add(allPossibleCoverPoints[j]);
                        }
                    }
                }

                foreach (var r in toRemoveEdges)
                    allPossibleCoverPoints.Remove(r);


                Debug.Log("Now it's close");
                int count = 0;
                foreach (var coverPoint in allPossibleCoverPoints)
                {
                    if (coverPoint.isEdge)
                        count++;
                }
                Debug.Log("Edge count: " + count + " | Total cover/safe point count: " + allPossibleCoverPoints.Count);

                // Assign cover points to closest data points
                int coverUniqueNo = 0;
                foreach (var coverPoint in allPossibleCoverPoints)
                {
                    coverPoint.coverUniqueNo = coverUniqueNo++;
                    float minDist = float.PositiveInfinity;
                    PointData willBeAssigned = null;
                    foreach (var dataPoint in dataPoints)
                    {
                        Vector3 position = dataPoint.Position;
                        float cDist = Vector3.Distance(position, coverPoint.CoverPosition);
                        if (cDist < minDist)
                        {
                            willBeAssigned = dataPoint;
                            minDist = cDist;
                        }
                    }
                    if (willBeAssigned.possibleCovers == null)
                        willBeAssigned.possibleCovers = new List<PointCoverData>();
                    willBeAssigned.possibleCovers.Add(coverPoint);
                }

                for (int i = 0; i < dataPoints.Count; i++)
                {
                    mapData.allPointDataMonos[i].pointData = dataPoints[i];

                    if (!mapData.allPointDataMonos[i])
                        mapData.allPointDataMonos[i].pointData.possibleCovers = null;
                }
            } // end generate map data button

        }
    }

    private PointCoverData CheckForSide(PointCoverData cd, bool toLeft)
    {
        int maxTry = 10;
        int tryCounter = 0;


        bool haveSideEdge = false; float mult = toLeft ? -1 : 1;
        Vector3 finalNormal = cd.CoverNormal;
        Vector3 finalCoverGroundPos = cd.CoverPosition;
        Ray rayN = new Ray(cd.CoverPosition + Vector3.up * analyzer.coverCheckParams.fromGroundPointStartUpHeight +
                Quaternion.Euler(0, 90 * mult, 0) * cd.CoverNormal * analyzer.coverEdgeCheckStep, -cd.CoverNormal);
        do
        {
            rayN = new Ray(finalCoverGroundPos + Vector3.up * analyzer.coverCheckParams.fromGroundPointStartUpHeight +
                Quaternion.Euler(0, 90 * mult, 0) * finalNormal * analyzer.coverEdgeCheckStep, -finalNormal);

            Vector3 newCoverNormalL = finalNormal; Vector3 newCoverPointL = finalCoverGroundPos;
            haveSideEdge = CoverTargetLogic.RequestCover(rayN, analyzer.coverCheckParams, analyzer.sideEdgeRayMaxDistance, ref newCoverNormalL, ref newCoverPointL, cd.CoverPosition);
            if (haveSideEdge)
            {
                finalNormal = newCoverNormalL;
                finalCoverGroundPos = newCoverPointL;
            }
            tryCounter++;
        } while (haveSideEdge && tryCounter < maxTry);

        if (tryCounter == 1)
        {
            cd.isEdge = true;
            cd.isLeftEdge = toLeft;
            return null;
        }
        PointCoverData newCoverData = new PointCoverData(finalNormal, finalCoverGroundPos,
            CoverTargetLogic.RequestCrouch(analyzer.coverCrouchCheckParams, analyzer.coverCheckParams.rayMask, finalCoverGroundPos, finalNormal));
        newCoverData.isLeftEdge = toLeft;
        newCoverData.isEdge = true;

        return newCoverData;
    }

    private List<Vector3> GetPointsOnNavMesh()
    {
        List<Vector3> points = new List<Vector3>();
        NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();

        float x = 0, z = 0;
        while (x < analyzer.maxDistanceX)
        {
            z = 0;
            while (z < analyzer.maxDistanceZ)
            {
                for (int i = 0; i < triangles.indices.Length; i += 3)
                {
                    Vector3 v0 = triangles.vertices[triangles.indices[i]];
                    Vector3 v1 = triangles.vertices[triangles.indices[i + 1]];
                    Vector3 v2 = triangles.vertices[triangles.indices[i + 2]];
                    Ray ray = new Ray(analyzer.checkStartPoint + Vector3.right * x + Vector3.forward * z, Vector3.down * analyzer.rayLength);
                    if (Intersect(v0, v1, v2, ray
                        ))
                    {
                        Vector3 normal = Vector3.Cross((v0 - v1), (v2 - v0)).normalized;
                        float d = Vector3.Dot(normal, ((v0 + v1 + v2) / 3) - ray.origin) / Vector3.Dot(normal, ray.direction);
                        Vector3 iPoint = ray.origin + ray.direction * d;
                        points.Add(iPoint);
                    }
                }
                z += analyzer.checkSpace;
            }
            x += analyzer.checkSpace;
        }
        return points;
    }

    //private static float CalculatePathLength(NavMeshPath path, Vector3 curPosition, Vector3 targetPosition)
    //{
    //    Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];

    //    allWayPoints[0] = curPosition;

    //    allWayPoints[allWayPoints.Length - 1] = targetPosition;

    //    for (int i = 0; i < path.corners.Length; i++)
    //    {
    //        allWayPoints[i + 1] = path.corners[i];
    //    }

    //    float pathLength = 0;

    //    for (int i = 0; i < allWayPoints.Length - 1; i++)
    //    {
    //        pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
    //    }
    //    return pathLength;
    //}

    public void OnSceneGUI()
    {
        Handles.color = Color.white;
        analyzer.maxDistanceX = Handles.ScaleSlider(analyzer.maxDistanceX, analyzer.checkStartPoint, Vector3.right, Quaternion.LookRotation(Vector3.up), .5f, 1);
        analyzer.maxDistanceX = analyzer.maxDistanceX < 0 ? 0 : analyzer.maxDistanceX;

        analyzer.maxDistanceZ = Handles.ScaleSlider(analyzer.maxDistanceZ, analyzer.checkStartPoint, Vector3.forward, Quaternion.LookRotation(Vector3.up), .5f, 1);
        analyzer.maxDistanceZ = analyzer.maxDistanceZ < 0 ? 0 : analyzer.maxDistanceZ;

        analyzer.checkSpace = Handles.ScaleValueHandle(analyzer.checkSpace, analyzer.checkStartPoint, Quaternion.LookRotation(Vector3.up), 1.5f, Handles.CircleCap, 1f);
        analyzer.checkSpace = analyzer.checkSpace <= 0 ? 0.05f : analyzer.checkSpace;

        Handles.color = Color.red;

        EditorGUI.BeginChangeCheck();
        Vector3 sP = Handles.PositionHandle(analyzer.checkStartPoint, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "");
            analyzer.checkStartPoint = sP;
        }

        NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
        Mesh mesh = new Mesh();
        mesh.vertices = triangles.vertices;
        mesh.triangles = triangles.indices;

        float x = 0, z = 0;
        while (x < analyzer.maxDistanceX)
        {
            z = 0;
            while (z < analyzer.maxDistanceZ)
            {
                Vector3 startPoint = analyzer.checkStartPoint + Vector3.right * x + Vector3.forward * z;
                Handles.color = Color.white;
                Handles.DrawLine(startPoint, startPoint + Vector3.down * analyzer.rayLength
                    );
                z += analyzer.checkSpace;
            }
            x += analyzer.checkSpace;
        }
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
}