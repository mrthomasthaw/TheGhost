using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MrThaw.Goap.AIMemory;
using MrThaw.Goap.AIMemory.AIInfo;
using MrThaw.AIMapData;
using System.Linq;
using System;

namespace MrThaw.Goap.AISensors
{
    public class AISensorTacticalPositionScaner : AISensor
    {
        private AIBlackBoard blackBoard;

        private AIMemory.AIMemory aiMemory;

        private Transform aiTransform;

        private LayerMask tacticalPointLayer;

        private LayerMask obstacleLayer;

        private int maxPositionLimitInMemory = 10;

        private float maxAssaultDist = 10f;

        private Transform aiAimPivotT;


        Collider[] collectedPoints = new Collider[150];

        List<AIInfoTacticalPosition> pointList = new List<AIInfoTacticalPosition>(150);


        public  AISensorTacticalPositionScaner(AIBlackBoard blackBoard, AIMemory.AIMemory aiMemory, Transform aiTransform, LayerMask tacticalPointLayer, LayerMask obstacleLayer, Transform aiAimPivotT)
        {
            this.blackBoard = blackBoard;
            this.aiMemory = aiMemory;
            this.aiTransform = aiTransform;
            this.tacticalPointLayer = tacticalPointLayer;
            this.obstacleLayer = obstacleLayer;
            this.aiAimPivotT = aiAimPivotT;
        }

        public override void OnUpdate()
        {
            aiMemory.RemoveAllIf(CheckInvalidPositions(), EnumType.AIMemoryKey.TacticalPositionInfo);


            AIBBDSelectedPrimaryThreat primaryThreatBB = blackBoard
                .GetOneBBData<AIBBDSelectedPrimaryThreat>(EnumType.AIBlackBoardKey.SelectedPrimaryThreat);

            int retrievedColliderSize = Physics.OverlapSphereNonAlloc(aiTransform.position, maxAssaultDist, collectedPoints, tacticalPointLayer);
            
            pointList.Clear();


            //Collect nearby position points
            for (int i = 0; i < collectedPoints.Length; i++)
            {
                if (collectedPoints[i] == null)
                    continue;

                TacticalPositionPoint tp = collectedPoints[i].GetComponent<TacticalPositionPoint>();
                if (tp == null)
                    continue;

                //Debug.Break();
                pointList.Add(new AIInfoTacticalPosition()
                {
                    Id = tp.Id,
                    Position = tp.Position,
                    TacticalPositionPoint = tp,
                    Score = 20f
                });

            }


            for (int i = 0; i < pointList.Count ; i++)
            {
                if (primaryThreatBB == null)
                    break;

                //Check LOS
                //CheckLineOfSight(pointList[i], primaryThreatBB.ThreatT);

                //Safe from primary threat (high cover)
                //CheckHighCoverProtectionFromPrimaryThreat(pointList[i], primaryThreatBB.ThreatT);

                //Safe from primary threat (low cover)
                //CheckLowCoverProtectionFromPrimaryThreat(pointList[i], primaryThreatBB.ThreatT);

                //Safe from other threats
                CheckHighCoverProtectionFromOtherThreat(pointList[i], primaryThreatBB.ThreatT);

                //Check valid attack range
                //CheckAssaultRange(pointList[i], primaryThreatBB.ThreatT.position);

            }

            pointList.Sort((a, b) => b.Score.CompareTo(a.Score));

            aiMemory.ClearData(EnumType.AIMemoryKey.TacticalPositionInfo);

            for (int i = 0; i < pointList.Count && i < maxPositionLimitInMemory; i++) 
            {
                //Debug.DrawRay(pointList[i].Position, pointList[i].Position + Vector3.up * 2f, Color.blue);
                aiMemory.AddData(pointList[i]);
            }

            Debug.Log("Best position Id : " + pointList[0].Id);
            Debug.Break();

           // Debug.Log("Scanned position");
        }



        private void CheckAssaultRange(AIInfoTacticalPosition aIInfoTacticalPosition, Vector3 primaryTargetPosition)
        {
            if (Vector3.Distance(aiTransform.position, primaryTargetPosition) < 12f)
            {
                aIInfoTacticalPosition.Score += 20f;
            }
        }

        private void CheckHighCoverProtectionFromOtherThreat(AIInfoTacticalPosition aIInfoTacticalPosition, Transform primaryThreatT)
        {
            List<AIInfoThreat> threatInfoList = aiMemory.GetAllMemoryDataByType<AIInfoThreat>(EnumType.AIMemoryKey.ThreatInfo).ToList();

            for(int i = 0;i < threatInfoList.Count;i++)
            {
                if(primaryThreatT == threatInfoList[i].TargetTransform)
                {
                    continue; // Only interested in other threats
                }

                Vector3 lineOfSightOrigin = aIInfoTacticalPosition.Position;
                Debug.DrawLine(lineOfSightOrigin, lineOfSightOrigin + Vector3.up * 1.5f, Color.white);
                lineOfSightOrigin.y += 1.5f;


                bool safeFromThreat = true;
                Vector3[] shiftedPos = new Vector3[2] { Vector3.right, Vector3.left };
                for (int j = 0; j < shiftedPos.Length; j++)
                {
                    Vector3 origin = lineOfSightOrigin + (shiftedPos[j] * 2f);
                    Vector3 direction = threatInfoList[i].TargetTransform.position - origin;
                    direction.y += 0.5f;
                    Debug.DrawRay(origin, direction, Color.green);
                    if (!Physics.Raycast(origin, direction, direction.magnitude, obstacleLayer)) // If there no obstacle?
                    { 
                        safeFromThreat = false;
                        break;
                    }
                }

                //Debug.Break();

                if (safeFromThreat)
                    aIInfoTacticalPosition.Score += 20f;
            }
        }

