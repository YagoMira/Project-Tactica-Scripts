using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class UnitsBounds : MonoBehaviour
    {
        public List<Unit> units_inside;
        public bool[] squadrons_inside;

        GameController game_controller;

        Factions base_faction = 0;

        private void Start()
        {
            game_controller = GameObject.Find("Game Logic").GetComponent<GameController>();

            squadrons_inside = new bool[game_controller.squadron_controller.num_squadrons];

            base_faction = this.gameObject.transform.parent.gameObject.GetComponent<Element>().faction;
        }

        private void Update()
        {
            if(squadrons_inside.Length <= 0)
            {
                squadrons_inside = new bool[game_controller.squadron_controller.num_units];
            }


            /*
            for (int i = 0; i < game_controller.squadron_controller.num_units; i++)
            {
                squadrons_inside[i] = false;

                if (units_inside.Count > 0)
                {
                    if (units_inside[i] != null)
                    {
                        SetSquadronsInside(units_inside[i]);
                    }
                }

            }
            */

        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.GetComponent<Unit>() != null)
            {
                Unit unit = other.gameObject.GetComponent<Unit>();

                units_inside.RemoveAll(u => u == null);

                if (unit.faction == base_faction)
                {
                    if (!units_inside.Contains(unit))
                    {
                        units_inside.Add(unit);
                        SetSquadronsInside(unit, false);
                    }
                        
                }
                    
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponent<Unit>() != null)
            {
                Unit unit = other.gameObject.GetComponent<Unit>();

                SetSquadronsInside(unit, true);

                if (units_inside.Count > 0)
                {
                    units_inside.Remove(unit);
                }
            }
        }

        public int CountAvaliableSquadrons()
        {
            int available_squadrons = 0;

            for (int i = 0; i < squadrons_inside.Length; i++)
            {
                if (squadrons_inside[i] == true)
                    available_squadrons++;
            }

            return available_squadrons;
        }

        public void SetSquadronsInside(Unit unit, bool exit)
        {
            if (unit != null)
            {
                int num_squad;

                if (exit == false)
                {
                    num_squad = game_controller.squadron_controller.getSquadron(game_controller.squadron_controller.squadrons, unit.gameObject);
                    squadrons_inside[num_squad] = true;
                }
                else
                {
                    int aux_unit_squad = -1;

                    num_squad = game_controller.squadron_controller.getSquadron(game_controller.squadron_controller.squadrons, unit.gameObject);

                    //Check if exits any other unit of the same squadron inside
                    for(int i = 0; i < units_inside.Count; i++)
                    {
                        aux_unit_squad = game_controller.squadron_controller.getSquadronIndex(game_controller.squadron_controller.squadrons, units_inside[i].gameObject);

                        if(num_squad == aux_unit_squad) //If exist a unit of the same squadron inside
                        {
                            squadrons_inside[num_squad] = true;
                        }
                        else
                        {
                            if(i == (units_inside.Count - 1))
                                squadrons_inside[num_squad] = false;
                        }
                    }
                    
                }
            }

        }
    }

}

