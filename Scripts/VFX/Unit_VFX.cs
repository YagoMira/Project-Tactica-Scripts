using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class Unit_VFX : MonoBehaviour
    {
        public float scale = 1.0f; //Scale of the bullets, particles and its effects

        public Transform spawnLocator;
        public Transform spawnLocatorMuzzleFlare;
        public Transform spawnLocatorSmoke;
        public Transform shellLocator;
        public Animator recoilAnimator;

        public Transform[] shotgunLocator;

        Unit_AudioManager audio_manager; //Unit Audio Manager

        //Unit Shooting variables
        public int counter_bullets = 0;
        public int num_bullets = 3;
        public float time_to_shoot = 2.0f;

        [System.Serializable]
        public class projectile
        {
            public string name;

            public Rigidbody bombPrefab;
            public GameObject muzzleflare;
            public GameObject smokeEffect;

            public float min_speed, max_Speed;

            public bool rapidFire;
            public float rapidFireDelay;
            public float rapidFireCooldown;
            public bool shotgunBehavior;
            public int shotgunPellets;

            public GameObject shellPrefab;
            public bool hasShells;
        }

        public projectile projectile_object;

        public float firingTimer;
        public bool firing;

        public bool CameraShake = true;
        public ECCameraShakeProjectile CameraShakeCaller;


        void Start()
        {
            audio_manager = this.GetComponent<Unit_AudioManager>();
        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetButtonDown("Fire1")) //Start Shooting #TO-DO: Change it for the Shoot() function
            //{
            //    firing = true;
            //    Fire();
            //}
            //if (Input.GetButtonUp("Fire1")) //Stop shooting #TO-DO: Change it for the Shoot() function
            //{
            //    firing = false;
            //    firingTimer = 0;
            //}

            if (projectile_object.rapidFire && firing)
            {
                if (firingTimer > projectile_object.rapidFireCooldown + projectile_object.rapidFireDelay)
                {
                    Fire();
                    firingTimer = 0;
                }
            }

            if (firing)
            {
                firingTimer += Time.deltaTime;
            }
        }

        public void Fire()
        {
            //Unit unit = this.GetComponent<Unit>();
            //if((unit.selected_enemy != null) && (unit.onAttack == true)) //TO-DO: REVIEW!
            //{
            //    spawnLocator.LookAt(unit.selected_enemy.transform);
            //    Debug.Log("EYYY");
            //}

            //Rotate the bullet spawner to face the enemy
            Vector3 enemy_position;

            if (this.transform.GetComponent<Unit>().selected_enemy != null)
            {
                enemy_position = this.transform.GetComponent<Unit>().selected_enemy.transform.position;

                //Vector3 dir = enemy_position - transform.position;
                //dir.y = 0;
                //Quaternion rot = Quaternion.LookRotation(dir);

                //if (transform.localRotation != rot) //Wait to unit face the desired location/enemy
                //{
                //    transform.rotation = Quaternion.Lerp(transform.rotation, rot, 5.0f * Time.deltaTime);
                //}

                Vector3 facing_pos = enemy_position;
                facing_pos.y = transform.position.y;

                Vector3 facing_dir = facing_pos - transform.position;
                facing_dir.Normalize();

                Vector3 new_facing_dir = Vector3.RotateTowards(transform.forward, facing_dir, this.transform.GetComponent<Unit>().rotation_speed * Time.deltaTime, 0.0f);


                if (transform.forward != new_facing_dir) //Wait to unit face the desired location/enemy
                {
                    transform.forward = new_facing_dir;
                }
                else
                {
                    if (CameraShake)
                    {
                        if (CameraShakeCaller != null)
                            CameraShakeCaller.ShakeCamera();
                    }

                    //Spawners (Muzzle and Smoke)
                    GameObject new_muzzleflare = Instantiate(projectile_object.muzzleflare, spawnLocatorMuzzleFlare.position, spawnLocatorMuzzleFlare.rotation);
                    new_muzzleflare.transform.localScale = new Vector3(scale, scale, scale); //SCALE

                    if (projectile_object.smokeEffect != null)
                    {
                        GameObject new_smokeEffect = Instantiate(projectile_object.smokeEffect, spawnLocatorSmoke.position, spawnLocatorSmoke.rotation); //Smoke Shoot Spawn
                        new_smokeEffect.transform.localScale = new Vector3(scale, scale, scale); //SCALE
                    }

                    audio_manager.shoot_src.Play(); //PLAY AUDIO

                    //Instantiate the bullet shells
                    if (projectile_object.hasShells)
                    {
                        GameObject new_shellPrefab = Instantiate(projectile_object.shellPrefab, shellLocator.position, shellLocator.rotation);
                        new_shellPrefab.transform.localScale = new Vector3(scale, scale, scale); //SCALE
                    }

                    //Instantiate bullets
                    Rigidbody bulletInstance;
                    bulletInstance = Instantiate(projectile_object.bombPrefab, spawnLocator.position, spawnLocator.rotation) as Rigidbody;
                    bulletInstance.gameObject.GetComponent<Bullet_VFX>().shooter = this.gameObject; //Set the actual shooter of the bullet for the damageReceive function of the enemy 
                                                                                                    //(Receive this Damage stat as parameter from them)
                    GameObject new_bulletInstance = bulletInstance.gameObject;
                    new_bulletInstance.transform.localScale = new Vector3(scale, scale, scale); //SCALE

                    bulletInstance.AddForce(spawnLocator.forward * Random.Range(projectile_object.min_speed, projectile_object.max_Speed));

                    //Shotgun Behaviour
                    if (projectile_object.shotgunBehavior)
                    {
                        for (int i = 0; i < projectile_object.shotgunPellets; i++)
                        {
                            Rigidbody rocketInstanceShotgun;
                            rocketInstanceShotgun = Instantiate(projectile_object.bombPrefab, shotgunLocator[i].position, shotgunLocator[i].rotation) as Rigidbody;

                            GameObject new_bombPrefab = rocketInstanceShotgun.gameObject;
                            new_bombPrefab.transform.localScale = new Vector3(scale, scale, scale); //SCALE

                            rocketInstanceShotgun = new_bombPrefab.GetComponent<Rigidbody>();
                            rocketInstanceShotgun.AddForce(shotgunLocator[i].forward * Random.Range(projectile_object.min_speed, projectile_object.max_Speed));
                        }
                    }
                }

            }

        }


    }
}
