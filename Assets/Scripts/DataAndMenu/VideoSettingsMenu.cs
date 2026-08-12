using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class VideoSettingsMenu : MonoBehaviour
{
    public Toggle fullscreenToggle;

    Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // Cargar guardado
        int savedRes = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        fullscreenToggle.isOn = savedFullscreen;

        ApplySettings();
    }

    public void SetResolution(int index)
    {
        PlayerPrefs.SetInt("ResolutionIndex", index);
        ApplySettings();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        ApplySettings();
    }

    void ApplySettings()
    {
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0) == 1;

    }
}
