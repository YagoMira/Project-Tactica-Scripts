using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project_Tactica
{
    public class Tactica_Menu : MonoBehaviour
    {
        //UI
        public GameObject generals_ui;
        public GameObject portrait_prefab;

        //Animators
        Animator panel_animator;

        //Tactica System
        TacticaSystem tactica_controller;

        private void Start()
        {
            tactica_controller = GameObject.Find("Game Logic").GetComponent<TacticaSystem>();

            GetUIElements();
            AddGeneralsPortraits();
        }

        public void GetUIElements()
        {
            string panel_str = "Panel";
            GameObject panel;

            //Get Panel
            panel = this.gameObject.transform.Find(panel_str).gameObject;
            panel_animator = this.gameObject.GetComponent<Animator>(); //panel.GetComponent<Animator>();

        }

        public void showPanel(bool enabled)
        {
            bool actual_enabled = panel_animator.GetBool("enabled");
            
            if (panel_animator != null)
            {
                if(actual_enabled == true)
                {
                    panel_animator.SetBool("enabled", false);
                }
                else
                {
                    panel_animator.SetBool("enabled", true);
                }
               
            }
            
        }

        public void AddGeneralsPortraits()
        {
            int max_generals = tactica_controller.num_generals;

            GameObject main_panel = this.gameObject.transform.GetChild(0).gameObject;


            if(max_generals > 0)
            {
                for(int i = 0; i < max_generals; i++)
                {
                    GameObject instantiate_general_ui = Instantiate(portrait_prefab, main_panel.transform);
                    

                    //Set the Image and text of the correct General
                    GameObject general_ui = GetGeneralUI(tactica_controller.generals[i].ToString().ToUpper());

                    instantiate_general_ui.name = general_ui.transform.Find("Name").GetComponent<TMP_Text>().text.ToString();
                    instantiate_general_ui.transform.Find("Image").GetComponent<RawImage>().texture = general_ui.transform.Find("Image_Base").GetComponent<RawImage>().texture;
                    instantiate_general_ui.transform.Find("Description").GetChild(0).GetComponent<TMP_Text>().text = general_ui.transform.Find("Text_Base").GetComponent<TMP_Text>().text;


                    UIText description_text = general_ui.transform.Find("Text_Base").GetComponent<UIText>();
                    UIText instantiate_description = instantiate_general_ui.transform.Find("Description").GetChild(0).gameObject.AddComponent<UIText>();
                    instantiate_description.ui_type = item_ui_type.UI;
                    instantiate_description.ui_id = description_text.ui_id;

                    //NAME
                    if (instantiate_general_ui.transform.Find("Name").GetChild(0).GetComponent<TMP_Text>().text == DataSaver.player_name)
                    {
                        instantiate_general_ui.transform.Find("Name").GetChild(0).GetComponent<TMP_Text>().text = DataSaver.player_name;
                    }
                    else
                    {
                        instantiate_general_ui.transform.Find("Name").GetChild(0).GetComponent<TMP_Text>().text = general_ui.transform.Find("Name").GetComponent<TMP_Text>().text;
                    }

                    //Set the button functionality
                    Button tactica_btn = instantiate_general_ui.transform.Find("TACTICA").gameObject.GetComponent<Button>();
                    tactica_btn.onClick.AddListener(() => ActivateGeneralTactica(instantiate_general_ui.transform.Find("Name").GetChild(0).GetComponent<TMP_Text>().text, instantiate_general_ui));

                }
            }

        }

        public void ActivateGeneralTactica(string general_name, GameObject general_portrait)
        {
            GameObject timer = general_portrait.transform.Find("Timer").gameObject;
            timer.gameObject.SetActive(true);

            //Get the tactica from the actual clicked GENERAL
            Tactica tactica = generals_ui.transform.Find(general_name).GetComponent<Tactica>();
            tactica_controller.ActivateTactica(tactica);

            //Set the Tactica Timer Countdown
            StartCoroutine(ModifyCooldown(timer, tactica));
        }

        public IEnumerator ModifyCooldown(GameObject timer, Tactica tactica)
        {
            float initial_cooldown = tactica.cooldown;

            timer.transform.Find("Text").GetComponent<TMP_Text>().text = tactica.cooldown + ".00";

            while (tactica.cooldown > 0.0)
            {
                timer.transform.Find("Text").GetComponent<TMP_Text>().text = tactica.cooldown + ".00";
                tactica.cooldown -= 1.0f;

                yield return new WaitForSeconds(1.0f);

                //#TO-DO: COUNT MILISECONDS!
                //float mili = 100.0f;

                //while (mili != 0.0f)
                //{
                //    mili -= 1.0f;
                //    Debug.Log("MILI? " + mili);
                    
                //}
            }

            timer.gameObject.SetActive(false);
            tactica.cooldown = initial_cooldown; //Restart the cooldown value
        }

        public void ActivateGeneralPortraits(float enabled)
        {
            int max_generals = tactica_controller.num_generals;

            GameObject main_panel = this.gameObject.transform.GetChild(0).gameObject;


            if (max_generals > 0)
            {
                for (int i = 1; i < max_generals+1; i++) //+1 For evade the "child Button"
                {
                    if(enabled == 1.0f) //Use float and not bool because animation/animator
                        main_panel.transform.GetChild(i).gameObject.SetActive(true);
                    else if (enabled == 0.0f) 
                        main_panel.transform.GetChild(i).gameObject.SetActive(false);
                }

            }
        }

        public GameObject GetGeneralUI(string name)
        {
            GameObject general = null;

            for (int i = 0; i < generals_ui.transform.childCount; i++)
            {
                if (generals_ui.transform.GetChild(i).gameObject.name == name)
                {
                    general = generals_ui.transform.GetChild(i).gameObject;
                }
            }

            return general;
        }
    }
}
