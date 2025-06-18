using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Project_Tactica; //BECAUSE OF MUTIPLE NAMESPACES USE
using ADAPT; //BECAUSE OF MUTIPLE NAMESPACES USE

//Allows Agent move to determinate position.
//***Target = Assign target with Unity Inspector.***
public class Attack_Squad_Action : Action
{
    string a_name = "Attack";
    Agent agent;
    NavMeshAgent actual_agent;

    void Awake()
    {
        //Assign values across Script and not appear in the inspector
        /*
            KeyValuePair<string, Resource> GoTo_preconditions =
                new KeyValuePair<string, Resource>("isNear", new WorldResource("isNear", target, target, 5, 50.0f));
            KeyValuePair<string, Resource> GoTo_effects =
                new KeyValuePair<string, Resource>("onPosition", new StatusResource("onPosition", true, true, 5));
        */

        //Add preconditions and effects and NOT appears in inspector
        /*
            preconditions.Add(GoTo_preconditions.Key, GoTo_preconditions.Value);
            effects.Add(GoTo_effects.Key, GoTo_effects.Value);
        */

        //Assign values across Inspector and not appears in it.
        //ResourceStruct GoTo_preconditions = new ResourceStruct("isNear", new WorldResource("isNear", target, 5, 50.0f));

        ResourceStruct AttackSquad_precondition_1 = new ResourceStruct("findEnemy", new StatusResource("findEnemy", true, 10));
        ResourceStruct AttackSquad_effect_1 = new ResourceStruct("killSquad", new StatusResource("killSquad", true, 5));

        actionName = a_name;

        if (addedPreviously == false)
        {
            //Add preconditions and effects and appears in inspector
            AddPreconditionToList(AttackSquad_precondition_1);
            AddEffectToList(AttackSquad_effect_1);
        }

        //GetComponents
        agent = gameObject.GetComponent<Agent>();

        //WARNING MESSAGE!.
        //Debug.Log("<color=blue> Action: </color>" + actionName + "<color=blue> has preconditions/effects added by code,</color> <color=red> DON'T ADD MORE VIA INSPECTOR!.</color>");
    }

    private void Update() { }


    public override void PerformAction()
    {
        if (this.gameObject.GetComponent<Squad_AI>() != null)
        {
            Squad_AI squad_ai = this.gameObject.GetComponent<Squad_AI>();

            int num_squad = squad_ai.num_squad;


            if (squad_ai.squadron_controller.countUnitsSquadron(this.gameObject.transform.parent.gameObject.GetComponent<Faction_AI>().squadrons, num_squad) > 0)
            {
                if (this.gameObject.transform.parent.gameObject.GetComponent<Faction_AI>().squadrons[num_squad].squadron[0] != null)
                {
                    if (squad_ai.enemyFound_gameobject != null)  //Using "enemyFound_gameobject" insted of "target" from tool for more easy way
                    {
                        Faction_AI faction = this.gameObject.transform.parent.gameObject.GetComponent<Faction_AI>();

                        GameObject squad_captain;
                        squad_captain = faction.squadrons[num_squad].squadron_captain;

                        NavMeshAgent squad_captain_agent = squad_captain.GetComponent<NavMeshAgent>();

                        squad_captain.GetComponent<Unit>().selected_enemy = squad_ai.enemyFound_gameobject;
                        //SET UNIT RANGE
                        squad_captain.GetComponent<Unit>().setUnitRange(1, true);

                        if(squad_ai.enemyFound_gameobject != null)
                        {
                            float d_to_enemy = Vector3.Distance(squad_captain.transform.position, squad_ai.enemyFound_gameobject.transform.position);
                            float range = squad_captain.GetComponent<Unit>().unit_stats.range;

                            if (d_to_enemy > range)
                            {
                                squad_captain.GetComponent<Unit>().onMove = true;
                                squad_captain.GetComponent<NavMeshAgent>().SetDestination(squad_ai.enemyFound_gameobject.transform.position);
                            }
                            else
                            {
                                squad_captain.GetComponent<Unit>().onSquadronMove = false;
                                squad_captain.GetComponent<Unit>().onMove = false;
                                squad_captain.GetComponent<Unit>().onAttack = true;
                            }

                        }


                        //StartCoroutine(squad_ai.squadron_controller.game_controller.MoveCaptainToTarget(squad_captain, 5.0f, squad_ai.enemyFound_gameobject));

                        //Squadron Captain variables values on attack
                        /*
                        faction.squadrons[num_squad].squadron[0].GetComponent<NavMeshAgent>().SetDestination(squad_ai.enemyFound_gameobject.transform.position);
                        faction.squadrons[num_squad].squadron[0].GetComponent<Unit>().selected_enemy = squad_ai.enemyFound_gameobject;
                        faction.squadrons[num_squad].squadron[0].GetComponent<Unit>().setUnitRange(1, true); //The range is reseted (with the default Stats value in the "FormSquadron")
                        */

                        //ATTACK ACTION
                        actual_agent = faction.squadrons[num_squad].squadron[0].GetComponent<NavMeshAgent>();
                        if (actual_agent != null)
                        {
                            if (squad_ai.squadron_controller != null)
                            {
                                StartCoroutine(squad_ai.squadron_controller.formSquadron(faction.squadrons, num_squad, squad_ai.enemyFound_gameobject, true)); //TO-DO: CHECK THE NUMBER OF UNITS TO DO A FORMATION OR ANOTHER
                            }
                        }


                        //In case of the enemy is dead
                        if (squad_ai.enemyFound_gameobject.GetComponent<Unit>() != null)
                        {
                            Unit player_unit = squad_ai.enemyFound_gameobject.GetComponent<Unit>();

                            if (player_unit.health <= 0 || player_unit.onDie == true)
                            {
                                finished = true;

                                //Respect the order for prevent posible "Patrol_Action" bufs
                                agent.agent_states.ModifyStatusItem("killSquad", true);
                                agent.agent_states.ModifyStatusItem("findEnemy", false); //VERY IMPORTANT PRECONDITION FOR PATROLLING!

                                //Restart the variables when enemy is dead
                                squad_ai.enemyFound = false;
                                squad_ai.enemyFound_gameobject = null;
                            }

                        }
                    }
                }
            }

        }
    }

}

