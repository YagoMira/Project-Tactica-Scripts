using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Project_Tactica; //BECAUSE OF MUTIPLE NAMESPACES USE
using ADAPT; //BECAUSE OF MUTIPLE NAMESPACES USE
using System.Linq;

//Allows Agent move to determinate position.
//***Target = Assign target with Unity Inspector.***
public class Patrol_Action : Action
{
    string a_name = "Patrol";
    Agent agent;
    NavMeshAgent actual_agent;

    public bool rdmPatrol = false;


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

        ResourceStruct PatrolSquad_preconditions_1 = new ResourceStruct("onPosition", new StatusResource("onPosition", false, 5));
        ResourceStruct PatrolSquad_preconditions_2 = new ResourceStruct("findEnemy", new StatusResource("findEnemy", false, 5));
        ResourceStruct PatrolSquad_effect_1 = new ResourceStruct("onPosition", new StatusResource("onPosition", true, 5));

        actionName = a_name;

        if(addedPreviously == false)
        {
            //Add preconditions and effects and appears in inspector
            AddPreconditionToList(PatrolSquad_preconditions_1);
            AddPreconditionToList(PatrolSquad_preconditions_2);
            AddEffectToList(PatrolSquad_effect_1);
        }
      

        //GetComponents
        agent = gameObject.GetComponent<Agent>();

        //WARNING MESSAGE!.
        //Debug.Log("<color=blue> Action: </color>" + actionName + "<color=blue> has preconditions/effects added by code,</color> <color=red> DON'T ADD MORE VIA INSPECTOR!.</color>");
    }

    private void Update() { }

    //ONLY FOR TESTS ###############################################
    public Vector3 RandomNavigation(float distance) //Allows agents to move around the map randomly.
    {
        Vector3 randomDirection, destination;
        NavMeshHit hit;

        destination = Vector3.zero;

        randomDirection = UnityEngine.Random.insideUnitSphere * distance;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out hit, distance * 1.30f, 1))
        {
            destination = hit.position;
        }

        return destination;
    }
    //ONLY FOR TESTS ###############################################

    public override void PerformAction()
    {
        agent.agent_states.ModifyStatusItem("onPosition", false); //Initialices the state in case of enter in a loop.

        if (this.gameObject.GetComponent<Squad_AI>() != null)
        {
            Squad_AI squad_ai = this.gameObject.GetComponent<Squad_AI>();

            if (squad_ai.enemyFound_gameobject == null)
            {
                int num_squad = squad_ai.num_squad;

                //GET PATROLLING LOCATION
                if (target == null)
                {
                    target = Instantiate(new GameObject());
                    target.name = "PATROL_" + num_squad;

                    if (rdmPatrol == true)
                    {
                        target.transform.position = RandomNavigation(500.0f);
                    }
                    else
                    {
                        target.transform.position = squad_ai.faction_ai.points[num_squad][squad_ai.faction_ai.actual_patrol_point[num_squad]].position;
                    }

                }

                if(squad_ai.squadron_controller.countUnitsSquadron(this.gameObject.transform.parent.gameObject.GetComponent<Faction_AI>().squadrons, num_squad) > 0)
                {
                    if (this.gameObject.transform.parent.gameObject.GetComponent<Faction_AI>().squadrons[num_squad].squadron[0] != null)
                    {
                        Faction_AI faction = this.gameObject.transform.parent.gameObject.GetComponent<Faction_AI>();
                        if ((faction.squadrons[num_squad].squadron[0].GetComponent<Unit>().onDamageReceived == false) && (faction.squadrons[num_squad].squadron[0].GetComponent<Unit>().onDie == false))
                        {
                            faction.squadrons[num_squad].squadron[0].GetComponent<Unit>().onMove = true;
                            faction.squadrons[num_squad].squadron[0].GetComponent<Unit>().animator.SetBool("onMove", true); //Move Animation
                        }
                        else
                        {
                            actual_agent.isStopped = true;
                        }


                        actual_agent = faction.squadrons[num_squad].squadron[0].GetComponent<NavMeshAgent>();


                        if (actual_agent != null)
                        {
                            if (target != null && hasTarget) //TARGET EXISTS
                            {
                                if ((faction.squadrons[num_squad].squadron[0].GetComponent<Unit>().onDamageReceived == false) && (faction.squadrons[num_squad].squadron[0].GetComponent<Unit>().onDie == false))
                                {
                                    actual_agent.stoppingDistance = stopDistance;
                                    actual_agent.SetDestination(target.transform.position);
                                }
                                else
                                {
                                    actual_agent.isStopped = true;
                                }

                                //SQUADRON MOVEMENT
                                if (squad_ai.squadron_controller != null)
                                {
                                    StartCoroutine(squad_ai.squadron_controller.formSquadron(faction.squadrons, num_squad, null, true)); //TO-DO: CHECK THE NUMBER OF UNITS TO DO A FORMATION OR ANOTHER
                                }
                            }

                            //Check if Agent NavMesh reach the actual target position
                            if (!actual_agent.pathPending && target != null)
                            {
                                if (actual_agent.remainingDistance <= actual_agent.stoppingDistance)
                                {
                                    if (!actual_agent.hasPath || actual_agent.velocity.sqrMagnitude == 0f)
                                    {
                                        finished = true;
                                        agent.agent_states.ModifyStatusItem("onPosition", true);

                                        //RESTART PATROLLING LOCATION
                                        if (target != null)
                                        {
                                            if (rdmPatrol == true)
                                            {
                                                target.transform.position = RandomNavigation(500.0f);
                                            }
                                            else
                                            {
                                                //Set the new Patrol Point of the Points array
                                                squad_ai.faction_ai.actual_patrol_point[num_squad] = (squad_ai.faction_ai.actual_patrol_point[num_squad] + 1) % squad_ai.faction_ai.points[num_squad].Length;
                                                target.transform.position = squad_ai.faction_ai.points[num_squad][squad_ai.faction_ai.actual_patrol_point[num_squad]].position;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    finished = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    finished = true;
                }
                
            }
            else //Abort the current action to interact with Attack
            {
                finished = true;
            }


        }

    }
}

