using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class UnitC_Selection_Board : MonoBehaviour
    {
        GameController game_controller;
        UIManager ui_manager;

        //GameObjects
        public GameObject selection_pivot;
        public GameObject test_object;

        //UI Elements
        [SerializeField]
        RectTransform boxVisual;
        Rect selectionBox;

        //UI Calculations
        Vector2 startPosition;
        Vector2 endPosition;
        public float fixedSize = 50.0f; //Use this variable as a complement for the collider size

        //Collider for get the Units
        public BoxCollider collider;

        //Know if the box is actually being draw
        public bool is_draw = false;

        private void Start()
        {
            ui_manager = GetComponent<UIManager>();
            game_controller = ui_manager.game_logic.GetComponent<GameController>();
            

            collider = selection_pivot.GetComponent<BoxCollider>();
            //collider.enabled = false; //Collider starts as hidden

            startPosition = Vector2.zero; //Start UI on position 0
            endPosition = Vector2.zero; //Start UI on position 0

            DrawVisual();
        }

        public void DrawVisualInput(bool input_press, bool input, bool input_release, bool unit_list_checker, Vector3 mousePosition) //By default: Left Mouse Click
        {
            // First Action (Def. Left Click)
            if (input_press == true)
            {
                startPosition = mousePosition;

                selectionBox = new Rect();
            }

            // Holding the Action (Def. Left Click)
            if (input == true)
            {
                endPosition = mousePosition;

                DrawVisual();
                DrawSelection(mousePosition);

                is_draw = true;
            }

            // When Releasing the Action (Def. Left Click)
            if (input_release == true)
            {
                if(is_draw == true)
                {
                    SelectUnits(unit_list_checker);

                    is_draw = false;

                    //RESTART THE BOX PANEL TO 0 (NO VISIBLE)
                    startPosition = Vector2.zero;
                    endPosition = Vector2.zero;
                    //RESTART THE BOX PANEL TO 0 (NO VISIBLE)

                    DrawVisual();

                    StartCoroutine(DisableCollider(1.0f)); //After get the collision with the units, hide the collider
                }

            }

        }


        //Calculate the dimensions of the box
        void DrawVisual()
        {
            // Calculate the starting and ending positions of the selection box.
            Vector2 boxStart = startPosition;
            Vector2 boxEnd = endPosition;

            // Calculate the center of the selection box.
            Vector2 boxCenter = (boxStart + boxEnd) / 2;

            // Set the position of the visual selection box based on its center.
            boxVisual.position = boxCenter;

            // Calculate the size of the selection box in both width and height.
            Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

            // Set the size of the visual selection box based on its calculated size.
            boxVisual.sizeDelta = boxSize;
        }


        //Calculate the position of the selection box in relation with the mouse position
        void DrawSelection(Vector3 mousePosition)
        {
            if (Input.mousePosition.x < startPosition.x)
            {
                selectionBox.xMin = mousePosition.x;
                selectionBox.xMax = startPosition.x;
            }
            else
            {
                selectionBox.xMin = startPosition.x;
                selectionBox.xMax = mousePosition.x;
            }


            if (Input.mousePosition.y < startPosition.y)
            {
                selectionBox.yMin = mousePosition.y;
                selectionBox.yMax = startPosition.y;
            }
            else
            {
                selectionBox.yMin = startPosition.y;
                selectionBox.yMax = mousePosition.y;
            }
        }


        void SelectUnits(bool unit_list_checker)
        {
            if(game_controller.camera_controller.actual_level == 3) //#TO-DO: CHECK THE LEVEL 2
            {
                if (unit_list_checker == false && ui_manager.over_ui_g == false) //Normal Left Click (Without Ctrl or Shift) && (Without Mouse Over any UI)
                {
                    game_controller.visualSelection(false); //Unselect all the NPC'S at the moment, because the next line will clean all ones
                    game_controller.units_list = new List<GameObject>(); //Clear the list of the selected units every time 
                }
            }
           

            //Vectors and position for the calculations
            Vector3 point_start = new Vector3();
            Vector3 point_aux = new Vector3();

            Vector3 mousePos = new Vector3(endPosition.x, startPosition.y, 0);
            Vector3 startPos = startPosition;
            Vector3 auxPos = endPosition;


            //Get the Canvas UI mouse position in the World position

            Ray rayStart = Camera.main.ScreenPointToRay(startPos);

            if (Physics.Raycast(rayStart, out RaycastHit hit2))
            {
                point_start = new Vector3(hit2.point.x, 0, hit2.point.z);

            }


            Ray rayAux = Camera.main.ScreenPointToRay(auxPos);

            if (Physics.Raycast(rayAux, out RaycastHit hit3))
            {
                point_aux = new Vector3(hit3.point.x, 0, hit3.point.z);

            }



            //Get a mid point in the line from initial position to the auxiliar one (Auxiliar: Right bottom of the square)
            float midPoint = Vector3.Distance(point_aux, point_start) / 2;
            Vector3 collider_center = midPoint * Vector3.Normalize(point_aux - point_start) + point_start;


            //----------------------DEBUG----------------------//
            //GameObject startPoint;
            //startPoint = (GameObject)Instantiate(test_object, point_start, Quaternion.identity);
            //Instantiate(test_object, point_aux, Quaternion.identity);
            //Instantiate(test_object, collider_center, Quaternion.identity);
            //Debug.Log("CAMERA FORWARD:" + Camera.main.transform.forward.x + " / " + Camera.main.transform.forward.y + " / " + Camera.main.transform.forward.z);
            //----------------------DEBUG----------------------//

            if(!ui_manager.IsPointerOverUIElement()) //Check if any UI element is in the ray of mouse direction
            {
                if (unit_list_checker == false) //Disable the collider for prevent reselection when press CTRL
                    collider.enabled = true; //Enable the collider at this moment
            }
            else //Disable collider if collision with UI element exists
            {
                collider.enabled = false;
            }



            //Check the collider position in relation with the camera orientation

            //if (Camera.main.transform.forward.x > 0 && Camera.main.transform.forward.z > 0) // FACING Z
            //    collider.size = new Vector3(midPoint + fixedSize, 500, midPoint * 2 + fixedSize); //FACING X
            //else if (Camera.main.transform.forward.x < 0 && Camera.main.transform.forward.z > 0)
            //    collider.size = new Vector3(midPoint, 500, midPoint * 2 + fixedSize); //FACING X
            //else if (Camera.main.transform.forward.x > 0 && Camera.main.transform.forward.z < 0)
            //    collider.size = new Vector3(midPoint, 500, midPoint * 2 + fixedSize); //FACING X
            //else
            //    collider.size = new Vector3(midPoint + fixedSize, 500, midPoint); //FACING  X


            if (Camera.main.transform.forward.x > 0 && Camera.main.transform.forward.z > 0) // FACING Z
                collider.size = new Vector3(midPoint + fixedSize, 500, midPoint + fixedSize); //FACING X
            else if (Camera.main.transform.forward.x < 0 && Camera.main.transform.forward.z > 0)
                collider.size = new Vector3(midPoint, 500, midPoint + fixedSize); //FACING X
            else if (Camera.main.transform.forward.x > 0 && Camera.main.transform.forward.z < 0)
                collider.size = new Vector3(midPoint, 500, midPoint + fixedSize); //FACING X
            else
                collider.size = new Vector3(midPoint + fixedSize, 500, midPoint); //FACING  X

            //Collider properties
            collider.center = collider_center; //Center of the collider;
            collider.isTrigger = true;

        }


        //Hide the collider after get all the collisions with the units
        IEnumerator DisableCollider(float time)
        {
            //Wait for the specified delay
            yield return new WaitForSeconds(time);

            //De the desired action after the timer
            collider.enabled = false;
        }



    }

}
