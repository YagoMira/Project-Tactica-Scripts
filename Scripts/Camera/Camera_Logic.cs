using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class Camera_Logic : MonoBehaviour
    {

        public Transform target;

        public GameObject game_logic;
        public Input_Logic input;

        public SquadronController squadron_controller;

        //Input 
        public bool disable_input = false;
        
        //FOW Plane
        public float fow_distance = 30.0f;
        public GameObject fow_plane;

        //Camera Attributes
        public float camera_movement_speed = 200.0f;
        public float shift_speed_multiplier = 5.0f;
        float camera_movement_speed_aux = 0.0f; //Used for the camera speed storage
        public float camera_rotation_speed = 100.0f;
        public float camera_rotation_speed_middle = 2.0f;
        public float camera_zoom_speed = 20.0f;

        public float screen_limit_range = 50.0f;

        public float zoom_distance = 0.0f;
        public float zoom_max_distance = 1000.0f;
        public float zoom_min_distance = 250.0f;

        //Camera Middle rotation
        public float minimum_y_angle = 45.0f;
        public float maximum_y_angle = 85.0f;

        //Camera Debug
        RaycastHit HitInfo;
        public bool cameraIsAiming = false;
        public Vector3 cameraTarget;

        //Camera Settings
        public bool enable_camera_on_mouse = false;
        public bool enableDebug = false;


        //Camera Heights
        public int actual_level = 2; //1 - MAP VIEW | 2 - STRATEGIC VIEW | 3 - TROOP VIEW
        public int[] heights = new int[3];

        public Vector3 current_rotation;
        public Vector3 smooth_velocity = Vector3.zero;
        public float smooth_time = 3.0f;
        public float distance_target = 3.0f;



        // Start is called before the first frame update
        void Start()
        {
            input = game_logic.GetComponent<Input_Logic>();
            squadron_controller = game_logic.GetComponent<SquadronController>();

            //Camera Heights Declaration
            /*
            heights[0] = (int) zoom_distance;//1000;
            heights[1] = 300;//2000;
            heights[2] = (int) zoom_max_distance;//3000;
            */

            camera_movement_speed_aux = camera_movement_speed;
        }



        // Update is called once per frame
        void Update()
        {
            if (disable_input != true)
            {
                cameraMovement();
                cameraRotationMiddle();
                cameraZoom();
            }
            //cameraHeight(Camera.main.transform); //Disable the debug when will be need it
        }

        //PHYSICS
        private void LateUpdate()
        {
            cameraRotation();
        }




        //----------------------CAMERA MOVEMENT----------------------//
        public void cameraMovement() //Press the correct camera input and MOVE the camera to the desired position
        {
            Transform cameraTransform = Camera.main.transform;

            Vector3 camera_vector_up_down = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;

            Vector3 camera_vector_left_right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

            //In case of Camera Input (KEYS) - The second conditional is in case of Mouse Input for camera movement

            //PRESS W - MOVE FORWARD
            if ((input.camera_input_up == true) || (enable_camera_on_mouse == true && (input.mousePosition.y >= Screen.height - screen_limit_range)))
            {
                transform.position += camera_vector_up_down * Time.unscaledDeltaTime * camera_movement_speed; //USING DELTA TIME TO PREVENT THE CAMERA BEING PAUSED WITH THE TIME SYSTEM !!!

                cameraIsAiming = false;
            }

            //PRES S - MOVE BACKWARDS
            if (input.camera_input_down == true || (enable_camera_on_mouse == true && (input.mousePosition.y <= screen_limit_range)))
            {
                transform.position -= camera_vector_up_down * Time.unscaledDeltaTime * camera_movement_speed;
                cameraIsAiming = false;
            }

            //PRESS E - MOVE RIGHT
            if (input.camera_input_right == true || (enable_camera_on_mouse == true && (input.mousePosition.x >= Screen.width - screen_limit_range)))
            {
                transform.position += camera_vector_left_right * Time.unscaledDeltaTime * camera_movement_speed;
                cameraIsAiming = false;
            }

            //PRESS Q - MOVE LEFT
            if (input.camera_input_left == true || (enable_camera_on_mouse == true && (input.mousePosition.x <= screen_limit_range)))
            {
                transform.position -= camera_vector_left_right * Time.unscaledDeltaTime * camera_movement_speed;
                cameraIsAiming = false;
            }

            cameraGetTarget(cameraTransform); //Only get the collisioned point when move the camera

            if (enableDebug == true)
                cameraDebug(camera_vector_up_down, camera_vector_left_right);

            //PRES SHIFT - CAMERA SPEED
            if (input.shift_left_click == true)
            {
                camera_movement_speed = camera_movement_speed_aux * shift_speed_multiplier;
            }
            else
            {
                camera_movement_speed = camera_movement_speed_aux;
            }

            RepositionateCamera();
        }


        //In case of camera zoom is less than the minimun: repositionate
        public void RepositionateCamera()
        {
            Vector3 aux_position;
            float time_to_repos = 0.3f;

            if(zoom_distance < zoom_min_distance)
            {
                while (zoom_distance < zoom_min_distance)
                {
                    zoom_distance = Vector3.Distance(cameraTarget, Camera.main.transform.position); //Zoom limitations

                    aux_position = transform.position + Camera.main.transform.forward * (-0.1f) * camera_zoom_speed * 100.0f * Time.unscaledDeltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, aux_position, time_to_repos * Time.deltaTime); //aux_position;
                }
            }

            if (zoom_distance > zoom_max_distance)
            {
                while (zoom_distance > zoom_max_distance)
                {
                    zoom_distance = Vector3.Distance(cameraTarget, Camera.main.transform.position); //Zoom limitations

                    aux_position = transform.position + Camera.main.transform.forward * (+0.1f) * camera_zoom_speed * 100.0f * Time.unscaledDeltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, aux_position, time_to_repos * Time.deltaTime); //aux_position;
                }
            }


        }

        //CAMERA ROTATES BUT STILL LOOKING AT SAME POINT
        public void cameraRotation() //Press the correct camera input and ROTATE the camera to the desired position
        {
            Quaternion rot = transform.rotation;

            if (input.camera_input_rotate_right == true)
            {
                transform.Translate(Vector3.right * Time.smoothDeltaTime * camera_rotation_speed);
            }

            if (input.camera_input_rotate_left == true)
            {
                transform.Translate(Vector3.right * Time.smoothDeltaTime * -camera_rotation_speed);
            }
        }

        public void cameraRotationMiddle()
        {

            if (input.camera_input_rotate_middle == true) //Hold the button 
            {
                cameraGetTarget(Camera.main.transform); //Update the camera target


                //Rotate Camera on Horizontal Angle
                Camera.main.transform.RotateAround(cameraTarget,
                                         Camera.main.transform.up,
                                        -Input.GetAxis("Mouse X") * camera_rotation_speed_middle);

                //Rotate Camera on Vertical Angle
                float newRotationY = Camera.main.transform.localEulerAngles.x - (camera_rotation_speed_middle * Input.GetAxis("Mouse Y"));
                newRotationY = Mathf.Clamp(newRotationY, minimum_y_angle, maximum_y_angle); //Limitation on Rotation
                float rotateY = newRotationY - Camera.main.transform.localEulerAngles.x;

                Camera.main.transform.RotateAround(cameraTarget,
                                                     Camera.main.transform.right,
                                                    rotateY);

                //UpdateFOWPlanePosition(fow_distance);
            }
        }


        public void cameraZoom() //Press the correct camera input and ZOOM the camera to the desired position
        {
            zoom_distance = Vector3.Distance(cameraTarget, Camera.main.transform.position); //Zoom limitations

            if (input.mouseScroll != input.initial_mouse_scroll) //Check if the middle mouse button is being used
            {

                if ((zoom_distance <= zoom_max_distance) && (zoom_distance >= zoom_min_distance)) //In case of stay between the zoom limits
                {
                    Vector3 aux_position; //Auxiliar variable to help with the zoom limits
                    float aux_distance; //Auxiliar variable to help with the zoom limits

                    aux_position = transform.position + Camera.main.transform.forward * input.mouseScroll * camera_zoom_speed * 100.0f * Time.unscaledDeltaTime;
                    aux_distance = Vector3.Distance(cameraTarget, aux_position);


                    if ((aux_distance <= zoom_max_distance) && (aux_distance >= zoom_min_distance)) //Stay between the zoom limits to apply the zoom to the camera transform
                    {
                        transform.position = aux_position;

                        input.initial_mouse_scroll = input.mouseScroll; //Save the actual middle mouse button axis to compare in the next iteration

                        cameraHeight(Camera.main.transform); //Check the camera Height

                        visibilitySquadron();

                        //UpdateFOWPlanePosition(fow_distance);
                    }
                }
            }
        }

        //----------------------CAMERA MOVEMENT----------------------//

        public void cameraGetTarget(Transform cameraTransform)
        {

            if (cameraIsAiming == false)
            {
                // if (Physics.Raycast(ray, out hit, 10000.0f, ground_layer, QueryTriggerInteraction.Collide))
                if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out HitInfo, 10000.0f))
                {
                    if (((HitInfo.point - cameraTarget).sqrMagnitude >= .01f)) //The aimning point is the same, the camera not move
                    {
                        cameraIsAiming = true;
                        cameraTarget = HitInfo.point;
                    }
                }
            }
            else
            {
                Camera.main.transform.LookAt(cameraTarget); //Get the new focal point for the camera
            }

        }

        //Calculate the actual layer where the camera is positionate (Layers from 0 to 2 - Amount maximum of 3)
        public void cameraHeight(Transform cameraTransform)
        {
            float actual_height = cameraTransform.position.y;

            if (Physics.Raycast(cameraTransform.position, -Vector3.up, out HitInfo, 10000.0f, 1 << LayerMask.NameToLayer("Terrain")))
            {
                actual_height = HitInfo.distance;

                if (actual_height <= (float)heights[0])
                {
                    //Debug.Log("HEIGHT 0: " + actual_height);
                    //Debug.Log("HEIGHT 3");

                    actual_level = 3;
                }
                else if ((actual_height <= (float)heights[1]) && (actual_height > (float)heights[0]))
                {
                    //Debug.Log("HEIGHT 2");

                    actual_level = 2;
                }
                else if ((actual_height <= (float)heights[2]) && (actual_height > (float)heights[1]))
                {
                    //Debug.Log("HEIGHT 1");

                    actual_level = 1;
                }
                else
                {
                    Debug.Log("HEIGHT OUT OF BOUNDS!");

                    actual_level = 0;
                }

            }
            else
            {
                Debug.Log("CAMERA OUT OF TERRAIN!");
            }
        }


        //Depend on the camera height, manage the visibility of the squadrons
        //REMEMBER: USE TAG "HIDE" TO SHOWN/HIDE THE DESIRED SKINNED MESHES !!!
        void visibilitySquadron()
        {
            Faction_AI[] factions = FindObjectsOfType(typeof(Faction_AI)) as Faction_AI[]; //Get the AI Squadrons from here


            //This For's are done this way for more easy visibility
            if (actual_level == 1 || actual_level == 2) //In case of LEVEL 2 - HIDE ALL MESHES
            {
                //PLAYER SQUADRONS
                for (int i = 0; i < squadron_controller.num_squadrons; i++)
                {
                    if (squadron_controller.countUnitsSquadron(squadron_controller.squadrons, i) > 0)
                    {
                        squadron_controller.hideSquadron(squadron_controller.squadrons, i); //Hide squadron meshes
                        squadron_controller.showUISquadron(squadron_controller.squadrons, i, true); //Show squadron sprite
                    }
                }

                //AI SQUADRONS
                for (int i = 0; i < factions.Length; i++) //Loop each faction
                {
                    for (int j = 0; j < factions[i].num_squadrons; j++) //Loop each faction
                    {
                        if (squadron_controller.countUnitsSquadron(factions[i].squadrons, j) > 0)
                        {
                            squadron_controller.hideSquadron(factions[i].squadrons, j); //Hide squadron meshes
                            squadron_controller.showUISquadron(factions[i].squadrons, j, true); //Show squadron sprite
                        }
                    }
                   
                }

            }

            else if (actual_level == 3)
            {
                //PLAYER SQUADRONS
                for (int i = 0; i < squadron_controller.num_squadrons; i++)
                {
                    if (squadron_controller.countUnitsSquadron(squadron_controller.squadrons, i) > 0)
                    {
                        squadron_controller.showSquadron(squadron_controller.squadrons, i); //Show squadron meshes
                        squadron_controller.showUISquadron(squadron_controller.squadrons, i, false); //Hide squadron sprite
                    }
                }

                //AI SQUADRONS
                
                for (int i = 0; i < factions.Length; i++) //Loop each faction
                {
                    for (int j = 0; j < factions[i].num_squadrons; j++) //Loop each faction
                    {
                        if(squadron_controller.countUnitsSquadron(factions[i].squadrons, j) > 0)
                        {
                            squadron_controller.showSquadron(factions[i].squadrons, j); //Show squadron meshes
                            squadron_controller.showUISquadron(factions[i].squadrons, j, false); //Hide squadron sprite
                        }
                    }
                }
            }
            else
            {
                Debug.Log("HEIGHT OUT OF BOUNDS!");
            }

        }


        //Updates the FOW plane to always been a 'X' distance from the camera
        public void UpdateFOWPlanePosition(float fow_distance)
        {
            fow_plane.transform.position = new Vector3(fow_plane.transform.position.x, this.gameObject.transform.position.y - fow_distance, fow_plane.transform.position.z); //#TO-DO: The plane position should be updated ONLY when the collision between the map and mountains is visible
        }


        public void cameraDebug(Vector3 vector_f_b, Vector3 vector_l_r)
        {

            Debug.DrawRay(Camera.main.transform.position, Vector3.forward * 10000.0f, Color.red);
            Debug.DrawRay(Camera.main.transform.position, vector_f_b * 10000.0f, Color.blue);
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 10000.0f, Color.yellow);
            Debug.DrawRay(Camera.main.transform.position, vector_l_r * 10000.0f, Color.green);

            Debug.DrawRay(Camera.main.transform.position, -Vector3.up * 10000.0f, Color.magenta); //Camera to terrain

        }


    }
}
