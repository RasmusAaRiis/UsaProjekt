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

    public void ChangeResolution()
    {
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
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
        AudioManager.instance.musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.Save();
    }

    public void changeSFXVolume()
    {
        sfxText.SetText(Mathf.RoundToInt((sfxSlider.value * 100)).ToString());
        AudioManager.instance.sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.Save();
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync("BlomScene", LoadSceneMode.Single);
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
