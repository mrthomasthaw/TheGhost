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

        private int maxPositionLimitInMemory = 30;

        private float maxAssaultDist = 15f;

        private Transform aiAimPivotT;


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


            Collider[] collectedPoints = Physics.OverlapSphere(aiTransform.position, maxAssaultDist, tacticalPointLayer);
            List<AIInfoTacticalPosition> pointList = aiMemory
                .GetAllMemoryDataByType<AIInfoTacticalPosition>(EnumType.AIMemoryKey.TacticalPositionInfo);

            if(pointList.Count < maxPositionLimitInMemory)
            {
                //Collect nearby position points
                for (int i = 0; i < collectedPoints.Length; i++)
                {
                    TacticalPositionPoint tp = collectedPoints[i].GetComponent<TacticalPositionPoint>();
                    if (tp != null)
                    {
                        if (Vector3.Distance(tp.Position, primaryThreatBB.ThreatT.position) < 12f && Vector3.Distance(tp.Position, primaryThreatBB.ThreatT.position) > 6f)
                        {
                            Debug.Break();
                            pointList.Add(new AIInfoTacticalPosition()
                            {
                                Id = tp.Id,
                                Position = tp.Position,
                                TacticalPositionPoint = tp,
                                Score = 20f
                            });
                        }
                        
                    }
                }
            }
            

            for (int i = 0; i < pointList.Count ; i++)
            {
                if (primaryThreatBB == null)
                    break;

                //Check LOS
                CheckLineOfSight(pointList[i], primaryThreatBB.ThreatT);

                //Safe from primary threat
                CheckProtectionFromPrimaryThreat(pointList[i], primaryThreatBB.ThreatT);

                //Safe from other threats
                CheckProtectionFromOtherThreat(pointList[i], primaryThreatBB.ThreatT);

                //Check valid attack range
                //CheckAssaultRange(pointList[i], primaryThreatBB.ThreatT.position);

            }

            pointList.Sort((a, b) => b.Score.CompareTo(a.Score));

            for (int i = 0; i < pointList.Count && i < maxPositionLimitInMemory; i++) 
            {
                Debug.DrawRay(pointList[i].Position, pointList[i].Position + Vector3.up * 2f, Color.blue);
                aiMemory.AddData(pointList[i]);
            }

            Debug.Log("Scanned position");
        }

        private void CheckAssaultRange(AIInfoTacticalPosition aIInfoTacticalPosition, Vector3 primaryTargetPosition)
        {
            if (Vector3.Distance(aiTransform.position, primaryTargetPosition) < 12f)
            {
                aIInfoTacticalPosition.Score += 20f;
            }
        }

        private void CheckProtectionFromOtherThreat(AIInfoTacticalPosition aIInfoTacticalPosition, Transform primaryThreatT)
        {
            List<AIInfoThreat> threatInfoList = aiMemory.GetAllMemoryDataByType<AIInfoThreat>(EnumType.AIMemoryKey.ThreatInfo).ToList();

            for(int i = 0;i < threatInfoList.Count;i++)
            {
                if(primaryThreatT == threatInfoList[i].TargetTransform)
                {
                    continue; // Only interested in other threats
                }

                Vector3 lineOfSightOrigin = aIInfoTacticalPosition.Position;
                Vector3 direction = threatInfoList[i].TargetTransform.position - lineOfSightOrigin;

                Debug.DrawRay(lineOfSightOrigin, direction, Color.green);
                bool safeFromThreat = Physics.Raycast(lineOfSightOrigin, direction.normalized, 4f, obstacleLayer);

                Debug.Break();

                if (safeFromThreat)
                    aIInfoTacticalPosition.Score += 20f;
            }
        }

        private void CheckProtectionFromPrimaryThreat(AIInfoTacticalPosition aIInfoTacticalPosition, Transform primaryThreatT)
        {
            Vector3 lineOfSightOrigin = aIInfoTacticalPosition.Position;
            Vector3 direction = primaryThreatT.position - lineOfSightOrigin;

            Debug.DrawRay(lineOfSightOrigin, direction, Color.green);
            bool safeFromPrimaryThreat = Physics.Raycast(lineOfSightOrigin, direction.normalized, 4f, obstacleLayer);

            Debug.Break();

            if (safeFromPrimaryThreat)
                aIInfoTacticalPosition.Score += 10f;
        }

        private void CheckLineOfSight(AIInfoTacticalPosition aIInfoTacticalPosition, Transform primaryThreatT)
        {
            Vector3 lineOfSightOrigin = aIInfoTacticalPosition.Position;
            Vector3 direction = primaryThreatT.position - lineOfSightOrigin;

            Debug.DrawRay(lineOfSightOrigin, direction, Color.red);
            bool obstructedLOS = Physics.Raycast(lineOfSightOrigin, direction.normalized, direction.magnitude, obstacleLayer);

            Debug.Break();

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
