using UnityEngine;

//class for serializing saves
[System.Serializable]
public class SaveData
{
    public int level = 0;
    public int checkpoint = 0;
    public bool finished = false;

    public int deaths = 0;
    public double playtime = 0;
    public int health = 100;

    public string name;

    public SaveData(int level, int checkpoint, int deaths, double playtime = 0, int health = 200)
    {
        this.level = level;
        this.checkpoint = checkpoint;
        this.deaths = deaths;
        this.playtime = playtime;
        this.health = (int)health;
    }
}
