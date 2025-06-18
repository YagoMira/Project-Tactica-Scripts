using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Project_Tactica
{
    public class GameController : MonoBehaviour
    {

        public GameObject ui_manager_object;
        public UIManager ui_manager;

        public Input_Logic input;

        public LayerMask terrain;

        //CRUCIAL BOOLS
        public bool disable_input = false;

        //UNITS
        public List<GameObject> units_list = new List<GameObject>(); //MULTIPLE UNITS
        public List<GameObject> squadrons_list = new List<GameObject>(); //MULTIPLE SQUADRONS
        public Vector3 last_clicked_position = new Vector3(); //Use this for repositionate the UI Menus (Unit Menu)
        public Vector3 last_clicked_position_edification = new Vector3(); //Use this for repositionate the UI Menus on the clicked edification (Base/Squadrons Menu)
        public GameObject last_clicked_enemy; //Use this variable for know if any enemy is being selected (ATTACK Action)

        //OTHER SYSTEMS
        public ResourcesController resources_controller;
        public SquadronController squadron_controller;
        public BaseManagementController base_management_controller;
        public SocialSystem social_controller;

        //Other
        public Camera_Logic camera_controller;

        //public GameObject selectedElement = null; //Check if an element is selected (INDIVIDUAL UNIT)

        public GameObject ubication_indicator;
        public GameObject enemy_indicator;

        public bool not_navigable = false;

        public GameObject load_screen_object;

        // Start is called before the first frame update
        void Start()
        {
            if(ui_manager_object == null)
                ui_manager_object = FindObjectOfType<UIManager>().gameObject;

            ui_manager = ui_manager_object.GetComponent<UIManager>();
            input = this.gameObject.GetComponent<Input_Logic>();

            resources_controller = this.gameObject.GetComponent<ResourcesController>();
            squadron_controller = this.gameObject.GetComponent<SquadronController>();
            base_management_controller = this.gameObject.GetComponent<BaseManagementController>();
            social_controller = this.gameObject.GetComponent<SocialSystem>();

            //Get the camera script
            if (Camera.main.gameObject != null)
                camera_controller = Camera.main.gameObject.GetComponent<Camera_Logic>();
        }

        // Update is called once per frame
        void Update()
        {
            input.disable_input = disable_input;
            if(camera_controller != null)
                camera_controller.disable_input = disable_input;

            if (disable_input == false)
            {
                onRightAction();
            }

            visualSelection(true);

            //Restart game if need it
            if(input.f5_press == true)
            {
                StartCoroutine(RestartGame(0.25f));
            }
        }


        private void FixedUpdate()
        {
            if (disable_input == false)
            {
                onLeftAction();
            }
        }

        public IEnumerator RestartGame(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Restart the scene
        }

        public void LoadScene(string scene_name)
        {
            StartCoroutine(LoadSceneAsync(scene_name));
        }

        IEnumerator LoadSceneAsync(string scene_name)
        {
            yield return new WaitForSeconds(1.0f); //Take time to play the "Press Button" sound

            AsyncOperation async_load = SceneManager.LoadSceneAsync(scene_name);

            //yield return new WaitForSeconds(5); //ONLY FOR TESTS
            //TO-DO: CHECK WHEN HAVE A BIG SCENE IF THE LOAD SCREEN WORKS PROPERLY

            load_screen_object.SetActive(true);

            //TO-DO: IF THE LOAD SCREEN WORKS PROPERLY -> THE CODE SHOULD BE HERE (DELETE WAITS)

            while (!async_load.isDone)
            {
                float progress = Mathf.Clamp01(async_load.progress / 0.9f); //Loading take .9 percent to change the scene out of 1

                yield return null;
            }
        }

        public void onLeftAction() //In case of get the "Left Mouse Button Click" or similar
        {

            if (!ui_manager.IsPointerOverUIElement())
            {
                //Debug.Log("NO UI");

                if (input.mouse_left_click == true)
                {
                    //last_clicked_position = new Vector3(); //Clear the position for the UI Menus (Unit Menu)

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        GameObject element = hit.collider.gameObject;

                        if(element.layer != 6) //TERRAIN //#TO-DO: Take in consideration the click on Pivot (UI Selection Unit Box)
                        {
                            if (camera_controller.actual_level == 3) //Level where you can see the Squad Units individually
                            {
                                if (hit.collider.gameObject.GetComponent<Element>() != null)
                                {

                                    if (element.GetComponent<Element>().type == Element.elem_type.Unit) //In case of action on Unit Element
                                    {
                                        if (element.GetComponent<Unit>().onDie != true) //Check if the Unit is alive
                                        {
                                            //CHECK IF THE UNIT IS A PLAYER ONE
                                            if(element.CompareTag("Player") == true)
                                            {

                                                /*
                                                //Clear the list of units before click a UNIQUE squadron
                                                units_list = new List<GameObject>(); //Clear the list of the selected units 

                                                squadron_controller.clearSquadronList(); //Set the Sprite of the squadrons to its default starter value
                                                squadrons_list = new List<GameObject>(); //Clear the list of the squadrons
                                                */

                                                if (input.ctrl_left_click == true) //Ctrl Action on Units
                                                {
                                                    if (units_list.Contains(hit.collider.gameObject))
                                                    {
                                                        units_list.Remove(hit.collider.gameObject);
                                                        hit.collider.gameObject.GetComponentInChildren<Outline>().OutlineWidth = 0.0f; //Remove the outline for the removed Unit
                                                    }

                                                }
                                                else if (input.shift_left_click == true) //Shit Action on Units
                                                {
                                                    if (!units_list.Contains(hit.collider.gameObject))
                                                        units_list.Add(hit.collider.gameObject);

                                                }
                                                else //Normal Left Click
                                                {
                                                    if (!units_list.Contains(hit.collider.gameObject))
                                                    {
                                                        visualSelection(false); //#TO-DO: CHECK!!!!!!!!!!!!!!!!

                                                        units_list = new List<GameObject>(); //Clear the list of the selected units 

                                                        units_list.Add(hit.collider.gameObject);
                                                    }
                                                        
                                                }

                                                last_clicked_position_edification = new Vector3(); //Restart the checker of the buildings click
                                            }
                                            
                                        }

                                        /*
                                        visualSelection(false);
                                        visualSelection(true); //#TO-DO: CHECK!!!!!!!!!!!!!!!!
                                        */
                                    }
                                    else if (element.GetComponent<Element>().type == Element.elem_type.Edification) //In case of action on Build/Edification
                                    {
                                        //Debug.Log("CLICK ON BUILD - PRESS!");

                                        last_clicked_position_edification = hit.point;

                                        if (element.GetComponent<MilitaryBase>() != null) //In case of Military Base
                                        {
                                            base_management_controller.military_base = true;
                                        }
                                        else
                                        {
                                            base_management_controller.military_base = false;
                                        }
                                    }
                                    else
                                    {
                                        last_clicked_position_edification = new Vector3(); //Restart the checker of the buildings click

                                        //Restart all the conditions of edifications
                                        base_management_controller.military_base = false;
                                    }
                                }
                                else
                                {
                                    visualSelection(false); //#TO-DO: CHECK!!!!!!!!!!!!!!!!

                                    units_list = new List<GameObject>(); //Clear the list of the selected units 

                                    squadron_controller.clearSquadronList(); //Set the Sprite of the squadrons to its default starter value
                                    squadrons_list = new List<GameObject>(); //Clear the list of the squadrons
                                }
                            }
                            else if (camera_controller.actual_level == 2) //Squadron Camera Level
                            {
                                if (hit.collider.gameObject.GetComponent<Squadron_UI_Sprite>() != null)
                                {
                                    Squadron_UI_Sprite squad_sprite = hit.collider.gameObject.GetComponent<Squadron_UI_Sprite>();

                                    if (input.ctrl_left_click == true) //Ctrl Action on Units
                                    {
                                        if (squadrons_list.Contains(hit.collider.gameObject))
                                        {
                                            squadrons_list.Remove(hit.collider.gameObject);
                                            squad_sprite.squadronVisualSelection(false);
                                            squadron_controller.clearPlayerSquadronSelect(squad_sprite.num_squad);//Clear the selected squadron from Unit List selection
                                        }

                                    }
                                    else if(input.shift_left_click == true) //Shit Action on Units
                                    {
                                        if (squad_sprite.selected == false)
                                        {
                                            if(squad_sprite.transform.parent.gameObject.CompareTag("Player"))
                                            {
                                                squad_sprite.squadronVisualSelection(true);
                                                squadron_controller.selectPlayerSquadron(squad_sprite.num_squad, true);


                                                if (!squadrons_list.Contains(hit.collider.gameObject))
                                                    squadrons_list.Add(hit.collider.gameObject);
                                            }
                                            

                                        }
                                    }
                                    else //Normal Left Click
                                    {
                                        ClearSelection();

                                        if (squad_sprite.selected == false)
                                        {
                                            if (squad_sprite.transform.parent.gameObject.CompareTag("Player"))
                                            {
                                                squad_sprite.squadronVisualSelection(true);
                                                squadron_controller.selectPlayerSquadron(squad_sprite.num_squad, false);

                                                if (!squadrons_list.Contains(hit.collider.gameObject))
                                                    squadrons_list.Add(hit.collider.gameObject);
                                            }
                                               
                                        }
                                    }

                                }
                            }
                        }
                        else //Click on terrain clean the Unit_List
                        {
                            ClearSelection();
                        }
                        
                    }
                    else
                    {
                        ClearSelection();
                    }
                }
            }
            else
            {
                //Debug.Log("ON UI");
            }

        }

        public void onRightAction() //In case of get the "Right Mouse Button Click" or similar
        {
            if (input.mouse_right_click_press == true)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


                if (Physics.Raycast(ray, out hit))
                {
                    GameObject element = hit.collider.gameObject;

                    if (camera_controller.actual_level == 3) //Unit Camera Level
                    {
                        if (hit.collider.gameObject.GetComponent<Element>() != null)
                        {
                            if (element.GetComponent<Element>().type == Element.elem_type.Unit) //In case of action on Unit Element
                            {
                                if (element.GetComponent<Unit>() != null)
                                {
                                    if (element.GetComponent<Unit>().onDie == false) //If the selected NPC is still ALIVE
                                    {
                                        if (social_controller.GetRelationBetween((int)social_controller.player_faction, (int)element.GetComponent<Unit>().faction) == Element.elem_status.Enemy) //ENEMY
                                        {
                                            //Debug.Log("NPC ENEMY!");
                                            last_clicked_enemy = element.GetComponent<Unit>().gameObject;
                                            GameObject enemy_point = Instantiate(enemy_indicator, hit.point, Quaternion.identity); //ONLY FOR TESTS ###############################################
                                            enemy_point.transform.SetParent(last_clicked_enemy.gameObject.transform);
                                        }
                                        else if (social_controller.GetRelationBetween((int)social_controller.player_faction, (int)element.GetComponent<Unit>().faction) == Element.elem_status.Ally) //ALLY
                                        {
                                            //Debug.Log("NPC ALLY!");
                                        }
                                        else if (social_controller.GetRelationBetween((int)social_controller.player_faction, (int)element.GetComponent<Unit>().faction) == Element.elem_status.Neutral) //NEUTRAL
                                        {
                                            //Debug.Log("NPC NEUTRAL!");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                last_clicked_enemy = null;
                            }
                        }
                        else
                        {
                            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrain))
                            {
                                //Check if the clicked position is a navegable area
                                GetPointOnNav(hit.point);

                                if(not_navigable == false)
                                {
                                    if (units_list.Count > 0) //Check if any Unit has been selected
                                    {
                                        Instantiate(ubication_indicator, hit.point, Quaternion.identity); //ONLY FOR TESTS ############################################### 
                                    }

                                }

                                last_clicked_enemy = null;
                            }
                        }
                    }
                    else if (camera_controller.actual_level == 2) //Squadron Camera Level
                    {
                        if (hit.collider.gameObject.GetComponent<Squadron_UI_Sprite>() != null)
                        {
                            Squadron_UI_Sprite squad_sprite = hit.collider.gameObject.GetComponent<Squadron_UI_Sprite>();

                            Faction_AI faction_ai = GameObject.Find("Faction AI").GetComponent<Faction_AI>();

                            Unit enemy_captain = faction_ai.squadrons[squad_sprite.num_squad].squadron_captain.GetComponent<Unit>(); //#TO-DO: SHOULD GET THE ENEMY SQUADRON

                            if (social_controller.GetRelationBetween((int)social_controller.player_faction, (int)enemy_captain.faction) == Element.elem_status.Enemy)
                            {
                                last_clicked_enemy = enemy_captain.gameObject; //Get the enemy Squadron captain and assing to the selected ALLY unit as enemy
                            }

                        }
                        else
                        {
                            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrain))
                            {
                                //Check if the clicked position is a navegable area
                                GetPointOnNav(hit.point);

                                if (not_navigable == false)
                                {
                                    if (units_list.Count > 0) //Check if any Unit has been selected
                                    {
                                        Instantiate(ubication_indicator, hit.point, Quaternion.identity); //ONLY FOR TESTS ############################################### 
                                    }

                                }

                                last_clicked_enemy = null;
                            }
                        }
                    }

                }
            }
        }

        //Clear the units/squads selection
        public void ClearSelection()
        {
            visualSelection(false); //#TO-DO: CHECK!!!!!!!!!!!!!!!!

            units_list = new List<GameObject>(); //Clear the list of the selected units 

            squadron_controller.clearSquadronList(); //Set the Sprite of the squadrons to its default starter value
            squadrons_list = new List<GameObject>(); //Clear the list of the squadrons
        }

        //Check if the clicked position is a navegable area
        public void GetPointOnNav(Vector3 hit)
        {
            float height = 85.0f;
            NavMeshHit nav_hit;

            if (hit.y < height)
            {
                last_clicked_position = new Vector3();

                not_navigable = true;
            }
            else
            {
                if (NavMesh.SamplePosition(hit, out nav_hit, 1.0f, NavMesh.AllAreas))
                {
                    last_clicked_position = hit;

                    not_navigable = false;
                }
                else
                {
                    last_clicked_position = new Vector3();

                    not_navigable = true;
                }
            }
        
        }


        //----------------------UNIT LOGIC----------------------//
        //This functions is being called when clicked on defeated almah
        public void actionOnSelectedAlmah(GameObject enemy_almah)
        {
            //GENERAL == SCENE WITH NAME TO LOAD #TO-DO:

            string justice_system = "Justice_System_Alice";
            Almah almah = enemy_almah.GetComponent<Almah>();
            string general = almah.general_name;
            //string final_scene = "Justice_System_" + general;


            //CHANGE SCENE (LOAD THE LIBERATE SCENE)
            LoadScene(justice_system);
        }


        //This function holds multiple actions of the units (Move, Attack) for simplify the complexity of the code
        public void actionOnSelectedUnits(Vector3 enemy_position, GameObject selected_enemy)
        {
            if (units_list.Count > 0) //Check if any Unit is selected
            {
                int squad_number = squadron_controller.checkPlayerSquadronSelection(units_list);

                //In case of click on enemy unit but press other button which is not == "ATTACK"
                if (enemy_position == Vector3.zero) //Vector3.zero received from button action from UnitC_Menu script
                    last_clicked_enemy = null;

                //CHECK FOR SQUADRON OR INDIVIDUAL MOVEMENT
                if (squad_number != -1) //Check the selected units - If the units selected are ALL the squadron, then move as squadron
                {
                    //-----SQUADRON MOVEMENT-----//
                    GameObject squad_captain;
                    squad_captain = squadron_controller.squadrons[squad_number].squadron_captain;

                    if((squad_captain.GetComponent<Unit>().onDie == false) && (squad_captain.GetComponent<Unit>().onDamageReceived == false))
                    {

                        if (last_clicked_enemy == null) //(ONLY) MOVE ACTION
                        {
                            GameObject.Find("AudioManager").GetComponent<AudioManager_Actions>().PlayClip(); //PLAY AUDIO ACTION

                            squad_captain.GetComponent<NavMeshAgent>().SetDestination(last_clicked_position);
                            squad_captain.GetComponent<Unit>().selected_enemy = null; //Remove the selected enemy from attack action
                            squad_captain.GetComponent<Unit>().onAttack = false;

                            //SET UNIT RANGE
                            squad_captain.GetComponent<Unit>().setUnitRange(0, true);
                        }
                        else //ATTACK ACTION
                        {
                            NavMeshAgent squad_captain_agent = squad_captain.GetComponent<NavMeshAgent>();

                            squad_captain.GetComponent<Unit>().selected_enemy = last_clicked_enemy;
                            //SET UNIT RANGE
                            squad_captain.GetComponent<Unit>().setUnitRange(1, true);

                            //squad_captain_agent.SetDestination(selected_enemy.transform.position);
                            StartCoroutine(MoveCaptainToTarget(squad_captain, 5.0f, selected_enemy));
                        }

                        squad_captain.GetComponent<Unit>().onMove = true;
                    }

                    StartCoroutine(squadron_controller.formSquadron(squadron_controller.squadrons, squad_number, last_clicked_enemy, true)); //TO-DO: CHECK THE NUMBER OF UNITS TO DO A FORMATION OR ANOTHER
                    //-----SQUADRON MOVEMENT-----//
                   
                }
                else  //-----INDIVIDUAL MOVEMENT-----//
                {
                    foreach (GameObject unit in units_list)
                    {
                        if (unit != null) //Prevent possible bugs
                        {
                            Unit selected_element = unit.GetComponent<Unit>();

                            if ((selected_element.onDie == false) && (selected_element.onDamageReceived == false))
                            {
                                NavMeshAgent agent = selected_element.GetComponent<NavMeshAgent>();

                                if (last_clicked_enemy == null) //(ONLY) MOVE ACTION
                                {
                                    GameObject.Find("AudioManager").GetComponent<AudioManager_Actions>().PlayClip(); //PLAY AUDIO ACTION

                                    //-----INDIVIDUAL MOVEMENT-----//
                                    agent.SetDestination(last_clicked_position);
                                    //-----INDIVIDUAL MOVEMENT-----//

                                    selected_element.selected_enemy = null; //Remove the selected enemy from attack action
                                    selected_element.onAttack = false;

                                    //SET UNIT RANGE
                                    selected_element.setUnitRange(0, false);
                                }
                                else //ATTACK ACTION
                                {
                                    //agent.SetDestination(enemy_position);
                                    selected_element.selected_enemy = last_clicked_enemy;

                                    //SET UNIT RANGE
                                    selected_element.setUnitRange(1, false);

                                    StartCoroutine(MoveCaptainToTarget(unit, 5.0f, last_clicked_enemy));

                                    //###########CHECK THE TYPE OF SELECTED OPONENT UNIT
                                    //Debug.Log("SELECTED UNIT STATUS: " + last_clicked_enemy.GetComponent<Unit>().status);
                                    //selected_element.onAttack = true;
                                }

                                selected_element.onMove = true;

                                //Debug.Log("DISTANCE TO REACH?: " + Vector3.Distance(unit.transform.position, last_clicked_position));
                            }

                        }
                    }
                }
            }
        }

        //Update the captain selected enemy position
        public IEnumerator MoveCaptainToTarget(GameObject captain, float range, GameObject enemy)
        {
            if(captain != null)
            {
                NavMeshAgent captain_agent = captain.GetComponent<NavMeshAgent>();
                Vector3 previousTargetPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity);
                Unit unit = captain.GetComponent<Unit>();

                if (enemy != null)
                {
                    float distance = Vector3.SqrMagnitude(captain.transform.position - enemy.transform.position);

                    while (distance > range)
                    {
                        if (unit.selected_enemy == null)
                            break;

                        if (Vector3.SqrMagnitude(previousTargetPosition - enemy.transform.position) > range)
                        {
                            captain_agent.SetDestination(enemy.transform.position);
                            previousTargetPosition = enemy.transform.position;
                        }

                        if((captain != null) && (enemy != null))
                        {
                            distance = Vector3.SqrMagnitude(captain.transform.position - enemy.transform.position);
                        }
                        else
                        {
                            break;
                        }
                        
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }

            yield return null;
        }

        //----------------------UNIT LOGIC----------------------//


        //ONLY FOR TESTS ###############################################
        public void visualSelection(bool outlineActivated)
        {
            Outline outline = null;

            if (units_list.Count > 0) //Are Elements in the list
            {
                foreach (GameObject unit in units_list)
                {
                    if (unit != null)
                    {
                        if (unit.GetComponentInChildren<Outline>() != null)
                        {
                            outline = unit.GetComponentInChildren<Outline>();

                            if (outlineActivated == true)
                            {
                                outline.OutlineWidth = 5.0f;
                                outline.OutlineColor = Color.blue;
                            }
                            else
                            {
                                outline.OutlineWidth = 0.0f;
                                outline.OutlineColor = Color.white;
                            }
                        }

                    }

                }
            }
        }

        //ONLY FOR TESTS ###############################################
    }
}
