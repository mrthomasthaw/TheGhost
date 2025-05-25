using MrThaw;
using MrThaw.Goap.AIMemory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MrThaw.Goap.AIActions;

public static class CommonUtil
{
    public static float CalculateInputAmount(float verticalInput, float horizontalInput)
    {
        return Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
    }


    public static Transform FindDeepestChildByName(Transform parent, string childName)
    {
        Transform deepestChild = null;
        int maxDepth = -1;

        // Iterate over all child transforms of the parent
        foreach (Transform child in parent)
        {
            // Recursively check for the deepest match
            Transform foundChild = FindDeepestChildByName(child, childName);

            // If this child matches the name and is deeper, update the deepestChild
            if (foundChild != null)
            {
                int childDepth = GetDepth(foundChild);
                if (childDepth > maxDepth)
                {
                    maxDepth = childDepth;
                    deepestChild = foundChild;
                }
            }
        }

        // Check if the current child has the target name and is deeper than the previous deepest
        if (parent.name == childName)
        {
            int parentDepth = GetDepth(parent);
            if (parentDepth > maxDepth)
            {
                maxDepth = parentDepth;
                deepestChild = parent;
            }
        }

        return deepestChild;
    }

    public  static string DictToString(Dictionary<object, object> dict)
    {
        return "{" + string.Join(", ", dict.Select(kv => $"{kv.Key}: {kv.Value}")) + "}";
    }



    public static string StringJoin<T>(IEnumerable<T> list)
    {
        return string.Join(", ", list);
    }

    public static string DictToString(Dictionary<string, object> dict)
    {
        return "{" + string.Join(", ", dict.Select(kv => $"{kv.Key}: {kv.Value}")) + "}";
    }

    public static Transform FindRootTransform(Transform transform)
    {
        Transform parent = transform.parent;
        while(parent != null)
        {
            if (parent.parent == null)
                return parent;

            parent = parent.parent;
        }

        return parent;
    }

    public static int GetDepth(Transform transform)
    {
        int depth = 0;
        while (transform.parent != null)
        {
            depth++;
            transform = transform.parent;
        }
        return depth;
    }



    public  static string AIActionListToString(List<AIAction> list)
    {
        return "[" + string.Join(", ", list) + "]";
    }

    public static void CopyWorldStates(Dictionary<string, object> fromData, Dictionary<string, object> toData)
    {
        foreach (KeyValuePair<string, object> kv in fromData)
        {
            if (!toData.ContainsKey(kv.Key))
            {
                toData.Add(kv.Key, kv.Value);
            }
            else
            {
                toData[kv.Key] = kv.Value;
            }
        }
    }

    public static bool MatchWorldStates(Dictionary<string, object> conditions, Dictionary<string, object> worldStateToCheck)
    {
        foreach (KeyValuePair<string, object> kv in conditions)
        {
            if (!worldStateToCheck.ContainsKey(kv.Key))
            {
                return false;
            }

            if (!worldStateToCheck[kv.Key].Equals(kv.Value))
            {
                return false;
            }

        }

        return true;
    }
}
