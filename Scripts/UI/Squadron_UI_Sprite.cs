using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Tactica
{
    public class Squadron_UI_Sprite : MonoBehaviour
    {
        public GameObject squad_sprite;
        public GameObject squad_num;
        public int num_squad;

        public bool selected;

        public void Start()
        {
            squad_sprite = this.gameObject.transform.Find("Sprite").gameObject;
            squad_num = this.gameObject.transform.Find("Squad").gameObject;
            setCaptainSquadSprite();
            setSquadNum();
        }

        public void setSquadNum()
        {
            squad_num.GetComponent<TMP_Text>().text = "SQUAD " + num_squad;
        }

        public void setCaptainSquadSprite()
        {
            if (squad_sprite != null)
            {
                //In case of being in a Faction's Squadron
                if (this.gameObject.transform.parent != null) //NPC Unit
                {
                    GameObject unit_gameobject = this.gameObject.transform.parent.gameObject;
                    Unit unit = unit_gameobject.GetComponent<Unit>();

                    squad_sprite.GetComponent<Image>().sprite = unit.sprite; //Set the Captain of the squadron sprite on the Squadron Sprite
                }

            }
        }


        public void squadronVisualSelection(bool outlineActivated)
        {
            if (outlineActivated == true)
            {
                selected = true;
                squad_sprite.GetComponent<Image>().color = new Color32(255, 255, 0, 255);
            }
            else
            {
                selected = false;
                squad_sprite.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
            }
        }


    }
}
