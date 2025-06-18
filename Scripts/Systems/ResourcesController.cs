using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class ResourcesController : MonoBehaviour
    {
        public float credits = 1000.0f;
        public float fuel = 1000.0f;

        GameController game_controller;

        private void Start()
        {
            game_controller = GameObject.Find("Game Logic").GetComponent<GameController>();
        }

        public float getCredits()
        {
            return credits;
        }

        public float getFuel()
        {
            return fuel;
        }

        public void setCredits(float new_credits)
        {
            credits = new_credits;
        }

        public void setFuel(float new_fuel)
        {
            fuel = new_fuel;
        }
        public void increaseCredits(float credits)
        {
            this.credits = this.credits + credits;
        }

        public void decreaseCredits(float credits)
        {
            if(this.credits >= credits)
            {
                this.credits = this.credits - credits;

                game_controller.ui_manager.CallOtherMenuSystems(false, true, credits);
            }
            else
            {
                Debug.Log("NOT ENOUGH CREDITS!");
            }
            
        }
    }

}
