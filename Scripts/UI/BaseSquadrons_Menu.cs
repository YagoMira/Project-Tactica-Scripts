using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project_Tactica
{
    public class BaseSquadrons_Menu : MonoBehaviour
    {
        //SYSTEMS
        GameController game_controller;
        SquadronController squadron_controller;

        //UI Facilities
        public GameObject squad_ui;
        public GameObject[] squads_ui;

        //SPRITES
        public Texture soldier_sprite;
        public Texture tank_sprite;
        public Texture almah_sprite;
        public Texture add_unit_sprite;

        //Variables
        public int selected_squad = -1;

        //Units menu - STATS
        public GameObject units;
        public TMP_Text[] stats_values;
        public GameObject stats_warning_text;
        public GameObject stats_texts;

        public GameObject selected_unit = null;

        public Button recruit_btn;

        void Start()
        {
            game_controller = GameObject.Find("Game Logic").GetComponent<GameController>();
            squadron_controller = GameObject.Find("Game Logic").GetComponent<SquadronController>();

            squads_ui = new GameObject[squadron_controller.num_squadrons];

            CreateSquadronUI();

        }

        private void Update()
        {
            checkSquadronBounds();
            AddUnitsSprites();
        }

        public void CreateSquadronUI()
        {
            float top = 0.0f, bottom = 0.0f;

            for (int i = 1; i < squadron_controller.num_squadrons; i++)
            {
                GameObject indv_squad_ui = Instantiate(squad_ui, squad_ui.transform.position, squad_ui.transform.rotation, gameObject.transform);
                indv_squad_ui.transform.localScale = new Vector3(squad_ui.transform.localScale.x, squad_ui.transform.localScale.y, squad_ui.transform.localScale.z);
                indv_squad_ui.name = i.ToString();

                //Rect
                indv_squad_ui.GetComponent<RectTransform>().offsetMin = new Vector2(squad_ui.GetComponent<RectTransform>().offsetMin.x, bottom);
                indv_squad_ui.GetComponent<RectTransform>().offsetMax = new Vector2(squad_ui.GetComponent<RectTransform>().offsetMax.x, top);

                top -= 120.0f;
                bottom -= 120.0f;
            }

            for (int i = 0; i < squadron_controller.num_squadrons; i++)
            {
                squads_ui[i] = this.gameObject.transform.GetChild(i).gameObject;
            }

            //Set the Toggle 1 as the default on (0 NO BECAUSE THE ALMAH IS THE SQUADRON 0!)
            squads_ui[2].GetComponent<Toggle>().isOn = true;
        }

        public void AddUnitsSprites()
        {
            GameObject units;//Get UNITS CHILD;

            for (int x = 0; x < squads_ui.Length; x++)
            {
                if (squads_ui[x].transform.GetChild(0).gameObject != null)
                {
                    units = squads_ui[x].transform.GetChild(0).gameObject;

                    if (squadron_controller.countUnitsSquadron(squadron_controller.squadrons, x) > 0)
                    {

                        for (int i = 0; i < squadron_controller.countUnitsSquadron(squadron_controller.squadrons, x); i++)
                        {
                            if (squadron_controller.squadrons[x].squadron[i] != null)
                            {
                                if (squadron_controller.squadrons[x].squadron[i].GetComponent<Unit>().sub_type == Unit.unit_type.Soldier)
                                {
                                    units.transform.GetChild(i).transform.GetChild(0).GetComponent<RawImage>().texture = soldier_sprite; //Unit portrait
                                }
                                else if (squadron_controller.squadrons[x].squadron[i].GetComponent<Unit>().sub_type == Unit.unit_type.Tank)
                                {
                                    units.transform.GetChild(i).transform.GetChild(0).GetComponent<RawImage>().texture = tank_sprite; //Unit portrait
                                }
                                else if (squadron_controller.squadrons[x].squadron[i].GetComponent<Unit>().sub_type == Unit.unit_type.Almah)
                                {
                                    units.transform.GetChild(i).transform.GetChild(0).GetComponent<RawImage>().texture = almah_sprite; //Unit portrait
                                }
                                else
                                {
                                    units.transform.GetChild(i).transform.GetChild(0).GetComponent<RawImage>().texture = add_unit_sprite; //Unit portrait
                                }

                            }
                            else
                            {
                                units.transform.GetChild(i).transform.GetChild(0).GetComponent<RawImage>().texture = add_unit_sprite; //Unit portrait
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < squadron_controller.num_units; i++)
                        {
                            units.transform.GetChild(i).transform.GetChild(0).GetComponent<RawImage>().texture = add_unit_sprite; //Unit portrait
                        }
                    }
                }
            }
        }

        //Check if a squadron is null (Create units without problem) or otherwhise, doesn't allow recruit if squadron is to far away
        public void checkSquadronBounds()
        {
            UnitsBounds bounds = game_controller.base_management_controller.military_base_object.transform.Find("Bounds").GetComponent<UnitsBounds>();
            bool[] squadrons_inside = bounds.squadrons_inside;

            for (int i = 0; i < squads_ui.Length; i++)
            {
                GameObject units = squads_ui[i].transform.Find("Units").gameObject;
                GameObject outside = squads_ui[i].transform.Find("Outside").gameObject;

                if (i == 0) //TOGGLE 0 TO ALMAH - NO INTERACTABLE TO RECRUIT
                {
                    squads_ui[i].transform.GetComponent<Toggle>().interactable = false;
                }
                else
                {
                    if (game_controller.squadron_controller.countUnitsSquadron(game_controller.squadron_controller.squadrons, i) == 0) //Squadron its empty
                    {
                        squads_ui[i].transform.GetComponent<Toggle>().interactable = true;
                        units.SetActive(true);
                        outside.SetActive(false);
                    }
                    else
                    {
                        if (squadrons_inside[i] == true)
                        {
                            squads_ui[i].transform.GetComponent<Toggle>().interactable = true;
                            units.SetActive(true);
                            outside.SetActive(false);
                        }
                        else
                        {
                            squads_ui[i].transform.GetComponent<Toggle>().interactable = false;
                            units.SetActive(false);
                            outside.SetActive(true);
                        }
                    }
                }
            }
        }

        //In case on click one of the squad's toggle set the actual squad as the selected one
        public void setSelectedSquad()
        {
            Toggle active_toggle = this.gameObject.GetComponent<ToggleGroup>().GetFirstActiveToggle();

            if(active_toggle.gameObject.name != null || active_toggle.gameObject.name != "")
            {
                selected_squad = int.Parse(active_toggle.gameObject.name[0].ToString());
                AvailableUnits();
            }
        }


        //Manage the squad creation on the UI
        public void AvailableUnits()
        {
            if (selected_squad != -1)
            {
                if(squadron_controller != null)
                {
                    if (squadron_controller.squadrons != null)
                    {
                        //Check the number of created units and its type
                        if (squadron_controller.squadrons[selected_squad] != null)
                        {
                            Squadron selected_squadron = squadron_controller.squadrons[selected_squad];

                            int num_units = squadron_controller.countUnitsSquadron(squadron_controller.squadrons, selected_squad);


                            //Recruit units its available when:

                            //Number of soldiers is less than the maximum amount (ONLY SOLDIERS)
                            if (num_units < squadron_controller.num_units)
                            {

                                if (num_units <= 1)
                                {
                                    //SOLDIERS AND TANKS AVAILABLE
                                    if (num_units == 0)
                                    {
                                        units.transform.Find("Soldier").GetComponent<Toggle>().interactable = true;
                                        //units.transform.Find("Tank").GetComponent<Toggle>().interactable = true;
                                    }
                                    else
                                    {
                                        //Number of tanks is less than the maximum amount (ONLY TANKS) + AND NOT EXISTS ANY SOLDIERS ON IT! - MAXIMUM 2 TANKS!
                                        if (squadron_controller.squadrons[selected_squad].squadron[0].GetComponent<Tank>() != null)
                                        {
                                            //TANKS AVAILABLE , DISABLE SOLDIERS
                                            units.transform.Find("Soldier").GetComponent<Toggle>().interactable = false;
                                            //units.transform.Find("Tank").GetComponent<Toggle>().interactable = true;
                                        }
                                        else
                                        {
                                            //SOLDIERS AVAILABLE
                                            units.transform.Find("Soldier").GetComponent<Toggle>().interactable = true;
                                            //units.transform.Find("Tank").GetComponent<Toggle>().interactable = false;
                                        }
                                    }
                                }
                                else
                                {
                                    //SOLDIERS AVAILABLE
                                    units.transform.Find("Soldier").GetComponent<Toggle>().interactable = true;
                                    //units.transform.Find("Tank").GetComponent<Toggle>().interactable = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void enableRecruitButton()
        {
            if(selected_unit == null)
            {
                recruit_btn.interactable = false;
            }
            else
            {
                recruit_btn.interactable = true;
            }
        }

        public void setActualUnit(string unit_name)
        {
            GameObject unit = units.transform.Find(unit_name).gameObject;

            Stats stats = unit.GetComponent<Stats>();

            //SET THE UI
            stats_warning_text.gameObject.SetActive(false);
            stats_texts.gameObject.SetActive(true);

            //SET VALUES TEXTS
            if (stats != null)
            {
                if(stats_values.Length > 0)
                {
                    stats_values[0].text = (100.0f + stats.extra_health).ToString(); //HEALTH (BY DEFAULT THE UNITS HAS 100.0F OF INITIAL HEALTH)
                    stats_values[1].text = (stats.damage).ToString(); //DAMAGE
                    stats_values[2].text = (stats.speed).ToString(); //SPEED
                    stats_values[3].text = (stats.resistance).ToString(); //RESISTANCE
                    stats_values[4].text = (stats.range).ToString(); //RANGE
                }
            }

            selected_unit = unit;
            enableRecruitButton();
        }

    }

}
