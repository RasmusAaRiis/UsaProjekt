using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI fovText;
    public TextMeshProUGUI sfxText;
    public TextMeshProUGUI musicText;

    [Header("Settings Dial References")]
    public Slider fovSlider;
    public Slider sfxSlider;
    public Slider musicSlider;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;

    GameObject page;

    Resolution[] resolutions;

    CharacterController player;

    private void Start()
    {
        if (FindObjectOfType<CharacterController>())
        {
            player = FindObjectOfType<CharacterController>();
        }

        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();
        List<string> dropdownOptions = new List<string>();

        int currentOption = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            dropdownOptions.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentOption = i;
            }
        }

        if (PlayerPrefs.HasKey("ResolutionValue"))
        {
            currentOption = PlayerPrefs.GetInt("ResolutionValue");
        }

        PlayerPrefs.SetInt("ResolutionValue", currentOption);
        resolutionDropdown.AddOptions(dropdownOptions);
        resolutionDropdown.value = currentOption;
        resolutionDropdown.RefreshShownValue();

        if (!PlayerPrefs.HasKey("FOV"))
        {
            PlayerPrefs.SetInt("FOV", 90);
        }
        if (!PlayerPrefs.HasKey("SFXVolume"))
        {
            PlayerPrefs.SetFloat("SFXVolume", 1f);
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 0.8f);
        }
        if (!PlayerPrefs.HasKey("QualityLevel"))
        {
            PlayerPrefs.SetInt("QualityLevel", 1);
        }

        fovSlider.value = PlayerPrefs.GetInt("FOV");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        qualityDropdown.value = PlayerPrefs.GetInt("QualityLevel");

        fovText.SetText(fovSlider.value.ToString());
        sfxText.SetText(Mathf.RoundToInt(sfxSlider.value * 100).ToString());
        musicText.SetText(Mathf.RoundToInt(musicSlider.value * 100).ToString());

        SetFullscreen(true);
        ChangeResolution();
    }

    private void Update()
    {
        fovText.SetText(fovSlider.value.ToString());
        sfxText.SetText(Mathf.RoundToInt(sfxSlider.value * 100).ToString());
        musicText.SetText(Mathf.RoundToInt(musicSlider.value * 100).ToString());

        if (player)
        {
            player.GetComponentInChildren<Camera>().fieldOfView = fovSlider.value;
        }

        AudioManager.instance.sfxVolume = sfxSlider.value;
        AudioManager.instance.musicVolume = musicSlider.value;
        QualitySettings.SetQualityLevel(qualityDropdown.value);

        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ChangeResolution()
    {
        Resolution resolution = resolutions[resolutionDropdown.value];
        PlayerPrefs.SetInt("ResolutionValue", resolutionDropdown.value);
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        if (isFullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    public void ChangePage(GameObject newPage)
    {
        if (page)
        {
            page.SetActive(false);
        }

        if (page != newPage)
        {
            page = newPage;
            page.SetActive(true);
        }
        else
        {
            page = null;
        }
    }

    public void Continue()
    {
        player.Pause(false);
        player.paused = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void ChangeFov()
    {
        fovText.SetText(fovSlider.value.ToString());
        PlayerPrefs.SetInt("FOV", (int)fovSlider.value);
        PlayerPrefs.Save();
    }

    public void ChangeQuality()
    {
        PlayerPrefs.SetInt("QualityLevel", qualityDropdown.value);
        QualitySettings.SetQualityLevel(qualityDropdown.value);
    }

    public void changeMusicVolume()
    {
        musicText.SetText(Mathf.RoundToInt((musicSlider.value * 100)).ToString());
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        AudioManager.instance.musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        PlayerPrefs.Save();
    }

    public void changeSFXVolume()
    {
        sfxText.SetText(Mathf.RoundToInt((sfxSlider.value * 100)).ToString());
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        AudioManager.instance.sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        PlayerPrefs.Save();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("BlomScene", LoadSceneMode.Single);
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("TutorialLevel");
    }

    public void KnapKnap()
    {
        Debug.Log("Knap Knap");
        AudioManager.instance.PlayOneShot(FMODEvents.instance.buttonButton, Camera.main.transform.position);
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
