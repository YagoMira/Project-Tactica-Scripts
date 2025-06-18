using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Project_Tactica
{
    [System.Serializable]
    public class Squadron
    {
        public GameObject sprite;
        public bool hasGeneral = false;
        public GameObject squadron_captain; //Squadron CAPTAIN for the General/Captain system
        public List<GameObject> squadron;
        public bool allSquadronSelected = false;
    }

    //5 UNITS ----- ARROW
    public enum Formation_Name { No_Formation, Line, Little_Arrow, Diamond, Arrow };

    [System.Serializable]
    public class Formation
    {
        public Formation_Name name;
        public GameObject formation_parent;
    }

    public class SquadronController : MonoBehaviour
    {
        [HideInInspector]
        public GameController game_controller;
        ResourcesController resources_controller;

        public GameObject sprite_test;
        public string squad_ui_str = "Squad_UI_";
        public GameObject squad_ui;

        public Squadron[] squadrons;  //Array of arrays of Units (Squadron Creation)
        public Squadron[] aux_squadrons;

        public Formation[] formations;

        public int num_units = 5; //Number of units (GENERAL INCLUDE)
        public int num_squadrons = 5; //Number of squadrons

        public int num_formations; //Number of formations

        string hide_tag = "HIDE";



        private void Awake()
        {
           
            num_formations = Enum.GetNames(typeof(Formation_Name)).Length;

            /*
            formations = new Formation[num_formations];

            //IMPORTANT: USE THIS TO INITIALIZE THE ARRAY OF OBJECTS!
            for (int i = 0; i < num_formations; i++)
            {
                formations[i] = new Formation();
            }
            //
            */
        }

        void Start()
        {
            game_controller = this.GetComponent<GameController>();
            resources_controller = this.GetComponent<ResourcesController>();

            //CREATE THE SQUADRONS
            if (squadrons.Length > 0) //INITIALIZE IN CASE OF NOT BEING 
            {
                aux_squadrons = new Squadron[squadrons.Length];

                for (int i = 0; i < squadrons.Length; i++)
                {
                    aux_squadrons[i] = squadrons[i];
                }

                //aux_squadrons = squadrons;
            }


            squadrons = new Squadron[num_squadrons];
            //IMPORTANT: USE THIS TO INITIALIZE THE ARRAY OF OBJECTS!
            for (int i = 0; i < num_squadrons; i++)
            {
                squadrons[i] = new Squadron();
            }
            //

            //ADD THE SQUADRONS OF THE INSPECTOR (IF IT EXISTS)
            if(aux_squadrons.Length > 0)
            {
                for(int i = 0; i < aux_squadrons.Length; i++)
                {
                    squadrons[i] = aux_squadrons[i];
                }
            }

            //Make this way to initialize the Squadrons array with the Number of Units and Squadrons in the same Class
            for (int i = 0; i < num_squadrons; i++)
            {
                if (squadrons[i] != null) //Prevent possible bugs
                {
                    squadrons[i].sprite = sprite_test; //*TO-DO: CHANGE FOR SPECIFIC SPRITE/UNIT

                    if(squadrons[i].squadron == null)
                    {
                        squadrons[i].squadron = new List<GameObject>(new GameObject[num_units]);//new GameObject[5];
                    }
                    else
                    {
                        int unit_to_create = squadrons[i].squadron.Count;

                        for (int j = 0; j < (num_units - unit_to_create); j++)
                        {
                            squadrons[i].squadron.Add(null);
                        }
                    }
                }

            }

        }

        //Check the squadron formation taking as parameter the number of the units of the squadron
        public Formation checkSquadronFormation(Squadron[] squadrons, int squad_number)
        {
            int squadron_units = countUnitsSquadron(squadrons, squad_number);

            if (formations.Length == 5)
            {
                if (squadron_units >= 1)
                {
                    return formations[squadron_units - 1];
                }

            }
            else
            {
                Debug.LogError("FORMATIONS IS LESS THAN 5!");
            }

            return null;
        }


        //Check if the actual selected units are an entire squadron
        public int checkPlayerSquadronSelection(List<GameObject> units_list)
        {
            int selected_units = units_list.Count; //Get the number of selected units

            //Go squad by squad
            for (int squad_number = 0; squad_number < num_squadrons; squad_number++)
            {
                if (countUnitsSquadron(squadrons, squad_number) == selected_units) //Check if any of the squadrons has the same number of selected units
                {
                    for (int j = 0; j < selected_units; j++)
                    {
                        if (squadrons[squad_number].squadron[j] != null) //Prevent death/null gameobject units
                        {
                            if (units_list.Contains(squadrons[squad_number].squadron[j])) //Check if the selected units has all the squadron units
                            {
                                if (j == (selected_units - 1)) //Final of the loop -- ALL UNITS SELECTED ARE PART OF SQUADRON
                                {
                                    //Debug.Log("SQUADRON SELECTED!!");
                                    return squad_number;
                                }
                            }
                            else
                            {
                                //Debug.Log("NOT SQUADRON!");
                                break;
                            }
                        }
                    }

                }
            }

            return -1;
        }

        /**************************
        //Check if a specific unit is part of a squadron - AND MODIFY THE SQUADRON WHEN THE CAPTAIN DIES
        public void checkUnitHasSquadron(GameObject unit)
        {
            //Go squad by squad
            for (int squad_number = 0; squad_number < num_squadrons; squad_number++)
            {
                foreach (GameObject squadron_unit in squadrons[squad_number].squadron)
                {
                    if (unit == squadron_unit)
                    {
                        Debug.Log("UNIT FOUND!!!!!!!!!!!!!!!");
                        //squadrons[squad_number].squadron[squadrons[squad_number].squadron.IndexOf((unit))] = null; //Get the index and not the element for preserve the maximum of squadron units (Evade to use List.REMOVE)
                                                                                                                     //In consideration: If the death soldier is null, then when you check the formation should ignore the NULL
                        squadrons[squad_number].squadron.Remove(unit);

                        if (squadrons[squad_number].squadron_captain == unit)  //TO-DO: CHECK IF THE CAPTAIN DIES - CAPTAIN/GENERAL SYSTEM
                        {
                            int i = 0;
                            if(squadrons[squad_number].squadron[i] != null)
                            {
                                squadrons[squad_number].squadron_captain = squadrons[squad_number].squadron[i];
                            }
                            else
                            {
                                while (squadrons[squad_number].squadron[i] == null)
                                {
                                    squadrons[squad_number].squadron_captain = squadrons[squad_number].squadron[i];
                                    i++;
                                }
                            }
                            //TO-DO: Reorder the list for get the other units as the first ones of the array (The empty field should be at last)
                        }
                    }
                        
                }
            }
        }
        **************************/

        //Reorder the squadron when an unit of it dies
        public void reorderSquadron(GameObject unit)
        {
            Squadron[] squadrons = getUnitSquadron(unit);

            int squadron_number = getSquadron(squadrons, unit);
            int unit_isCaptain = isCaptain(unit);

            if (getSquadron(squadrons, unit) != -1) //If the actual unit is member of a squadron
            {
                int unit_index = getSquadronIndexBySquad(squadrons, unit, squadron_number);

                //squadrons[squadron_number].squadron.Remove(unit); //Remove the dead unit from the squadron
                squadrons[squadron_number].squadron[unit_index] = null; //Remove with a NULL value a dead unit from the squadron

                if (unit_isCaptain != -1) //If the actual unit is the captain of the squadron
                {
                    addCaptainSquadron(squadrons, squadron_number, 0); //TO-DO: Check the add first unit of the squadron as the captain (MAYBE SENIORITY SYSTEM?)
                }

            }
        }



        //Add the sprite for the camera level 2 to the desired squadron
        public void addUISquadron(Squadron[] squadrons, int squad_number)
        {
            int squadron_units = game_controller.units_list.Count; //Number of units in the actual selected squadron

            GameObject captain = null; //*TO-DO: Captain system?
            GameObject new_squadron_ui = null;

            if (squadrons[squad_number].squadron[0] != null)
                captain = squadrons[squad_number].squadron[0];  //Add the sprite to the first unit of the squadron (Interpretate this as the parent)         
                                                                //*TO-DO: Maybe in the future calculate the center point of the squadron?

            if (captain != null)
            {
                new_squadron_ui = Instantiate(squad_ui, captain.transform.position, squad_ui.transform.rotation);
                new_squadron_ui.SetActive(false); //Starts as hidden
                new_squadron_ui.name = squad_ui_str + squad_number; //squad_ui_str + squad_number == UI SQUADRON REFERENCE
                new_squadron_ui.transform.parent = captain.transform;
                new_squadron_ui.transform.localPosition += new Vector3(0.0f, 15.0f, 0.0f); //Elevate the Squad UI position
                new_squadron_ui.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                //In case of ENEMY UI 
                if(!captain.CompareTag("Player"))
                {
                    new_squadron_ui.transform.Find("Sprite").GetComponent<Button>().interactable = false;
                }

                //MODIFY THE VALUES OF THE VARIABLES INSIDE THE SQUAD_UI
                Squadron_UI_Sprite squad_UI = new_squadron_ui.GetComponent<Squadron_UI_Sprite>();
                squad_UI.num_squad = squad_number;

            }
        }


        //Add the facilities for the squadron UI (Add the Sprite UI and other UI facilities to the captain of the desired squadron)
        public void addUISquadron(Squadron[] squadrons, int squad_number, int squad_soldier)
        {
            removeUISquadron(squadrons, squad_number);
            reorderCaptainSquadron(squadrons, squad_number, squad_soldier);
            addUISquadron(squadrons, squad_number);
        }

        //Remove the sprite of the level 2 of the desired squadron
        public void removeUISquadron(Squadron[] squadrons, int squad_number)
        {
            GameObject captain = null; //*TO-DO: Captain system?
            GameObject squadron_ui = null;

            if (squadrons[squad_number].squadron[0] != null)
                captain = squadrons[squad_number].squadron[0];  //The sprite of the Squadron is child of first unit of the squadron (Interpretate this as the parent)         


            if (captain != null)
            {
                if (captain.transform.Find((squad_ui_str + squad_number).ToString()) != null)
                    squadron_ui = captain.transform.Find((squad_ui_str + squad_number).ToString()).gameObject;

                if (squad_ui != null)
                    Destroy(squadron_ui);
            }
        }

        //Add the Captain to the actual Squadron - OR REASSIGN
        public void addCaptainSquadron(Squadron[] squadrons, int squad_number, int squad_soldier)
        {
            if (squadrons[squad_number] != null) //Prevent possible bugs
            {
                if(countUnitsSquadron(squadrons, squad_number) > 0)
                {
                    if (squadrons[squad_number].squadron[squad_soldier] != null)
                    {
                        squadrons[squad_number].squadron_captain = squadrons[squad_number].squadron[squad_soldier];

                        addUISquadron(squadrons, squad_number, squad_soldier);
                    }
                    else
                    {
                        int aux_squad_soldier = squad_soldier;

                        while((squadrons[squad_number].squadron[aux_squad_soldier] == null))
                        {
                            aux_squad_soldier++;
                        }

                        squadrons[squad_number].squadron_captain = squadrons[squad_number].squadron[aux_squad_soldier];

                        addUISquadron(squadrons, squad_number, aux_squad_soldier);
                    }
                }
               
            }
        }


        //Set the captain as the first element of the squadron array
        public void reorderCaptainSquadron(Squadron[] squadrons, int squad_number, int squad_soldier)
        {
            //Get the new captain object
            GameObject new_captain = squadrons[squad_number].squadron[squad_soldier];
            //Save the actual first captain
            GameObject old_captain = squadrons[squad_number].squadron[0]; //Captain == Index [0]
            //Set the new captain
            squadrons[squad_number].squadron[0] = new_captain;
            //Set the old one in the previous new captain position
            squadrons[squad_number].squadron[squad_soldier] = old_captain;
        }

        //Get the squadron of the actual unit
        public Squadron[] getUnitSquadron(GameObject unit)
        {
            Squadron[] unit_squadron = null;

            if (unit.transform.parent != null)
            {
                if (unit.transform.parent.GetComponent<Squad_AI>() != null)
                {
                    unit_squadron = unit.transform.parent.GetComponent<Squad_AI>().faction_ai.squadrons;
                }
                else //PLAYER SQUADRONS - NOT SQUAD AI
                {
                    unit_squadron = squadrons;
                }
            }
            else //MAYBE its an player unit (Not depends of SQUAD_AI) #TO-DO: INSERT ALL PLAYER UNITS INSIDE PARENT "SQUADRON_X" GAMEOBJECT (AS THE SQUAD_AI)?
            {
                //#TO-DO: TAKE CARE IF THE PLAYER UNIT ARE INSIDE PARENT "SQUADRON_X" GAMEOBJECT IN THE FUTURE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                unit_squadron = squadrons;
            }

            return unit_squadron;
        }

        //Check if the unit as parameter is a captain of a squadron
        public int isCaptain(GameObject unit)
        {
            Squadron[] squadrons = getUnitSquadron(unit);

            // Debug.Log("SQUADRONS: " + squadrons + " NUM: " + getSquadron(squadrons, unit));

            if (squadrons != null)
            {
                if (countUnitsSquadron(squadrons, getSquadron(squadrons, unit)) > 0)
                {
                    for (int i = 0; i < countUnitsSquadron(squadrons, getSquadron(squadrons, unit)); i++)
                    {
                        if(squadrons[i] != null)
                        {
                            if (squadrons[i].squadron[0] != null)
                            {
                                if ((squadrons[i].squadron[0] == unit) || (squadrons[i].squadron_captain == unit)) //Always the index 0 will be the captain
                                {
                                    return i; //Num Squadron
                                }
                            }
                        }
               


                        //for (int j = 0; j < num_units; j++)
                        //{
                        //    if (squadrons[i].squadron[j] == unit)
                        //    {
                        //        return i; //Num Squadron
                        //    }
                        //}
                    }
                }

            }


            return -1;
        }

        public GameObject getCaptain(Squadron[] squadrons, int squad_number)
        {
            if (squadrons[squad_number].squadron_captain != null)
            {
                return squadrons[squad_number].squadron_captain;
            }
            else
            {
                return squadrons[squad_number].squadron[0];
            }

        }

        //Clear the PLAYER squadron as parameter of the unit selection
        public void clearPlayerSquadronSelect(int squad_number)
        {
            Debug.Log("TEST: COUNT: " + game_controller.units_list.Count);

            for (int i = game_controller.units_list.Count - 1; i >= 0; i--) //for(int i = 0; i < game_controller.units_list.Count; i++)
            {
                if (getSquadron(squadrons, game_controller.units_list[i]) == squad_number)
                {
                    Debug.Log("TEST: I: " + game_controller.units_list[i].name);
                    game_controller.units_list.Remove(game_controller.units_list[i]);
                }
            }

        }

        //Check if the unit as parameter is member of any squadron (Get the squadron ID)
        public int getSquadron(Squadron[] squadrons, GameObject unit)
        {
            for (int squad_number = 0; squad_number < squadrons.Length; squad_number++)
            {
                foreach (GameObject squadron_unit in squadrons[squad_number].squadron)
                {
                    if (squadron_unit == unit)
                        return squad_number;
                }
            }

            return -1;
        }

        //Get the squadron of the parameter GameObject (AI or Player's Squadron)
        public Squadron getAnySquadron(GameObject last_clicked_enemy)
        {
            Squadron[] aux_enemy_squadron = null; //Used for diferentiation between player units and AI SQUAD ones
            Squadron enemy_squadron = null;

            //Check if the Unit is an Enemy NPC (Squadron AI)
            if (last_clicked_enemy.transform.parent != null)
            {
                if (last_clicked_enemy.transform.parent.GetComponent<Squad_AI>() != null)
                {
                    aux_enemy_squadron = last_clicked_enemy.transform.parent.GetComponent<Squad_AI>().faction_ai.squadrons;
                }
                else //PLAYER SQUADRONS - NOT SQUAD AI
                {
                    aux_enemy_squadron = this.squadrons;
                }
            }
            else //MAYBE its an player unit (Not depends of SQUAD_AI) #TO-DO: INSERT ALL PLAYER UNITS INSIDE PARENT "SQUADRON_X" GAMEOBJECT (AS THE SQUAD_AI)?
            {
                //#TO-DO: TAKE CARE IF THE PLAYER UNIT ARE INSIDE PARENT "SQUADRON_X" GAMEOBJECT IN THE FUTURE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                aux_enemy_squadron = this.squadrons; //PLAYER SQUADRONS - NOT SQUAD AI
            }

            //Check if the selected enemy has a squadron or not
            if (getSquadron(aux_enemy_squadron, last_clicked_enemy) != -1)
            {
                enemy_squadron = aux_enemy_squadron[getSquadron(aux_enemy_squadron, last_clicked_enemy)];
            }
            else
            {
                enemy_squadron = null;
            }

            return enemy_squadron;
        }

        //Get the index of a unit searching on all squadrons
        public int getSquadronIndex(Squadron[] squadrons, GameObject unit)
        {
            int index = 0;

            for (int squad_number = 0; squad_number < squadrons.Length; squad_number++)
            {
                foreach (GameObject squadron_unit in squadrons[squad_number].squadron)
                {
                    if (squadron_unit == unit)
                        return index;

                    index++;
                }
            }

            return -1;
        }

        //Get the index of a unit of specific squadron
        public int getSquadronIndexBySquad(Squadron[] squadrons, GameObject unit, int squad_number)
        {
            int index = 0;


            foreach (GameObject squadron_unit in squadrons[squad_number].squadron)
            {
                if (squadron_unit == unit)
                    return index;

                index++;
            }

            return -1;
        }

        //Check if the defeated enemy has a squadron. If has one, select an enemy from there
        public GameObject getNextEnemy(Squadron enemy_squadron, GameObject selected_enemy) //#TO-DO: The if's of the "SQUAD_AI" can be optimized in other lines of code of this same file.
        {
            GameObject next_enemy = null;
            
            if (enemy_squadron != null)
            {
                for (int i = 0; i < countUnitsOnIndividualSquadron(enemy_squadron); i++)
                {
                    if (countUnitsOnIndividualSquadron(enemy_squadron) > 1)
                    {
                        if(enemy_squadron.squadron[i] != null)
                        {
                            if (enemy_squadron.squadron[i].gameObject != null)
                            {
                                if (selected_enemy != null)
                                {
                                    if (enemy_squadron.squadron[i].gameObject != selected_enemy)
                                    {
                                        next_enemy = enemy_squadron.squadron[i].gameObject;
                                        return next_enemy;
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        return next_enemy; //Null
                    }
                }
            }

            return next_enemy; //Null
        }

        //Check if the actual enemy is selected by other member of the actual squadron
        public bool isEnemySelected(Squadron[] squadrons, GameObject enemy_unit, int squad_number)
        {
            bool selected = false;

            if(enemy_unit != null)
            {
                for (int i = 0; i < countUnitsSquadron(squadrons, squad_number); i++)
                {
                    if(squadrons[squad_number].squadron[i] != null)
                    {
                        if ((squadrons[squad_number].squadron[i].GetComponent<Unit>().selected_enemy) != null)
                        {
                            if ((squadrons[squad_number].squadron[i].GetComponent<Unit>().selected_enemy) == enemy_unit)
                            {
                                selected = true;
                            }
                        }
                    }

                }
            }

            return selected;
        }

        //Add new unit to the desired squadron
        public void addUnitToSquadron(int squad_number, GameObject unit)
        {
            if (countUnitsSquadron(squadrons, squad_number) < num_units)
            {
                for(int i = 0; i < num_units; i++)
                {
                    if(squadrons[squad_number].squadron[i] == null)
                    {
                        squadrons[squad_number].squadron[i] = unit;

                        if(squadrons[squad_number].squadron_captain == null)
                        {
                            addCaptainSquadron(squadrons, squad_number, 0);
                            //squadrons[squad_number].squadron_captain = squadrons[squad_number].squadron[i];
                        }
                        break;
                    }
                }
                
            }
            
        }

        //Create squadron with the actual selected units
        public void createPlayerSquadron(int squad_number)
        {
            int squadron_units = game_controller.units_list.Count; //Units for the "NEW" squadron

            if (squadrons[squad_number] != null) //Prevent possible bugs
            {
                for (int i = 0; i < squadron_units; i++)
                {
                    //squadrons[squad_number].squadron.Add(game_controller.units_list[i]);
                    squadrons[squad_number].squadron[i] = game_controller.units_list[i];
                }

                addCaptainSquadron(squadrons, squad_number, 0); //Add the first soldier as the captain - #############ONLY FOR TESTS

            }
        }

        //Select all units of the desired squadron
        public void selectPlayerSquadron(int squad_number, bool additive) //Bool additive : enables to shift selection on the squadron
        {
            int squadron_units = squadrons[squad_number].squadron.Count;//squadrons[squad_number].squadron.Length;
                                                                        //Reset the actual selected units for ONLY SELECT THE SQUADRON

            if(additive == false)
                game_controller.units_list = new List<GameObject>();

            //Add the squadron units to the selected ones
            for (int i = 0; i < squadron_units; i++)
            {
                if(squadrons[squad_number].squadron[i] != null) //NOT EMPTY GAMEOBJECT
                {
                    game_controller.units_list.Add(squadrons[squad_number].squadron[i]);
                    selectedPlayerUnitOutline(squadrons[squad_number].squadron[i], true);
                }
            }

         
        }

        //Selected Unit will activate its outline
        public void selectedPlayerUnitOutline(GameObject unit, bool activate)
        {
            Outline outline = unit.GetComponent<Outline>();

            if(activate == true)
            {
                outline.OutlineWidth = 5.0f;
            }
            else
            {
                outline.OutlineWidth = 0.0f;
            }
        }

        //Delete the entire squadron of units and modify the amount of resources
        public void deleteSquadron(int squad_number)
        {
            int squadron_units = squadrons[squad_number].squadron.Count; //squadrons[squad_number].squadron.Length;

            if (countUnitsSquadron(squadrons, squad_number) > 0) //Squadron has units, remove
            {
                Debug.Log("REMOVING SQUADRON: " + squad_number);

                for (int i = 0; i < squadron_units; i++)
                {
                    squadrons[squad_number].squadron[i] = null;

                }

                //APPLY THE DESIRED OPERATIONS TO THE RESOURCES SYSTEM:
                float credits_value = resources_controller.getCredits();
                resources_controller.setCredits(credits_value + 100);
            }
            else
            {
                Debug.Log("SELECTED SQUADRON HAS NOT ANY UNITS");
            }
                
           
        }
        
        //TO-DO: REFACTOR HIDE/SHOW SQUADRON ???
        //Hide the squadron MESH (Depends on the camera height)
        public void hideSquadron(Squadron[] squadrons, int squad_number)
        {
            if(squadrons[squad_number] != null)
            {

                int squadron_units = 0;
                squadron_units = countUnitsSquadron(squadrons, squad_number); //num_units; 

                //Hide the mesh of the specific squadron unit (and its childs)
                for (int i = 0; i < squadron_units; i++)
                {
                    if (squadrons[squad_number].squadron[i] != null)
                    {
                        //WITH MESH
                        //if (squadrons[squad_number].squadron[i].GetComponent<SkinnedMeshRenderer>() != null)
                        //    squadrons[squad_number].squadron[i].GetComponent<SkinnedMeshRenderer>().enabled = false;

                        //RECURSIVELY HIDES ALL MESHES IN ALL CHILDREN OF THE UNIT
                        foreach (Transform c in squadrons[squad_number].squadron[i].transform)
                        {
                            visibilitySquadronMesh(c, false);
                        }

                    }
                }
            }
        }

        //Show the squadron MESH (Depends on the camera height)
        public void showSquadron(Squadron[] squadrons, int squad_number)
        {
            int squadron_units = 0;
            squadron_units = countUnitsSquadron(squadrons, squad_number); //num_units; 

            //Show the mesh of the specific squadron unit (and its childs)
            for (int i = 0; i < squadron_units; i++)
            {
                if (squadrons[squad_number].squadron[i] != null)
                {
                    //WITH MESH
                    //if(squadrons[squad_number].squadron[i].GetComponent<SkinnedMeshRenderer>() != null)
                    //    squadrons[squad_number].squadron[i].GetComponent<SkinnedMeshRenderer>().enabled = true;

                    //RECURSIVELY HIDES ALL MESHES IN ALL CHILDREN OF THE UNIT
                    foreach (Transform c in squadrons[squad_number].squadron[i].transform)
                    {
                        visibilitySquadronMesh(c, true);
                    }
                }
                    
            }
        }

        //Hide or show the desired unit mesh
        public void visibilitySquadronMesh(Transform children, bool enabled)
        {
            //WITH MESH
            /*
            if (children.gameObject.GetComponent<SkinnedMeshRenderer>() != null)
            {
                if (enabled == true)
                {
                    children.GetComponent<SkinnedMeshRenderer>().enabled = true;
                }
                else
                {
                    children.GetComponent<SkinnedMeshRenderer>().enabled = false;
                }
            }

            foreach (Transform c in children)
            {
                Debug.Log("NAME:?" + c.gameObject.name);

                if (c.GetComponent<SkinnedMeshRenderer>() != null)
                {
                  
                    if (enabled == true)
                    {
                        c.GetComponent<SkinnedMeshRenderer>().enabled = true;
                    }
                    else
                    {
                        c.GetComponent<SkinnedMeshRenderer>().enabled = false;
                    }
                }
            }
            */

            //WITH TAGS
            if (children.gameObject.CompareTag(hide_tag) == true)
            {
                if (enabled == true)
                {
                    children.gameObject.SetActive(true);
                }
                else
                {
                    children.gameObject.SetActive(false);
                }
            }

            foreach (Transform c in children)
            {
                if (c.gameObject.CompareTag(hide_tag) == true)
                {
                    if (enabled == true)
                    {
                        c.gameObject.SetActive(true);
                    }
                    else
                    {
                        c.gameObject.SetActive(false);
                    }
                }

            }
        }

        //Show the squadron SPRITE (Depends on the camera height)
        public void showUISquadron(Squadron[] squadrons, int squad_number, bool activated) //Activated = Show or hide the squadron script
        {
            if (squadrons[squad_number] != null)
            {
                if (squadrons[squad_number].squadron != null)
                {

                    if (countUnitsSquadron(squadrons, squad_number) > 0)
                    {
                        int squadron_units = squadrons[squad_number].squadron.Count; //squadrons[squad_number].squadron.Length;

                        //GameObject captain_sprite = null; //*TO-DO: Captain system?
                        GameObject squadron_ui = null;

                        if (squadrons[squad_number].squadron[0] != null)
                            if (squadrons[squad_number].squadron[0].transform.Find(squad_ui_str + squad_number).gameObject != null) //If the sprite searched by name is not null
                                squadron_ui = squadrons[squad_number].squadron[0].transform.Find(squad_ui_str + squad_number).gameObject;  //Get the sprite of the first unit of the squadron

                        if (squadron_ui != null) //Enable or disable the squadron sprite
                            squadron_ui.SetActive(activated);
                    }

                }

            }

        }

        //Clear the squadron list 
        public void clearSquadronList()
        {
            for(int i = 0; i < game_controller.squadrons_list.Count; i++)
            {
                if(game_controller.squadrons_list[i].GetComponent<Squadron_UI_Sprite>() != null)
                {
                    game_controller.squadrons_list[i].GetComponent<Squadron_UI_Sprite>().squadronVisualSelection(false);
                }
            }
        }

        //Count the number of units in the desired squadron (ALL THE GAMEOBJECTS ATTACHED TO THE SQUADRON)
        public int countUnitsSquadron(Squadron[] squadrons, int squad_number)
        {
            int units = 0;

            if (squadrons[squad_number] != null)
            {
                if(squadrons[squad_number].squadron != null)
                {
                    int squadron_units = squadrons[squad_number].squadron.Count; //squadrons[squad_number].squadron.Length;

                    for (int i = 0; i < squadron_units; i++)
                    {
                        if (squadrons[squad_number].squadron[i] != null)
                            units += 1;
                    }

                    return units;
                }

                return units;
            }
            return units;
        }

        public int countUnitsOnIndividualSquadron(Squadron squadron)
        {
            int units = 0;


            if (squadron != null)
            {
                int squadron_units = squadron.squadron.Count; //squadrons[squad_number].squadron.Length;

                for (int i = 0; i < squadron_units; i++)
                {
                    if (squadron.squadron[i] != null)
                        units += 1;
                }

                return units;
            }

            return units;
        }


        //----------------------FORMATIONS----------------------//
        //Selec the desired formation
        public Formation selectFormation(Formation_Name formation_name)
        {
            for(int i = 0; i < formations.Length; i++)
            {
                if(formations[i].name == formation_name)
                {
                    //Debug.LogWarning(formations[i].name);
                    return formations[i];
                }
            }

            return null;
        }

        public GameObject getFormationGameObject(Squadron[] squadrons, int squad_number, GameObject squad_captain)
        {
            GameObject formation = null;
            Formation_Name formation_name = checkSquadronFormation(squadrons, squad_number).name;
            Formation squad_formation = selectFormation(formation_name);

            if (squad_captain.transform.Find(formation_name.ToString() + "(Clone)") == null) //Do this for prevent multiple instantiations of the formation
            {
                if(squad_captain.transform.Find(formation_name.ToString() + "_Base(Clone)") == null) //#TO-DO: Refactor and redo this method without "Base" and "Clone"
                {
                    formation = Instantiate(squad_formation.formation_parent, squad_captain.transform.position, squad_captain.transform.rotation);
                }
            }
            else
            {
                formation = squad_captain.transform.Find(formation_name.ToString() + "(Clone)").gameObject;
            }

            return formation;
        }

        public bool onGetPositionateAsync(Squadron[] squadrons, int squad_number)
        {
            int unit_positionate = 0;

            for (int i = 0; i < countUnitsSquadron(squadrons, squad_number); i++)
            {
                if(!squadrons[squad_number].squadron[i].GetComponent<NavMeshAgent>().pathPending)
                {
                    unit_positionate = unit_positionate + 1;
                }
            }

            if (unit_positionate == countUnitsSquadron(squadrons, squad_number)) //If all the units of the squadron are positionated
                return true;
            else
                return false;
        }


        public void moveSquadron(Squadron[] squadrons, int squad_number, Transform moveTo_position, bool onMove, bool independentCaptain) //For player squadrons this functions doesn't affect the captain behaviour
        {
            int squad_units = countUnitsSquadron(squadrons, squad_number);

            for (int i = 0; i < squad_units; i++)
            {

                if (squadrons[squad_number].squadron[i] != null)
                {

                    if (independentCaptain == true) //Player's squadrons - Or want and independent captain behaviour
                    {
                        if (i == 0) //Evade the captain index
                            i++;
                    }

                    if (onMove == true)
                    {
                       
                        if (squadrons[squad_number].squadron[0].GetComponent<Unit>().onAttack != true) //DO THIS FOR THE ENEMY AI ENTER THE ATTACK STATE(NOT CAPTAIN)
                        {
                            if(squad_units > 1)
                            {
                                if ((squadrons[squad_number].squadron[i]) != null)
                                {
                                    if ((squadrons[squad_number].squadron[i]).GetComponent<Unit>().onDamageReceived == false)
                                    {
                                        Transform formation_position = moveTo_position.transform.GetChild(i);

                                        //#TO-DO: POLISH
                                        //(squadrons[squad_number].squadron[i]).GetComponent<Unit>().speed = actual_child_unit.unit_stats.speed + (actual_child_unit.unit_stats.speed % 10);
                                        (squadrons[squad_number].squadron[i]).GetComponent<NavMeshAgent>().stoppingDistance = 0.0f; //TO-DO: RESET THE STOPPINGDISTANCE TO RANGE PARAMETERS WHEN ATTACK
                                        (squadrons[squad_number].squadron[i]).GetComponent<Unit>().squadron_position = formation_position.transform.gameObject; //If NULL, move to destination with navmesh

                                        (squadrons[squad_number].squadron[i]).GetComponent<Unit>().onMove = true;
                                        (squadrons[squad_number].squadron[i]).GetComponent<Unit>().onSquadronMove = true;
                                    }
                                }
                            }
                           
                            
                        }
                    }
                    else
                    {
                        if (squad_units > 1)
                        {
                            if ((squadrons[squad_number].squadron[i]) != null)
                            {
                                (squadrons[squad_number].squadron[i]).GetComponent<Unit>().squadron_position = null;

                                (squadrons[squad_number].squadron[i]).GetComponent<Unit>().onMove = false;
                                (squadrons[squad_number].squadron[i]).GetComponent<Unit>().onSquadronMove = false;
                                (squadrons[squad_number].squadron[i]).GetComponent<Unit>().onMoveSquadronAction(false);
                            }
                        }

                    }
                }
            }

        }

        public void attackSquadron(Squadron[] squadrons, int squad_number, GameObject last_clicked_enemy, int camera_level, bool independentCaptain) //For player squadrons/Captain independent behaviour this functions doesn't affect the captain behaviour
        {
            Squadron enemy_squadron = null;

            bool onAttack; //Boolean facility

            int squad_units = countUnitsSquadron(squadrons, squad_number);

            if (last_clicked_enemy == null)
            {
                onAttack = false;
            }
            else
            {
                onAttack = true;

                //ENEMY SQUADRON
                enemy_squadron = getAnySquadron(last_clicked_enemy);
            }


            for (int i = 0; i < squad_units; i++)
            {
                if (squadrons[squad_number].squadron[i] != null)
                {
                    if (independentCaptain == true) //Player's squadrons - Or want and independent captain behaviour
                    {
                        if (i == 0) //Evade the captain index
                            i++;
                    }

                    if (squad_units > 1)
                    {
                        if (onAttack == true)
                        {
                            if (camera_level == 2) //LEVEL 2 - AUTOMATIC ENEMY SELECTION
                            {
                                //NEW ATTACK : ATTACK THE MOST NEAR ENEMY AND NOT SELECTED BY OTHER MEMBER OF THE ACTUAL SQUADRON 

                                //Select the first found enemy on the array of the squadron (DON'T NEED TO DO THE CALCULATIONS FOR TWO REASONS: 1º THE FORMATIONS SITUATE THE SOLDIERS WITH THE INDEX, 2º MORE RANDOMNESS ON THE COMBAT)
                                if (i == 0) //CAPTAIN OF THE SQUAD
                                {
                                    (squadrons[squad_number].squadron[i]).GetComponent<Unit>().selected_enemy = last_clicked_enemy;
                                }
                                else //OTHER MEMBERS OF THE SQUAD
                                {
                                    if ((squadrons[squad_number].squadron[i]) != null)
                                    {
                                        if (enemy_squadron != null) //If the enemy has squadron
                                        {
                                            for (int enemy_unit = 0; enemy_unit < enemy_squadron.squadron.Count; enemy_unit++)
                                            {
                                                if (enemy_squadron.squadron[enemy_unit] != null)
                                                {
                                                    //Debug.Log("ENEMY: " + enemy_squadron.squadron[enemy_unit].gameObject.name);

                                                    //If the Attacker squadron has LESS units than the Defender
                                                    if (i <= enemy_unit)
                                                    {
                                                        if (isEnemySelected(squadrons, enemy_squadron.squadron[enemy_unit], squad_number) == false)
                                                        {
                                                            (squadrons[squad_number].squadron[i]).GetComponent<Unit>().selected_enemy = enemy_squadron.squadron[enemy_unit];
                                                        }
                                                    }
                                                    else  //If the Attacker squadron has MORE units than the Defender
                                                    {
                                                        if ((squadrons[squad_number].squadron[i]) != null)
                                                            (squadrons[squad_number].squadron[i]).GetComponent<Unit>().selected_enemy = last_clicked_enemy; //Select the first enemy as target
                                                    }
                                                }
                                            }
                                        }
                                        else //Individual enemy
                                        {
                                            if ((squadrons[squad_number].squadron[i]).GetComponent<Unit>().selected_enemy == null)
                                            {
                                                (squadrons[squad_number].squadron[i]).GetComponent<Unit>().selected_enemy = last_clicked_enemy;
                                            }
                                        }
                                        //Set Attack Range
                                        (squadrons[squad_number].squadron[i]).GetComponent<NavMeshAgent>().stoppingDistance = (squadrons[squad_number].squadron[i]).GetComponent<Unit>().unit_stats.range;
                                        //Move to the enemy
                                        (squadrons[squad_number].squadron[i]).GetComponent<NavMeshAgent>().SetDestination(last_clicked_enemy.transform.position);
                                    }
                                }
                            }
                            else if (camera_level == 3) //LEVEL 3 - ATTACK THE FIRST FOUND ENEMY
                            {
                                if((squadrons[squad_number].squadron[i]) != null)
                                {
                                    float range = (squadrons[squad_number].squadron[i]).GetComponent<Unit>().unit_stats.range;
                                    float d_to_enemy = Vector3.Distance((squadrons[squad_number].squadron[i]).transform.position, last_clicked_enemy.transform.position);

                                    (squadrons[squad_number].squadron[i]).GetComponent<NavMeshAgent>().stoppingDistance = range;

                                    if (d_to_enemy > range)
                                    {
                                        (squadrons[squad_number].squadron[i]).GetComponent<Unit>().onMove = true;
                                        (squadrons[squad_number].squadron[i]).GetComponent<NavMeshAgent>().SetDestination(last_clicked_enemy.transform.position);
                                    }
                                    else
                                    {
                                        (squadrons[squad_number].squadron[i]).GetComponent<Unit>().onSquadronMove = false;
                                        (squadrons[squad_number].squadron[i]).GetComponent<Unit>().onMove = false;
                                        (squadrons[squad_number].squadron[i]).GetComponent<Unit>().onAttack = true;
                                    }


                                    if ((squadrons[squad_number].squadron[i]).GetComponent<Unit>().selected_enemy == null) //Prevent from desascalate from Level 2 to Level 3
                                    {
                                        if(last_clicked_enemy != null)
                                            (squadrons[squad_number].squadron[i]).GetComponent<Unit>().selected_enemy = last_clicked_enemy;
                                    }

                                }

                            }
                        }
                        else
                        {
                            //CUIDADO AQUÍIIIIII!!!!!########################################################################
                            /*
                            Debug.Log("NO ENTRES AQUÍ!");
                            (squadrons[squad_number].squadron[i]).GetComponent<Unit>().selected_enemy = null; //Remove the selected enemy from attack action
                            (squadrons[squad_number].squadron[i]).GetComponent<Unit>().onAttack = false;
                            */
                        }
                    }
                }
            }
        }



        //Positionate the units of the squadron with the desired formation
        public IEnumerator formSquadron(Squadron[] squadrons, int squad_number, GameObject last_clicked_enemy, bool independentCaptain)
        {

            GameObject squad_captain = getCaptain(squadrons, squad_number);
            GameObject squad_captain_copy = squad_captain; //Do this for prevent any posible Unity Crashes

            GameObject new_formation = getFormationGameObject(squadrons, squad_number, squad_captain);

            int camera_level = game_controller.camera_controller.actual_level;

            //-----SQUADRON MOVEMENT -----//

            if (new_formation != null)
            {
                new_formation.transform.parent = squad_captain_copy.transform; //-----Formation parenting-----//

                moveSquadron(squadrons, squad_number, new_formation.transform, true, independentCaptain);

            }


            yield return new WaitUntil(squad_captain.GetComponent<Unit>().onMoveAsync); //When the captain finish the movement, the other members of the squadron should finish the movement

            //-----SQUADRON MOVEMENT-----//


            //#############################################################TO-DO: CHECK!!!!!!!!!!!!!!!!!!!!!!!!!!!???????????????????????????
            //while (onGetPositionateAsync(countUnitsSquadron(squadrons, squad_number)) != true)
            //{
            //    Debug.Log("REPOSITIONATE");
            //}


            //-----SQUADRON STOP!-----//

            moveSquadron(squadrons, squad_number, new_formation.transform, false, independentCaptain);

            //-----SQUADRON STOP!-----//

            //-----SQUADRON ATTACK-----//
            if (last_clicked_enemy == null) //MOVE ACTION
            {
                attackSquadron(squadrons, squad_number, null, camera_level, independentCaptain);

            }
            else
            {
                attackSquadron(squadrons, squad_number, last_clicked_enemy, camera_level, independentCaptain);
            }
            //-----SQUADRON ATTACK-----//


            //StopCoroutine(formSquadron(squadrons, squad_number, last_clicked_enemy, independentCaptain));
        }

        //----------------------FORMATIONS----------------------//

    }

}

