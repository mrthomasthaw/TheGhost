using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BlackBoardManager
{
    public Dictionary<BlackBoardKey, object> Data {  get; private set; }

    public BlackBoardManager() 
    {
        Data = new Dictionary<BlackBoardKey, object>();
    }

    public void AddData<T>(T data, BlackBoardKey key)
    {
        if (data == null)
            return;

        if(Data.ContainsKey(key))
        {
            List<T> list = (List<T>) Data[key];
            list.Add(data);
        }
        else
        {
            Data.Add(key, new List<T> { data });
        }
    }

    public void RemoveData<T>(T data, BlackBoardKey key)
    {
        if (data == null)
            return;


        if (!Data.ContainsKey(key))
        {
            return;
        }

        List<T> list = (List<T>)Data[key];
        list.Remove(data);
    }

    public void RemoveLastData<T>(BlackBoardKey key)
    {

        if (! Data.ContainsKey(key)) 
        {
            return;
        }

        List<T> list = (List<T>)Data[key];

        if(list.Count > 0)
            list.RemoveAt(list.Count - 1);
    }

    public void RemoveAllByKey(BlackBoardKey key)
    {
        Data.Remove(key);
    }

    public List<T> GetAllDataByKey<T>(BlackBoardKey key)
    {
        if (!Data.ContainsKey(key)) return EmptyList<T>.Value;
        return Data[key] as List<T>;
    }

    public T GetOneDataByKey<T>(BlackBoardKey key) where T : class
    {
        if (!Data.ContainsKey(key)) return null;
        List<T> list = Data[key] as List<T>;
        return list.LastOrDefault();
    }

    public override string ToString()
    {
        return $"{{ DataList = {CommonUtil.StringJoin(Data)} }}";
    }
}
