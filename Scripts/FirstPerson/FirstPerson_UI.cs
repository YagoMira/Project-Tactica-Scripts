using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_Tactica;
using TMPro;

public class FirstPerson_UI : MonoBehaviour
{
    public static FirstPerson_UI ui_instance;

    [SerializeField]
    TMP_Text interact_text;
    [SerializeField]
    TMP_Text item_text;

    public bool disable_text = false;

    private void Awake()
    {
        ui_instance = this;
        interact_text = this.gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
        item_text = this.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        SetInteractionText();

        if (disable_text == true)
            interact_text.gameObject.SetActive(false);

    }

    public void SetInteractionText()
    {
        string f_key = "[F]";
        string aux_text; //For not duplicate the actual text of the Text component

        aux_text = interact_text.text;
        interact_text.text = ""; //Clean
        interact_text.text = aux_text + " " + f_key;
    }

    public void ManageInteractText(bool enabled)
    {
        interact_text.gameObject.SetActive(enabled);
    }

    public void ShowItemText(Item item)
    {
        string item_text = item.msg;

        this.item_text.text = item_text; //Set the "msg" TEXT (Text of the Item)

        if (this.item_text.gameObject.activeSelf == false)
        {
            this.item_text.gameObject.SetActive(true);
        }
        else //In case of are already displaying a text and user interact with other item in this time
        {
            this.item_text.gameObject.SetActive(false);
            this.item_text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, -1.0f); //Start as hidde: In case of multiple item interactions in a short period of time
            StopAllCoroutines(); //In the Initial level has no importance (TAKE CARE WITH THE OTHER ONES!!!)

            ShowItemText(item); //Recall
        }
        
        StartCoroutine(DisplayItemText());

    }

    private IEnumerator DisplayItemText()
    {
        float dilate = this.item_text.fontMaterial.GetFloat(ShaderUtilities.ID_FaceDilate); //Use the dilate property of the material for show or hide the text
        float hide_text_time = 2.0f;

        while (dilate < 0.0f)
        {
            yield return new WaitForSeconds(0.1f);
            dilate += 0.1f;
            this.item_text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilate);
            dilate = this.item_text.fontMaterial.GetFloat(ShaderUtilities.ID_FaceDilate);
        }

        yield return new WaitForSeconds(hide_text_time);

        this.item_text.gameObject.SetActive(false);
        this.item_text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, -1.0f);
    }
}
