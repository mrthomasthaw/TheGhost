using MrThaw.Goap.AIMemory.AIInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MrThaw.Goap.AIMemory
{
    public class AIMemory
    {
        public AIMemory()
        {
            DataDict = new Dictionary<EnumType.AIMemoryKey, object>();
        }

        public Dictionary<EnumType.AIMemoryKey, object> DataDict { get; private set; }

        public void AddData<T>(T data) where T : AIMemoryData
        {
            EnumType.AIMemoryKey key = data.Key;
            if (!DataDict.ContainsKey(key))
                DataDict[key] = new AIMemoryDataContainer();

            var container = DataDict[key] as AIMemoryDataContainer;
            container.DataList.Add(data);
        }

        public AIInfoThreat GetThreatInfoByTransform(Transform transform)
        {
            if (transform == null) return null;
            return GetAllMemoryDataByType<AIInfoThreat>(EnumType.AIMemoryKey.ThreatInfo)
                .Where(s => s.TargetTransform != null && s.TargetTransform == transform).FirstOrDefault();
        }


        public List<T> GetAllMemoryDataByType<T>(EnumType.AIMemoryKey key) where T : AIMemoryData
        {
            var container = GetContainer(key);
            if (container == null) return EmptyList<T>.Value;
            return container.DataList.OfType<T>().ToList();
        }

        public AIMemoryDataContainer GetContainer(EnumType.AIMemoryKey key)
        {
            if(DataDict.TryGetValue(key, out var container))
            {
                return container as AIMemoryDataContainer;
            }

            return null;
        }

        public void RemoveAllIf(System.Predicate<AIMemoryData> predicate, EnumType.AIMemoryKey key)
        {           
            GetAllMemoryDataByType<AIMemoryData>(key).RemoveAll(predicate);
        }
    }
}

