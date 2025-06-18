using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class MedicalBase : Element
    {
        //Values
        public float healing = 10.0f;
        public float credits = 25.0f;
        public float seconds = 5.0f;

        //OTHER SYSTEMS
        public GameObject game_logic_object;
        GameController game_controller;

        UnitsBounds bounds;

        public bool heal_applied = false;

        public void Awake()
        {
            game_logic_object = GameObject.Find("Game Logic");
            game_controller = game_logic_object.GetComponent<GameController>();
            bounds = this.gameObject.transform.Find("Bounds").gameObject.GetComponent<UnitsBounds>();
        }

        public MedicalBase(string name, string description, elem_type type, float health) : base(name, description, type, health)
        {
            this.type = elem_type.Edification;
        }


        public void SetSystem(bool enabled)
        {
            game_controller.ui_manager.medical_base_system_enabled = enabled;
        }

        private void Update()
        {
            if(bounds.units_inside.Count > 0)
            {
                if(heal_applied == false)
                    StartCoroutine(ApplyHeal(seconds));
            }
        }

        IEnumerator ApplyHeal(float seconds)
        {
            heal_applied = true;

            int num_squadrons = bounds.CountAvaliableSquadrons();

            if (game_controller.resources_controller.credits >= credits)
            {
                //for (int i = 0; i < num_squadrons; i++)
                //{
                for (int x = 0; x < bounds.units_inside.Count; x++)
                {
                    if (bounds.units_inside[x].health != bounds.units_inside[x].max_health)
                    {
                        if (bounds.units_inside[x].onDie != true)
                        {
                            bounds.units_inside[x].ApplyHeal(healing);
                            game_controller.resources_controller.decreaseCredits(credits); //Apply the credit decrease (Num of squadrons inside * credits value)

                        }

                    }
                }

                //}
            }
            else
            {
                Debug.LogError("NOT ENOUGH CREDITS!");
            }
          

            yield return new WaitForSeconds(seconds);

            heal_applied = false;
        }

        private void OnMouseOver()
        {
            if (game_controller.ui_manager.over_ui_g == false)
            {
                if (game_controller.input.mouse_left_click_press) //Open the medical base menu
                {
                    game_controller.ui_manager.medical_base_system_enabled = true;
                }

                if (this.gameObject.GetComponent<Outline>() != null)
                {
                    this.gameObject.GetComponent<Outline>().OutlineWidth = 5.0f;

                    this.gameObject.transform.Find("Bounds").GetChild(0).gameObject.SetActive(true);
                }
            }

        }

        private void OnMouseExit()
        {
            if (this.gameObject.GetComponent<Outline>() != null)
            {
                this.gameObject.GetComponent<Outline>().OutlineWidth = 0.0f;

                this.gameObject.transform.Find("Bounds").GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
