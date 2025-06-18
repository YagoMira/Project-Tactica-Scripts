using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Tactica
{
    public class Time_Menu : MonoBehaviour
    {
        public UIManager ui_manager;
        public TimeController time_controller;

        //BUTTONS
        public Button[] time_menu_buttons;

        void Awake()
        {
            ui_manager = this.GetComponentInParent<UIManager>();
            time_controller = ui_manager.game_logic.GetComponent<TimeController>();

            time_menu_buttons = this.GetComponentsInChildren<Button>();
        }


        //----------------------BUTTONS ACTIONS----------------------//
        void onStopTimeButtonClick()
        {
            //Debug.Log("STOP TIME");
            time_controller.stopTime();
        }

        void onPlayTimeButtonClick()
        {
            //Debug.Log("RESUME TIME");
            time_controller.resumeTime();
        }

        void onSpeedTimeButtonClick(int speed)
        {
            if (speed == 2)
            {
                //Debug.Log("SPEED X2");
                time_controller.speedTime(2);
            }
            else if (speed == 4)
            {
                //Debug.Log("SPEED X4");
                time_controller.speedTime(4);
            }
                
        }


        //----------------------BUTTONS ACTIONS----------------------//

        //----------------------MENU MANAGEMENT----------------------//

        //Hide or show the menu
        public void showMenu(bool enabled)
        {
            this.gameObject.SetActive(enabled);
        }

        //Add the actions to the desired Menu Buttons
        public void AddButtonListeners()
        {

            //ADD TIME SYSTEM BUTTON LISTENERS
            // 0 -- STOP
            // 1 -- PLAY
            // 2 -- X2
            // 3 -- X4

            time_menu_buttons[0].onClick.AddListener(() => onStopTimeButtonClick());
            time_menu_buttons[1].onClick.AddListener(() => onPlayTimeButtonClick());
            time_menu_buttons[2].onClick.AddListener(() => onSpeedTimeButtonClick(2));
            time_menu_buttons[3].onClick.AddListener(() => onSpeedTimeButtonClick(4));

        }


        //----------------------MENU MANAGEMENT----------------------//
    }
}
