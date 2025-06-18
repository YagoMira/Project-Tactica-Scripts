using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class BaseManagementController : MonoBehaviour
    {
        public bool military_base = false; //In case of selection of Military Base
        public int military_base_number = 0;
        public GameObject military_base_object;

        public string[,] soldier_data;


        //Recover the raw data from CSV to manage in the Base Manager
        public void getSoldiersNameData(string[,] textData)
        {
            soldier_data = textData;
        }

        public string getRandomSoldierName()
        {
            string name;
            int random_index;

            random_index = Random.Range(0, soldier_data.GetLength(1));
            name = soldier_data[0, random_index];

            return name;
        }

        public void setMilitaryBase(GameObject m_base)
        {
            military_base_object = m_base;
        }

        public int getMilitaryBaseNumber()
        {
            return military_base_number;
        }

        public void militaryBaseSelected (bool enabled)
        {
            military_base = enabled;
        }
    }
}
