using UnityEngine;
using System.Collections.Generic;

public static class Helper
{
    private static Dictionary<string, Dictionary<int, float>> weightSets = new Dictionary<string, Dictionary<int, float>>();
    private static float weightIncrement = 0.2f;
    private static float weightDecrease = 0.5f;

    public static int GenerateWeightedRandom(string key, int startNumber, int endNumber)
    {
        int count = endNumber - startNumber + 1;
        if (!weightSets.ContainsKey(key) || weightSets[key].Count != count)
        {
            Dictionary<int, float> newWeights = new Dictionary<int, float>();
            for (int i = startNumber; i <= endNumber; i++)
                newWeights[i] = 1f;
            weightSets[key] = newWeights;
        }

        Dictionary<int, float> weights = weightSets[key];
        float totalWeight = 0f;
        for (int i = startNumber; i <= endNumber; i++)
            totalWeight += weights[i];

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        int chosen = startNumber;
        for (int i = startNumber; i <= endNumber; i++)
        {
            cumulative += weights[i];
            if (randomValue <= cumulative)
            {
                chosen = i;
                break;
            }
        }

        weights[chosen] = Mathf.Max(0f, weights[chosen] - weightDecrease);
        for (int i = startNumber; i <= endNumber; i++)
        {
            if (i != chosen)
                weights[i] += weightIncrement;
        }

        return chosen;
    }

    public static T Find<T>(string name) where T : Component
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            return obj.GetComponent<T>();
        }
        return null;
    }
    public static Transform FindChildByNameContains(this Transform parent, string substring)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(substring))
                return child;
            Transform found = child.FindChildByNameContains(substring);
            if (found != null)
                return found;
        }
        return null;
    }
}
