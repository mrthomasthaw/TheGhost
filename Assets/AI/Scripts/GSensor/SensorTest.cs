using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorTest : MonoBehaviour
{
    VisionSensor visionSensor;
    BlackBoardManager blackBoardManager;
    GWorldState agentWorldState;

    [SerializeField]
    LayerMask obstacleLayer;

    // Start is called before the first frame update
    void Start()
    {
        SetUpWorldStates();

        blackBoardManager = new BlackBoardManager();
        visionSensor = new VisionSensor(transform, transform, blackBoardManager, obstacleLayer, agentWorldState);
    }

    private void SetUpWorldStates()
    {
        agentWorldState = new GWorldState();
        agentWorldState.Add(AIWorldStateKey.HasPrimaryTarget.ToString(), false);
        agentWorldState.Add(AIWorldStateKey.AimWeapon.ToString(), false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            visionSensor.OnUpdate();
        }
    }
}
