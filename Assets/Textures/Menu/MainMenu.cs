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


    GameObject page;

    private void Start()
    {
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
