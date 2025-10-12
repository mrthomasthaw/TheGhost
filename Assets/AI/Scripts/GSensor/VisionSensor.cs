using MrThaw;
using MrThaw.Goap.AIMemory;
using MrThaw.Goap.AIMemory.AIInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VisionSensor : GSensor
{
    private float sightRadius = 10f;
    private Transform aiTransform;
    private Transform aiHeadT;
    private BlackBoardManager blackBoardManager;
    private GWorldState agentWorldState;
    private LayerMask obstacleLayer;

    public VisionSensor(Transform head, Transform transform, BlackBoardManager blackBoardManager, LayerMask obstacleLayer, GWorldState agentWorldState)
    {
        this.aiHeadT = head;
        this.aiTransform = transform;
        this.blackBoardManager = blackBoardManager;
        this.obstacleLayer = obstacleLayer;
        this.agentWorldState = agentWorldState;
    }

    public override void OnUpdate()
    {
        List<KnownThreatInfoData> threatInfoBlackBoardDatas = blackBoardManager.GetAllDataByKey<KnownThreatInfoData>(BlackBoardKey.KnownThreatInfo);
        Debug.Log("Before remove : " + CommonUtil.StringJoin(threatInfoBlackBoardDatas));


        threatInfoBlackBoardDatas.RemoveAll(CheckInvalidLineOfSightTargets());

        Debug.Log("After remove : " + CommonUtil.StringJoin(threatInfoBlackBoardDatas));

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

            KnownThreatInfoData threatInfo = GetThreatInfoByTransform(cols[x].transform);
            if (threatInfo == null) // create info
            {
                blackBoardManager.AddData<KnownThreatInfoData>(new KnownThreatInfoData
                {
                    ThreatTransform = cols[x].transform,
                    IsStillValid = !targetHealth.IsDeath,
                    HealthControl = targetHealth,
                    Score = score
                }, BlackBoardKey.KnownThreatInfo);
            }
            else // update info
            {
                threatInfo.Score = score;
            }
        }

        if (threatInfoBlackBoardDatas.Count > 0) 
        {
            threatInfoBlackBoardDatas = threatInfoBlackBoardDatas.OrderByDescending(t => t.Score).ToList();
            
            KnownThreatInfoData highPriorityThreat = threatInfoBlackBoardDatas[0];

            //It make sure that only one data in the list of blackboard
            blackBoardManager.RemoveLastData<SelectedThreatInfoData>(BlackBoardKey.SelectedPrimaryThreat);
            blackBoardManager.AddData<SelectedThreatInfoData>(new SelectedThreatInfoData 
            { 
                HealthControl = highPriorityThreat.HealthControl,
                ThreatTransform = highPriorityThreat.ThreatTransform,
                IsStillValid = highPriorityThreat.IsStillValid,
                Score = highPriorityThreat.Score
            }, BlackBoardKey.SelectedPrimaryThreat);

            agentWorldState.Set(AIWorldStateKey.HasPrimaryTarget.ToString(), true);
        }
        else
        {
            blackBoardManager.RemoveLastData<SelectedThreatInfoData>(BlackBoardKey.SelectedPrimaryThreat);
            agentWorldState.Set(AIWorldStateKey.HasPrimaryTarget.ToString(), false);
        }

        Debug.Log("After scanned : " + CommonUtil.StringJoin(threatInfoBlackBoardDatas));

        Debug.Log("Selected Primary threat " + CommonUtil.StringJoin(blackBoardManager.GetAllDataByKey<SelectedThreatInfoData>(BlackBoardKey.SelectedPrimaryThreat)));
    }

    private bool TargetBehindObstacle(Transform source, Transform destination, out RaycastHit hitInfo)
    {
        Vector3 dir = destination.position - source.position;
        Debug.DrawRay(source.position, dir, Color.green);
        return Physics.Raycast(source.position, dir.normalized, out hitInfo, dir.magnitude, LayerMask.GetMask("Obstacle"));
    }

    private System.Predicate<BlackBoardData> CheckInvalidLineOfSightTargets()
    {
        return t =>
        {
            Debug.Log(t.ToString());
            if (t is KnownThreatInfoData threat)
            {
                Debug.Log(t.ToString());


                if (Vector3.Distance(aiHeadT.position, threat.ThreatTransform.position) > sightRadius)
                {
                    threat.IsStillValid = false;
                    return true;
                }


                RaycastHit hitInfo;
                // Here you can check properties of 'threat'
                if (TargetBehindObstacle(aiHeadT, threat.ThreatTransform, out hitInfo))
                {
                    threat.IsStillValid = false;
                    //Debug.Log("hit the obstacle");
                    return true; // remove this item
                }

                if (threat.HealthControl.IsDeath)
                {
                    threat.IsStillValid = false;
                    return true;
                }
            }
            return false; // keep this item
        };
    }

    private KnownThreatInfoData GetThreatInfoByTransform(Transform transform)
    {
        List<KnownThreatInfoData> knownThreatList = blackBoardManager.GetAllDataByKey<KnownThreatInfoData>(BlackBoardKey.KnownThreatInfo) as List<KnownThreatInfoData>;
        return knownThreatList
            .Where(s => s.ThreatTransform != null && s.ThreatTransform == transform && s.IsStillValid).FirstOrDefault();
    }
}
