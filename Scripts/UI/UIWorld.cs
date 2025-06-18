using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_Tactica;

//Use this class to show the limitation of a base when the mouse is over the world UI
public class UIWorld : MonoBehaviour
{
    public GameObject limitation;
    public BaseConquer baseconquer_controller;

    private void Start()
    {
        baseconquer_controller = this.gameObject.transform.parent.Find("Trigger").gameObject.GetComponent<BaseConquer>();
    }

    private void OnMouseOver()
    {
        Debug.Log("ENTRA");
        if(limitation.activeSelf == false)
        {
            limitation.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if(baseconquer_controller != null)
        {
            if(baseconquer_controller.start_conquer == false)
            {
                if (limitation.activeSelf == true)
                {
                    limitation.SetActive(false);
                }
            }
        }
        
    }
}
