using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Project_Tactica;

public class Item : MonoBehaviour
{
    Outline outline;
    public int item_id = -1;
    public item_ui_type item_type;
    public string msg = "";
    public AudioClip audio_clip;

    public UnityEvent onInteract;

    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
        ManageOutline(false);

        if(item_id != -1)
            getItemText();
    }

    public void Interact()
    {
        onInteract.Invoke();
    }

    public void ManageOutline(bool outline_activated)
    {
        outline.enabled = outline_activated;
    }

    //Get the correct Item/UI Text from CSV
    public void getItemText()
    {
        string item_ui_manager_name = "ItemUI_Manager";
        ItemsUIManager items_ui_manager = GameObject.Find(item_ui_manager_name).GetComponent<ItemsUIManager>();

        msg = items_ui_manager.getItemsUIDataByID(item_type, item_id);

        if((msg == "") || (msg == null))
        {
            Debug.Log("ERROR EN: " + this.gameObject.name);
        }
    }
}
