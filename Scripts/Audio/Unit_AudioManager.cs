using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Project_Tactica
{
    public class Unit_AudioManager : MonoBehaviour
    {
        public AudioClip moving_audio;
        public AudioClip shoot_audio;
        public AudioClip received_damage_audio;

        public AudioSource moving_src;
        public AudioSource shoot_src;
        public AudioSource received_damage_src;

        private void Awake()
        {
            AudioSource[] audio_srcs;
            audio_srcs = this.GetComponents<AudioSource>();

            moving_src = audio_srcs[0];
            shoot_src = audio_srcs[1];
            received_damage_src = audio_srcs[2];

            moving_src.clip = moving_audio;
            shoot_src.clip = shoot_audio;
            received_damage_src.clip = received_damage_audio;
        }

    }
}
