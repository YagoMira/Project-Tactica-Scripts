using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class StartMenu_Values : MonoBehaviour
    {
        public static StartMenu_Values instance;

        public int language_selected = 0; //English == 0 | Spanish == 1

        private void Awake()
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }

        public void SetAllValues(int language)
        {
            language_selected = language;
        }
    }
}
