using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class Input_Logic : MonoBehaviour
    {
        public bool disable_input = false; //Disable or enable the input

        //----------------------MOUSE CONTROLS----------------------//
        public bool mouse_left_click = false; //Press "Left Click" of the mouse
        public bool mouse_left_click_hold = false; //Press "Left Click HOLDED" of the mouse
        public bool mouse_left_click_press = false; //Only check the "Press Left Click" of the mouse
        public bool mouse_left_click_release = false; //Only check the "Release Left Click" of the mouse
        public bool mouse_right_click = false; //Press "Right Click" of the mouse
        public bool mouse_right_click_press = false; //Only check the "Press Right Click" of the mouse
        public bool mouse_right_click_release = false; //Only check the "Release Right Click" of the mouse



        //MOUSE AXIS(FIRSTPERSON_CONTROLLER)
        //Horizontal and Vertical
        public float horizontal;
        public float vertical;
        //X and Y
        public float mouseX = 0.0f;
        public float mouseY = 0.0f;

        public Vector3 mousePosition;
        public float mouseScroll; //Get the amount of mouse scroll - middle button of mouse
        public float initial_mouse_scroll; //Get the initial amount of the mouse scroll axis - middle button of mouse
        //----------------------MOUSE CONTROLS----------------------//




        //----------------------KEY CONTROLS----------------------//
        public bool f_press = false; //Press "F Key"
        public bool f5_press = false; //Press "F5 Key"
        public bool space_press_up = false; //Press "Space Bar"
        public bool space_press_down = false; //Press "Space Bar"
        public bool ctrl_left_click = false; //Press "Ctrl Left"
        public bool shift_left_click = false; //Press "Shift Left"
        public bool esc_press = false; //Press "ESC"
        //----------------------KEY CONTROLS----------------------//




        //----------------------CAMERA CONTROLS----------------------//

        public bool camera_input_up = false;       //Press "W" to move camera up.
        public bool camera_input_down = false;     //Press "S" to move camera down.
        public bool camera_input_right = false;    //Press "D" to move camera right.
        public bool camera_input_left = false;     //Press "A" to move camera left.


        public bool camera_input_rotate_right = false;     //Press "E" to move rotate the camera to right.
        public bool camera_input_rotate_left = false;       //Press "Q" to move rotate the camera to left.


        public bool camera_input_rotate_middle = false;     //Check the "Middle Mouse Button" to move rotate the camera free.
        public bool camera_input_rotate_middle_press = false;       //Press of the "Middle Mouse Button" 
        public bool camera_input_rotate_middle_release = false;       //Release of the "Middle Mouse Button" 

        //*TO-DO: Maybe in the future allow change the input keys?

        //----------------------CAMERA CONTROLS----------------------//

        // Start is called before the first frame update
        void Start()
        {
            initial_mouse_scroll = Input.GetAxis("Mouse ScrollWheel");
        }

        // Update is called once per frame
        void Update()
        {
            if (disable_input == false)
            {
                //Get the input of the mouse
                mouse_left_click = Input.GetMouseButton(0); //"0" IS LEFT
                mouse_left_click_press = Input.GetMouseButtonDown(0); //IS LEFT PRESS
                mouse_left_click_release = Input.GetMouseButtonUp(0); //IS LEFT RELEASE
                mouse_right_click = Input.GetMouseButton(1); //"1" IS RIGHT
                mouse_right_click_press = Input.GetMouseButtonDown(1); //IS RIGHT PRESS
                mouse_right_click_release = Input.GetMouseButtonUp(1); //IS RIGHT RELEASE
                mousePosition = Input.mousePosition;

                //Get the input of the mouse axis
                horizontal = Input.GetAxisRaw("Horizontal");
                vertical = Input.GetAxisRaw("Vertical");
                mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime;
                mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime;

                //Get the input of specific keys
                f_press = Input.GetKeyDown(KeyCode.F); //F Key
                f5_press = Input.GetKeyDown(KeyCode.F5); //F Key
                space_press_up = Input.GetKeyUp(KeyCode.Space); //Space Bar Key
                space_press_down = Input.GetKeyDown(KeyCode.Space); //Space Bar Key
                ctrl_left_click = Input.GetKey(KeyCode.LeftControl); //Left Ctrl
                shift_left_click = Input.GetKey(KeyCode.LeftShift); //Left Shift
                esc_press = Input.GetKey(KeyCode.Escape);//Escape

                //Get the input of the mouse's middle button
                mouseScroll = Input.GetAxis("Mouse ScrollWheel");


                //Get the keys in relation with the assigned controls
                camera_input_up = Input.GetKey(KeyCode.W); //*TO-DO: KEYCODE SHOULD BE CHANGED
                camera_input_down = Input.GetKey(KeyCode.S);
                camera_input_right = Input.GetKey(KeyCode.D);
                camera_input_left = Input.GetKey(KeyCode.A);

                camera_input_rotate_right = Input.GetKey(KeyCode.E);
                camera_input_rotate_left = Input.GetKey(KeyCode.Q);


                camera_input_rotate_middle = Input.GetMouseButton(2);
                camera_input_rotate_middle_press = Input.GetMouseButtonDown(2);
                camera_input_rotate_middle_release = Input.GetMouseButtonUp(2);

                CheckLeftMouseHold();
            }
        }

        public void CheckLeftMouseHold()
        {

            if (mouse_left_click_hold != true && mouse_left_click == true)
                StartCoroutine(LeftMouseHold()); // Check LEFT MOUSE HOLD

            if (mouse_left_click == false)
                mouse_left_click_hold = false;
        }

        IEnumerator LeftMouseHold()
        {
            yield return new WaitForSeconds(0.15f);

            if (mouse_left_click == true)
            {
                mouse_left_click_hold = true;
            }
            else
            {
                mouse_left_click_hold = false;
            }
        }

    }
}
