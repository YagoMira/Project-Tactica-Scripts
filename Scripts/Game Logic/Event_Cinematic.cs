using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class Event_Cinematic : MonoBehaviour
    {
        public GameObject[] event_gameObjects;

        //#TO-DO: Polish and use this class as the Event for the EventManager (to enable/disable objects)
        private void Awake()
        {
            //GetEventObjects();
        }

        public void GetEventObjects()
        {
            int event_objects = this.transform.childCount;

            event_gameObjects = new GameObject[event_objects];

            for(int i = 0; i < event_objects; i++)
            {
                event_gameObjects[i] = this.transform.GetChild(i).gameObject;
            }
        }
    }
}
