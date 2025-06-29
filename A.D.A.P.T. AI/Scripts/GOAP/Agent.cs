﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


namespace ADAPT
{
    public abstract class Agent : MonoBehaviour
    {
        [Serializable]
        public class Goal
        {
            public Action.ResourceStruct goal_precondition; //Precondition resource to achieve the goal (basically the goal).
            public bool hasAction; //In case of the goal has any action to perform.
            public Action goal_Action; //Action to perfom.
            public int priority; //Same priority as the precondition of the goal.

            public Goal(Resource resource, bool hasAction)
            {
                this.goal_precondition = new Action.ResourceStruct(resource.resourceName, resource);
                this.hasAction = false;
            }

            public Goal(Resource resource, bool hasAction, Action action)
            {
                this.goal_precondition = new Action.ResourceStruct(resource.resourceName, resource);
                this.hasAction = hasAction;
                this.goal_Action = action;
            }

            public int getGoalPriority()
            {
                if (goal_precondition.selectedType == ResourceType.WorldElement)
                    return goal_precondition.w_resource.priority;
                else if (goal_precondition.selectedType == ResourceType.Position)
                    return goal_precondition.p_resource.priority;
                else if (goal_precondition.selectedType == ResourceType.InventoryObject)
                    return goal_precondition.i_resource.priority;
                else if (goal_precondition.selectedType == ResourceType.Status)
                    return goal_precondition.s_resource.priority;
                else
                    return 0;
            }

            public object getGoalValue()
            {
                if (goal_precondition.selectedType == ResourceType.WorldElement)
                    return goal_precondition.w_resource.value;
                else if (goal_precondition.selectedType == ResourceType.Position)
                    return goal_precondition.p_resource.value;
                else if (goal_precondition.selectedType == ResourceType.InventoryObject)
                    return goal_precondition.i_resource.value;
                else if (goal_precondition.selectedType == ResourceType.Status)
                    return goal_precondition.s_resource.value;
                else
                    return 0;
            }

            public void setResourceEnumType(Action.ResourceStruct precondition)
            {
                if (precondition.selectedType == ResourceType.WorldElement)
                {
                    precondition.w_resource.resourceEnumType = ResourceType.WorldElement.ToString();
                }
                else if (precondition.selectedType == ResourceType.Position)
                {
                    precondition.p_resource.resourceEnumType = ResourceType.Position.ToString();
                }
                else if (precondition.selectedType == ResourceType.InventoryObject)
                {
                    precondition.i_resource.resourceEnumType = ResourceType.InventoryObject.ToString();
                }
                else if (precondition.selectedType == ResourceType.Status)
                {
                    precondition.s_resource.resourceEnumType = ResourceType.Status.ToString();
                }
            }

            public void setResourceValue(Action.ResourceStruct precondition)
            {
                if (precondition.selectedType == ResourceType.WorldElement)
                {
                    precondition.w_resource.value = precondition.w_resource.resource_value;
                }
                else if (precondition.selectedType == ResourceType.Position)
                {
                    precondition.p_resource.value = precondition.p_resource.resource_value;
                }
                else if (precondition.selectedType == ResourceType.InventoryObject)
                {
                    precondition.i_resource.value = precondition.i_resource.resource_value;
                }
                else if (precondition.selectedType == ResourceType.Status)
                {
                    precondition.s_resource.value = precondition.s_resource.resource_value;
                }
            }


        }

        public string agentName; //Name of the Agent
        public bool showDebug = false; //Show the Debug Log or not

        public List<Goal> goals = new List<Goal>(); //List of goals who planner work with
        public Dictionary<string, Goal> goals_list = new Dictionary<string, Goal>(); //Set of multiple agent's goals: <Name, Priority>
        public List<Action> actions = new List<Action>(); //List of agent's actions to achieve a goal
        [ReadOnly]
        public Action currentAction; //Current running Action
        [ReadOnly]
        public string currentGoal; //Current running Goal
        public Planner planner; //Planer for get the actual action sequence
        [HideInInspector]
        public Queue<Action> actionsToRun; //Queue of Action
        [HideInInspector]
        public List<Action> receivedActions; //List of actionsToRun
        [HideInInspector]
        public NavMeshAgent agent;
        public bool addNavMesh; //Use it in case of want to add navmesh to the actual agent
        public bool onIdle; //If is TRUE then in case of Agent don't have a plan, stays on position. Otherwise (FALSE), goes around the world with random positions.
        public bool useRdmPatrolling; //Use it in case of want the agent go to patrolling around the world.

