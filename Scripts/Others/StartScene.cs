using Project_Tactica;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class StartScene : MonoBehaviour
{
    public GameObject audio;
    public GameObject audio_rain;
    public GameObject player;
    public VideoPlayer video;
    public GameObject wnoise_Audio;

    // Start is called before the first frame update
    void Start()
    {
        if(video.isPlaying == false)
        {
            video.SetDirectAudioVolume(0, DataSaver.effects_volume);
            video.Play();
            StartCoroutine(StartInitialScene());
        }
    }

    private IEnumerator StartInitialScene()
    {
        while (video.isPlaying == true)
        {
            yield return null;
        }

        //Activate gameobjects when finish the first cutscene
        audio.SetActive(true);
        audio_rain.SetActive(true);
        player.SetActive(true);

        ApplyAudio();


        wnoise_Audio.SetActive(true);

    }

    public void ApplyAudio()
    {
        audio.GetComponent<AudioSource>().volume = DataSaver.effects_volume;
        audio_rain.GetComponent<AudioSource>().volume = DataSaver.effects_volume;
        player.GetComponent<AudioSource>().volume = DataSaver.effects_volume;
    }
}
