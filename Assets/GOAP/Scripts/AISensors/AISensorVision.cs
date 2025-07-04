using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using MrThaw.Goap.AIMemory;
using MrThaw.Goap.AIMemory.AIInfo;

namespace MrThaw.Goap.AISensors
{
    public class AISensorVision : AISensor
    {
        private int maxLimitInMemory = 10;
        private float sightRadius = 10f;
        private Transform aiTransform;
        private Transform aiHeadT;
        private AIMemory.AIMemory memory;
        private LayerMask obstacleLayer;

        public AISensorVision(Transform head, Transform transform, AIMemory.AIMemory memory, LayerMask obstacleLayer)
        {
            this.aiHeadT = head;
            this.aiTransform = transform;
            this.memory = memory;
            this.obstacleLayer = obstacleLayer;
        }

        public override void OnUpdate()
        {
            //Debug.Log("Before remove : " + CommonUtil.StringJoin(memory.DataDict));
            int removedCount = memory.RemoveAllIf(CheckInvalidLineOfSightTargets(), EnumType.AIMemoryKey.ThreatInfo);

            //Debug.Log("After remove : " + CommonUtil.StringJoin(memory.DataDict));

            float minDist = float.MaxValue;
            Collider[] cols = Physics.OverlapSphere(aiHeadT.position, sightRadius, LayerMask.GetMask("Human"));

            float scoreBonus = 0;
            for (int x = 0; x < cols.Length; x++)
            {
                HealthControl targetHealth = cols[x].transform.GetComponentInParent<HealthControl>();

                if (targetHealth == null || targetHealth.IsDeath)
                {
                    continue;
                }

                Vector3 dir = cols[x].transform.position - aiHeadT.position;

                RaycastHit hitInfo;

                bool targetBehindObstacle = TargetBehindObstacle(aiHeadT, cols[x].transform, out hitInfo); // Obstacle layer

                //Debug.Log("Scanned colider " + cols[x].name);

                //Debug.DrawLine(cols[x].transform.position, cols[x].transform.position + Vector3.up * 2, Color.black);
                if (targetBehindObstacle)
                {
                    //Debug.Log("hit the obstacle " + hitInfo.collider.name);
                    continue;
                }

                float score = 1;

                float dist = Vector3.Distance(aiHeadT.position, cols[x].transform.position);
                if (minDist > dist)
                {
                    minDist = dist;
                    scoreBonus += 1;
                }

                score += scoreBonus;

                AIInfoThreat threatInfo = GetThreatInfoByTransform(cols[x].transform);
                if (threatInfo == null) // create info
                {
                    memory.AddData(new AIInfoThreat()
                    {
                        TargetTransform = cols[x].transform,
                        IsStillValid = ! targetHealth.IsDeath,
                        TargetHealthControl = targetHealth,
                        Score = score
                    });
                }
                else // update info
                {
                    threatInfo.Score = score;
                }
            }
        }

        private System.Predicate<AIMemoryData> CheckInvalidLineOfSightTargets()
        {
            return t =>
            {
                Debug.Log(t.ToString());
                if (t is AIInfoThreat threat)
                {
                    Debug.Log(t.ToString());


                    if (Vector3.Distance(aiHeadT.position, threat.TargetTransform.position) > sightRadius)
                    {
                        threat.IsStillValid = false;
                        return true;
                    }


                    RaycastHit hitInfo;
                    // Here you can check properties of 'threat'
                    if (TargetBehindObstacle(aiHeadT, threat.TargetTransform, out hitInfo))
                    {
                        threat.IsStillValid = false;
                        //Debug.Log("hit the obstacle");
                        return true; // remove this item
                    }

                    if(threat.TargetHealthControl.IsDeath)
                    {
                        threat.IsStillValid = false;
                        return true;
                    }
                }
                return false; // keep this item
            };
        }

        private bool TargetBehindObstacle(Transform source, Transform destination, out RaycastHit hitInfo)
        {
            Vector3 dir = destination.position - source.position;
            Debug.DrawRay(source.position, dir, Color.green);
            return Physics.Raycast(source.position, dir.normalized, out hitInfo, dir.magnitude, LayerMask.GetMask("Obstacle"));
        }

        private AIInfoThreat GetThreatInfoByTransform(Transform transform)
        {
            return memory.GetAllMemoryDataByType<AIInfoThreat>(EnumType.AIMemoryKey.ThreatInfo)
                .Where(s => s.TargetTransform != null && s.TargetTransform == transform && s.IsStillValid).FirstOrDefault();
        }
    }

}