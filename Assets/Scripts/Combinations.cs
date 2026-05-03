using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combinations : MonoBehaviour
{
    private static readonly List<List<int>> answer = new();
    private static readonly List<int> tempList = new();

    public static void CreateCombinationsUtil(int n, int left, int k)
    {
        if (k == 0)
        {
            var newList = new List<int>();
            for (int i = 0; i < tempList.Count; i++)
            {
                newList.Add(tempList[i]);
            }
            answer.Add(newList);
            return;
        }

        for (int i = left; i <= n; i++)
            {
                tempList.Add(i);

                CreateCombinationsUtil(n, i + 1, k - 1);

                tempList.RemoveAt(tempList.Count - 1);
            }
    }

    public static List<List<int>> CreateCombinations(int n, int k)
    {
        CreateCombinationsUtil(n, 1, k);
        return answer;
    }
}
