using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class LiberateSystem : MonoBehaviour
    {
        public bool liberate_system_activated = false;

        public float speed = 2.0f; //STARTS AS HIGH VALUE INTO THE ANIMATOR (Nervous)

        public GameObject x_character;
        public GameObject enemy_character;

        public GameObject main_camera;
        public GameObject liberate_camera;


        public Animator x_animator;
        public Animator enemy_animator;


        //GAME CONTROLLER & OTHER SYSTEMS
        public GameController game_controller;

        private void Awake()
        {
            if(x_character != null)
                x_animator = x_character.GetComponent<Animator>();
            if (enemy_character != null)
                enemy_animator = enemy_character.GetComponent<Animator>();

            main_camera = Camera.main.gameObject;
            game_controller = this.gameObject.GetComponent<GameController>();

            //////
            ///

            if (liberate_system_activated == true)
            {
                //setPulse(game_controller.social_controller.general_deaths);
                changeCamera(0);
            }
            else
            {
                changeCamera(1);
            }
        }

        private void Start()
        {
            Time.timeScale = 1.0f;
            enableDialogueSystem();
        }

        private void Update()
        {
          
        }

        //Change the speed of the animation to simulate the player pulse (Event Manager variable to count the deaths)
        public void setPulse(int deaths)
        {
            speed = speed - ((float)deaths / 10.0f); //TO-DO: Limit on DEATHS!
            //Debug.Log("SPEED: " + speed);

            if (x_animator != null)
                x_animator.SetFloat("pulse", speed); //Get the pulse parameter from the animator
        }

        public void changeCamera(int flag)
        {
            if(liberate_camera != null)
            {
                if (flag == 0) //Deactivate MainCamera - Activate LiberateSystem Camera
                {
                    main_camera.SetActive(false);
                    liberate_camera.SetActive(true);
                    game_controller.disable_input = true;

                }
                else if (flag == 1)  //Deactivate LiberateSystem Camera - Activate MainCamera
                {
                    main_camera.SetActive(true);
                    liberate_camera.SetActive(false);
                    //game_controller.disable_input = false;
                }
            }
            else
            {
                Debug.LogError("LIBERATE CAMERA NOT SET UP");
            }
           
        }

        public void enableDialogueSystem()
        {
            if(game_controller.ui_manager != null)
            {
                if (game_controller.ui_manager.dialogue_menu != null)
                {
                    //game_controller.ui_manager.dialogue_system_enabled = liberate_system_activated;
                    game_controller.ui_manager.dialogue_menu.liberate_system_activated = liberate_system_activated;
                    game_controller.ui_manager.dialogue_menu.activateDialogueType(liberate_system_activated);
                }
            }
        }
    }
}
