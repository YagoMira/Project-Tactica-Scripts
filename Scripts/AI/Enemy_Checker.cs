using Project_Tactica;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Checker : MonoBehaviour
{
    public GameObject squad_ai_gameobject;
    public Squad_AI squad_ai;
    public Unit actual_unit;

    private void Start()
    {
        if(this.gameObject.transform.parent.gameObject != null)
        {
            if(this.gameObject.transform.parent.gameObject.transform.parent != null)
            {
                if (this.gameObject.transform.parent.gameObject.transform.parent.gameObject != null)
                {
                    squad_ai_gameobject = this.gameObject.transform.parent.gameObject.transform.parent.gameObject; //Get SQUAD AI TRANSFORM GAMEOBJECT
                    squad_ai = squad_ai_gameobject.GetComponent<Squad_AI>();
                }
            }
           
        }
    }

    private void Update()
    {
        //if(squad_ai != null)
        //{
            //squad_ai.enemyFound = false;
        //}

    }

    void OnTriggerEnter(Collider other)
    {
        string player_tag = "Player";

        if (other.gameObject.CompareTag(player_tag))
        {
            if (squad_ai != null)
            {
                if(squad_ai.enemyFound_gameobject == null)
                {
                    if (other.gameObject.GetComponent<Unit>() != null)
                    {
                        Unit enemy = other.gameObject.GetComponent<Unit>();
                        
                        if((enemy.health > 0) && (enemy.onDie != true)) //ALWAYS CHECK THE HEALTH AND ONDIE STATE !!!
                        {
                            squad_ai.enemyFound = true;
                            squad_ai.enemyFound_gameobject = other.gameObject;
                        }

                        if(actual_unit != null)
                        {
                            if (actual_unit.selected_enemy == null)
                            {
                                if (other.gameObject != null)
                                {
                                    actual_unit.selected_enemy = other.gameObject;
                                }

                            }
                        }
                       
                    }

                }
            }
        }
    }

    //void OnTriggerStay(Collider other)
    //{
    //    string player_tag = "Player";

    //    if (other.gameObject.CompareTag(player_tag) == false)
    //    {
    //        squad_ai.enemyFound = false;
    //    }
    //}

}
