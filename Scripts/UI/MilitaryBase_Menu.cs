using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Tactica
{
    public class MilitaryBase_Menu : MonoBehaviour
    {
        public UIManager ui_manager;
        public BaseManagementController base_management_controller;
        public BaseSquadrons_Menu base_squadrons_menu;
        public SquadronController squadron_controller;

        public GameObject military_interaction_panel;

        //BUTTONS
        string military_interaction_panel_name = "MilitaryBase_Interaction"; 
        string squad_creation_btn_name = "Squadron_Creation";

        public Button squadron_creation_menu_button;

        void Awake() //TAKE CARE WITH THE AWAKE IN THIS SCRIPT - DONE THIS WAY BECAUSE THE MENU BUTTONS DOESN'T BEING RECOGNIZES AT "START"
        {
            ui_manager = this.GetComponentInParent<UIManager>();

            squadron_controller = ui_manager.game_logic.GetComponent<SquadronController>();
            base_management_controller = ui_manager.game_logic.GetComponent<BaseManagementController>();
        }


        //----------------------BUTTONS ACTIONS----------------------//
        void onCreateSquadronButtonClick(int military_base_number)
        {
            MilitaryBase m_base = base_management_controller.military_base_object.GetComponent<MilitaryBase>();

            GameObject selected_unit = base_squadrons_menu.selected_unit;

            if(squadron_controller.countUnitsSquadron(squadron_controller.squadrons, base_squadrons_menu.selected_squad) == squadron_controller.num_units)
            {
                Debug.LogError("MAXIMUM UNITS REACHED IN SQUADRON: " + base_squadrons_menu.selected_squad);
            }
            else
            {
                string selected_unit_name = (selected_unit.gameObject.name.ToString());
                string cost = selected_unit.gameObject.transform.Find("Cost").GetComponent<TMP_Text>().text.ToString();
                float final_cost = float.Parse(cost) / 10.0f; //Division by 10.0f because the .0f of the string text

                //Debug.Log(selected_unit_name + " COST: " + cost);

                //m_base.spawnSquadron(military_base_number);
                //Debug.Log("BASE_NUMBER::: " + military_base_number);
                //squadron_controller.createSquadron(squad_number); //############################### MAYBE OPTIMIZE THE GET COMPONENT

                m_base.SpawnUnit(selected_unit_name, final_cost, base_squadrons_menu.selected_squad);

                base_squadrons_menu.selected_unit = null;
                base_squadrons_menu.enableRecruitButton();
            }
           
        }

        //----------------------BUTTONS ACTIONS----------------------//


        //----------------------MENU MANAGEMENT----------------------//



        //Hide or show the menu
        public void showMenu(bool enabled)
        {
            this.gameObject.SetActive(enabled);

            if (enabled == false)
            {
                //base_management_controller.military_base_object.transform.Find("Bounds").gameObject.SetActive(false);
                base_management_controller.setMilitaryBase(null);
                base_management_controller.military_base = false;
            }
            else
            {
               //base_squadrons_menu.AddUnitsSprites();
               base_management_controller.military_base_object.transform.Find("Bounds").GetChild(0).gameObject.SetActive(true);
            }
        }

        //Add the actions to the desired Menu Buttons
        public void AddButtonListeners()
        {
            //squadron_creation_menu_button = military_interaction_panel.transform.Find(squad_creation_btn_name).GetComponentInChildren<Button>();
            squadron_creation_menu_button.onClick.AddListener(() => onCreateSquadronButtonClick(base_management_controller.getMilitaryBaseNumber()));
        }

        //----------------------MENU MANAGEMENT----------------------//
    }
}
