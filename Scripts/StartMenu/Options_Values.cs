using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Tactica
{
    public class Values
    {
        public int language { get; set; }
        public float music_volume { get; set; }
        public float effects_volume { get; set; }
        public float fov { get; set; }

        public Values()
        {
            this.language = 0;
            this.music_volume = 0.5f;
            this.effects_volume = 0.5f;
            this.fov = 80.0f;
        }

        public Values(int language, float music_volume, float effects_volume, float fov)
        {
            this.language = language;
            this.music_volume = music_volume;
            this.effects_volume = effects_volume;
            this.fov = fov;
        }


    }

    public class Options_Values : MonoBehaviour
    {
        public TMP_Dropdown languages_dropbox;
        public Slider music_volume_slider;
        public Slider effects_volume_slider;
        public Slider fov_slider;

        public GameObject CSV_reader;

        public Values GetOptionsValues()
        {

            Values option_values = new Values();
            option_values.language = languages_dropbox.value;
            option_values.music_volume = music_volume_slider.value;
            option_values.effects_volume = effects_volume_slider.value;
            option_values.fov = fov_slider.value;

            return option_values;
        }

        public void ApplyAudio(float music_volume, float effects_volume)
        {
            GameObject audio_manager = GameObject.Find("AudioManager");

            if(audio_manager != null)
            {
                AudioSource[] audios = audio_manager.GetComponents<AudioSource>();

                for (int i = 0; i < audios.Length; i++)
                {
                    if (i == 0) //MUSIC
                    {
                        audios[i].volume = music_volume;
                    }
                    else
                    {
                        audios[i].volume = effects_volume;
                    }
                }
            }

            //In world scene, decrease audio of the effects for the Units
            Unit[] units = (Unit[])GameObject.FindObjectsOfType(typeof(Unit));

            if(units.Length > 0)
            {
                for(int i = 0; i < units.Length; i++)
                {
                    AudioSource[] audios = units[i].gameObject.GetComponents<AudioSource>();
                    for(int j = 0; j < audios.Length; j++)
                    {
                        audios[j].volume = effects_volume;
                    }
                }
            }
        }

        public void ApplyLanguage()
        {
            if(CSV_reader != null)
            {
                if(CSV_reader.GetComponent<CSVReader>() != null)
                {
                    CSV_reader.GetComponent<CSVReader>().InitiateReader();
                }
                else
                {
                    //World Scene, get from Item_UIManager
                    if (CSV_reader.GetComponent<ItemsUIManager>() != null)
                    {
                        CSV_reader.GetComponent<ItemsUIManager>().csv_data.InitiateReader();
                    }

                }


                string item_ui_manager_name = "ItemUI_Manager";

                ItemsUIManager items_ui_manager = GameObject.Find(item_ui_manager_name).GetComponent<ItemsUIManager>();
                items_ui_manager.Clear();

                //Get all UI components and refresh the Text
                UIText[] ui_texts = (UIText[])FindObjectsOfType(typeof(UIText), true);

                for (int i = 0; i < ui_texts.Length; i++)
                {
                    ui_texts[i].RefreshUI();
                }
            }
            
        }
    }
}


