using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ADAPT
{
    //Inventory for actual states of the agent.
    public class AgentStates : MonoBehaviour
    {
        public List<string> worldElements = new List<string>();
        public List<string> positions = new List<string>();
        //public Dictionary<string, float> inventory = new Dictionary<string, float>();
        //public Dictionary<string, bool> status = new Dictionary<string, bool>();

        [Serializable]
        public class Inventory_Pair
        {
            public string inventory_key;
            public float inventory_value;

            public Inventory_Pair(string inventory_key, float inventory_value)
            {
                this.inventory_key = inventory_key;
                this.inventory_value = inventory_value;
            }
        }

        [Serializable]
        public class Status_Pair
        {
            public string status_key;
            public bool status_value;

            public Status_Pair(string status_key, bool status_value)
            {
                this.status_key = status_key;
                this.status_value = status_value;
            }
        }

        public List<Inventory_Pair> inventory_list = new List<Inventory_Pair>();
        public Dictionary<string, float> inventory = new Dictionary<string, float>();

        public List<Status_Pair> status_list = new List<Status_Pair>();
        public Dictionary<string, bool> status = new Dictionary<string, bool>();

        //Update all the dictionary and lists to show in inspector -- //#TO-DO: REVIEW!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public void UpdateStatesLists()
        {

            //ADD DATA TO DICTIONARY - THE USER CAN ADD DATA VIA INSPECTOR
            foreach (var pair in inventory_list)
            {
                if (!inventory.ContainsKey(pair.inventory_key))
                    inventory[pair.inventory_key] = pair.inventory_value;
            }

            foreach (var pair in status_list)
            {

                if (!status.ContainsKey(pair.status_key))
                    status[pair.status_key] = pair.status_value;
            }


            //ADD DATA TO LISTS - VISUALIZATE IN INSPECTOR
            foreach (var pair in inventory)
            {
                bool key_exists = false;
                int index = -1;

                foreach (var p in inventory_list)
                {
                    index++;

                    if (p.inventory_key == pair.Key)
                        key_exists = true;
                }

                if (key_exists != true)
                {
                    inventory_list.Add(new Inventory_Pair(pair.Key, pair.Value));
                }
                else
                {
                    inventory_list[index].inventory_value = pair.Value;
                }
            }

            foreach (var pair in status)
            {
                bool key_exists = false;
                int index = -1;

                foreach(var p in status_list)
                {
                    index++;

                    if (p.status_key == pair.Key)
                        key_exists = true;
                }

                if(key_exists != true)
                {
                    status_list.Add(new Status_Pair(pair.Key, pair.Value));
                }
                else
                {
                    status_list[index].status_value = pair.Value;
                }
            }


        }


        public void AddWorldItem(string name) //Add new WorldElement Item to the actual inventory collection.
        {
            worldElements.Add(name);
        }

        public void AddPositionItem(string name) //Add new Position Item to the actual inventory collection.
        {
            positions.Add(name);
        }

        public void AddInventoryItem(string name, float initialValue) //Add new Inventory Item to the actual inventory collection.
        {
            inventory.Add(name, initialValue);
        }

        public void AddStatusItem(string name, bool initialValue) //Add new Status Item to the actual status collection.
        {
            status.Add(name, initialValue);
        }

        public void RemoveWorldItem(string name) //Remove WorldElement Item from the actual inventory collection.
        {
            worldElements.Remove(name);
        }

        public void RemovePositionItem(string name) //Remove Position Item from the actual inventory collection.
        {
            positions.Remove(name);
        }

        public void RemoveInventoryItem(string name) //Remove Inventory Item from the actual inventory collection.
        {
            inventory.Remove(name);
        }

        public void RemoveStatusItem(string name) //Remove  Status Item from the actual status collection.
        {
            status.Remove(name);
        }

        public void ModifyInventoryItem(string name, float newValue) //Modify an actual inventory item of the collection.
        {
            if (inventory.ContainsKey(name))
            {
                inventory[name] = newValue;
            }

        }

        public void IncreaseInventoryItem(string name, float newValue) //Modify an actual inventory item of the collection (Increase Value).
        {
            if (inventory.ContainsKey(name))
            {
                inventory[name] += newValue;
            }

        }

        public void DecreaseInventoryItem(string name, float newValue) //Modify an actual inventory item of the collection (Decrease Value).
        {
            if (inventory.ContainsKey(name))
            {
                inventory[name] -= newValue;
            }

        }

        public void ModifyStatusItem(string name, bool newValue) //Modify an actual status item of the collection.
        {
            if (status.ContainsKey(name))
            {
                status[name] = newValue;
            }
        }

    }
}
