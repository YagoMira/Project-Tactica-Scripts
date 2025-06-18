using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{

    public class Bullet_VFX : MonoBehaviour
    {
        public GameObject shooter;
        public GameObject impact_object;

        public Rigidbody rigidbody;
        private Collider collider;

        public bool ignorePrevRotation = false;
        public bool lookRotation = true;

        float timer;

        private Vector3 previousPosition;

        public bool collision_npc = false;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            previousPosition = transform.position;
        }

        void Update()
        {
            timer += Time.deltaTime;

            if(previousPosition != null)
                CheckCollision(previousPosition);

            previousPosition = transform.position;
        }

        void FixedUpdate()
        {
            if (lookRotation && timer >= 0.05f)
            {
                transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
            }
        }


        void CheckCollision(Vector3 prevPos)
        {
            RaycastHit hit;
            Vector3 direction = transform.position - prevPos;
            Ray ray = new Ray(prevPos, direction);

            float dist = Vector3.Distance(transform.position, prevPos);

            if (Physics.Raycast(ray, out hit, dist))
            {
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                Vector3 pos = hit.point;

                transform.position = pos;

                //CHECK THE COLLIDED GAMEOBJECT
                if(hit.transform.gameObject.GetComponent<Unit>() == null) //If the collided gameobject is NOT and NPC
                {
                    Instantiate(impact_object, pos, rot); //Instantiate the IMPACT prefab
                    Destroy(gameObject);  //Destroy THIS gameobject when the hit is done

                    //Debug.Log("NOT NPC");
                }
                else
                {
                    if (collision_npc == false)
                    {
                        Unit unit_hit = hit.transform.gameObject.GetComponent<Unit>();
                        Unit unit_shooter = shooter.gameObject.GetComponent<Unit>();

                        SocialSystem social_controller = unit_hit.game_controller.social_controller;

                        //If the relationship between this Units (Their factions) are enemies, then apply damage
                        if (social_controller.GetRelationBetween((int)unit_hit.faction, (int)unit_shooter.faction) == Element.elem_status.Enemy)
                        {
                            unit_hit.receiveDamage(unit_shooter.unit_stats.damage, pos);
                            unit_hit.responseReceiveDamage(unit_shooter);

                            Destroy(this.gameObject);
                        }

                        collision_npc = true;
                    }


                }
            }

            collision_npc = false;
        }

    }
}
