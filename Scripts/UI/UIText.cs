using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_Tactica;
using static Project_Tactica.ItemsUIManager;
using TMPro;

public class UIText : MonoBehaviour
{
    public item_ui_type ui_type;
    public int ui_id;

    string text = "";

    // Start is called before the first frame update
    void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        getUIText();
        setUIText();
    }

    //Get the correct Item/UI Text from CSV
    public void getUIText()
    {
        string item_ui_manager_name = "ItemUI_Manager";
        ItemsUIManager items_ui_manager = GameObject.Find(item_ui_manager_name).GetComponent<ItemsUIManager>();

        if(items_ui_manager != null)
            text = items_ui_manager.getItemsUIDataByID(ui_type, ui_id);
    }

    //Set the correct Item/UI Text from CSV
    public void setUIText()
    {
        TMP_Text ui_text = this.gameObject.GetComponent<TMP_Text>();

        if((text == "") || (text == null))
        {
            //Debug.LogError("Not Text found for UI Element");
        }
        else
        {
            ui_text.text = text;
        }
      
            
        
        
    }
}
