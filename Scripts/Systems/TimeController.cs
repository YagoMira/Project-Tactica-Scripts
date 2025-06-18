using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class TimeController : MonoBehaviour
    {
        //Stop the time
        public void stopTime()
        {
            //Debug.Log("Time is going to be stopped...");
            Time.timeScale = 0;
        }

        //In case of the time of the time system is stopped, reanudate it
        public void resumeTime()
        {
            Time.timeScale = 1;
        }

        //Increase the time after the interaction with the timer system (x2 or x4 speed)
        public void speedTime(int speed)
        {
            Time.timeScale = speed;
        }
    }
}
