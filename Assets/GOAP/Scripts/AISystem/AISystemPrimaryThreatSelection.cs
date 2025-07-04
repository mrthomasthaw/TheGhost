using MrThaw;
using MrThaw.Goap.AIMemory;
using MrThaw.Goap.AIMemory.AIInfo;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

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
            List<AIInfoThreat> threatInfoList = memory.GetAllMemoryDataByType<AIInfoThreat>
                (EnumType.AIMemoryKey.ThreatInfo).ToList();

            if (threatInfoList.Count == 0)
            {
                //Debug.Log("Threat not found : ");
                if(selectedPrimaryThreat != null)
                {
                    selectedPrimaryThreat.UpdateThreatInfo(selectedThreatInfo);

                    blackBoard.RemoveBBData(selectedPrimaryThreat);

                    selectedThreatInfo = null;
                    selectedPrimaryThreat = null;
                }

                //Debug.Log(CommonUtil.StringJoin(blackBoard.Datas));



                if (aIController.AgentWorldState.CheckEqual(EnumType.AIWorldStateKey.HasTarget.ToString(), true))
                {
                    aIController.AgentWorldState.Set(EnumType.AIWorldStateKey.HasTarget.ToString(), false);
                    aIController.Replan = true;
                }
                
                //Debug.Log("Updated by " + this.GetType() + " " + CommonUtil.DictToString(aIController.WorldState));
                return;
            }
                
            threatInfoList.Sort((a, b) => b.Score.CompareTo(a.Score));


            AIInfoThreat primaryThreatInfo = threatInfoList[0];

            if(! IsSameTarget(primaryThreatInfo)) // If the transform doesn't match
            {
                selectedThreatInfo = primaryThreatInfo;

                if (selectedPrimaryThreat == null)
                {
                    selectedPrimaryThreat = new AIBBDSelectedPrimaryThreat(primaryThreatInfo);
                    blackBoard.AddData(selectedPrimaryThreat);
                }
                else
                {
                    selectedPrimaryThreat.UpdateThreatInfo(primaryThreatInfo);
                }
            }


            //Debug.Log("Threat found : " + primaryThreatInfo);

            if(aIController.AgentWorldState.CheckEqual(EnumType.AIWorldStateKey.HasTarget.ToString(), false))
            {
                aIController.AgentWorldState.Set(EnumType.AIWorldStateKey.HasTarget.ToString(), true); // The Goal will either be Kill Target, Take Cover, similar to hasTarget
                                                                                                       // If the alert type is danger, it will set one of the following goal priority high [Kill Target, Take Cover]
                aIController.Replan = true;
            }

            ///Debug.Log(CommonUtil.StringJoin(blackBoard.Datas));
            Debug.Log("Updated by " + this.GetType() + " " + CommonUtil.StringJoin(aIController.AgentWorldState.WorldStates));
        }

        private bool IsSameTarget(AIInfoThreat aIInfo)
        {
            if(aIInfo == null)
                return false;

            if(selectedThreatInfo == null)
                return false;

            return EqualityComparer<Transform>.Default.Equals(aIInfo.TargetTransform, selectedThreatInfo.TargetTransform);
        }
    }

}

