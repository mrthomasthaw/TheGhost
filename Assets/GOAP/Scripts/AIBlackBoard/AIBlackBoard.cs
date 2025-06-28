using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace MrThaw
{
    public class AIBlackBoard
    {
        private Dictionary<EnumType.AIBlackBoardKey, object> dataDict;

        public Dictionary<EnumType.AIBlackBoardKey, object> DataDict { get { return dataDict; } }

        public AIBlackBoard()
        {
            dataDict = new Dictionary<EnumType.AIBlackBoardKey, object>();
        }


        public T GetOneBBData<T>(EnumType.AIBlackBoardKey key) where T : AIBlackBoardData
        {
            var container = GetContainer(key);
            if (container == null)
                return null;

            return container.GetBBData() as T;
        }

        public List<T> GetAllBBDataByType<T>(EnumType.AIBlackBoardKey key) where T : AIBlackBoardData
        {
            var container = GetContainer(key);
            if (container == null)
                return EmptyList<T>.Value;

            return container.DataList.OfType<T>().ToList();
        }

        public void AddData<T>(T data) where T : AIBlackBoardData
        {
            EnumType.AIBlackBoardKey key = data.Key;
            if (!dataDict.ContainsKey(key))
                dataDict[key] = new AIBlackBoardDataContainer();

            var container = dataDict[key] as AIBlackBoardDataContainer;
            container.DataList.Add(data);
        }

        public void RemoveBBData<T>(T bbdata) where T : AIBlackBoardData
        {
            var container = GetContainer(bbdata.Key);
            if (container == null) return;
            container.RemoveBBData(bbdata);
        }


        public AIBlackBoardDataContainer GetContainer(EnumType.AIBlackBoardKey key)
        {
            if(dataDict.TryGetValue(key, out var container))
            {
                return container as AIBlackBoardDataContainer;
            }

            return null;    
        }

        public void RemoveAllByType<T>(EnumType.AIBlackBoardKey key) where T : AIBlackBoardData
        {
            var container = GetContainer(key);
            if(container != null)
                container.Clear();
        }

        public void PrintDataDict()
        {
            Debug.Log(CommonUtil.StringJoin(dataDict));
        }


        public void RemoveFirstBBData<T>(EnumType.AIBlackBoardKey key) where T : AIBlackBoardData
        {
            var container = GetContainer(key);
            if(container != null)
                container.DataList.RemoveAt(0);
        }


        public bool ContainsData<T>(EnumType.AIBlackBoardKey key) where T : AIBlackBoardData
        {
            var container = GetContainer(key);
            if (container == null)
                return false;

            return container.ContainsData();
        }

    }
}
