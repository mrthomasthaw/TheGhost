using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MrThaw.Goap.AIActions;


namespace MrThaw {
    public class GoapPlannerTest : MonoBehaviour
    {
        [SerializeField] private List<AIGoal> goalList;

        private AIPlanner aiPlanner;

        private AIBlackBoard blackBoard;

        // Start is called before the first frame update
        void Start()
        {
            //aiPlanner = new AIPlanner();
            //aiPlanner.SetUp(goalList, blackBoard);
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("Key press F");
                Queue<AIAction> actionQ = aiPlanner.CalculateActionPlan();
                while(actionQ.Count > 0)
                {
                    Debug.Log("action name : " + actionQ.Peek().GetType().Name);
                    actionQ.Dequeue();
                }
            }

        }
    }
}
