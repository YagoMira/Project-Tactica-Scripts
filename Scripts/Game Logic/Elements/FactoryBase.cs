using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class FactoryBase : Element
    {
        //Values
        public float credits = 25.0f;
        public float seconds = 5.0f;

        //OTHER SYSTEMS
        public GameObject game_logic_object;
        GameController game_controller;

        public GameObject conquest_limitation;

        //UnitsBounds bounds;

        public void Awake()
        {
            game_logic_object = GameObject.Find("Game Logic");
            game_controller = game_logic_object.GetComponent<GameController>();
        }

        public FactoryBase(string name, string description, elem_type type, float health) : base(name, description, type, health)
        {
            this.type = elem_type.Edification;
        }


        public void SetSystem(bool enabled)
        {
            game_controller.ui_manager.factory_base_system_enabled = enabled;
        }

        public IEnumerator IncreaseCredits(float seconds)
        {
            while (faction == Factions.VIRION)
            {
                game_controller.resources_controller.increaseCredits(credits);

                game_controller.ui_manager.CallOtherMenuSystems(true, false, credits);

                yield return new WaitForSeconds(seconds);
            }
        }

        private void OnMouseOver()
        {
            if (game_controller.ui_manager.over_ui_g == false)
            {
                if (game_controller.input.mouse_left_click_press) //Open the Factory base menu
                {
                    game_controller.ui_manager.factory_base_system_enabled = true;
                }

                if (this.gameObject.GetComponent<Outline>() != null)
                {
                    this.gameObject.GetComponent<Outline>().OutlineWidth = 5.0f;
                }

            }


            if (conquest_limitation != null)
            {
                conquest_limitation.SetActive(true);
            }
        }

        private void OnMouseExit()
        {
            if (this.gameObject.GetComponent<Outline>() != null)
            {
                this.gameObject.GetComponent<Outline>().OutlineWidth = 0.0f;
            }

            if (conquest_limitation != null)
            {
                conquest_limitation.SetActive(false);
            }
        }
    }
}
