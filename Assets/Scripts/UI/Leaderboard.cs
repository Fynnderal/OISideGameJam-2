using UnityEngine;
using System.Collections.Generic;

// class for serializing leaderboard stats. inactive yet.
[System.Serializable]
public class Leaderboard
{
    public List<SaveData> saves;

    public Leaderboard()
    {
        saves = new List<SaveData>();
    }

}
