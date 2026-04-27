using System.Collections.Generic;

public abstract class ShuffleList
{
    public static List<T> ShuffleListItems<T>(List<T> inputList)
    {
        List<T> originalList = new List<T>();
        originalList.AddRange(inputList);
        List<T> randomList = new List<T>();

        System.Random r = new System.Random();
        int randomIndex = 0;
        while (originalList.Count > 0)
        {
            randomIndex = r.Next(0, originalList.Count);
            randomList.Add(originalList[randomIndex]);
            originalList.RemoveAt(randomIndex);
        }
        return randomList;
    }
}