using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MrThaw.Goap.AISensors;
using MrThaw;
using MrThaw.Goap.AIMemory;
using MrThaw.Goap.AIMemory.AIInfo;

public class TacticalPositionScanerTest : MonoBehaviour
{
    private AISensorTacticalPositionScaner positionScaner;
    public LayerMask tacticalPointLayer;
    public LayerMask obstacleLayer;
    public Transform aimPivot;
    public Transform primaryThreat;
    public List<Transform> allThreatList;
    private AIBlackBoard blackBoard;
    private AIMemory memory;

    // Start is called before the first frame update
    void Start()
    {
        blackBoard = new AIBlackBoard();
        memory = new AIMemory();

        AIBBDSelectedPrimaryThreat selectedPrimaryThreat = new AIBBDSelectedPrimaryThreat() 
        { 
            ThreatT = primaryThreat,
            IsStillValid = true,
        };


        blackBoard.AddData(selectedPrimaryThreat);

        for(int i = 0; i < allThreatList.Count; i++)
        {
            memory.AddData(new AIInfoThreat()
            {
                TargetTransform = allThreatList[i],
                IsStillValid = true,             
            });
        }

        positionScaner = new AISensorTacticalPositionScaner(blackBoard, memory,
            transform, tacticalPointLayer, obstacleLayer, aimPivot);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            positionScaner.OnUpdate();
        }
    }
}
