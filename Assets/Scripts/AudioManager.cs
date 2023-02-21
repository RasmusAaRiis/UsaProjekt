using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{

    //Declare variabler
    public static AudioManager instance { get; private set; }
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private void Awake()
    {
        //Kast en fejl hvis der er mere end én audiomanager aktiv i scenen
        if (instance != null)
        {
            Debug.LogError("Hov du har mere end 1 audio manager >:--(");
        }
        instance = this;

    }

    private void Start()
    {
        //Preload de to banks med henholdsvis musik og effekter, så de ikke skal loades hver gang en lyd afspilles
        RuntimeManager.LoadBank("Music", true);
        RuntimeManager.LoadBank("Effects", true);

    }

    public void Update()
    {
        //Audiomanagerens volume styrer FMOD's volume, der er knyttet til hver banks lydstyrke
        SetParameter("EffectsVolume", sfxVolume);
        SetParameter("MusicVolume", musicVolume);
    }

    //To forskellige metoder til afspilning af såkaldte "oneshot" lyde, der spilles én gang
    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    
    //Metode til nemt at sætte parametre, som er FMODs svar på variabler
    public void SetParameter(string parameterName, float parameterValue)
    {
        RuntimeManager.StudioSystem.setParameterByName(parameterName, parameterValue);
    }
}
