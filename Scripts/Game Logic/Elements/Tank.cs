using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Project_Tactica
{
    public class Tank : Unit, Unit_Interface
    {
        public GameObject aiming_object; //######### SHOULD BE "selected_enemy" FROM THE PARENT SCRIPT!!!

        Transform aim_transform;
        Transform aim_pivot;
        
        //public float rotation_speed = 0.5f;
        float fixed_angle = -90.0f;

        [HideInInspector]
        public Unit_VFX shoots_manager;

        private void Start()
        {
            shoots_manager = this.GetComponent<Unit_VFX>();

            if (transform.Find("Body") != null)
            {
                aim_transform = (transform.Find("Body")).Find("Aim");
                aim_pivot = ((transform.Find("Body")).Find("Aim")).Find("Aim_Pivot");
            }
            
        }

        protected override void Update()
        {
            base.Update();
            //Aim(); //TO-DO: SHOULD BE CHANGED, AIM WHEN FIND A NEAR ENEMY. NECESSARY MANAGE ENEMY/ATACK

            if(onDie == false)
            {
                if (onAttack == true)
                {
                    if (shoots_manager.counter_bullets != shoots_manager.num_bullets)
                    {
                        Shoot(); //TO-DO: SHOULD BE CHANGED, AIM WHEN FIND A NEAR ENEMY. NECESSARY MANAGE ENEMY/ATACK


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
            else
            {
                onAttack = false;
            }
            

        }

        public Tank(string name, string description, elem_type type, unit_type sub_type, float health) : base(name, description, type, sub_type, health)
        {
            type = elem_type.Unit;
            sub_type = unit_type.Tank;
        }

        //The NPC Aiming to the desired enemy //TO-DO: Check the moment when the Tank is AIMING (BOOLEAN), WHEN AIMING IS FINISH AND OBJECTIVE IS NEAR -> SHOOT!
        public void Aim()
        {

            if(selected_enemy != null)
            {
                aiming_object = selected_enemy;
            }

            float angle_limit = 70.0f;

            if(aiming_object != null) //Prevent possible null exceptions
            {
                aim_pivot.LookAt(aiming_object.transform);
                aim_pivot.transform.Rotate(0, -90, 0); //Fixing Pivot rotation
                aim_pivot.eulerAngles = new Vector3(0, aim_pivot.eulerAngles.y, 0); //Limitate the rotation only to Y Axis

                float angle = getRealAngle(aim_transform.rotation, aim_pivot.rotation);

                //Angle Limitations
                if ((angle < angle_limit) && (angle > -(angle_limit)))
                {
                    aim_transform.rotation = Quaternion.Lerp(aim_transform.rotation, aim_pivot.rotation, Time.deltaTime * rotation_speed); //Rotation with time
                }
                else //Prevent possible Bugs of no rotation for exceed the limits
                {
                    aim_pivot.rotation = aim_transform.rotation;
                }

                
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
            if (shoots_manager.firing == false)
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
            if (onDie == false) //onDamageReceived == false && 
            {
                base.audio_manager.received_damage_src.Play(); //PLAY AUDIO 
                                                               //Instantiate(received_damage_particle, hit_point, received_damage_particle.transform.rotation); //Instantiate the IMPACT prefab//Instantiate the explosion on the hit point
                                                               //TO-DO: PROPER CALCULATIONS TO APPLY THE DAMAGE (CHARACTERISTICS OF THE ACTUAL NPC). THE RECEIVED ONE IS THE BASE DAMAGE!

                Instantiate(received_damage_particle, hit_point, received_damage_particle.transform.rotation); //Instantiate the IMPACT prefab//Instantiate the explosion on the hit point

                //Debug.Log("Damage Received");
                finalHealthCalculation(damage);

                if (health <= 0) //Die State
                {
                    onDie = true;
                    //animator.SetBool("onDie", true); //animator.SetBool("onDie", true);

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
                    //animator.SetBool("onDamage", true);
                    onDamageReceived = true;

                    StartCoroutine(StopReceivedDamage());
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
            yield return new WaitForSeconds(0.5f); //TO-DO: THIS VALUE CAN CHANGE!

            if (onDamageReceived == true)
            {
                onDamageReceived = false;

                //animator.SetBool("onDamage", false);

                health_damage.SetActive(false);
            }

            //StopCoroutine(StopReceivedDamage()); //TO-DO: CHECK THIS STOP!!!
        }

        //Get the Euler Y angle of the inspector (based on the rotation of the tank pivot)
        public float getRealAngle(Quaternion from_rot, Quaternion to_rot)
        {
            float angle = Quaternion.Lerp(from_rot, to_rot, Time.deltaTime * rotation_speed).eulerAngles.y;

            if (angle > 180.0f) //In case of SHOULD BE negative angle
                angle = angle - 360.0f;

            return angle;
        }
    }
}
