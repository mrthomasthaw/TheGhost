using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIWorldState
{
    public class AIWorldState
    {
        public Dictionary<string, object> WorldStates { get; set; }

        public AIWorldState() 
        { 
            WorldStates = new Dictionary<string, object>();
        }

        public void CopyWorldStates(Dictionary<string, object> fromData)
        {
            CommonUtil.CopyWorldStates(fromData, WorldStates);
        }

        public void Add(string key, object value)
        {
            WorldStates.Add(key, value);
        }

        public void Set(string key, object value)
        {
            WorldStates[key] = value;
        }

        public object Get(string key)
        {
            return WorldStates[key];
        }

        public void Remove(string key)
        {
            WorldStates.Remove(key);
        }

        public bool Contains(string key)
        {
            return WorldStates.ContainsKey(key);
        }

        public bool CheckEqual(string key, object value)
        {
            return WorldStates[key].Equals(value);
        }

        public string PrintWorldStates()
        {
            return "Print ----- " + CommonUtil.StringJoin(WorldStates);
        }
    }
}
