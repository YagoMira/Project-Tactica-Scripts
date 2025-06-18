using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_Tactica;

public class FirstPerson_Contoller : MonoBehaviour
{
    //PLAYER PROPERTIES
    [Header("Player Movement")]
    public float speed;
    public float drag;
    public bool onGround;

    [Header("Terrain")]
    public LayerMask terrain_layer;

    public Transform orientation;
    Vector3 direction;
    Rigidbody rb;

    //----------------------CAMERA PROPERTIES----------------------//
    string camera_holder_str = "Camera_Holder";

    public Transform camera_pivot;
    public GameObject camera_holder;
    [HideInInspector]
    public GameObject camera;

    public float sensibility_X = 400.0f;
    public float sensibility_Y = 400.0f;

    public float xRotation;
    public float yRotation;

    public AudioSource footsteps;
    //----------------------CAMERA PROPERTIES----------------------//


    //INTERACT SYSTEM
    public float interact_range = 3.0f;
    Item currentItem;




    //Other Systems
    public Input_Logic input;

    private void Start()
    {
        input = this.gameObject.GetComponent<Input_Logic>();

        //Rigidbody
        rb = this.transform.Find("Body").gameObject.GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //CAMERA PROPERTIES
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Get the necessary transforms for the Camera
        //camera_holder = this.transform.Find(camera_holder_str).gameObject;
        camera = camera_holder.transform.GetChild(0).gameObject; //The first and UNIQUE child is the Camera
        camera.GetComponent<Camera>().fieldOfView = DataSaver.fov;
    }

    private void Update()
    {
        float height = 2.0f; //Need it for the raycast

        //Check onGround bool
        onGround = Physics.Raycast(transform.position, Vector3.down, height * 0.5f + 0.3f, terrain_layer);

        //Rigidbody Drag
        if (onGround)
            rb.drag = drag;
        else
            rb.drag = 0;

        //CAMERA
        UpdateCameraControls();
        UpdateCameraPosition();

        //Player Speed
        SpeedLimit();


        //INTERACT SYSTEM
        onInteraction();

        //Sound
        PlayFootsteps();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }


    //----------------------PLAYER PROPERTIES----------------------//
    private void MovePlayer()
    {
        //Calculate the direction vector
        direction = orientation.forward * input.vertical + orientation.right * input.horizontal;

        // Check bool onGround
        if (onGround)
        {
            rb.AddForce(direction.normalized * speed * 10f, ForceMode.Force);
        }
           
    }

    public void PlayFootsteps()
    {
        if ((input.vertical != 0) || (input.horizontal != 0))
        {
            if (footsteps.isPlaying == false)
            {
                StartCoroutine(PlayFootstepsSound());
            }
        }
    }


    private IEnumerator PlayFootstepsSound()
    {
        footsteps.Play();
        yield return new WaitForSeconds(0.5f);
    }

    private void SpeedLimit()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Velocity Limit
        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }


    //----------------------CAMERA PROPERTIES----------------------//
    public void UpdateCameraControls()
    {

        //Get the input values
        yRotation += (input.mouseX * sensibility_X);
        xRotation -= (input.mouseY * sensibility_Y);
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Rotate camera and orientation
        camera.transform.rotation = Quaternion.Euler(xRotation, yRotation + 180.0f, 0); //+180 because the initial 0 angle!
        orientation.rotation = Quaternion.Euler(0, yRotation + 180.0f, 0);


    }

    public void UpdateCameraPosition()
    {
        //Set Camera Holder position into Camera Pivot position
        //camera_holder.transform.position = camera_pivot.transform.position;
    }


    //----------------------INTERACTABLE SYSTEM----------------------//
    public void onInteraction()
    {
        if((input.f_press) && currentItem != null)
        {
            currentItem.Interact();

            if(currentItem != null)
                AudioManager.instance.PlayClip(currentItem.audio_clip);
        }

        RaycastHit hit;
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        //On collision
        if(Physics.Raycast(ray, out hit, interact_range))
        {
            if(hit.collider.tag == "Item")
            {
                Item newItem = hit.collider.GetComponent<Item>();

                //Check for current Item
                if(currentItem && newItem != currentItem)
                {
                    currentItem.ManageOutline(false);
                }

                if(newItem != null)
                {
                    if (newItem.enabled)
                    {
                        ManageItem(newItem);
                    }
                    else
                    {
                        ManageItem(null);
                    }
                }
            }
            else
            {
                ManageItem(null);
            }
        }
        else
        {
            ManageItem(null);
        }
    }

    public void ManageItem(Item newItem)
    {
        if(newItem == null)
        {
            if (currentItem) //Check if currentItem is not null
            {
                currentItem.ManageOutline(false);
                currentItem = null;
            }

            FirstPerson_UI.ui_instance.ManageInteractText(false);
        }
        else
        {
            currentItem = newItem;
            newItem.ManageOutline(true);

            //Display the UI
            FirstPerson_UI.ui_instance.ManageInteractText(true);
        }
    }

    //----------------------INTERACTABLE SYSTEM----------------------//

}