using UnityEngine;
using UnityEngine.Audio;

// singleton for managing audio mixer
public static class AudioManager
{
    [SerializeField]
    public static AudioMixer mixer;

    public static void SetMasterVolume(float linearVolume)
    {
        if (mixer != null) mixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(linearVolume, 0.0001f, 1f)) * 20f);
    }

    public static void SetSFXVolume(float linearVolume)
    {
        if (mixer != null) mixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(linearVolume, 0.0001f, 1f)) * 20f);
    }

    public static void SetMusicVolume(float linearVolume)
    {
        if (mixer != null) mixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(linearVolume, 0.0001f, 1f)) * 20f);
    }
}
