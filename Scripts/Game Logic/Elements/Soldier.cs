using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Project_Tactica
{
    public class Soldier : Unit, Unit_Interface
    {
        [HideInInspector]
        public Unit_VFX shoots_manager;

        private void Start()
        {
            shoots_manager = this.GetComponent<Unit_VFX>();
        }

        protected override void Update()
        {
            base.Update();
            //Aim(); //TO-DO: SHOULD BE CHANGED, AIM WHEN FIND A NEAR ENEMY. NECESSARY MANAGE ENEMY/ATACK

            if (onAttack == true)
            {
                if(shoots_manager.counter_bullets != shoots_manager.num_bullets)
                {
                    Shoot();

                    shoots_manager.counter_bullets++;
                }
                else //MAXIMUM NUMBER OF BULLETS REACHED!
                {
                    StartCoroutine(Shooting());
                }
                
            }
            else
            {
                StopShooting();
                //StopAllCoroutines(); //TO-DO: CHECK THIS STOP ALL!!!
            }

            //Recheck destruction
            if(onDie == true)
            {
                destroyUnit(); //Destroy the unit after some time and also after it dies.
            }
            else
            {
                if(onDamageReceived == true)
                     StartCoroutine(StopReceivedDamage());
            }


            /*
            if (selected_enemy != null)
            {
                CalculateEnemyDistance(selected_enemy.gameObject);
            }
            */
        }

        public Soldier(string name, string description, elem_type type, unit_type sub_type, float health) : base(name, description, type, sub_type, health)
        {
            type = elem_type.Unit;
            sub_type = unit_type.Soldier;
        }

        //The NPC Aiming to the desired enemy //TO-DO: Check the moment when the Tank is AIMING (BOOLEAN), WHEN AIMING IS FINISH AND OBJECTIVE IS NEAR -> SHOOT!
        public void Aim()
        {

        }

        public void CalculateEnemyDistance(GameObject enemy)
        {
            Vector3 direction = new Vector3(0, 0, -1);
            Vector3 startPosition;
            Vector3 tDirection;

            Vector3 newDestination;

            float maxDistance = 5.0f;

            float d_to_enemy = Vector3.Distance(this.gameObject.transform.position, enemy.transform.position); //Distance

            if (d_to_enemy < maxDistance)
            {
                onAttack = false;
                animator.SetBool("onAttack", false);

                //Recalculate new position -> Move -> Then Attack
                startPosition = new Vector3(enemy.transform.position.x, enemy.transform.position.y, enemy.transform.position.z);
                tDirection = enemy.transform.rotation * direction;
                newDestination = startPosition + tDirection * maxDistance;

                this.gameObject.GetComponent<NavMeshAgent>().SetDestination(newDestination);
                onMove = true;
                animator.SetBool("onMove", true);

                GameObject newPosition = new GameObject();
                newPosition.transform.position = newDestination;

                Instantiate(newPosition);
                //Debug.Log("ENTRAS!!!! " + newDestination);
            }
            else
            {
                onAttack = true;
                animator.SetBool("onAttack", true);
            }
        }

        public IEnumerator Shooting()
        {
            StopShooting();
            yield return new WaitForSecondsRealtime(shoots_manager.time_to_shoot);
            shoots_manager.counter_bullets = 0;
            StopAllCoroutines(); //TO-DO: CHECK THIS STOP ALL!!!
        }

        public void Shoot()
        {
            if(shoots_manager.firing == false)
                shoots_manager.firing = true;
            shoots_manager.Fire();
        }

        public void StopShooting()
        {
            shoots_manager.firing = false;
            shoots_manager.firingTimer = 0;
        }


        //DAMAGE CHECKER - COLLISIONS
        public override void receiveDamage(float damage, Vector3 hit_point)
        {
            if(onDie == false) //onDamageReceived == false && 
            {
                base.audio_manager.received_damage_src.Play(); //PLAY AUDIO 
                                                               //Instantiate(received_damage_particle, hit_point, received_damage_particle.transform.rotation); //Instantiate the IMPACT prefab//Instantiate the explosion on the hit point
                                                               //TO-DO: PROPER CALCULATIONS TO APPLY THE DAMAGE (CHARACTERISTICS OF THE ACTUAL NPC). THE RECEIVED ONE IS THE BASE DAMAGE!

                //Debug.Log("Damage Received");
                finalHealthCalculation(damage);

                if (health <= 0) //Die State
                {
                    onDie = true;

                    onMove = false;
                    onAttack = false;

                    animator.SetBool("onDie", true); //animator.SetBool("onDie", true);

                    /*
                    //Check if this unit is part of squadron and remove it
                    squadron_controller.checkUnitHasSquadron(this.gameObject);
                    */

                    //Reorder the actual squadron of this unit if it dies
                    squadron_controller.reorderSquadron(this.gameObject);


                    destroyUnit(); //Destroy the unit after some time and also after it dies.

                    this.GetComponent<Collider>().enabled = false; //Disable the collider for prevent bugs.
                }
                else //Receive Damage State
                {
                    if(onDamageReceived != true)
                    {
                        onDamageReceived = true;
                        animator.SetBool("onDamage", true);

                        StartCoroutine(StopReceivedDamage());
                    }
                    else
                    {
                        onDamageReceived = false;
                        animator.SetBool("onDamage", false);
                    }
                    
                }
              
            }
        }

        public override void finalHealthCalculation(float damage)
        {
            //Debug.Log("RECEIVED DAMAGE!!!! " + damage);

            float final_damage = (damage - (damage * (this.unit_stats.resistance / 100)));
            this.health -= final_damage;

            float final_health_amount = Mathf.Lerp(0.0f, 100.0f, health / max_health);
            health_filler.fillAmount = final_health_amount / 100.0f; //UPDATE HEALTH UI

            health_damage.GetComponent<TMP_Text>().text = ("-" + damage.ToString());

            //Debug.Log("FINAL DAMAGE!!!! " + final_damage);

            if (health_damage.activeSelf == true)
            {
                health_damage.SetActive(false);
                health_damage.SetActive(true);
            }
            else
            {
                health_damage.SetActive(true);
            }
            
            //SHOW THE DAMAGE UI

        }

        public IEnumerator StopReceivedDamage()
        {
            yield return new WaitForSeconds(1.0f); //TO-DO: THIS VALUE CAN CHANGE!


            onDamageReceived = false;

            animator.SetBool("onDamage", false);

            health_damage.SetActive(false);


            //StopCoroutine(StopReceivedDamage()); //TO-DO: CHECK THIS STOP!!!
        }
    }
}
