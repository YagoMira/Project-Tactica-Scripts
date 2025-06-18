using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_Tactica;
using TMPro;

public class SO_UI : MonoBehaviour
{
    public GameObject start_Menu;

    public Canvas ui_so;

    public TMP_Text text_field;

    public AudioClip email_audio;
    public AudioClip transportation_audio;

    public GameObject flash;

    public FirstPerson_UI fps_ui;

    public GameObject camera_load;
    public GameObject load_screen;

    //When the audio of the "Login button (SO Window - Initial Level)" is finished. Then change the scene
    public void LoginOnPC()
    {
        AudioManager.instance.PlayClip(email_audio);

        ui_so = this.gameObject.transform.parent.GetComponent<Canvas>();

        //SET THE PLAYER NAME
        DataSaver.SetPlayerName((text_field.text).ToString());

        StartCoroutine(StartFlash());
    }

    public void ReactivateUIControls()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    private IEnumerator StartFlash()
    {
        while (AudioManager.instance.audio_source.isPlaying == true)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1.0f);

        AudioManager.instance.PlayClip(this.transportation_audio);
        flash.gameObject.SetActive(true);

        if(flash.activeSelf == true)
        {
            Item flash_item = flash.GetComponent<Item>();
            fps_ui.ShowItemText(flash_item);
        }
            

        //When the audio stops playing...
        float plane_distance = ui_so.planeDistance;

        if (ui_so != null)
        {
            while (plane_distance < 1.0f)
            {
                yield return new WaitForSeconds(0.1f);
                ui_so.planeDistance += 0.01f;
                plane_distance = ui_so.planeDistance;
            }

        }

        load_screen.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
        camera_load.gameObject.SetActive(true);

        //CHANGE THE SCENE - INITIAL LEVEL FINISHED!!!
        //#TO-DO: SELECT THE CORRECT SCENE!!!
        start_Menu.GetComponent<StartMenu>().LoadScene("World");

    }
}
