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

    GameObject page;

    public void ChangePage(GameObject newPage)
    {
        if (page)
        {
            page.SetActive(false);
        }

        page = newPage;
        page.SetActive(true);
    }

    public void ChangeFov(Slider slider)
    {
        Debug.Log("FOV: " + (int)slider.value);
        PlayerPrefs.SetInt("FOV", (int)slider.value);
        PlayerPrefs.Save();
    }

    public void ChangeQuality(TMP_Dropdown dropdown)
    {
        Debug.Log("Quality: " + dropdown.value);
        QualitySettings.SetQualityLevel(dropdown.value);
    }

    public void changeMusicVolume(Slider slider)
    {
        Debug.Log("Music Volume: " + slider.value);
        AudioManager.instance.musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        PlayerPrefs.SetFloat("MusicVolume", slider.value);
        PlayerPrefs.Save();
    }

    public void changeSFXVolume(Slider slider)
    {
        Debug.Log("SFX Volume: " + slider.value);
        AudioManager.instance.sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        PlayerPrefs.SetFloat("SFXVolume", slider.value);
        PlayerPrefs.Save();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("BlomScene");
    }

    public void Tutorial()
    {
        Debug.Log("Tutorial");
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
