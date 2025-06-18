using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWindow : MonoBehaviour
{
    public int actual_tutorial = 0;

    public GameObject[] tutorial_panels;
    
    public GameObject btn_left;
    public GameObject btn_right;

    private void Start()
    {
        int num_panels = this.transform.Find("Panels").childCount;
        tutorial_panels = new GameObject[num_panels];

        for (int i = 0; i < num_panels; i++)
        {
            tutorial_panels[i] = this.transform.Find("Panels").GetChild(i).gameObject;
        }
        
    }

    public void SetActivePanel()
    {
        if(actual_tutorial > 0)
        {
            btn_left.SetActive(true);

            if (actual_tutorial == (tutorial_panels.Length - 1))
            {
                btn_right.SetActive(false);
            }
            else
            {
                btn_right.SetActive(true);
            }
        }
        else if(actual_tutorial == 0)
        {
            btn_left.SetActive(false);
        }

        if(tutorial_panels.Length > 0)
        {
            for (int i = 0; i < tutorial_panels.Length; i++)
            {
                if(i == actual_tutorial)
                {
                    tutorial_panels[i].SetActive(true);
                }
                else
                {
                    tutorial_panels[i].SetActive(false);
                }
            }
        }
       
    }

    public void IncreaseActualPanel()
    {
        if (actual_tutorial < (tutorial_panels.Length - 1))
        {
            actual_tutorial++;
        }

        SetActivePanel();
    }

    public void DecreaseActualPanel()
    {
        if(actual_tutorial > 0)
        {
            actual_tutorial--;
        }

        SetActivePanel();
    }


}
