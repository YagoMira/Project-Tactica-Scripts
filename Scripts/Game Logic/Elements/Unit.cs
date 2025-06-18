using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System;

namespace Project_Tactica
{
    public class Unit : Element
    {
        public enum unit_type { Soldier, Tank, Almah };
        [Space]
        public unit_type sub_type;
        [Space]
        public Sprite sprite; //Unit sprite (Display it if this unit is a captain of a squadron)
        [Space]
        public bool onMove;
        public bool onAttack;
        public bool onRest;
        public bool onDamageReceived;
        public bool onDie;
        [Space]
        public float max_health;
        public Stats unit_stats;

        public float rotation_speed = 5.0f;

        [Space]
        //Squadron System
        public GameObject squadron_position = null;
        public bool onSquadronMove = true;

        //Combat System
        public GameObject selected_enemy;
        Squadron aux_enemy_squad = null; //STATIC ENEMY SQUAD (NOT ALTERABLE BY KILLING AN ENEMY PREVIOUSLY)

        //OTHER SYSTEMS
        [HideInInspector]
        public GameObject game_logic_object;
        [HideInInspector]
        public GameController game_controller;
        [HideInInspector]
        public SquadronController squadron_controller;

        //UI of the Unit
        protected GameObject unit_ui_object;

        //Variables of Unit
        float time; //Time to reach the final destination of the path
        int seconds = 0, minutes = 0; //Auxiliar variables for time system calculations
        //Time to reach the destination (final of the path)
        string time_text_value;
        TMP_Text time_text; //Previously: Text (2D)

        //Unit Health UI
        protected Image health_filler;
        protected GameObject health_damage;
        protected GameObject health_heal;
        //Unit Name UI
        protected TMP_Text name_text; //Unit Name
                                      //TO-DO: MAYBE ADD GENDER UI REPRESENTATION?

        //Unit Effect UI
        [HideInInspector]
        public GameObject tactica_effect_ui;

        [HideInInspector]
        public Unit_AudioManager audio_manager; //Unit Audio Manager
        [Space]
        public GameObject received_damage_particle;

        [HideInInspector]
        public Animator animator; //Unit Animator
        [HideInInspector]
        public NavMeshAgent agent; //Unit Agent NavMesh

        public GameObject line_renderer;

        public void Awake()
        {
            game_logic_object = GameObject.Find("Game Logic");
            game_controller = game_logic_object.GetComponent<GameController>();
            squadron_controller = GameObject.Find("Game Logic").gameObject.GetComponent<SquadronController>(); //TO-DO: OPTIMIZE???
            unit_ui_object = gameObject.transform.Find("Unit_UI").gameObject;
            unit_stats = this.GetComponent<Stats>();
            //FOR INDIVIDUAL: //time_text = unit_ui_object.transform.Find("Unit_UI_Tracker").transform.Find("Time").GetComponent<TMP_Text>(); //.GetComponent<Text>();
            health_filler = unit_ui_object.transform.Find("Unit_UI_Tracker").transform.Find("Health").transform.Find("Fill").GetComponent<Image>();
            health_damage = unit_ui_object.transform.Find("Unit_UI_Tracker").transform.Find("Damage").gameObject;
            health_heal = unit_ui_object.transform.Find("Unit_UI_Tracker").transform.Find("Heal").gameObject;
            name_text = unit_ui_object.transform.Find("Unit_UI_Tracker").transform.Find("Name").GetComponent<TMP_Text>();
            tactica_effect_ui = unit_ui_object.transform.Find("Unit_UI_Tracker").transform.Find("TacticaEffect").gameObject;

            audio_manager = this.GetComponent<Unit_AudioManager>();
            animator = this.GetComponent<Animator>();
            agent = this.gameObject.GetComponent<NavMeshAgent>();

            applyStats(); //Apply the initial stats modifications
        }

        protected virtual void Update()
        {
            if (this.onMove == true)
            {
                if (squadron_position == null)
                {
                    onMoveAction();
                }
                else
                {
                    onMoveSquadronAction(onSquadronMove);
                }

                if(this.gameObject.CompareTag("Player")) //ONLY LINES OF THE PLAYER UNITS WILL BE VISIBLE
                {
                    //Line Rendering
                    RenderUILine();
                    UpdateUILine();
                }
                
            }
            else
            {
                if (this.gameObject.transform.Find("LineRenderer") != null)
                {
                    Destroy(this.gameObject.transform.Find("LineRenderer").gameObject);
                }

                onAttackAction();
            }

            if (onDie == true)
            {
                NavMeshAgent actual_agent = this.gameObject.GetComponent<NavMeshAgent>();

                actual_agent.isStopped = true;
                actual_agent.ResetPath();
            }


            CheckSelectedEnemy();

            UnitDebug();
        }
   

