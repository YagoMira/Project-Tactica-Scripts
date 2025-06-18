using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Project_Tactica
{
    public class Resources_Menu : MonoBehaviour
    {
        public Text credits_text;
        public Text fuel_text;

        public void showMenu(float credits, float fuel)
        {
            credits_text.text = credits.ToString();
            fuel_text.text = fuel.ToString();
        }

        public void showAddCredits(float credits)
        {
            GameObject addcredits = this.gameObject.transform.GetChild(0).Find("AddCredits").gameObject; //Child 0 because the "Panel" Object

            addcredits.GetComponent<TMP_Text>().text = ("+" + credits.ToString());

            if (addcredits.activeSelf == true)
            {
                addcredits.SetActive(false);
                addcredits.SetActive(true);
            }
            else
            {
                addcredits.SetActive(true);
            }
        }

        public void showDecreaseCredits(float credits)
        {
            GameObject decreaseCredits = this.gameObject.transform.GetChild(0).Find("DecreaseCredits").gameObject; //Child 0 because the "Panel" Object

            decreaseCredits.GetComponent<TMP_Text>().text = ("-" + credits.ToString());

            if (decreaseCredits.activeSelf == true)
            {
                decreaseCredits.SetActive(false);
                decreaseCredits.SetActive(true);
            }
            else
            {
                decreaseCredits.SetActive(true);
            }
        }
    }

}
