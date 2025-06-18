using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Video;

namespace Project_Tactica
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager event_manager_instance;
        public GameObject camera_effects_obj;
        public Volume camera_effects;

        public int actual_event = 0;
        public bool start_from_init = false; //Start dialogue from EVENT 0
        
        public GameObject events_parent;
        GameObject[] events;

        //Other Systems
        GameController game_controller;
        UIManager ui_controller;
        Dialogue_Menu dialogue_menu;

        //BG MUSIC
        public GameObject BG_Music;

        private void Awake()
        {
            event_manager_instance = this;
        }

        private void Start()
        {
            game_controller = GameObject.Find("Game Logic").GetComponent<GameController>();
            ui_controller = GameObject.Find("UI Manager").GetComponent<UIManager>();
            dialogue_menu = GameObject.Find("Dialogue Menu").GetComponent<Dialogue_Menu>();

            int childs = events_parent.transform.childCount;
            events = new GameObject[childs];

            camera_effects = camera_effects_obj.GetComponent<Volume>();

            GetAllCinematics();
            StartEvents();
        }

        public void ShowAllCinematics(bool activate)
        {
            for (int i = 0; i < events.Length; i++)
            {
                events[i].SetActive(activate);
            }
        }

        //#TO-DO: Polish, should get the cinematic by exclusively id, not the array index one
        public GameObject GetCinematic(int cinematic_id)
        {
            return events[cinematic_id];
        }

        public void ActivateCinematic(int cinematic_id, bool activate)
        {
            events[cinematic_id].SetActive(activate);
        }

        public void GetAllCinematics()
        {
            for (int i = 0; i < events.Length; i++)
            {
                events[i] = events_parent.transform.GetChild(i).transform.gameObject;
            }
        }

        public void StartEvents()
        {
            if(start_from_init == true)
                StartCoroutine(Event_0()); //ENABLE FOR START THE DIALOGUE FROM ITS ORIGIN
        }

        //Manage the proper event (Control if a dialogue is finished)
        /*
        public void ManageEvents(int actual_event, bool finish_event)
        {
            //AT THE MOMENT ANY CONTROL OF EVENT_0 IS NOT NECESSARY (STARTS WITH THE GAME!!!)
            StopAllCoroutines();

            if (actual_event == 1)
            {
                if (finish_event == false)
                {
                    StartCoroutine(Event_1());
                }
            }
            else if (actual_event == 2)
            {
                if (finish_event == false)
                {
                    StartCoroutine(Event_2());
                }
            }
            else if (actual_event == 3)
            {
                if (finish_event == false)
                {
                    StartCoroutine(Event_3());
                }
            }
            else if (actual_event == 4)
            {
                if(finish_event == false)
                {
                    StartCoroutine(Event_4());
                }
                else
                {
                    Event_4_FINISH();
                }
            }
        }
        */
        public void ManageEvents(int actual_event)
        {
            //Debug.Log("EVENT: " + actual_event);
            //AT THE MOMENT ANY CONTROL OF EVENT_0 IS NOT NECESSARY (STARTS WITH THE GAME!!!)
            StopAllCoroutines();

            if (actual_event == 1)
            {
                StartCoroutine(Event_1());
            }
            else if (actual_event == 2)
            {
                StartCoroutine(Event_2());
            }
            else if (actual_event == 3)
            {
                StartCoroutine(Event_3());
            }
            else if (actual_event == 4)
            {
                StartCoroutine(Event_4());
            }
            else if (actual_event == 5)
            {
                StartCoroutine(Event_5());
            }
            else if (actual_event == 7)
            {
                StartCoroutine(Event_7());
            }
            else if (actual_event == 9)
            {
                StartCoroutine(Event_8_FINISH());
                StartCoroutine(WaitTimeFinishLASTCinematic(8));
            }

        }


        IEnumerator Event_0()
        {
            //DISABLE PLAYER INPUT
            game_controller.disable_input = true;
            //game_controller.gameObject.GetComponent<TimeController>().stopTime();

            GameObject cinematic_0 = GetCinematic(0);
            cinematic_0.SetActive(true);

            GameObject bg = cinematic_0.transform.GetChild(0).transform.Find("Background").gameObject;
            bg.GetComponent<Animator>().SetBool("transition", true);

            ui_controller.dialogue_system_enabled = false;
            yield return new WaitForSeconds(2.0f);
            ui_controller.dialogue_system_enabled = true;
            yield return new WaitForSeconds(0.1f);

            if (dialogue_menu.gameObject.activeSelf == true)
                dialogue_menu.StartCoroutine("DelayStart", 0.0f);

            //Apply Effects to Camera
            DepthOfField dph;
            if (camera_effects.profile.TryGet<DepthOfField>(out dph))
            {
                dph.active = true;
            }
        }

        IEnumerator Event_1()
        {
            //DISABLE PLAYER INPUT
            game_controller.disable_input = true;

            yield return new WaitForSeconds(0.1f);

            ActivateCinematic(0, false);

            Debug.Log("EVENTO 1");

            //ENABLE BG MUSIC
            BG_Music.SetActive(true);

            GameObject cinematic_1 = GetCinematic(1);
            cinematic_1.SetActive(true);

            GameObject bg = cinematic_1.transform.GetChild(0).transform.Find("Background").gameObject;
            bg.GetComponent<Animator>().SetBool("transition", true);
        }

        IEnumerator Event_2()
        {
            //DISABLE PLAYER INPUT
            game_controller.disable_input = true;

            yield return new WaitForSeconds(0.1f);

            ActivateCinematic(1, false);

            Debug.Log("EVENTO 2");

            GameObject cinematic_2 = GetCinematic(2);
            cinematic_2.SetActive(true);

            GameObject bg = cinematic_2.transform.GetChild(0).transform.Find("Background").gameObject;
            bg.GetComponent<Animator>().SetBool("transition", true);
        }

        IEnumerator Event_3()
        {
            //DISABLE PLAYER INPUT
            game_controller.disable_input = true;

            yield return new WaitForSeconds(0.1f);

            ActivateCinematic(2, false);

            GameObject cinematic_3 = GetCinematic(3);
            cinematic_3.SetActive(true);

            GameObject bg = cinematic_3.transform.GetChild(0).transform.Find("Background").gameObject;
            bg.GetComponent<Animator>().SetBool("transition", true);

            GameObject cinematic_video = cinematic_3.transform.GetChild(0).transform.Find("Cinematic").gameObject;
            VideoPlayer cinematic_player = cinematic_video.GetComponent<VideoPlayer>();

            cinematic_player.Play();
        }

        IEnumerator Event_4()
        {
            //DISABLE PLAYER INPUT
            game_controller.disable_input = true;

            yield return new WaitForSeconds(0.1f);

            ActivateCinematic(3, false);
        }

        IEnumerator Event_5() //FINISH DIALOGUE ON "WORLD" SCENE
        {
            //DISABLE PLAYER INPUT
            game_controller.disable_input = true;

            //DISABLE DIALOGUE
            ui_controller.dialogue_system_enabled = false; //#TO-DO: POLISH!

            yield return new WaitForSeconds(0.1f);

            ActivateCinematic(4, false);

            //Debug.Log("EVENTO 5");
            
            GameObject cinematic_5 = GetCinematic(5);
            cinematic_5.SetActive(true);

            GameObject bg = cinematic_5.transform.GetChild(0).transform.Find("Background").gameObject;
            bg.GetComponent<Animator>().SetBool("transition", true);


            //DISABLE BG MUSIC
            BG_Music.SetActive(false);

            //GameObject.Find("Dialogue Menu").gameObject.SetActive(false);

            //Apply Effects to Camera
            DepthOfField dph;
           if (camera_effects.profile.TryGet<DepthOfField>(out dph))
           {
               dph.active = false;
           }

            yield return new WaitForSeconds(0.2f);
            cinematic_5.SetActive(false);

            //ENABLE PLAYER INPUT
            game_controller.disable_input = false;

            game_controller.ui_manager.tutorial.SetActive(true);


            //ENABLE SYSTEMS
            game_controller.ui_manager.tactica_system_enabled = true;
            game_controller.ui_manager.resources_system_enabled = true;
            game_controller.ui_manager.military_base_system_enabled = true;
            game_controller.ui_manager.time_system_enabled = true;
            game_controller.ui_manager.squadrons_system_ui_enabled = true;
            game_controller.ui_manager.options_enabled = true;

            //ENABLE PLAYER INPUT
            game_controller.disable_input = false;

            //ENABLE MUSIC
            AudioManager.instance.audio_source.Stop();
            AudioManager.instance.audio_source.Play();
            AudioManager.instance.audio_source.mute = false;

        }

        IEnumerator Event_7()
        {

            Debug.Log("EVENT 7");

            GameObject cinematic_7 = GetCinematic(7);
            cinematic_7.SetActive(true);

            if(cinematic_7.GetComponent<Event_Cinematic>() != null)
            {
                GameObject[] event_objects = cinematic_7.GetComponent<Event_Cinematic>().event_gameObjects;

                event_objects[0].SetActive(true); //HAIR OF VARIANT
                event_objects[1].SetActive(false); //HELMET OF VARIANT
            }

            GameObject bg = cinematic_7.transform.GetChild(0).transform.Find("Background").gameObject;
            bg.GetComponent<Animator>().SetBool("transition", true);


            //DISABLE BG MUSIC
            BG_Music.SetActive(false);

            yield return new WaitForSeconds(0.2f);
            cinematic_7.SetActive(false);

        }

        public void Event_4_FINISH()
        {
            StopAllCoroutines();

            GameObject cinematic_4 = GetCinematic(4);
            cinematic_4.SetActive(true);

            GameObject bg = cinematic_4.transform.GetChild(0).transform.Find("Background").gameObject;
            bg.GetComponent<Animator>().SetBool("transition", true);

            //DISABLE DIALOGUE


            ui_controller.dialogue_system_enabled = false;
            //GameObject.Find("Dialogue Menu").gameObject.SetActive(false);


            //Apply Effects to Camera
            DepthOfField dph;
            if (camera_effects.profile.TryGet<DepthOfField>(out dph))
            {
                dph.active = false;
            }

            cinematic_4.SetActive(false);

            //ENABLE SYSTEMS
            game_controller.ui_manager.tactica_system_enabled = true;
            game_controller.ui_manager.resources_system_enabled = true;
            game_controller.ui_manager.military_base_system_enabled = true;
            game_controller.ui_manager.time_system_enabled = true;
            game_controller.ui_manager.squadrons_system_ui_enabled = true;
            game_controller.ui_manager.options_enabled = true;

            //ENABLE PLAYER INPUT
            game_controller.disable_input = false;

            //ENABLE MUSIC
            AudioManager.instance.audio_source.mute = false;

        }

        IEnumerator Event_8_FINISH()
        {
            StopAllCoroutines();

            game_controller.disable_input = true;

            GameObject cinematic_8 = GetCinematic(8);
            cinematic_8.SetActive(true);

            //DEACTIVE THE CARACTERS
            GameObject.Find("BB").SetActive(false);
            GameObject.Find("X").SetActive(false);
            //ACTIVE MAIN CAMERA
            //GameObject.Find("Main Camera").SetActive(true);

            GameObject bg = cinematic_8.transform.GetChild(0).transform.Find("Background").gameObject;
            bg.GetComponent<Animator>().SetBool("transition", true);

            //DISABLE DIALOGUE
            ui_controller.dialogue_system_enabled = false;

            GameObject cinematic_video = cinematic_8.transform.GetChild(0).transform.Find("Cinematic").gameObject;
            VideoPlayer cinematic_player = cinematic_video.GetComponent<VideoPlayer>();

            cinematic_player.Play();

            yield return new WaitForSeconds(0.2f);
            //cinematic_8.SetActive(false);

            // yield return new WaitForSeconds(0.2f);


        }

        IEnumerator WaitTimeFinishLASTCinematic(int event_id)
        {
            float video_length = (float)EventManager.event_manager_instance.GetCinematicByEvent(event_id).length;

            yield return new WaitForSeconds(video_length);

            Debug.Log("LOAD START MENU!");

            string scene_name = "Start_Menu_v2";
            game_controller.LoadScene(scene_name);
        }


        public VideoPlayer GetCinematicByEvent(int event_id)
        {
            GameObject cinematic_video = GetCinematic(event_id).transform.GetChild(0).transform.Find("Cinematic").gameObject;
            VideoPlayer cinematic_player = cinematic_video.GetComponent<VideoPlayer>();

            return cinematic_player;
        }

        /*
        public IEnumerator WaitFinishCinematic(VideoPlayer video)
        {
            long playerCurrentFrame = video.frame;
            long playerFrameCount = Convert.ToInt64(video.frameCount);

            while(playerCurrentFrame < playerFrameCount)
            {
                playerCurrentFrame = video.frame;
                playerFrameCount = Convert.ToInt64(video.frameCount);


                if (playerCurrentFrame < playerFrameCount)
                {
                    print("VIDEO IS PLAYING");
                }
                else
                {
                    print("VIDEO IS OVER");
                }

                yield return new WaitUntil(() => playerCurrentFrame >= playerFrameCount);
            }
               
        }
        */

    }
}

