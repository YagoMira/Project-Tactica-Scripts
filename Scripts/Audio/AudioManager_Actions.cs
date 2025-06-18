using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class AudioManager_Actions : MonoBehaviour
    {
        public AudioClip[] movement;
        public AudioSource source;

        public int getRandomClip()
        {
            if (movement.Length > 0)
                return Random.Range(1, movement.Length);
            else
                return 0;
        }

        public void PlayClip()
        {
            source.clip = movement[getRandomClip()];
            source.Play();
        }
    }

}
