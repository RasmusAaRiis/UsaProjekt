using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private EventInstance musicInstance; 

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Hov du har mere end 1 audio manager >:--(");
        }
        instance = this;
        RuntimeManager.LoadBank("Music");
    }

    private void Start()
    {


    }

    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }

    private void InitMusic(EventReference sound)
    {
        musicInstance = CreateInstance(sound);
        ChangeMusicParameter("Situation", 0f);
        musicInstance.start();
        Debug.Log("music started");
    }

    public void ChangeMusicParameter(string parameterName, float parameterValue)
    {
        musicInstance.setParameterByName(parameterName, parameterValue);
    }
}