using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Project_Tactica
{
    public class BaseConquer : MonoBehaviour
    {
        public Factions base_faction = 0; //ELEMENT Faction (BY DEFAULT == 0)
        public GameObject limitation;

        public GameObject conquer_ui;
        Image conquer_health;
        Image owner_health;

        GameController game_controller;
        SocialSystem social_controller;

        public List<Unit> ally_units;
        public List<Unit> enemy_units;

        public List<Element> edifications;

        public bool lose_conditions = false;

        public bool start_conquer = false;

        private void Start()
        {
            game_controller = GameObject.Find("Game Logic").GetComponent<GameController>();
            social_controller = GameObject.Find("Game Logic").GetComponent<SocialSystem>();
            conquer_health = conquer_ui.transform.GetChild(0).transform.Find("Enemy_Fill").GetComponent<Image>();
            owner_health = conquer_ui.transform.GetChild(0).transform.Find("Fill").GetComponent<Image>();

            SetColor();
        }

        private void Update()
        {
            //In case of conquest of the main base... Lose and restart the game (SCENE)
            if(lose_conditions == true)
            {
                LoseConditions();
            }
        }

        public void LoseConditions()
        {
            if(base_faction != 0) //NOT THE PLAYER
            {
                lose_conditions = false; //PREVENT MULTIPLE COROUTINE CALLS ON RESTART_GAME!

                game_controller.disable_input = true;
                game_controller.ui_manager.lose_window.SetActive(true); //Show the Lose window

                StartCoroutine(game_controller.RestartGame(5.0f));
            }
        }



        //Set the color of the material
        public void SetColor()
        {
            if (base_faction != 0) //Now the enemy is the owner
            {
                Color limitation_color = Color.red;
                limitation_color.a = 0.15f;

                limitation.GetComponent<Renderer>().material.color = limitation_color;

                owner_health.color = Color.red;
                conquer_health.fillAmount = 0.0f;
                conquer_health.color = Color.blue;
            }
            else //Now the player is the owner
            {
                Color limitation_color = Color.blue;
                limitation_color.a = 0.15f;

                limitation.GetComponent<Renderer>().material.color = limitation_color;

                owner_health.color = Color.blue;
                conquer_health.fillAmount = 0.0f;
                conquer_health.color = Color.red;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<Unit>() != null)
            {
                Unit unit = other.gameObject.GetComponent<Unit>();

                if (unit.faction == base_faction)
                {
                    //Check for any null units inside list and remove it
                    ally_units.RemoveAll(u => u == null);

                    if (!ally_units.Contains(unit))
                        ally_units.Add(unit);
                }

                if (social_controller.GetRelationBetween((int)base_faction, (int)unit.faction) == Element.elem_status.Enemy)
                {
                    //Check for any null units inside list and remove it
                    enemy_units.RemoveAll(u => u == null);

                    if (!enemy_units.Contains(unit))
                        enemy_units.Add(unit);
                }

            }
        }


        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.GetComponent<Unit>() != null)
            {
                Unit unit = other.gameObject.GetComponent<Unit>();

                if (social_controller.GetRelationBetween((int)base_faction, (int)unit.faction) == Element.elem_status.Enemy)
                {
                    //ON ENEMY ENTER ON BASE
                    if (conquer_ui.activeSelf == false)
                        conquer_ui.SetActive(true);

                    if (limitation.activeSelf == false)
                        limitation.SetActive(true);


                    //IF THERE ARE ENEMY UNITS INSIDE THE TRIGGER BUT NOT ALLIES!
                    if (AllyUnitsInside() == false)
                    {
                        if (conquer_health.fillAmount != 1.0)
                        {
                            StartConquer();
                        }
                        else //CONQUER FINALIZE
                        {
                            start_conquer = false;
                            //SET THE UI AS THE START: NOW THE CONQUEROR ITS THE OWNER!
                            base_faction = unit.faction;

                            //#TO-DO: EACH FACTION SHOULD HAVE ITS OWN COLOR!
                            if (base_faction != 0)
                                limitation.GetComponent<Renderer>().material.color = Color.red;
                            else
                                limitation.GetComponent<Renderer>().material.color = Color.blue;

                            StartCoroutine(DeactivateUI());

                        }
                    }
                }

                if(EnemyUnitsInside() == false)
                {
                    if (conquer_ui.activeSelf == true)
                        conquer_ui.SetActive(false);

                    if (limitation.activeSelf == true)
                        limitation.SetActive(false);
                }
            }
            else
            {
                
                if (other.gameObject.GetComponent<Element>() != null)
                {
                    if(other.gameObject.GetComponent<Element>().type == Element.elem_type.Edification)
                    {
                       
                        Element edification = other.gameObject.GetComponent<Element>();

                        if (!edifications.Contains(edification))
                        {
                            edifications.Add(edification);
                        }
                    }
                }
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Unit>() != null)
            {
                Unit unit = other.gameObject.GetComponent<Unit>();

                //ALLY
                if (unit.faction == base_faction)
                {
                    if(ally_units.Count > 0)
                    {
                        ally_units.Remove(unit);
                    }
                }

                //ENEMY
                if (social_controller.GetRelationBetween((int)base_faction, (int)unit.faction) == Element.elem_status.Enemy)
                {
                    if (enemy_units.Count > 0)
                    {
                        enemy_units.Remove(unit);
                    }

                }

            }
        }


        public void StartConquer()
        {
            //Debug.Log("START CONQUER");
            start_conquer = true;
            StartCoroutine(FillConquerHealth());
        }

        IEnumerator FillConquerHealth()
        {
            yield return new WaitForSeconds(5.0f);
            conquer_health.fillAmount = conquer_health.fillAmount + 0.01f;
        }

        IEnumerator DeactivateUI()
        {
            yield return new WaitForSeconds(1f);

            if (conquer_ui.activeSelf == true)
                conquer_ui.SetActive(false);

            if (limitation.activeSelf == true)
                limitation.SetActive(false);

            if (base_faction != 0) //Now the enemy is the owner
            {
                owner_health.color = Color.red;
                conquer_health.fillAmount = 0.0f;
                conquer_health.color = Color.blue;
            }
            else //Now the player is the owner
            {
                owner_health.color = Color.blue;
                conquer_health.fillAmount = 0.0f;
                conquer_health.color = Color.red;
            }

            ManageEdifications();
        }

        public void ManageEdifications()
        {
            //All buildings now are property of the conqueror
            if (edifications.Count > 0)
            {
                for (int i = 0; i < edifications.Count; i++)
                {
                    edifications[i].faction = base_faction;

                    //If the conqueror of the buildings are now the player, set "BUILDINGS" as parent to apply the FOW! (Otherwise the parent will be null)
                    if (base_faction == 0) //PLAYER
                    {
                        edifications[i].gameObject.transform.parent = GameObject.Find("BUILDINGS").transform;
                    }
                    else
                    {
                        edifications[i].gameObject.transform.parent = null;
                    }

                    //In case of FACTORY:
                    if(edifications[i].gameObject.GetComponent<FactoryBase>() != null)
                    {
                        FactoryBase factory = edifications[i].gameObject.GetComponent<FactoryBase>();

                        StartCoroutine(factory.IncreaseCredits(factory.seconds));
                    }
                }
            }



        }

        public bool AllyUnitsInside()
        {
            bool ally_inside = false;

            if(ally_units.Count > 0)
            {
                for (int i = 0; i < ally_units.Count; i++)
                {
                    if(ally_units[i] != null) //In case of Null identifiers inside list (Kill ally)
                    {
                        ally_inside = true;
                        break;
                    }
                }
            }
            else
            {
                ally_inside = false;
            }

            return ally_inside;
        }

        public bool EnemyUnitsInside()
        {
            bool enemy_inside = false;

            if (enemy_units.Count > 0)
            {
                for (int i = 0; i < enemy_units.Count; i++)
                {
                    if (enemy_units[i] != null) //In case of Null identifiers inside list (Kill ally)
                    {
                        enemy_inside = true;
                        break;
                    }
                }
            }
            else
            {
                enemy_inside = false;
            }

            return enemy_inside;
        }
    }
}