        //***OTHER PARAMS***
        public float rdmDistance = 100f; //Radius to get a random direction for navmesh agent. 
        [HideInInspector]
        public AgentStates agent_states;
        [HideInInspector]
        protected AgentStates global_states = GlobalStates.GetGlobalStatesInstance.GetGlobalStates(); //Any agent get a reference to the global states (Is more easy to call it in the specific Agent classes).
                                                                                                      //**PLANNER PARAM**
        int runnedActions = 0; //Calculate the amount of runned actions at time.
        bool plannerEnded = false;  //In case of achieve a goal and finish the planner.
        float restart = 2f; //Use this parameter to set the actual amount of wait to planner restart. (CAUTION!: Set the value: > 0).
                            //**ANIMATOR PARAM**
        public bool hasAnimations; //TRUE IF ANY ACTION WILL HAVE SOME ANIMATION TO RUN!.
        Animator agentAnimator;
        AnimatorOverrideController animatorOverrided; //Need it for change animation on states.
        public AnimationClip onSiteIdle; //Animation to play on idle (on site) action
        public AnimationClip onWalkingIdle; //Animation to play on idle (walking) action
        public float transitionTime = 0.2f; //Transition time between states in to the animator.

        public void Start()
        {
            SetActions();

            foreach (Goal g in goals)
            {
                //Add Goal_Actions (as Component) in case of hame some.
                if (g.hasAction == true)
                {
                    Type action = g.goal_Action.GetType();
                    if ((gameObject.GetComponent(action.ToString()) as Action) == null) //In case of Goal-Action is actually attached to Agent
                    {
                        gameObject.AddComponent(action);
                        g.goal_Action = gameObject.GetComponent(action.ToString()) as Action; //In case of inspector bug of action_goal
                    }
                }


                //Truncate data to dictionary.
                g.setResourceEnumType(g.goal_precondition);
                g.setResourceValue(g.goal_precondition);
                AddGoalToList(g);//goals_list.Add(g.goal_precondition.key, g);
            }

            //Add Agent States
            if (agent_states == null)
            {
                agent_states = gameObject.AddComponent<AgentStates>();
            }
            ///

            if(addNavMesh == true)
            {
                agent = gameObject.AddComponent<NavMeshAgent>();
            }
            

            if (hasAnimations == true) //In case of has idle animations or any in the actions.
            {
                agentAnimator = gameObject.GetComponent<Animator>();
                animatorOverrided = new AnimatorOverrideController(agentAnimator.runtimeAnimatorController);
                agentAnimator.runtimeAnimatorController = animatorOverrided;
            }
        }

        public void LateUpdate()
        {
            //In case of some goal has beend added previously via code.
            foreach (Goal g in goals)
            {
                //Truncate data to dictionary.
                if (!goals_list.ContainsKey(g.goal_precondition.key))
                {
                    g.setResourceEnumType(g.goal_precondition);
                    g.setResourceValue(g.goal_precondition);
                    goals_list.Add(g.goal_precondition.key, g);
                    g.getGoalPriority();
                }

            }

            PerformPlanner();
        }

        public void PlayAnimation(AnimationClip anim) //Play the action animation
        {
            if (agentAnimator != null)
            {
                if (anim.Equals(onSiteIdle) || anim.Equals(onWalkingIdle))
                    animatorOverrided["Idle"] = anim; //DON'T CHANGE Idle on Animator!.
                else
                    animatorOverrided["T-Pose"] = anim; //DON'T CHANGE T-Pose on Animator!.
            }

            //agentAnimator.CrossFade("runnableaction", 0.1f, 0); //Used for smooth animation transitions.
        }

