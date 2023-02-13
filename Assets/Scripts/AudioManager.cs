using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private EventInstance musicInstance; 

    public static AudioManager instance { get; private set; }

    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Hov du har mere end 1 audio manager >:--(");
        }
        instance = this;

    }

    private void Start()
    {
        RuntimeManager.LoadBank("Music", true);
        RuntimeManager.LoadBank("Effects", true);

    }

    public void Update()
    {
        SetParameter("EffectsVolume", sfxVolume);
        SetParameter("MusicVolume", musicVolume);
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



    public void SetParameter(string parameterName, float parameterValue)
    {
        RuntimeManager.StudioSystem.setParameterByName(parameterName, parameterValue);
    }
}
