using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIMemory
{
    public class AIMemoryDataContainer
    {
        private List<AIMemoryData> dataList = new List<AIMemoryData>();

        public List<AIMemoryData> DataList { get { return dataList; } }
    }
}