        public void RestartAnimator() //Restart animator when finish and achieve some goal.
        {
            agentAnimator.Rebind();
            agentAnimator.Update(0f);
        }

        public void AddGoalToList(Goal g)
        {
            if (!goals_list.ContainsKey(g.goal_precondition.key))
                goals_list.Add(g.goal_precondition.key, g);
        }


        public void PerformPlanner()
        {
            Dictionary<string, object> worldStates; //Initialice and ADD all states to the current dictionary.

            //**LIMITED ACTIONS TO RUN BEFORE RESTART THE PLANNER** (Can change the value if you want).
            int runneableActions = 3; //Change this value to set the number of actions to run before restart the planner.
            bool planFinished = false;  //In case of finish all actions to run.

            //RUN PLANNER.
            if (planner == null || actionsToRun == null)
            {
                worldStates = new Dictionary<string, object>();
                receivedActions = new List<Action>();

                GetAllStates(worldStates, agent_states);
                GetAllStates(worldStates, global_states);

                if (this.agent_states != null)
                    this.agent_states.UpdateStatesLists();


                runnedActions = 0; //Used for count the actual number of perfomed actions.
                plannerEnded = false;


                //In case of previous planner don't restart the values of the components attached to agent.
                if (plannerEnded == true)
                {
                    foreach (Action a in actionsToRun)
                    {
                        a.finished = false;
                        plannerEnded = false; //Restart planner.
                    }

                    //RestartAgentStates(); //Restart the agent states after finish the actual plan
                }

                if (goals_list.Count == 0)
                {
                    //Debug.Log("RESTART GOALS!");
                    foreach (Goal g in goals)
                    {
                        //Add Goal_Actions (as Component) in case of hame some.
                        if (g.hasAction == true)
                        {
                            Type action = g.goal_Action.GetType();
                            if ((gameObject.GetComponent(action.ToString()) as Action) == null) //In case of Goal-Action is actually attached to Agent
                            {
                                gameObject.AddComponent(action);
                                g.goal_Action = gameObject.GetComponent(action.ToString()) as Action; //In case of inspector bug of action_goal
                            }
                        }

                        //Truncate data to dictionary.
                        goals_list.Add(g.goal_precondition.key, g);
                    }

                    foreach (Action a in actions)
                    {
                        if (a.finished)
                            a.finished = false;
                    }

                   
                }

                goals_list = goals_list.OrderBy(x => x.Value.getGoalPriority()).ToDictionary(x => x.Key, x => x.Value); //Order Goals by priority
                planner = new Planner();
                planner.showDebug = showDebug;
                actionsToRun = planner.Plan(actions, worldStates, goals_list);

                if (actionsToRun == null)
                {
                    Component[] agent_Actions = null;
                    
                    agent_Actions = GetComponents(typeof(Action));

                    foreach (Action a in agent_Actions) //Restart all finish condition of the actions.
                    {
                        a.finished = false;
                    }
                }


                if(agent != null)
                {
                    if (agent.pathPending && actionsToRun != null) //Reset all navigation paths for prevent any destination bug.
                    {
                        agent.isStopped = true;
                        agent.ResetPath();
                    }
                    else if (actionsToRun == null)
                    {
                        IdleState();
                    }
                    else if (actionsToRun != null)
                    {
                        foreach (Action a in actionsToRun) //Store Actions into permanent list.
                        {
                            receivedActions.Add(a);
                        }
                    }
                }

            }

            if (actionsToRun != null) //Set the actual Action running.
            {
                if (currentAction != null)
                {
                    if (actionsToRun.Count <= 1 && currentAction.finished == true) //If last action
                    {
                        if (actionsToRun.Count != 0)
                            currentAction = actionsToRun.Peek(); //Get last action has current one.

                        if (currentAction.finished == true) //In case of last action is finished
                        {
                            //If last action is an action_goal
                            if (goals_list[currentGoal].hasAction == true)
                            {
                                if (currentAction.name == goals_list[currentGoal].goal_Action.name)
                                {
                                    if (actionsToRun.Count != 0)
                                        actionsToRun.Dequeue();
                                    currentAction = gameObject.GetComponent(goals_list[currentGoal].goal_Action.GetType().ToString()) as Action;
                                    planFinished = true;
                                }
                                else //Last action is not an action_goal but have one, add it!.
                                {
                                    Type goal_action = goals_list[currentGoal].goal_Action.GetType(); //Get the actual goal-action from Agent to perform
                                    Action finish_action = gameObject.GetComponent(goal_action.ToString()) as Action;
                                    if (actionsToRun.Count != 0)
                                        actionsToRun.Dequeue();
                                    actionsToRun.Enqueue(finish_action); //Add goal action to the queue.
                                }
                            }
                            else //Other case, is normal action and planner is finished.
                            {
                                if (actionsToRun.Count != 0)
                                    actionsToRun.Dequeue();
                                //FINISHED PLAN!!
                                planFinished = true;
                            }
                        }
                    }

                    if (currentAction.finished == true) //In case of last action is finished, perform GOAL ACTION.
                    {
                        if (currentGoal != null || !currentGoal.Equals("")) //In case of not empty goal
                        {
                            foreach (KeyValuePair<string, Goal> g in goals_list)
                            {
                                if (g.Key == currentGoal)
                                {
                                    if (g.Value.hasAction == true) //In case of have some Action to perform
                                    {
                                        Type goal_action = g.Value.goal_Action.GetType(); //Get the actual goal-action from Agent to perform
                                        Action finish_action = gameObject.GetComponent(goal_action.ToString()) as Action;

                                        actionsToRun.Dequeue();
                                        actionsToRun.Enqueue(finish_action); //Add goal action to the queue.
                                    }
                                }
                            }
                        }
                        runnedActions++;
                    }

                }

                if (actionsToRun.Count >= 1)
                {
                    //Check if action is finished or not!.
                    if (currentAction == null)
                    {
                        currentAction = actionsToRun.Dequeue();
                    }
                    else
                    {
                        if (currentAction == currentAction.finished)
                        {
                            currentAction.running = false;
                            currentAction = actionsToRun.Dequeue();
                        }
                    }
                }

                if (currentAction != null)
                {
                    if (currentAction.finished != true) //Run the actual Action.
                    {
                        currentAction.running = true;
                        if (currentAction.actionAnimation != null && agentAnimator != null) //Set running bool into the states.
                            agentAnimator.SetBool("running", currentAction.running);
                    }
                    else //In case of the actual action is finished but the effects are not achieved...Restart planner.
                    {

                        currentAction.running = false; //Set running to false only if action is finished.

                        if (currentAction.actionAnimation != null && agentAnimator != null) //Set running bool into the states.
                            agentAnimator.SetBool("running", currentAction.running);

                        if (runnedActions > runneableActions) //Check if actually the limit of actions is achieve
                        {
                            //Restart the planner.
                            if(showDebug == true)
                                Debug.Log("<color=yellow>RESTART...</color>");
                            StartCoroutine(RestartPlanner_Wait());
                        }
                        else
                        {
                            foreach (KeyValuePair<string, Resource> eff in currentAction.effects)
                            {
                                if (RestartPlanner(eff, false) == true)
                                {
                                    planFinished = false;
                                }
                                else //In case of achieve the states-effects, check if should restar.
                                {
                                    planFinished = true;

                                    if (actionsToRun.Count != 0)
                                    {
                                        //Restart the planner.
                                        if (showDebug == true)
                                            Debug.Log("<color=yellow>RESTART...</color>");
                                        StartCoroutine(RestartPlanner_Wait());
                                    }
                                }
                            }
                        }
                    }
                }

                /******************************************************/

                if (planner != null)
                {
                    //Current Goal to achieve
                    currentGoal = planner.goal_to_achieve;
                }

                /******************************************************/

                if (actionsToRun.Count == 0 && actionsToRun != null && planFinished == true) //PLANNER ACHIEVED THE GOAL
                {
                    bool goalAchieved = false;

                    if (goals_list[currentGoal].goal_precondition.selectedType == ResourceType.WorldElement)
                    {
                        KeyValuePair<string, Resource> goal_resource = new KeyValuePair<string, Resource>(goals_list[currentGoal].goal_precondition.key, goals_list[currentGoal].goal_precondition.w_resource);
                        goalAchieved = RestartPlanner(goal_resource, true);
                    }
                    else if (goals_list[currentGoal].goal_precondition.selectedType == ResourceType.Position)
                    {
                        KeyValuePair<string, Resource> goal_resource = new KeyValuePair<string, Resource>(goals_list[currentGoal].goal_precondition.key, goals_list[currentGoal].goal_precondition.p_resource);
                        goalAchieved = RestartPlanner(goal_resource, true);
                    }
                    else if (goals_list[currentGoal].goal_precondition.selectedType == ResourceType.InventoryObject)
                    {
                        KeyValuePair<string, Resource> goal_resource = new KeyValuePair<string, Resource>(goals_list[currentGoal].goal_precondition.key, goals_list[currentGoal].goal_precondition.i_resource);
                        goalAchieved = RestartPlanner(goal_resource, true);
                    }
                    else if (goals_list[currentGoal].goal_precondition.selectedType == ResourceType.Status)
                    {
                        KeyValuePair<string, Resource> goal_resource = new KeyValuePair<string, Resource>(goals_list[currentGoal].goal_precondition.key, goals_list[currentGoal].goal_precondition.s_resource);
                        goalAchieved = RestartPlanner(goal_resource, true);
                    }

                    if (showDebug == true)
                    {
                        if (goalAchieved != true)
                            Debug.Log("<color=red>GOAL ACHIEVED!!!!!!!!</color> -" + currentGoal);
                        else
                            Debug.Log("<color=orange>GOAL NOT ACHIEVED!!!!!!!!</color> -" + currentGoal);
                    }



                    plannerEnded = true;
                    if (plannerEnded == true) //Restart planner when achieve goal
                    {
                        RestartAgentStates(); //Restart the agent states

                        planner = null;
                        currentAction = null;

                        if (goals_list.Count > 0)
                            goals_list.Remove(currentGoal);
                    }
                }
                /******************************************************/
            }
            else
            {
                actionsToRun = null;
                currentAction = null;
            }

            if (currentAction != null) //This line of code is indepent of other ones for possible bugs.
            {
                if (currentAction.running && currentAction.finished != true) //In case of some Action is running.
                {
                    currentAction.PerformAction();
                    if (currentAction.actionAnimation != null && agentAnimator != null) //Play animation if have one.
                    {
                        PlayAnimation(currentAction.actionAnimation);
                    }
                }
            }
        }

