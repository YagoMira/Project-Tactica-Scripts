using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryBase_Menu : MonoBehaviour
{
    //Hide or show the menu
    public void showMenu(bool enabled)
    {
        GameObject main_panel = this.gameObject.transform.GetChild(0).gameObject;

        main_panel.SetActive(enabled);
    }
}
