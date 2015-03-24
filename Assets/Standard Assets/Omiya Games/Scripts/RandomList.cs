using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RandomList<T> : List<T>
{
	int[] randomizedIndexes = null;
	int index = int.MinValue;

    public T CurrentElement
    {
        get
        {
            T returnElement = default(T);
            if(Count > 0)
            {
                // Check if I need to setup a list
                if(randomizedIndexes == null)
                {
                    SetupList();
                    OmiyaGamesUtility.ShuffleList<int>(randomizedIndexes);
                    index = 0;
                }
                else if(randomizedIndexes.Length != Count)
                {
                    SetupList();
                    OmiyaGamesUtility.ShuffleList<int>(randomizedIndexes);
                    index = 0;
                }
                else if((index >= randomizedIndexes.Length) || (index < 0))
                {
                    // Shuffle the list if we got to the last element
                    OmiyaGamesUtility.ShuffleList<int>(randomizedIndexes);
                    index = 0;
                }

                // Grab the current element
                returnElement = this[randomizedIndexes[index]];
            }
            return returnElement;
        }
    }

	public T RandomElement
	{
		get
		{
			T returnElement = default(T);
			if(Count > 0)
			{
                ++index;
                returnElement = CurrentElement;
			}
			return returnElement;
		}
	}

    public void Reshuffle()
    {
        index = int.MinValue;
    }

    #region Helper Methods

	void SetupList()
	{
		// Generate a new list, populated with entries based on frequency
		randomizedIndexes = new int[Count];
        for(index = 0; index < randomizedIndexes.Length; ++index)
		{
            randomizedIndexes[index] = index;
		}
    }

	#endregion
}
