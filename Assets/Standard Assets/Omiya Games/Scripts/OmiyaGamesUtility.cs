using UnityEngine;
using System.Collections;

public static class OmiyaGamesUtility
{
    /// <summary>
    /// Shuffles the list.
    /// </summary>
    /// <param name="list">The list to shuffle.</param>
    /// <typeparam name="H">The list type parameter.</typeparam>
    public static void ShuffleList<H>(H[] list)
    {
        int index = 0, randomIndex = 0;
        H swapObject = default(H);
        for(; index < list.Length; ++index)
        {
            // Swap a random element
            randomIndex = Random.Range(0, list.Length);
            if(index != randomIndex)
            {
                swapObject = list[index];
                list[index] = list[randomIndex];
                list[randomIndex] = swapObject;
            }
        }
    }
}
