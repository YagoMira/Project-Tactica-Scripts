using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class Almah : Unit, Unit_Interface
    {
        [HideInInspector]
        public Unit_VFX shoots_manager;

        [Space]
        public string general_name = "";

        public bool on_liberate = false;

        private void Start()
        {
            shoots_manager = this.GetComponent<Unit_VFX>();

            CheckGeneral();
        }

        protected override void Update()
        {
            base.Update();
            //Aim(); //TO-DO: SHOULD BE CHANGED, AIM WHEN FIND A NEAR ENEMY. NECESSARY MANAGE ENEMY/ATACK

            if(on_liberate != true)
            {
                if (onAttack == true)
                {
                    if (shoots_manager.counter_bullets != shoots_manager.num_bullets)
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

            }
        }

        public Almah(string name, string description, elem_type type, unit_type sub_type, float health) : base(name, description, type, sub_type, health)
        {
            type = elem_type.Unit;
            sub_type = unit_type.Almah;
        }

        public void CheckGeneral()
        {
            if ((general_name == "") || (general_name == null))
            {
                Debug.LogError("ALMAH: " + name + " GENERAL NOT ASSIGNED!");
            }

            if(general_name == "PLAYER")
            {
                general_name = DataSaver.player_name.ToString();
            }
        }

        //The NPC Aiming to the desired enemy //TO-DO: Check the moment when the Tank is AIMING (BOOLEAN), WHEN AIMING IS FINISH AND OBJECTIVE IS NEAR -> SHOOT!
        public void Aim()
        {

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
            if(on_liberate == false) //onDamageReceived == false && 
            {
                //base.audio_manager.received_damage_src.Play(); //PLAY AUDIO 
                                                               //Instantiate(received_damage_particle, hit_point, received_damage_particle.transform.rotation); //Instantiate the IMPACT prefab//Instantiate the explosion on the hit point
                                                               //TO-DO: PROPER CALCULATIONS TO APPLY THE DAMAGE (CHARACTERISTICS OF THE ACTUAL NPC). THE RECEIVED ONE IS THE BASE DAMAGE!

                //Debug.Log("Damage Received");
                finalHealthCalculation(damage);

                if (health <= 0) //Die State
                {
                    onDie = true;

                    onMove = false;
                    onAttack = false;

                    on_liberate = true;
                    animator.SetBool("onLiberate", true); //animator.SetBool("onDie", true);

                    /*
                    //Check if this unit is part of squadron and remove it
                    squadron_controller.checkUnitHasSquadron(this.gameObject);
                    */

                    //ACTIVATE LIBERATE ACTION!
                    if(this.faction != game_controller.social_controller.player_faction) //ONLY FOR ENEMIES
                    {
                        GameObject liberate_ui = unit_ui_object.transform.Find("Unit_UI_Tracker").transform.Find("LIBERATE").gameObject;
                        liberate_ui.SetActive(true);
                        onDie = false;
                    }
                    

                    StartCoroutine(IdleDeath(0.5f));
                }
                else //Receive Damage State
                {
                    //animator.SetBool("onDamage", true);
                    //onDamageReceived = true;

                    //StartCoroutine(StopReceivedDamage());
                }
              
            }
        }

        public IEnumerator IdleDeath(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            //Enter the Idle Liberate/Death anim
            animator.SetBool("onLiberate", false);
        }

        public override void finalHealthCalculation(float damage)
        {
            //Debug.Log("RECEIVED DAMAGE!!!! " + damage);

            float final_damage = (damage - (damage * (this.unit_stats.resistance / 100)));
            this.health -= final_damage;

            float final_health_amount = Mathf.Lerp(0.0f, 100.0f, health / max_health);
            health_filler.fillAmount = final_health_amount / 100.0f; //UPDATE HEALTH UI
            
            //Debug.Log("FINAL DAMAGE!!!! " + final_damage);

        }

        public IEnumerator StopReceivedDamage()
        {
            yield return new WaitForSeconds(3.0f); //TO-DO: THIS VALUE CAN CHANGE!

            if (onDamageReceived == true)
            {
                onDamageReceived = false;
                animator.SetBool("onDamage", false);
            }

            //StopCoroutine(StopReceivedDamage()); //TO-DO: CHECK THIS STOP!!!
        }
    }
}
