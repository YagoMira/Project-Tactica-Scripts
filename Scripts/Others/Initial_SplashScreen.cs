using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Project_Tactica;

public class Initial_SplashScreen : MonoBehaviour
{
    public GameObject audio;
    public GameObject main_menu;
    public GameObject effects;
    public VideoPlayer video;
    public GameObject bg_color;

    // Start is called before the first frame update
    void Start()
    {
        if(video.isPlaying == false)
        {
            video.Play();
            StartCoroutine(StartInitialScene());
        }
    }

    private IEnumerator StartInitialScene()
    {
        while (video.isPlaying == true)
        {
            if (bg_color.activeSelf == true)
            {
                yield return new WaitForSeconds(0.5f);

                bg_color.SetActive(false);
            }

            yield return null;
        }

        //Activate gameobjects when finish the first cutscene
        audio.SetActive(true);
        main_menu.SetActive(true);
        effects.SetActive(true);

        ApplyAudio();

        this.gameObject.SetActive(false);
    }

    public void ApplyAudio()
    {
        audio.GetComponent<AudioSource>().volume = DataSaver.effects_volume;
    }
}
