using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_Tactica;

//Manage the audio player on the First Person Controller (INITIAL LEVEL)
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource audio_source;
    public AudioClip actual_clip = null;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audio_source = this.GetComponent<AudioSource>();
    }

    //Play the actual clip
    public void PlayClip(AudioClip current_clip)
    {
        if(actual_clip != null)
        {
            if(audio_source.isPlaying)
            {
                audio_source.Stop();
            }
        }
        else
        {
            SetActualClip(current_clip);
            
        }

        audio_source.Play();
    }

    public void SetActualClip(AudioClip current_clip)
    {
        audio_source.clip = current_clip;
    }

}
