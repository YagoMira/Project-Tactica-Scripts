using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Tactica
{
    public class Squadrons_Menu : MonoBehaviour
    {
        public UIManager ui_manager;
        public SquadronController squadron_controller;

        public GameObject squadron_menu_object; //Squadron Canvas
        public GameObject squadron_menu_units; //Squadron Units Canvas
        public GameObject squadron_menu_units_prefab; //Squadron Units Prefab
        public GameObject squadron_menu_units_individual_prefab; //Squadron Units - Individual Unit Prefab

        //BUTTONS
        string squad_creation_panel_name = "Squadron_Creation";
        string squad_interaction_panel_name = "Squadron_Interaction";
        string squad_elimination_panel_name = "Squadron_Elimination";

        public List<Button> squadron_creation_menu_buttons;
        public List<Button> squadron_interaction_menu_buttons;
        public List<Button> squadron_elimination_menu_buttons;

        //SQUADRONS UI
        public float squadron_unit_ui_y_position = 180.0f;
        public float squadron_unit_ui_y_spacing = 100.0f;

        public float squadron_unit_indv_ui_y_spacing = 20.0f;

        void Awake() //TAKE CARE WITH THE AWAKE IN THIS SCRIPT - DONE THIS WAY BECAUSE THE MENU BUTTONS DOESN'T BEING RECOGNIZES AT "START"
        {
            ui_manager = this.GetComponentInParent<UIManager>();
            squadron_controller = ui_manager.game_logic.GetComponent<SquadronController>();

            ManageSquadUI();
            AddSquadUI();
        }


        //----------------------BUTTONS ACTIONS----------------------//
        void onCreateSquadronButtonClick(string button_name)
        {
            int squad_number = int.Parse(button_name);
            Debug.Log("SQUAD_NUMBER::: " + squad_number);
            squadron_controller.createPlayerSquadron(squad_number); //############################### MAYBE OPTIMIZE THE GET COMPONENT

            //UPDATE SQUADRON UI
            UpdateSquadUnitsUI();
        }

        void onInteractSquadronButtonClick(string button_name)
        {
            int squad_number = int.Parse(button_name);
            squadron_controller.selectPlayerSquadron(squad_number, false); //############################### MAYBE OPTIMIZE THE GET COMPONENT
        }
        
        void onDeleteSquadronButtonClick(string button_name)
        {
            int squad_number = int.Parse(button_name);
            squadron_controller.deleteSquadron(squad_number); //############################### MAYBE OPTIMIZE THE GET COMPONENT
        }


        //----------------------BUTTONS ACTIONS----------------------//

        //----------------------SQUADRON UI MANAGEMENT----------------------//

        //Get the squadron management buttons        
        public void ManageSquadUI()
        {
            //squadron_menu_object = gameObject.transform.GetChild(0).gameObject; //Get the Unit Menu (Panel)
            Transform panel_squadrons_transform = squadron_menu_object.transform;

            for (int i = 0; i < panel_squadrons_transform.childCount; i++)
            {
                //WITH ARRAYS:
                //squadron_creation_menu_buttons = squadron_menu_object.transform.Find(squad_creation_panel_name).GetComponentsInChildren<Button>();
                //squadron_interaction_menu_buttons = squadron_menu_object.transform.Find(squad_interaction_panel_name).GetComponentsInChildren<Button>();
                //squadron_elimination_menu_buttons = squadron_menu_object.transform.Find(squad_elimination_panel_name).GetComponentsInChildren<Button>();

                //WITH LISTS:
                squadron_creation_menu_buttons.Add(panel_squadrons_transform.GetChild(i).transform.Find(squad_creation_panel_name).GetComponentInChildren<Button>());
                squadron_interaction_menu_buttons.Add(panel_squadrons_transform.GetChild(i).transform.Find(squad_interaction_panel_name).GetComponentInChildren<Button>());
                squadron_elimination_menu_buttons.Add(panel_squadrons_transform.GetChild(i).transform.Find(squad_elimination_panel_name).GetComponentInChildren<Button>());
            }
        }

        public void AddSquadUI() //Add each of squadron units ui to the general panel
        {
            for(int i = 0; i < squadron_controller.num_squadrons; i++)
            {
                GameObject ui_squadron = Instantiate(squadron_menu_units_prefab, squadron_menu_units.transform, worldPositionStays: false);

                //PROPERTIES - SQUADRON::

                //Name
                ui_squadron.name = "Squadron_" + i.ToString();
                ui_squadron.transform.GetChild(0).GetComponent<Text>().text = ui_squadron.name; //Get Squadron_ID

                //Position
                Vector3 ui_position = ui_squadron.transform.localPosition;
                ui_position.y = squadron_unit_ui_y_position;
                ui_squadron.transform.localPosition = ui_position;
                squadron_unit_ui_y_position -= squadron_unit_ui_y_spacing;

                //PROPERTIES - SQUADRON/UNITS::
                //AddIndividualUnitUI(ui_squadron, i);
            }

        }

        public void UpdateSquadUnitsUI()
        {
            GameObject ui_squadron_units = squadron_menu_units;
            int squadrons_ui_number = ui_squadron_units.transform.childCount;


            for (int i = 0; i < squadrons_ui_number; i++)
            {
                int ui_squadron_individual_unit_count = ui_squadron_units.transform.GetChild(i).GetChild(1).transform.childCount;

                for(int j = 0; j < ui_squadron_individual_unit_count; j++)
                {
                    GameObject.Destroy(ui_squadron_units.transform.GetChild(i).GetChild(1).transform.GetChild(j).gameObject); //Remove all individual Unit UI
                }

                AddIndividualUnitUI(ui_squadron_units.transform.GetChild(i).gameObject, i); //Add all individual Unit UI
            }
        }

        public void AddIndividualUnitUI(GameObject ui_squadron, int squad_number)
        {
            float squadron_unit_indv_ui_y_position = 30.0f;
            

            GameObject ui_squadron_units = ui_squadron.transform.GetChild(1).gameObject; //Get Squadron_Units
            int squadron_units_count = squadron_controller.countUnitsSquadron(squadron_controller.squadrons, squad_number);

            //Debug.Log("SQUADRON: " + squad_number + " UNITS: " + squadron_units_count);

            for (int j = 0; j < squadron_units_count; j++)
            {
                GameObject ui_squadron_individual_unit = Instantiate(squadron_menu_units_individual_prefab, ui_squadron_units.transform, worldPositionStays: false);


                //PROPERTIES - SQUADRON INDIVIDUAL UNIT::

                //Name
                ui_squadron_individual_unit.name = squadron_controller.squadrons[squad_number].squadron[j].name; //"Unit_" + j.ToString();
                ui_squadron_individual_unit.transform.GetChild(0).GetComponent<Text>().text = ui_squadron_individual_unit.name; //Get Unit_ID

                //Position
                Vector3 ui_position = ui_squadron_individual_unit.transform.localPosition;
                ui_position.y = squadron_unit_indv_ui_y_position;
                ui_squadron_individual_unit.transform.localPosition = ui_position;
                squadron_unit_indv_ui_y_position -= squadron_unit_indv_ui_y_spacing;

                //BUTTON LISTENERS:

                AddButtonListeners_Units(ui_squadron_individual_unit.transform.GetChild(1).GetComponent<Button>(), squad_number, j); //Child (1) == Button_Captain

                //Other properties

                //Make the captain button invisible if the actual unit is already the captain
                string button_captain = "Button_Captain";

                if (ui_squadron_individual_unit.transform.Find(button_captain) != null)
                {
                    if (squadron_controller.squadrons[squad_number].hasGeneral == true)
                    {
                        ui_squadron_individual_unit.transform.Find(button_captain).gameObject.SetActive(false);
                    }
                    else
                    {
                        if (squadron_controller.isCaptain(squadron_controller.squadrons[squad_number].squadron[j]) != -1)
                        {
                            ui_squadron_individual_unit.transform.Find(button_captain).gameObject.SetActive(false);
                        }
                        else
                        {
                            ui_squadron_individual_unit.transform.Find(button_captain).gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
        //----------------------SQUADRON UI MANAGEMENT----------------------//

        //----------------------MENU MANAGEMENT----------------------//

        //Hide or show the menu
        public void showMenu(bool enabled)
        {
            this.gameObject.SetActive(enabled);
        }

        //Add the actions to the desired Menu Buttons
        public void AddButtonListeners()
        {

            //ADD SQUADRON CREATION BUTTON LISTENERS
            int btnCount_squadron_creation = squadron_creation_menu_buttons.Count;

            for (int i = 0; i < btnCount_squadron_creation; i++)
            {
                string actual_button = squadron_creation_menu_buttons[i].name;
                squadron_creation_menu_buttons[i].onClick.AddListener(() => onCreateSquadronButtonClick(actual_button));
            }


            //ADD SQUADRON INTERACTION BUTTON LISTENERS
            int btnCount_squadron_interaction = squadron_interaction_menu_buttons.Count;

            for (int i = 0; i < btnCount_squadron_interaction; i++)
            {
                string actual_button = squadron_interaction_menu_buttons[i].name;
                squadron_interaction_menu_buttons[i].onClick.AddListener(() => onInteractSquadronButtonClick(actual_button));
            }

            //ADD SQUADRON ELIMINATION BUTTON LISTENERS
            int btnCount_squadron_elimination = squadron_elimination_menu_buttons.Count;

            for (int i = 0; i < btnCount_squadron_interaction; i++)
            {
                string actual_button = squadron_elimination_menu_buttons[i].name;
                squadron_elimination_menu_buttons[i].onClick.AddListener(() => onDeleteSquadronButtonClick(actual_button));
            }

        }

        //Add the actions to the desired Menu -Individual Squadron Unit- Buttons
        public void AddButtonListeners_Units(Button unit_button, int squad_number, int unit_number) //TO-DO: SELECT UNIT ON NAME CLICK
        {
            unit_button.onClick.AddListener(() => squadron_controller.addCaptainSquadron(squadron_controller.squadrons, squad_number, unit_number)); //Get the Player Squadrons
            unit_button.onClick.AddListener(() => UpdateSquadUnitsUI());
        }

        //----------------------MENU MANAGEMENT----------------------//

    }

}
