using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_Tactica; //BECAUSE OF MUTIPLE NAMESPACES USE
using ADAPT; //BECAUSE OF MUTIPLE NAMESPACES USE
using UnityEngine.AI;

public class Squad_AI : Agent
{
    [HideInInspector]
    public Faction_AI faction_ai;
    [Space]
    [Space]
    [Header("----------------")]
    public int num_squad = -1;
    [HideInInspector]
    public SquadronController squadron_controller;
    public bool enemyFound = false;
    public GameObject enemyFound_gameobject = null;

    [Header("----------------")]
    public string SPACE_ON_INSPECTOR; //ONLY FOR TESTS ###############################################

    public bool previouslyCreated = false;

    new void Start()
    {

        //Get the Faction component
        if(this.transform.parent != null)
            faction_ai = this.transform.parent.GetComponent<Faction_AI>();

        agentName = (("SQUADRON_" + num_squad).ToString());

        //Previously created squadrons
        if(previouslyCreated == false)
        {
            //******A.D.A.P.T. AI******//
            AddGoals();
            /************/
            base.Start(); //DON'T DELETE THIS LINE!!!
                          /************/
            ManageStates();
            //******A.D.A.P.T. AI******//
        }

    }


    private void Update()
    {
        FindEnemy();

        //DestroySquad();
        CheckFinish();
    }

    //In case of squad been defeat it
    public void DestroySquad()
    {
        if(squadron_controller.countUnitsSquadron(faction_ai.squadrons, num_squad) <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    //In case of squad been defeat it
    public void CheckFinish()
    {
        if (squadron_controller.countUnitsSquadron(faction_ai.squadrons, num_squad) <= 0)
        {
            planner = null;
        }

        if(enemyFound_gameobject == null)
        {
            enemyFound = false;
            agent_states.ModifyStatusItem("findEnemy", false);

            if(currentAction != null)
            {
                if (currentAction.name == "Attack")
                    currentAction.finished = true;
            }

        }
    }

    //******A.D.A.P.T. AI******//
    public void AddGoals()
    {
        StatusResource killPlayerSquad = (new StatusResource("killSquad", true, 100));
        goals.Add(new Goal(killPlayerSquad, false));

        //PATROLLING GOAP
        StatusResource onPosition = (new StatusResource("onPosition", true, 50));
        goals.Add(new Goal(onPosition, false));
        //PATROLLING GOAP
    }

    public void ManageStates()
    {
        agent_states.AddStatusItem("findEnemy", false);
        agent_states.AddStatusItem("killSquad", false);

        //PATROLLING GOAP
        agent_states.AddStatusItem("onPosition", false);
        //PATROLLING GOAP
    }
    //******A.D.A.P.T. AI******//

    //Check if an enemy is near the squad
    public void FindEnemy()
    {
        if (enemyFound == true) //If the AI found and Player/Enemy
        {
            agent_states.ModifyStatusItem("findEnemy", true);
        }
        else
        {
            agent_states.ModifyStatusItem("findEnemy", false);

            //In case of not find and enemy near, but one of the units of the squadron received damage from enemy. Set this new enemy as the found one.
            if(enemyFound_gameobject == null)
            {
                for (int i = 0; i < squadron_controller.countUnitsSquadron(faction_ai.squadrons, num_squad); i++)
                {
                    if (faction_ai.squadrons[num_squad].squadron[i] != null)
                    {
                        faction_ai.squadrons[num_squad].squadron[i].TryGetComponent<Unit>(out Unit unit);

                        if (unit.selected_enemy != null)
                        {
                            if ((unit.selected_enemy.GetComponent<Unit>().health > 0) && (unit.selected_enemy.GetComponent<Unit>().onDie != true))
                            {
                                enemyFound = true;
                                enemyFound_gameobject = faction_ai.squadrons[num_squad].squadron[i].GetComponent<Unit>().selected_enemy;
                                agent_states.ModifyStatusItem("findEnemy", true);
                                break;
                            }

                        }
                    }
                    
                }
            }

        }
    }
}
