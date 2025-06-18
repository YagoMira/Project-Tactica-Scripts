using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Tactica
{
    public class UnitC_Menu : MonoBehaviour
    {
        public GameObject ui_manager_object;
        public UIManager ui_manager;

        GameObject unit_menu_object; //Selection Unit Canvas

        public Button[] unit_menu_buttons;

        private void Start()
        {
            ui_manager = ui_manager_object.GetComponent<UIManager>();
            unit_menu_object = gameObject.transform.GetChild(0).gameObject; //Get the Unit Menu (Panel)
            unit_menu_buttons = unit_menu_object.GetComponentsInChildren<Button>();

            if (ui_manager_object.activeSelf == false) //Prevent possible bugs in the subMenus.
                ui_manager_object.SetActive(true);
        }

        private void Update()
        {
            //transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, Camera.main.transform.rotation, Time.deltaTime);
        }


        //----------------------BUTTONS ACTIONS----------------------//
        void onMoveButtonClick()
        {
            ui_manager.game_logic.GetComponent<GameController>().actionOnSelectedUnits(Vector3.zero, null); //############################### MAYBE OPTIMIZE THE GET COMPONENT
        }

        void onAttackButtonClick()
        {
            ui_manager.game_logic.GetComponent<GameController>().actionOnSelectedUnits(ui_manager.game_logic.GetComponent<GameController>().last_clicked_enemy.transform.position, ui_manager.game_logic.GetComponent<GameController>().last_clicked_enemy); //############################### MAYBE OPTIMIZE THE GET COMPONENT
        }

        void onLiberateButtonClick()
        {
            ui_manager.game_logic.GetComponent<GameController>().actionOnSelectedAlmah(ui_manager.game_logic.GetComponent<GameController>().last_clicked_enemy); //############################### MAYBE OPTIMIZE THE GET COMPONENT
        }

        //----------------------BUTTONS ACTIONS----------------------//





        //----------------------MENU MANAGEMENT----------------------//
        //Check if an Enemy is being selected and activate the button Attack on it
        public void attackButtonActivate(GameObject last_clicked_enemy)
        {
            int btnCount = unit_menu_buttons.Length;

            for (int i = 0; i < btnCount; i++)
            {
                if (unit_menu_buttons[i].name == "Attack") //In case of "Move" Action Button
                {
                    if (last_clicked_enemy == null) //Enemy is NOT being selected
                    {
                        unit_menu_buttons[i].interactable = false;
                    }
                    else //Enemy IS being selected
                    {
                        unit_menu_buttons[i].interactable = true;
                    }
                }
            }
        }

        //Check if an Enemy ALMAH is being selected and activate the button Liberate on it
        public void liberateButtonActivate(GameObject last_clicked_enemy)
        {
            int btnCount = unit_menu_buttons.Length;

            for (int i = 0; i < btnCount; i++)
            {

                if (unit_menu_buttons[i].name == "Liberate") //In case of "Liberate" Action Button
                {
                    if (last_clicked_enemy == null) //Enemy is NOT being selected
                    {
                        unit_menu_buttons[i].interactable = false;
                    }
                    else
                    {
                        Almah almah = last_clicked_enemy.GetComponent<Almah>();

                        if (almah != null)
                        {

                            if(almah.on_liberate == true)
                                unit_menu_buttons[i].interactable = true;

                        }
                    }
                }
            }
        }

        //Repositionate the Unit Menu
        public void positionateMenu(Vector3 last_clicked_position, GameObject unit_menu)
        {
            if (last_clicked_position != null)
                unit_menu.transform.position = new Vector3(last_clicked_position.x, Camera.main.transform.position.y - 110.0f, last_clicked_position.z);
            //unit_menu.transform.position = new Vector3(last_clicked_position.x, last_clicked_position.y + 150.0f, last_clicked_position.z);

        }

        //Show the Unit Menu
        public void showMenu(bool leftAction, bool rightAction, bool unitIsSelected, bool onUI)
        {

            if(unitIsSelected == true) //Are Units in the Unit List (Unit Selection)
            {
                //If press the Left Mouse Click -> Hide otherwhise Show the menu
                if (leftAction == true && onUI == false)
                {
                    this.gameObject.SetActive(false);
                }         
                else if (rightAction == true)
                {
                    this.gameObject.SetActive(true);
                }
                    
            }
            else
            {
                this.gameObject.SetActive(false);
            }    

        }

        //Add the actions to the desired Menu Buttons (TAKE CARE WITH THE MULTIPLE CANVAS! (Canvas Raycast Disabled in Main))
        public void AddButtonListeners()
        {

            int btnCount = unit_menu_buttons.Length;

            for (int i = 0; i < btnCount; i++)
            {
                if(unit_menu_buttons[i].name == "Move") //In case of "Move" Action Button
                {
                    unit_menu_buttons[i].onClick.AddListener(onMoveButtonClick);
                }
                else if (unit_menu_buttons[i].name == "Attack") //In case of "Attack" Action Button
                {
                    unit_menu_buttons[i].onClick.AddListener(onAttackButtonClick);
                }
                else if (unit_menu_buttons[i].name == "Liberate") //In case of "Attack" Action Button
                {
                    unit_menu_buttons[i].onClick.AddListener(onLiberateButtonClick);
                }
                //...
                //*TO-DO: Add the other action functions
            }
        }

        //----------------------MENU MANAGEMENT----------------------//
    }
}
