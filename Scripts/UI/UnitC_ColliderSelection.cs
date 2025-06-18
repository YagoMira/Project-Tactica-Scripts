using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class UnitC_ColliderSelection : MonoBehaviour
    {
        GameController game_controller;

        private void Start()
        {
            game_controller = gameObject.transform.parent.parent.gameObject.GetComponentInParent<UIManager>().game_logic.GetComponent<GameController>();
        }

        public void OnTriggerStay(Collider collider)
        {
            GameObject element;

            if (collider.gameObject != null)
            {
                element = collider.gameObject;

                if(game_controller.camera_controller.actual_level == 3)
                {
                    if (element.GetComponent<Element>() != null)
                    {
                        if (element.GetComponent<Element>().type == Element.elem_type.Unit)
                        {
                            if (element.GetComponent<Unit>().onDie != true) //Check if the Unit is alive
                            {
                                if(element.CompareTag("Player") == true)
                                {
                                    //Debug.Log("NPC GET!!");

                                    if (!game_controller.units_list.Contains(collider.gameObject))
                                        game_controller.units_list.Add(collider.gameObject);

                                    //game_controller.visualSelection(true);
                                }
                            }
                        }

                    }
                }
                else if (game_controller.camera_controller.actual_level == 2) //#TO-DO: Box selection on LEVEL 2
                {

                }


            }

        }

    }
}