        private void CheckHighCoverProtectionFromPrimaryThreat(AIInfoTacticalPosition aIInfoTacticalPosition, Transform primaryThreatT)
        {
            Vector3 lineOfSightOrigin = aIInfoTacticalPosition.Position;
            Debug.DrawLine(lineOfSightOrigin, lineOfSightOrigin + Vector3.up * 1.5f, Color.white);
            lineOfSightOrigin.y += 1.5f;


            bool safeFromPrimaryThreat = true;
            Vector3[] shiftedPos = new Vector3[2] { Vector3.right, Vector3.left};
            for (int i = 0; i < shiftedPos.Length; i++)
            {
                Vector3 origin = lineOfSightOrigin + (shiftedPos[i] * 2f);
                Vector3 direction = primaryThreatT.position -  origin;
                direction.y += 0.5f;
                Debug.DrawRay(origin, direction, Color.green);
                if (! Physics.Raycast(origin, direction, direction.magnitude, obstacleLayer)) // If there no obstacle?
                {
                    safeFromPrimaryThreat = false;
                    break;
                }
            }

            

            //Debug.Break();

            if (safeFromPrimaryThreat)
                aIInfoTacticalPosition.Score += 10f;
        }

        private void CheckLowCoverProtectionFromPrimaryThreat(AIInfoTacticalPosition aIInfoTacticalPosition, Transform primaryThreatT)
        {
            Vector3 lineOfSightOrigin = aIInfoTacticalPosition.Position;
            Debug.DrawLine(lineOfSightOrigin, lineOfSightOrigin + Vector3.up * 0.5f, Color.white);
            lineOfSightOrigin.y += 0.5f;


            bool safeFromPrimaryThreat = true;
            Vector3[] shiftedPos = new Vector3[4] 
            { 
                lineOfSightOrigin + (Vector3.right * 2f), 
                lineOfSightOrigin + (Vector3.left * 2f),
                lineOfSightOrigin + (Vector3.up * 0.4f) + (Vector3.right * 2f),
                lineOfSightOrigin + (Vector3.up * 0.4f) + (Vector3.left * 2f)
            };

            for (int i = 0; i < shiftedPos.Length; i++)
            {
                Vector3 direction = primaryThreatT.position - shiftedPos[i];
                direction.y += 0.5f;
                Debug.DrawRay(shiftedPos[i], direction, Color.green);
                if (!Physics.Raycast(shiftedPos[i], direction, direction.magnitude, obstacleLayer)) // If there no obstacle?
                {
                    safeFromPrimaryThreat = false;
                    break;
                }
            }
            //Debug.Break();

            if (safeFromPrimaryThreat)
                aIInfoTacticalPosition.Score += 10f;
        }


        private void CheckLineOfSight(AIInfoTacticalPosition aIInfoTacticalPosition, Transform primaryThreatT)
        {
            Vector3 lineOfSightOrigin = aIInfoTacticalPosition.Position;

            Debug.DrawLine(lineOfSightOrigin, lineOfSightOrigin + Vector3.up * 1.5f, Color.white);

            lineOfSightOrigin.y += 1.5f;

            Vector3 direction = primaryThreatT.position - lineOfSightOrigin;

            Debug.DrawRay(lineOfSightOrigin, direction, Color.red);
            
            bool obstructedLOS = Physics.Raycast(lineOfSightOrigin, direction.normalized, direction.magnitude, obstacleLayer);

            //Debug.Break();

            if (!obstructedLOS)
                aIInfoTacticalPosition.Score += 20f;
        }

        System.Predicate<AIMemoryData> CheckInvalidPositions()
        {
            return t =>
            {
                if (t is AIInfoTacticalPosition tacticalPosition)
                {
                    if (Vector3.Distance(aiTransform.position, tacticalPosition.Position) > maxAssaultDist)
                    {
                        return true;
                    }

                    Vector3 lineOfSightOrigin = tacticalPosition.Position;
                    Vector3 direction = tacticalPosition.Position - lineOfSightOrigin;
                    bool obstructedLOS = Physics.Raycast(lineOfSightOrigin, direction.normalized, direction.magnitude, obstacleLayer);
                    if(obstructedLOS)
                        return true;

                }

                return false;
            };
        }
    }
}