        IEnumerator RestartPlanner_Wait() //Use this function for programmatically restart the planner when achieve an amount of runned actions.
        {
            yield return new WaitForSeconds(restart);
            planner = null;
        }

        public bool RestartPlanner(KeyValuePair<string, Resource> e, bool receivedGoal) //In case of actual state dont achieve any of the effects, restart planner until some of state's value is equals some effect's value.
        {
            //*receivedGoal -> //In case of goal has been accomplished, then indicate into the console. Other ways, do the same.

            //World and Position states are setter finished by the user.!!!
            if (global_states.inventory.ContainsKey(e.Key) || global_states.status.ContainsKey(e.Key))
            {
                if (global_states.inventory.ContainsKey(e.Key)) //Lines are writted like this for prevent possible dictionary bugs.
                {
                    if (global_states.inventory[e.Key] < (float)e.Value.value)
                    {
                        if (receivedGoal == false)
                        {
                            planner = null;
                            currentAction.finished = false;
                        }

                        return true;
                    }
                }
                else if (global_states.status.ContainsKey(e.Key))
                {
                    if (global_states.status[e.Key] != (bool)e.Value.value)
                    {
                        if (receivedGoal == false)
                        {
                            planner = null;
                            currentAction.finished = false;
                        }

                        return true;
                    }
                }
            }
            else if (agent_states.inventory.ContainsKey(e.Key) || agent_states.status.ContainsKey(e.Key)) //Lines are writted like this for prevent possible dictionary bugs.
            {
                if (agent_states.inventory.ContainsKey(e.Key)) //Lines are writted like this for prevent possible dictionary bugs.
                {
                    if (agent_states.inventory[e.Key] < (float)e.Value.value)
                    {
                        if (receivedGoal == false)
                        {
                            planner = null;
                            currentAction.finished = false;
                        }

                        return true;
                    }
                }
                else if (agent_states.status.ContainsKey(e.Key))
                {
                    if (agent_states.status[e.Key] != (bool)e.Value.value)
                    {
                        if (receivedGoal == false)
                        {
                            planner = null;
                            currentAction.finished = false;
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public void RestartAgentStates() //Use this function to restart all the agent states after finish the plan. 
        {
            if (showDebug == true)
                Debug.Log("RESTART STATES!");

            //Restart Inventory_States
            List<string> inventory_keys = new List<string>(agent_states.inventory.Keys);
            foreach (var key in inventory_keys)
            {
                agent_states.inventory[key] = -1.0f;
            }

            //Restart Status_States
            List<string> status_keys = new List<string>(agent_states.status.Keys);
            foreach (var key in status_keys)
            {
                agent_states.status[key] = false;
            }
        }

        public void GetAllStates(Dictionary<string, object> worldStates, AgentStates states) //Use this function for get all states (individual and global ones).
        {
            if(states != null)
            {
                if (states.worldElements != null)
                {
                    if (states.worldElements.Count > 0)
                    {
                        foreach (string worldItem in states.worldElements)
                        {
                            if ((worldItem != null) || (worldItem != "") || (worldItem.Length != 0))
                            {
                                worldStates.Add(worldItem, null);
                            }
                        }
                    }
                }

                if (states.positions != null)
                {
                    if (states.positions.Count > 0)
                    {
                        foreach (string positionItem in states.positions)
                        {
                            if ((positionItem != null) || (positionItem != "") || (positionItem.Length != 0))
                            {
                                worldStates.Add(positionItem, null);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, float> inventoryItem in states.inventory)
                {
                    worldStates.Add(inventoryItem.Key, inventoryItem.Value);
                }
                foreach (KeyValuePair<string, bool> statusItem in states.status)
                {
                    worldStates.Add(statusItem.Key, statusItem.Value);
                }
            }

        }

        public Action GetCurrentAction()
        {
            return currentAction;
        }

        public string GetCurrentGoal()
        {
            return currentGoal;
        }

        public void SetCurrentAction(Action action)
        {
            currentAction = action;
        }

        public void SetCurrentGoal(string key)
        {
            currentGoal = key;
        }

        public void SetActions()
        {
            Action[] agent_actions = this.GetComponents<Action>();
            foreach (Action action in agent_actions)
            {
                actions.Add(action);
            }
            currentAction = null;
            currentGoal = null;
        }

        public void IdleState()
        {
            if(agent != null)
            {
                          //Call this function when (Planner == null)
            if (onIdle == true) //Then stays on position.
            {
                //Play Idle animation...
                agent.isStopped = true;
                agent.ResetPath();
                PlayAnimation(onSiteIdle); //Play on site idle animation.
            }
            else //Go around the map.
            {
                if(useRdmPatrolling == true)
                {
                    agent.SetDestination(RandomNavigation());
                    PlayAnimation(onWalkingIdle); //Play waliking idle animation.
                }
            }
            }
  
        }

        public Vector3 RandomNavigation() //Allows agents to move around the map randomly.
        {
            Vector3 randomDirection, destination;
            NavMeshHit hit;

            destination = Vector3.zero;

            randomDirection = UnityEngine.Random.insideUnitSphere * rdmDistance;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out hit, rdmDistance, 1))
            {
                destination = hit.position;
            }

            return destination;
        }



    }
}
