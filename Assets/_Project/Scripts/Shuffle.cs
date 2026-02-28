using UnityEngine;

public static class Shuffle
{
    public static void Shuffle_List<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
