using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using MrThaw;
using MrThaw.Goap.AIMemory;
using MrThaw.Goap.AIMemory.AIInfo;

namespace MrThaw.Goap.AISystem
{
    public class AISystemPrimaryThreatSelection : AISystem
    {
        private AIMemory.AIMemory memory;

        private AIBlackBoard blackBoard;

        private AIInfoThreat selectedThreatInfo;

        private AIBBDSelectedPrimaryThreat selectedPrimaryThreat;


        public AISystemPrimaryThreatSelection(AIMemory.AIMemory memory, AIBlackBoard blackBoard)
        {
            this.memory = memory;
            this.blackBoard = blackBoard;
        }

        public override void OnUpdate(AIController aIController)
        {
            List<AIInfoThreat> threatInfoList = memory.Data.OfType<AIInfoThreat>().ToList();

            if (threatInfoList.Count == 0)
            {
                //Debug.Log("Threat not found : ");
                blackBoard.RemoveBBData(selectedPrimaryThreat);

                //Debug.Log(CommonUtil.StringJoin(blackBoard.Datas));

                selectedThreatInfo = null;
                selectedPrimaryThreat = null;

                if(! aIController.WorldState["aiAlertType"].Equals(EnumType.AIAlertType.Normal))
                {
                    aIController.WorldState["aiAlertType"] = EnumType.AIAlertType.Normal;
                    aIController.Replan = true;
                }
                
                //Debug.Log("Updated by " + this.GetType() + " " + CommonUtil.DictToString(aIController.WorldState));
                return;
            }
                
            threatInfoList.Sort((a, b) => b.Score.CompareTo(a.Score));


            AIInfoThreat primaryThreatInfo = threatInfoList[0];

            if(! primaryThreatInfo.Equals(selectedThreatInfo))
            {
                selectedThreatInfo = primaryThreatInfo;

                if (selectedPrimaryThreat == null)
                {
                    selectedPrimaryThreat = new AIBBDSelectedPrimaryThreat(primaryThreatInfo);
                    blackBoard.Add(selectedPrimaryThreat);
                }
                else
                {
                    selectedPrimaryThreat.UpdateThreatInfo(primaryThreatInfo);
                }
            }

            //Debug.Log("Threat found : " + primaryThreatInfo);

            if(! aIController.WorldState["aiAlertType"].Equals(EnumType.AIAlertType.Danger))
            {
                aIController.WorldState["aiAlertType"] = EnumType.AIAlertType.Danger; // The Goal will either be Kill Target, Take Cover, similar to hasTarget
                                                                                      // If the alert type is danger, it will set one of the following goal priority high [Kill Target, Take Cover]

                aIController.Replan = true;
            }

            ///Debug.Log(CommonUtil.StringJoin(blackBoard.Datas));
            Debug.Log("Updated by " + this.GetType() + " " + CommonUtil.DictToString(aIController.WorldState));
        }
    }

}

