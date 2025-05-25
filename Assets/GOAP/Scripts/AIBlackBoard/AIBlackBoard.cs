using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace MrThaw
{
    public class AIBlackBoard
    {
        private List<AIBlackBoardData> datas;

        public List<AIBlackBoardData> Datas { get => datas; }


        public AIBlackBoard()
        {
            datas = new List<AIBlackBoardData>();
        }


        public void RemoveBBData(AIBlackBoardData bbdata)
        {
            if (datas.Contains(bbdata))
                datas.Remove(bbdata);
        }

        public void RemoveAll<T>() where T : AIBlackBoardData
        {
            var rmvDatas = datas.FindAll(x => x.GetType() == typeof(T));
            foreach (var rmv in rmvDatas)
            {
                RemoveBBData(rmv);
            }
        }

        public void RemoveFirstBBData<T>() where T : AIBlackBoardData
        {
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i].GetType() == typeof(T))
                {
                    RemoveBBData(datas[i]);
                    break;
                }
            }
        }

        public T GetBBData<T>() where T : AIBlackBoardData
        {
            return datas.OfType<T>().LastOrDefault();
        }

        public List<T> GetAllBBDatasOfType<T>() where T : AIBlackBoardData
        {
            return datas.OfType<T>().ToList();
        }

        public AIBlackBoardData GetWithIndex(int i)
        {
            return datas[i];
        }

        public void Add<T>(T bbdata) where T : AIBlackBoardData
        {
            datas.Add(bbdata);
        }

        public int Count
        {
            get { return datas.Count; }
        }


        public bool ContainsData<T>() where T : AIBlackBoardData
        {
            return GetBBData<T>() != null;
        }
    }
}
