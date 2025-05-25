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
            //bug.Log("Before remove : " + CommonUtil.StringJoin(memory.Data));
            memory.Data.RemoveAll(d =>
            {
                if (d is AIInfoThreat threat)
                {
                    if (Vector3.Distance(aiHeadT.position, threat.TargetTransform.position) > sightRadius)
                        return true;

                    RaycastHit hitInfo;
                    // Here you can check properties of 'threat'
                    if (TargetBehindObstacle(aiHeadT, threat.TargetTransform, out hitInfo))
                    {
                        //Debug.Log("hit the obstacle");
                        return true; // remove this item
                    }
                }
                return false; // keep this item
            });

            //Debug.Log("After remove : " + CommonUtil.StringJoin(memory.Data));

            float minDist = float.MaxValue;
            Collider[] cols = Physics.OverlapSphere(aiHeadT.position, sightRadius, LayerMask.GetMask("Human"));

            int scoreBonus = 0;
            for(int x = 0; x < cols.Length; x++)
            {
                Vector3 dir = cols[x].transform.position - aiHeadT.position;

                RaycastHit hitInfo;

                bool targetBehindObstacle = TargetBehindObstacle(aiHeadT, cols[x].transform, out hitInfo); // Obstacle layer

                //Debug.Log("Scanned colider " + cols[x].name);

                //Debug.DrawLine(cols[x].transform.position, cols[x].transform.position + Vector3.up * 2, Color.black);
                if(targetBehindObstacle)
                {
                    //Debug.Log("hit the obstacle " + hitInfo.collider.name);
                    continue;
                }

                int score = 1;

                float dist = Vector3.Distance(aiHeadT.position, cols[x].transform.position);
                if (minDist > dist)
                {
                    minDist = dist;
                    scoreBonus += 1;
                }

                score += scoreBonus;

                AIInfoThreat threatInfo = memory.GetThreatInfoByTransform(cols[x].transform);
                if(threatInfo == null) // create info
                {
                    memory.Data.Add(new AIInfoThreat()
                    {
                        TargetTransform = cols[x].transform,
                        Score = score
                    });
                }
                else // update info
                {
                    threatInfo.Score = score;
                }
            }
        }


        private bool TargetBehindObstacle(Transform source, Transform destination, out RaycastHit hitInfo)
        {
            Vector3 dir = destination.position - source.position;
            Debug.DrawRay(source.position, dir, Color.green);
            return Physics.Raycast(source.position, dir.normalized, out hitInfo, dir.magnitude, LayerMask.GetMask("Obstacle"));
        }
    }

}