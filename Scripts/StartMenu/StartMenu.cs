using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project_Tactica
{
    public class StartMenu : MonoBehaviour
    {
        public GameObject start_menu;
        public GameObject load_screen;

        //*****OPTIONS******//
        //Option Window Values
        public Options_Values options;
        Values options_values;
        //*****OPTIONS******//

        //*****SCENES******//
        string initial_level = "Initial_Level";
        string justice_system = "Justice_System_Alice";
        //*****SCENES******//

        public void StartGame()
        {
            DataSaver.SetAllValues(options.GetOptionsValues());
            LoadScene(initial_level);
        }
        public void QuitGame()
        {
            Application.Quit();
        }

        public void LoadScene(string scene_name)
        {
            StartCoroutine(LoadSceneAsync(scene_name));
        }

        IEnumerator LoadSceneAsync(string scene_name)
        {
            yield return new WaitForSeconds(1.0f); //Take time to play the "Press Button" sound

            AsyncOperation async_load = SceneManager.LoadSceneAsync(scene_name);

            //yield return new WaitForSeconds(5); //ONLY FOR TESTS
                                                //TO-DO: CHECK WHEN HAVE A BIG SCENE IF THE LOAD SCREEN WORKS PROPERLY

            start_menu.SetActive(false);
            load_screen.SetActive(true);

            //TO-DO: IF THE LOAD SCREEN WORKS PROPERLY -> THE CODE SHOULD BE HERE (DELETE WAITS)


            while (!async_load.isDone)
            {
                float progress = Mathf.Clamp01(async_load.progress / 0.9f); //Loading take .9 percent to change the scene out of 1

                yield return null;
            }
        }

        /*
        public void SetLanguage(int language_index)
        {
            language_selected = language_index;
        }

        public void SetVolumes(float music_volume, float effects_volume)
        {
            this.music_volume = music_volume;
            this.effects_volume = effects_volume;
        }

        public void SetOtherOptions(float fov)
        {
            this.fov = fov;
        }
        */

        //On click apply settings button Save the settings ON DATASAVER (And apply it!)
        public void SaveSettings()
        {
            if(options != null)
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
    }
}
