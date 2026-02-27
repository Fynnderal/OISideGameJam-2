using UnityEngine;

//class for serializing settings
[System.Serializable]
public class Settings
{
    public float masterVolume;
    public float sfxVolume;
    public float musicVolume;

    public int width;
    public int height;
    public uint refreshRateNum;
    public uint refreshRateDen;
    public FullScreenMode windowMode;

    public Settings(int width, int height, uint refreshRateNum, uint refreshRateDen, FullScreenMode windowMode, float mastervolume = 1f)
    {
        this.width = width;
        this.height = height;
        this.refreshRateNum = refreshRateNum;
        this.refreshRateDen = refreshRateDen;
        this.windowMode = windowMode;
        this.masterVolume = mastervolume;
        sfxVolume = 0.75f;
        musicVolume = 0.5f;
}
}

