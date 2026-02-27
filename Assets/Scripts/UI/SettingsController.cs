using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// controlling settings ui
public class SettingsController : MonoBehaviour
{
    [SerializeField] GameObject videoContainer;
    [SerializeField] GameObject audioContainer;

    [SerializeField] Dropdown resolutionDropdown;
    [SerializeField] Dropdown fullscreenDropdown;

    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;

    Resolution[] availableResolutions;

    void Start()
    {
        if (SettingsManager.globalSettings == null)
        {
            SettingsManager.loadSettings();
        }

        OnVideoClick();

        if (resolutionDropdown != null)
            PopulateResolutions();

        if (fullscreenDropdown != null)
            PopulateFullscreenDropdown();

        // Audio sliders from settings
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = SettingsManager.globalSettings.masterVolume;
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = SettingsManager.globalSettings.sfxVolume;
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = SettingsManager.globalSettings.musicVolume;

    }

    void PopulateFullscreenDropdown()
    {
        if (fullscreenDropdown == null) return;

        fullscreenDropdown.ClearOptions();

        List<string> options = new List<string>();
        foreach (FullScreenMode mode in Enum.GetValues(typeof(FullScreenMode)))
        {
            options.Add(mode.ToString());
        }

        fullscreenDropdown.AddOptions(options);

        int savedIndex = 0;
        if (SettingsManager.globalSettings != null)
        {
            savedIndex = Mathf.Clamp((int)SettingsManager.globalSettings.windowMode, 0, options.Count - 1);
        }

        fullscreenDropdown.value = savedIndex;
        fullscreenDropdown.RefreshShownValue();
    }


    void PopulateResolutions()
    {
        availableResolutions = Screen.resolutions;
        if (resolutionDropdown == null || availableResolutions == null || availableResolutions.Length == 0) return;

        resolutionDropdown.ClearOptions();
        var options = new List<string>();
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            Resolution r = availableResolutions[i];
            options.Add($"{r.width} x {r.height} @ {r.refreshRateRatio.value}Hz");
        }
        resolutionDropdown.AddOptions(options);

        int selectedIndex = 0;

        // Use SettingsManager.globalSettings to find the selected index
        if (SettingsManager.globalSettings != null)
        {
            int wantW = SettingsManager.globalSettings.width;
            int wantH = SettingsManager.globalSettings.height;
            uint wantNum = SettingsManager.globalSettings.refreshRateNum;
            uint wantDen = SettingsManager.globalSettings.refreshRateDen;

            for (int i = 0; i < availableResolutions.Length; i++)
            {
                Resolution r = availableResolutions[i];
                if (r.width == wantW && r.height == wantH &&
                    r.refreshRateRatio.numerator == wantNum &&
                    r.refreshRateRatio.denominator == wantDen)
                {
                    selectedIndex = i;
                    break;
                }
            }
        }

        resolutionDropdown.value = Mathf.Clamp(selectedIndex, 0, availableResolutions.Length - 1);
        resolutionDropdown.RefreshShownValue();
    }


    public void OnVideoClick()
    {
        SetActiveContainers(videoContainer, audioContainer);
    }

    public void OnAudioClick()
    {
        SetActiveContainers(audioContainer, videoContainer);
    }

    public void OnApplyClick()
    {
        if (SettingsManager.globalSettings == null)
            return;

        if (masterVolumeSlider != null)
            SettingsManager.globalSettings.masterVolume = masterVolumeSlider.value;

        if (sfxVolumeSlider != null)
            SettingsManager.globalSettings.sfxVolume = sfxVolumeSlider.value;

        if (musicVolumeSlider != null)
            SettingsManager.globalSettings.musicVolume = musicVolumeSlider.value;

        SettingsManager.ApplyAudioSettings();

        if (resolutionDropdown != null && availableResolutions != null && availableResolutions.Length > 0)
        {
            int idx = Mathf.Clamp(resolutionDropdown.value, 0, availableResolutions.Length - 1);
            Resolution r = availableResolutions[idx];
            SettingsManager.globalSettings.width = r.width;
            SettingsManager.globalSettings.height = r.height;
            SettingsManager.globalSettings.refreshRateNum = r.refreshRateRatio.numerator;
            SettingsManager.globalSettings.refreshRateDen = r.refreshRateRatio.denominator;
        }

        if (fullscreenDropdown != null)
            SettingsManager.globalSettings.windowMode = (FullScreenMode)fullscreenDropdown.value;

        SettingsManager.ApplyScreenSettings();

        SettingsManager.saveSettings();

        Debug.Log("Settings Applied & Saved");
    }

    void SetActiveContainers(GameObject active, GameObject off)
    {
        if (active != null) active.SetActive(true);
        if (off != null) off.SetActive(false);
    }


}

