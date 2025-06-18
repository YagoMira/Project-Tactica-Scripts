using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Project_Tactica
{
    public class MilitaryBase : Element
    {
        public int military_base_number; //Number for identificate the different bases
        public GameObject spawn_point; //Point in the world to spawn the squadrons/units
        public GameObject spawn_move; //Point in the world to move the spawned  squadrons/units

        //OTHER SYSTEMS
        public GameObject game_logic_object;
        GameController game_controller;
        public BaseManagementController base_managament_controller;

        //ONLY FOR TESTS
        public GameObject[] units; //0 - SOLDIER, 1 - TANK

        public void Awake()
        {
            game_logic_object = GameObject.Find("Game Logic");
            game_controller = game_logic_object.GetComponent<GameController>();
            base_managament_controller = game_logic_object.GetComponent<BaseManagementController>();
        }

        public MilitaryBase(string name, string description, elem_type type, float health) : base(name, description, type, health)
        {
            this.type = elem_type.Edification;
        }

        public void SpawnSquadron(int militaryBase)
        {
            int unitTestSize = 3; //Amount of units to the squadron (ONLY FOR TESTS!)
            //Add the new units to the selection list
            for (int i = 0; i < unitTestSize; i++)
            {
                //game_controller.units_list.Add(Instantiate(squadron, spawn_point.transform.position, Quaternion.identity));
                GameObject new_unit = Instantiate(units[0], spawn_point.transform.position, Quaternion.identity);
                string name = base_managament_controller.getRandomSoldierName();
                new_unit.GetComponent<Unit>().name = name;
                new_unit.gameObject.name = name;
                new_unit.GetComponent<Unit>().updateName(new_unit.GetComponent<Unit>().name);
                game_controller.units_list.Add(new_unit);
            }

            game_controller.squadron_controller.createPlayerSquadron(0);

            //APPLY THE DESIRED OPERATIONS TO THE RESOURCES SYSTEM:
            float credits_value = game_controller.resources_controller.getCredits();
            game_controller.resources_controller.setCredits(credits_value - 300);
        }

        public int SelectSpawnableUnit(string unit_name)
        {
            switch (unit_name)
            {
                case "Tank":
                    return 1;
                case "Soldier":
                    return 0;
                default:
                    return 0;
            }
        }

        public void SpawnUnit(string unit_name, float unit_credit, int num_squadron)
        {
            
            GameObject new_unit = Instantiate(units[SelectSpawnableUnit(unit_name)], spawn_point.transform.position, Quaternion.identity);

            string name = base_managament_controller.getRandomSoldierName();
            new_unit.GetComponent<Unit>().name = name;
            new_unit.gameObject.name = name;
            new_unit.GetComponent<Unit>().updateName(new_unit.GetComponent<Unit>().name);
            
            //Need if for apply the TACTICA effects
            GameObject UNITS = GameObject.Find("UNITS");
            new_unit.transform.SetParent(UNITS.transform);

            game_controller.squadron_controller.addUnitToSquadron(num_squadron, new_unit);

            //APPLY THE DESIRED OPERATIONS TO THE RESOURCES SYSTEM:
            game_controller.resources_controller.decreaseCredits(unit_credit);

            StartCoroutine(moveNewUnit(new_unit));
            
        }

        IEnumerator moveNewUnit(GameObject unit)
        {
            yield return new WaitForSeconds(0.25f);

            //Debug.Log(unit.gameObject.GetComponent<NavMeshAgent>().speed);
           
            Unit unit_instantiated = unit.gameObject.GetComponent<Unit>();
            Vector3 new_spawn_position = new Vector3(spawn_move.transform.position.x + ((float)Random.Range(0, 10)), spawn_move.transform.position.y, spawn_move.transform.position.z + ((float)Random.Range(0, 10)));

            unit_instantiated.GetComponent<NavMeshAgent>().stoppingDistance = ((float)Random.Range(5, 15));
            unit.gameObject.GetComponent<NavMeshAgent>().SetDestination(new_spawn_position);

            unit_instantiated.selected_enemy = null; //Remove the selected enemy from attack action
            unit_instantiated.onAttack = false;

            //unit_instantiated.setUnitRange(0, true); //True in this case because is a movement to spawn point

            unit_instantiated.onMove = true;
            unit_instantiated.animator.SetBool("onMove", true); //Move Animation
        }


        private void OnMouseOver()
        {
            if (game_controller.ui_manager.over_ui_g == false)
            {
                if (game_controller.input.mouse_left_click_press)
                {
                    base_managament_controller.setMilitaryBase(this.gameObject);
                    base_managament_controller.military_base = true;
                }

                if (this.gameObject.GetComponent<Outline>() != null)
                {
                    this.gameObject.GetComponent<Outline>().OutlineWidth = 5.0f;

                    this.gameObject.transform.Find("Bounds").GetChild(0).gameObject.SetActive(true);
                }
            }

        }

        private void OnMouseExit()
        {
            if (this.gameObject.GetComponent<Outline>() != null)
            {
                this.gameObject.GetComponent<Outline>().OutlineWidth = 0.0f;

                this.gameObject.transform.Find("Bounds").GetChild(0).gameObject.SetActive(false);
            }
        }

    }
}
