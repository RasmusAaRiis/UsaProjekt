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

    [Header("FOV settings")]
    public int minFOV;
    public int maxFOV;

    [Header("Settings Dial References")]
    public Slider fovSlider;
    public Slider sfxSlider;
    public Slider musicSlider;
    public TMP_Dropdown qualityDropdown;


    GameObject page;

    private void Awake()
    {
        fovSlider.value = PlayerPrefs.GetInt("FOV");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        qualityDropdown.value = PlayerPrefs.GetInt("QualityLevel");
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

    public void ChangeFov()
    {
        Debug.Log("FOV: " + (int)fovSlider.value);
        PlayerPrefs.SetInt("FOV", (int)fovSlider.value);
        PlayerPrefs.Save();
    }

    public void ChangeQuality()
    {
        Debug.Log("Quality: " + qualityDropdown.value);
        PlayerPrefs.SetInt("QualityLevel", qualityDropdown.value);
        QualitySettings.SetQualityLevel(qualityDropdown.value);
    }

    public void changeMusicVolume()
    {
        Debug.Log("Music Volume: " + musicSlider.value);
        AudioManager.instance.musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.Save();
    }

    public void changeSFXVolume()
    {
        Debug.Log("SFX Volume: " + sfxSlider.value);
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
        SceneManager.LoadScene("TutorialScene");
    }

    public void KnapKnap()
    {
        Debug.Log("Knap Knap");
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
