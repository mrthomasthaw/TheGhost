using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace MrThaw
{
    public class AIBlackBoardDataContainer
    {
        public List<AIBlackBoardData> DataList { get; private set; } = new List<AIBlackBoardData>();


        public void Add(AIBlackBoardData bbdata)
        {
            DataList.Add(bbdata);
        }

        public int Count
        {
            get { return DataList.Count; }
        }

        public bool ContainsData()
        {
            return GetBBData() != null;
        }

        public AIBlackBoardData GetBBData()
        {
            return DataList.LastOrDefault();
        }

        public void RemoveBBData(AIBlackBoardData bbdata)
        {
            if (DataList.Contains(bbdata))
                DataList.Remove(bbdata);
        }

        public void Clear()
        {
            DataList.Clear();
        }

        public override string ToString()
        {
            return $"{{ DataList = {CommonUtil.StringJoin(DataList)} }}";
        }
    }
}