        public Unit(string name, string description, elem_type type, unit_type sub_type, float health) : base(name, description, type, health)
        {
            type = elem_type.Unit;
            this.sub_type = sub_type;
        }

        public void UnitDebug()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward) * 50;
            Debug.DrawRay(transform.position, forward, Color.green);
        }

        public void CheckSelectedEnemy()
        {
            if(onAttack == true)
            {
                try
                {

                    if (selected_enemy.GetComponent<Unit>() == null)
                    {
                        onAttack = false;
                    }

                }
                catch (MissingReferenceException e)
                {
                    onAttack = false;
                }
            }
        }

        public void RenderUILine()
        {
            if (this.gameObject.transform.Find("LineRenderer") == null)
            {
                GameObject line = Instantiate(this.line_renderer, this.gameObject.transform);
                line.name = "LineRenderer";

                
                LineRendering l_render = line.GetComponent<LineRendering>();
                l_render.SetRenderer(l_render.GetComponent<LineRenderer>());
                l_render.SetLine(this.gameObject.transform.position, this.gameObject.GetComponent<NavMeshAgent>().destination); //Set the line with its positions
            }
        }

        public void UpdateUILine()
        {
            if (this.gameObject.transform.Find("LineRenderer") != null)
            {
                GameObject lineRenderer = this.gameObject.transform.Find("LineRenderer").gameObject;
                LineRendering l_render = lineRenderer.GetComponent<LineRendering>();

                l_render.SetLine(this.gameObject.transform.position, this.gameObject.GetComponent<NavMeshAgent>().destination); //Set the line with its positions
            }
        }

        public float calculatePathLength(NavMeshPath path)
        {
            if (path.corners.Length < 2)
                return 0;

            Vector3 previousCorner = path.corners[0];
            float lengthSoFar = 0.0F;
            int i = 1;
            while (i < path.corners.Length)
            {
                Vector3 currentCorner = path.corners[i];
                lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
                previousCorner = currentCorner;
                i++;
            }
            return lengthSoFar;
        }


        public float calculateDistance(Vector3 unit_position, Vector3 destination_position, float speed)
        {
            float distance_destination = 0.0f;
            float time_destination = 0.0f;

            distance_destination = Vector3.Distance(unit_position, destination_position);
            time_destination = distance_destination / speed;

            return time_destination;
        }

        public void applyStats() //Apply the initial stats to the unit
        {
            max_health = health + (health * (unit_stats.extra_health / 100.0f));
            health = max_health;
            agent.speed = unit_stats.speed;
            agent.stoppingDistance = unit_stats.range;
        }

        public virtual void receiveDamage(float damage, Vector3 hit_point) { }

        public virtual void ApplyHeal(float health)
        {
            this.health_heal.SetActive(true);
            this.health_heal.GetComponent<TMP_Text>().text = ("+" + health.ToString());

            if ((this.health + health) >= max_health)
            {
                this.health = max_health;
            }
            else
            {
                this.health += health;
            }
            

            float final_health_amount = Mathf.Lerp(0.0f, 100.0f, this.health / max_health);
            health_filler.fillAmount = final_health_amount / 100.0f; //UPDATE HEALTH UI

  
            if(this.health == max_health)
            {
                health_filler.fillAmount = 1.0f;
            }

            StartCoroutine(HideHeal(1.0f));
        }

        IEnumerator HideHeal(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            this.health_heal.SetActive(false);
        }

        public void responseReceiveDamage(Unit enemy_shooter)
        {

            if (onDamageReceived == true) //On receive damage, set the enemy shooter as the current enemy
            {
                if (this.selected_enemy == null)
                {
                    selected_enemy = enemy_shooter.gameObject;
                }
            }

            if(enemy_shooter != null)
                SetEnemyToSquad(enemy_shooter.gameObject); //If any companions of this same Units squadrons has not any enemy selected, selected this
        }


        //If any companions of this same Units squadrons has not any enemy selected, selected this
        public void SetEnemyToSquad(GameObject enemy_shooter)
        {   
            Squadron unit_squadron = squadron_controller.getAnySquadron(this.gameObject);
            int squad_units = squadron_controller.countUnitsOnIndividualSquadron(unit_squadron);

            for (int i = 0; i < squad_units; i++)
            {
                if(unit_squadron.squadron[i] != null)
                {
                    if (unit_squadron.squadron[i].GetComponent<Unit>().selected_enemy == null)
                        unit_squadron.squadron[i].GetComponent<Unit>().selected_enemy = enemy_shooter;
                }
                    
            }

        }

        public virtual void finalHealthCalculation(float damage) { }

        //Calculate the time to reach the enemy/desired destination
        public void timeCalculation()
        {
            time = Vector3.Distance(agent.transform.position, agent.pathEndPosition) / agent.speed; //In SECONDS!

            minutes = Mathf.FloorToInt(time / 60F);
            seconds = Mathf.FloorToInt(time - minutes * 60);


            string formatted_time = string.Format("{0:0}:{1:00}", minutes, seconds);
            time_text_value = formatted_time;
            time_text.text = time_text_value;
        }

        //----------------------UNIT ACTIONS----------------------//
        public void onMoveAction()
        {
            if ((this.onMove == true))
            {
                if (this.onDamageReceived == true)
                {
                    audio_manager.received_damage_src.Play(); //PLAY AUDIO 

                    onMove = false;
                    animator.SetBool("onMove", false); //Move Animation
                }
                else
                {
                    if(this.gameObject != null)
                    {
                        //Check time calculation in case of being in squadron view (LEVELS 1 AND 2 OF VIEW)
                        if (squadron_controller.isCaptain(this.gameObject) != -1) //If the actual gameobject is captain of a squadron
                        {
                            if (this.gameObject.transform.Find(squadron_controller.squad_ui_str + squadron_controller.isCaptain(this.gameObject)).gameObject != null) //If Squadron_UI is instantiated
                            {
                                GameObject squad_ui = this.gameObject.transform.Find(squadron_controller.squad_ui_str + squadron_controller.isCaptain(this.gameObject)).gameObject;
                                time_text = squad_ui.gameObject.transform.Find("Unit_UI_Tracker").transform.Find("Time").GetComponent<TMP_Text>();
                                timeCalculation(); //Calculate the time to reach the destination
                            }
                            else
                            {
                                Debug.Log("NULL!");
                            }
                        }
                    }
                  

                    if (!agent.pathPending) //Evitate the first frame (Prevent possible incorrect comprobation with the reamining distance == 0)
                    {
                        if (agent.remainingDistance <= agent.stoppingDistance)
                        {
                            onMove = false;
                            animator.SetBool("onMove", false); //Move Animation
                        }
                    }
                    else
                    {
                        audio_manager.moving_src.Play(); //PLAY AUDIO 

                        animator.SetBool("onMove", true); //Move Animation
                        animator.SetBool("onAttack", false);
                    }
                }

                animator.SetBool("onAttack", false);
            }
        }

        //Set Unit range depend on its action
        public void setUnitRange(int action, bool isQuadMovement) //0 == MOVE/OTHER | 1 == ATTACK
        {
            float move_range = 0.0f;
            float attack_range = 10.0f;

            attack_range = unit_stats.range;

            if(isQuadMovement == true)
            {
                move_range = 0.0f;
            }
            else
            {
                move_range = 5.0f;
            }

            if (action == 0)
            {
                agent.stoppingDistance = move_range;
            }
            else if (action == 1)
            {
                agent.stoppingDistance = attack_range;
            }
            else
            {
                agent.stoppingDistance = move_range;
            }
        }

        //The movement will repeat every time the squadron has a objective. The false condition only will be reachable when the Captain of the squadron finish the movement
        public void onMoveSquadronAction(bool onSquadronMove)
        {
            if(onDamageReceived == false)
            {
                if (onSquadronMove == true)
                {
                    agent.SetDestination(squadron_position.transform.position);
                    animator.SetBool("onMove", true); //Move Animation
                }
                else
                {
                    if (!agent.pathPending) //Evitate the first frame (Prevent possible incorrect comprobation with the reamining distance == 0)
                    {
                        animator.SetBool("onMove", false); //Move Animation
                    }
                    else
                    {
                        animator.SetBool("onMove", true); //Move Animation
                    }
                }
            }
        }

        public bool onMoveAsync()
        {
            if (this.onMove == false) //If movement finish
            {
                return true;
            }
            else if (this.onAttack == true) //If captain is on attack the other units should done the attack: consider it as they are a good amount of distance
            {
                return true;
            }
            else //Otherwhise
            {
                return false;
            }
        }

        public void onAttackAction()
        {
            if((onDie == false || health > 0) && (onDamageReceived == false))
            {
                if (selected_enemy != null)
                {
                    if (selected_enemy.GetComponent<Unit>().health > 0) //If the movement is finished, and attack is activated -> ATTACK ACTION
                    {
                        //SET THE "STATIC" vars for the possibility of "getNextEnemy" when the actual one dies
                        aux_enemy_squad = squadron_controller.getAnySquadron(selected_enemy); //STATIC ENEMY SQUAD (NOT ALTERABLE BY KILLING AN ENEMY PREVIOUSLY)


                        //Rotate the unit to face the desired enemy

                        //Vector3 dir = selected_enemy.transform.position - transform.position;
                        //dir.y = 0;
                        //Quaternion rot = Quaternion.LookRotation(dir);

                        Vector3 facing_pos = selected_enemy.transform.position;
                        facing_pos.y = transform.position.y;

                        Vector3 facing_dir = facing_pos - transform.position;
                        facing_dir.Normalize();

                        Vector3 new_facing_dir = Vector3.RotateTowards(transform.forward, facing_dir, rotation_speed * Time.deltaTime, 0.0f);

                        Debug.DrawRay(transform.position, facing_dir * 100, Color.blue);

                        if (transform.forward != new_facing_dir) //(transform.rotation != rot) //Wait to unit face the desired location/enemy
                        {
                            //transform.rotation = Quaternion.Lerp(transform.rotation, rot, rotation_speed * Time.deltaTime);
                            transform.forward = new_facing_dir;
                        }
                        else
                        {
                            // Start the attack
                            onAttack = true;
                            animator.SetBool("onAttack", true); //Move Animation

                            if (!agent.pathPending) //Check if the enemy is in the firing range
                            {
                                float distance_to_enemy; //TO-DO: Check if the range is optimus to the NAVMESH STOP_DISTANCE
                                distance_to_enemy = Vector3.Distance(this.transform.position, selected_enemy.transform.position);

                                if (distance_to_enemy > this.unit_stats.range)
                                {
                                    agent.SetDestination(selected_enemy.transform.position);
                                    onAttack = false;
                                    onMove = true;
                                    animator.SetBool("onAttack", false); //Attack Animation
                                    animator.SetBool("onMove", true); //Move Animation
                                }
                            }

                        }
                    }
                    else //In case of defeat the previously selected enemy
                    {
                        if ((selected_enemy != null) && (aux_enemy_squad != null))
                        {
                            //Check if the defeated enemy has a squadron. If has one, select an enemy from there
                            if (squadron_controller.getNextEnemy(aux_enemy_squad, selected_enemy) != null)
                            {
                                selected_enemy = squadron_controller.getNextEnemy(aux_enemy_squad, selected_enemy);
                                aux_enemy_squad = squadron_controller.getAnySquadron(selected_enemy); //RESET - STATIC ENEMY SQUAD
                            }
                            else
                            {
                                //Otherwhise, finish the attack action
                                selected_enemy = null;
                                onAttack = false;
                                animator.SetBool("onAttack", false); //Attack Animation
                                animator.SetBool("onMove", false); //Move Animation
                            }
                        }
                        else
                        {
                            //Otherwhise, finish the attack action
                            selected_enemy = null;
                            onAttack = false;
                            animator.SetBool("onAttack", false); //Attack Animation
                            animator.SetBool("onMove", false); //Move Animation
                        }
                       
                    }
                }
            }
            

        }

        //----------------------UNIT ACTIONS----------------------//

        //Remove the GameObject unit from the scene
        public void destroyUnit()
        {
            Destroy_VFX destroy = this.gameObject.AddComponent<Destroy_VFX>();

            destroy.deathtimer = 2.0f;

        }

        //----------------------UNIT UI----------------------//
        //Update the name in the UI Text of the Unit
        public void updateName(string name)
        {
            name_text.text = name;
        }


        private void OnMouseOver()
        {
            Outline outline = this.gameObject.GetComponent<Outline>();

            outline.OutlineWidth = 5.0f;
        }

        public void OnMouseExit()
        {
            Outline outline = this.gameObject.GetComponent<Outline>();

            outline.OutlineWidth = 0.0f;

            /*if (faction == game_controller.social_controller.player_faction) //Player
            {
                Outline outline = this.gameObject.GetComponent<Outline>();

                outline.OutlineWidth = 5.0f;
            }
            else //Enemy
            {

            }
            */
        }
        //----------------------UNIT UI----------------------//

    }
}
