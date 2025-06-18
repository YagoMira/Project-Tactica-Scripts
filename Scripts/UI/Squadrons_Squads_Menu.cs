using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Project_Tactica
{
    public class Squadrons_Squads_Menu : MonoBehaviour
    {
        //Animators
        Animator panel_animator;

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

        private void Start()
        {
            game_controller = GameObject.Find("Game Logic").GetComponent<GameController>();
            squadron_controller = GameObject.Find("Game Logic").GetComponent<SquadronController>();

            squads_ui = new GameObject[squadron_controller.num_squadrons];
            CreateSquadronUI();

        }

        private void Update()
        {
            AddUnitsSprites();
        }

        public void CreateSquadronUI()
        {
            RectTransform initial_ui_rect = squad_ui.GetComponent<RectTransform>();

            float value = 180.0f;
            float top = initial_ui_rect.offsetMin.y - value, bottom = initial_ui_rect.offsetMax.y - value;

            for (int i = 1; i < squadron_controller.num_squadrons; i++)
            {
                GameObject indv_squad_ui = Instantiate(squad_ui, squad_ui.transform.position, squad_ui.transform.rotation, gameObject.transform);
                indv_squad_ui.transform.localScale = new Vector3(squad_ui.transform.localScale.x, squad_ui.transform.localScale.y, squad_ui.transform.localScale.z);
                indv_squad_ui.name = i.ToString();

                //Rect
                indv_squad_ui.GetComponent<RectTransform>().offsetMin = new Vector2(squad_ui.GetComponent<RectTransform>().offsetMin.x, bottom);
                indv_squad_ui.GetComponent<RectTransform>().offsetMax = new Vector2(squad_ui.GetComponent<RectTransform>().offsetMax.x, top);
      
                bottom -= value;
                top -= value;

                //SQUAD NUM
                indv_squad_ui.transform.Find("Squad_Num").GetComponent<TMP_Text>().text = "SQUAD " + i;
            }

            for (int i = 0; i < squadron_controller.num_squadrons; i++)
            {
                squads_ui[i] = this.gameObject.transform.GetChild(i).gameObject;
            }

            //Set the Toggle 0 as the default on
            if(squad_ui.gameObject.activeSelf == true)
            {
                if (squad_ui != null)
                {
                    squad_ui.TryGetComponent<Toggle>(out Toggle toggle_0);

                    if (toggle_0 != null)
                    {
                        toggle_0.isOn = true;
                    }
                }
            }
           
          
        }

        public void AddUnitsSprites()
        {
            GameObject units;//Get UNITS CHILD;

            for (int x = 0; x < squads_ui.Length; x++)
            {
                if (squads_ui[x].transform.Find("Units").gameObject != null)
                {
                    units = squads_ui[x].transform.Find("Units").gameObject;

                    if (squadron_controller.countUnitsSquadron(squadron_controller.squadrons, x) > 0)
                    {

                        for (int i = 0; i < squadron_controller.countUnitsSquadron(squadron_controller.squadrons, x); i++)
                        {
                            if (squadron_controller.squadrons[x].squadron[i] != null)
                            {
                                Unit unit = squadron_controller.squadrons[x].squadron[i].GetComponent<Unit>();

                                if (unit.sub_type == Unit.unit_type.Soldier)
                                {
                                    if((unit.health <= 0) || (unit.onDie == true))
                                    {
                                        units.transform.GetChild(i).transform.GetChild(0).GetComponent<RawImage>().texture = add_unit_sprite; //Unit portrait
                                    }
                                    else
                                    {
                                        units.transform.GetChild(i).transform.GetChild(0).GetComponent<RawImage>().texture = soldier_sprite; //Unit portrait
                                    }
                                }
                                else if (unit.sub_type == Unit.unit_type.Tank)
                                {
                                    units.transform.GetChild(i).transform.GetChild(0).GetComponent<RawImage>().texture = tank_sprite; //Unit portrait
                                }
                                else if (unit.sub_type == Unit.unit_type.Almah)
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

        //In case on click one of the squad's toggle set the actual squad as the selected one
        public void setSelectedSquad()
        {
            this.gameObject.TryGetComponent<ToggleGroup>(out ToggleGroup group_toggle);

            if(group_toggle != null)
            {
                Toggle active_toggle = group_toggle.GetFirstActiveToggle();

                if(active_toggle != null)
                {
                    if (active_toggle.gameObject.name != null || active_toggle.gameObject.name != "")
                    {
                        selected_squad = int.Parse(active_toggle.gameObject.name.ToString());
                    }

                    SelectSquadron(); //Select the PLAYER squadron
                }
               
            }
        }

        public void SelectSquadron()
        {
            if(selected_squad != -1)
            {
                game_controller.squadron_controller.selectPlayerSquadron(selected_squad, false);
            }
           
        }

    }
}
