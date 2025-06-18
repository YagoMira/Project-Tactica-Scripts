using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace Project_Tactica
{
    public class UIManager : MonoBehaviour
    {
        //CORE SYSTEMS
        public GameObject game_logic;
        GameController game_controller;
        Input_Logic input;

        //OTHER SYSTEMS
        ResourcesController resources_controller;
        SquadronController squadron_controller;

        [HideInInspector]
        public UnitC_Selection_Board unit_selectionBoard; //Selection Unit Canvas
        UnitC_Menu unit_menu; //Selection Unit Canvas

        Resources_Menu resources_menu; //Resources Menu
        Squadrons_Menu squadrons_menu; //Squadrons Menu
        Squadrons_Menu_UI squadrons_menu_ui; //Squadrons Menu UI
        FactoryBase_Menu factory_base_menu; //Factory Base Menu
        MedicalBase_Menu medical_base_menu; //Medical Base Menu
        MilitaryBase_Menu military_base_menu; //Military Base Menu
        Time_Menu time_menu; //Time System Menu
        public Dialogue_Menu dialogue_menu; //Dialogue System Menu
        DialogueManager dialogue_manager; //Dialogue Manager System
        Tactica_Menu tactica_menu; //Resources Menu
        BaseManagementController base_manager; //Base Manager System
        CSVReader csv_data; //Dialogue CSV DATA
        public GameObject tutorial;
        public GameObject game_options;

        //*****OPTIONS******//
        //Option Window Values
        public Options_Values options;
        Values options_values;
        //*****OPTIONS******//

        int ui_layer;

        //CANVAS MANAGEMENT
        public GameObject unit_menu_panel;
        public GameObject not_navigable_menu_panel;
        public bool over_ui_g = false; //Use this bool in case of the mouse is over any UI element


        //SYSTEMS ENABLED
        public bool tactica_system_enabled = false;
        public bool resources_system_enabled = false;
        public bool squadrons_system_enabled = false;
        public bool squadrons_system_ui_enabled = false;
        public bool factory_base_system_enabled = false;
        public bool medical_base_system_enabled = false;
        public bool military_base_system_enabled = false;
        public bool time_system_enabled = false;
        public bool dialogue_system_enabled = false;
        public bool options_enabled = false;

        //SYSTEMS ENABLED

        //UNITS LINE GUIDANCE
        public GameObject line;

        //LOSE WINDOW
        public GameObject lose_window;
        

        void Start()
        {
            if (game_logic == null)
                game_logic = FindObjectOfType<GameController>().gameObject;

            input = game_logic.GetComponent<Input_Logic>();
            game_controller = game_logic.GetComponent<GameController>();

            resources_controller = game_logic.GetComponent<ResourcesController>();
            squadron_controller = game_logic.GetComponent<SquadronController>();

            unit_selectionBoard = this.GetComponent<UnitC_Selection_Board>();
            unit_menu = unit_menu_panel.GetComponent<UnitC_Menu>();
            resources_menu = this.GetComponentInChildren<Resources_Menu>();
            tactica_menu = this.GetComponentInChildren<Tactica_Menu>();
            squadrons_menu = this.GetComponentInChildren<Squadrons_Menu>();
            squadrons_menu_ui = this.GetComponentInChildren<Squadrons_Menu_UI>();
            factory_base_menu = this.GetComponentInChildren<FactoryBase_Menu>();
            medical_base_menu = this.GetComponentInChildren<MedicalBase_Menu>();
            military_base_menu = this.GetComponentInChildren<MilitaryBase_Menu>();
            time_menu = this.GetComponentInChildren<Time_Menu>();
            dialogue_menu = this.GetComponentInChildren<Dialogue_Menu>();
            dialogue_manager = this.GetComponentInChildren<DialogueManager>();
            dialogue_menu.dialogue_manager = dialogue_manager;
            base_manager = game_logic.GetComponent<BaseManagementController>();
            csv_data = this.GetComponent<CSVReader>();
            //tutorial = this.transform.Find("Tutorial").gameObject;

            //UNIT MENU
            unit_menu.AddButtonListeners();
            unit_menu_panel.SetActive(false);

            //SQUADRONS MENU
            squadrons_menu.AddButtonListeners();
            //squadrons_menu.squadron_menu_object.transform.parent.gameObject.SetActive(false);

            //MILITARY BASE MENU
            military_base_menu.AddButtonListeners();
            military_base_menu.gameObject.SetActive(false);

            //TIME SYSTEM MENU
            time_menu.AddButtonListeners();

            ui_layer = LayerMask.NameToLayer("UI");

            lose_window = this.gameObject.transform.Find("GameOver").gameObject;
        }

        void Update()
        {
            if(game_controller.disable_input == false)
            {
                over_ui_g = mouseOnUI();

                //Check if the Click happens over the UI or not
                if (input.mouse_left_click == true)
                {
                    if (over_ui_g == false)
                    {
                        game_controller.last_clicked_position = new Vector3(); //Clear the position for the UI Menus (Unit Menu)
                                                                               //game_controller.last_clicked_position_edification = new Vector3(); //Clear the position for the UI Menus (Squadron Menu)
                    }
                }

                //Selection Unit Box
                unit_selectionBoard.DrawVisualInput(input.mouse_left_click_press, input.mouse_left_click_hold, input.mouse_left_click_release, unitListChecker(), input.mousePosition); //Draw the Canvas Rectangle for Unit Selection

                //Unit Menu
                if (input.mouse_right_click == true)
                {
                    //Positionate the Unit menu
                    unit_menu.positionateMenu(game_controller.last_clicked_position, unit_menu_panel);

                    //Enemy clicked, activate the Attack button on it
                    unit_menu.attackButtonActivate(game_controller.last_clicked_enemy);

                    //Enemy ALMAH clicked, activate the Liberate button on it
                    unit_menu.liberateButtonActivate(game_controller.last_clicked_enemy);
                }


                //MILITARY BASE UI
                if (game_controller.base_management_controller.military_base == true)
                {
                    if (unit_selectionBoard.is_draw == false)
                    {
                        military_base_system_enabled = true; //TO-DO: USE THE SAME BOOLEAN TYPE FOR ALL SYSTEMS
                        military_base_menu.showMenu(true);
                    }
                }
                else
                {
                    military_base_system_enabled = false;
                    military_base_menu.showMenu(false);
                }

                if (unit_selectionBoard.is_draw == false)
                {
                    if (medical_base_menu != null)
                        medical_base_menu.showMenu(medical_base_system_enabled);

                    if (factory_base_menu != null)
                        factory_base_menu.showMenu(factory_base_system_enabled);
                }
                  
            }

            //In case where a Unit is selected, show the menu
            if (game_controller.not_navigable == false)
            {
                not_navigable_menu_panel.SetActive(false);
                unit_menu.showMenu(input.mouse_left_click, input.mouse_right_click_press, game_controller.units_list.Count > 0 ? true : false, over_ui_g);
            }
            else
            {
                not_navigable_menu_panel.SetActive(true);
                unit_menu.gameObject.SetActive(false);

                StartCoroutine(HideNotNavigable(2.0f));
            }
                

            //Show the Resources Menu
            resources_menu.showMenu(resources_controller.credits, resources_controller.fuel);

            //GET THE DATA FROM CSV TO THE DIALOGUE SYSTEM
            //if(dialogue_system_enabled == true)
                dialogue_manager.getDialogueData(csv_data.textData);
            //dialogue_menu.getTextToPrint(dialogue_manager.getPrinteableText());

            //GET THE DATA FROM CSV TO THE BASE SYSTEM
            if (military_base_system_enabled == true)
                base_manager.getSoldiersNameData(csv_data.textData_soldiers);

            //ENABLE-DISABLE SYSTEMS UI
            enableSystemUI();

            //TUTORIAL PANEL
            if(input.esc_press == true)
            {
                if(tutorial != null)
                {
                    if(tutorial.activeSelf == false)
                    {
                        tutorial.SetActive(true);
                    }
                }
                
            }

            TutorialWindow();
        }

        IEnumerator HideNotNavigable(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            not_navigable_menu_panel.SetActive(false);
            game_controller.not_navigable = false;
        }

        public void TutorialWindow()
        {
            if(tutorial.activeSelf == true)
            {
                time_menu.time_controller.stopTime();
            }
        }

        /*****OPTIONS PANELS*****/
        public void QuitGame()
        {
            Application.Quit();
        }

        //On click apply settings button Save the settings ON DATASAVER (And apply it!)
        public void SaveSettings()
        {
            if (options != null)
            {
                Values options_values = options.GetOptionsValues();
                this.options_values = options_values;

                DataSaver.SetAllValues(options.GetOptionsValues());

                /*
                //Get Language value
                SetLanguage(options_values.language);
                //Get music & effects volume
                SetVolumes(options_values.music_volume, options_values.effects_volume);
                //Get First Person FOV value
                SetOtherOptions(options_values.fov);
                */
            }
        }

        public void ApplySettings()
        {
            options.ApplyAudio(this.options_values.music_volume, this.options_values.effects_volume);
            options.ApplyLanguage();
        }
        /*****OPTIONS PANELS*****/

        //ENABLE-DISABLE SYSTEMS UI ON START
        void enableSystemUI()
        {
            tactica_menu.gameObject.SetActive(tactica_system_enabled);
            resources_menu.gameObject.SetActive(resources_system_enabled);
            squadrons_menu.gameObject.SetActive(squadrons_system_enabled);
            squadrons_menu_ui.gameObject.SetActive(squadrons_system_ui_enabled);
            military_base_menu.gameObject.SetActive(military_base_system_enabled);
            time_menu.gameObject.SetActive(time_system_enabled);
            dialogue_menu.gameObject.SetActive(dialogue_system_enabled);
            game_options.gameObject.SetActive(options_enabled);
        }


        //Check if the Mouse/Controller is over the UI
        bool mouseOnUI()
        {
            bool over_ui = false;

            if (EventSystem.current.currentSelectedGameObject != null)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    if (EventSystem.current.currentSelectedGameObject.layer == LayerMask.NameToLayer("UI"))
                    {
                        over_ui = true;
                    }
                }
            }
            else
            {
                over_ui = false;
            }

            return over_ui;
        }

        //Add or Remove new units to the current selection with the Shift and Ctrl keys respectively
        bool unitListChecker()
        {
            bool unit_list_checker = false; //Check the actual state of the Unit list (For remove or add new units with Ctrl/Shift keys press)

            if (input.ctrl_left_click == true)
                unit_list_checker = true;
            else if (input.shift_left_click == true)
                unit_list_checker = true;
            else
                unit_list_checker = false;

            return unit_list_checker;
        }

        //Auxiliar function for particular cases
        public void CallOtherMenuSystems(bool addCredits, bool decreaseCredits, float credits)
        {
            if(addCredits == true)
            {
                resources_menu.showAddCredits(credits);
            }

            if (decreaseCredits == true)
            {
                resources_menu.showDecreaseCredits(credits);
            }
        }



        //----------------------EXHAUSTIVE CHECK OF MOUSE ON UI----------------------//

        //Returns 'true' if we touched or hovering on Unity UI element.
        public bool IsPointerOverUIElement()
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults());
        }


        //Returns 'true' if we touched or hovering on Unity UI element.
        private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == ui_layer)
                    return true;
            }
            return false;
        }


        //Gets all event system raycast results of current mouse or touch position.
        static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }

        //----------------------EXHAUSTIVE CHECK OF MOUSE ON UI----------------------//

    }
}
