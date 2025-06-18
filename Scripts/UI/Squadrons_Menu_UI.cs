using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Project_Tactica
{
    public class Squadrons_Menu_UI : MonoBehaviour
    {
        //Animators
        Animator panel_animator;

        //SYSTEMS
        GameController game_controller;
        Squadrons_Squads_Menu squadrons_squads_menu;

        private void Start()
        {
            game_controller = GameObject.Find("Game Logic").GetComponent<GameController>();
            squadrons_squads_menu = this.gameObject.transform.GetChild(0).Find("Squads").GetComponent<Squadrons_Squads_Menu>();

            GetUIElements();
        }

        public void GetUIElements()
        {
            string panel_str = "Panel";
            GameObject panel;

            //Get Panel
            panel = this.gameObject.transform.Find(panel_str).gameObject;
            panel_animator = this.gameObject.GetComponent<Animator>(); //panel.GetComponent<Animator>();

        }

        public void showPanel(bool enabled)
        {
            bool actual_enabled = panel_animator.GetBool("enabled");

            if (panel_animator != null)
            {
                if (actual_enabled == true)
                {
                    panel_animator.SetBool("enabled", false);
                    squadrons_squads_menu.AddUnitsSprites();
                }
                else
                {
                    panel_animator.SetBool("enabled", true);
                    //squadrons_squads_menu.AddUnitsSprites();
                }

            }
        }

        public void deactivatePanel(bool enabled)
        {
            bool actual_enabled = panel_animator.GetBool("enabled");

            if (actual_enabled == true)
            {
                panel_animator.SetBool("enabled", false);
            }
        }


        public void ActivateSquadronsUI(float enabled)
        {
            GameObject main_panel = this.gameObject.transform.GetChild(0).gameObject;

            if (game_controller.squadron_controller.num_squadrons > 0)
            {
                GameObject squads_panel = main_panel.transform.Find("Squads").gameObject;

                for (int i = 0; i < game_controller.squadron_controller.num_squadrons; i++)
                {
                    if (enabled == 1.0f) //Use float and not bool because animation/animator
                        squads_panel.transform.GetChild(i).gameObject.SetActive(true);
                    else if (enabled == 0.0f)
                        squads_panel.transform.GetChild(i).gameObject.SetActive(false);
                }

            }
        }



    }
}
