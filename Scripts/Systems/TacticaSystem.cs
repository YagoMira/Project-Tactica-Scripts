using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class TacticaSystem : MonoBehaviour
    {
        //ELEMENTS - UNITS (For search for Almah's and its generals
        public GameObject units;

        //GENERALS 
        public List<string> generals;
        public int num_generals = 1;

        private void Awake()
        {
            //GetGenerals(); #TO-DO: Fix
            generals.Add("PLAYER"); //#TO-DO: Fix
        }

        public void GetGenerals()
        {
            for (int i = 0; i < units.transform.childCount; i++)
            {
                if (units.transform.GetChild(i).gameObject.GetComponent<Almah>() != null)
                {
                    Almah almah = units.transform.GetChild(i).gameObject.GetComponent<Almah>();

                    generals.Add(almah.general_name);
                }

                int child_child_count = units.transform.GetChild(i).childCount;

                for (int j = 0; j < child_child_count; i++)
                {
                    if (units.transform.GetChild(i).GetChild(j).gameObject.GetComponent<Almah>() != null)
                    {
                        Almah almah = units.transform.GetChild(i).GetChild(j).gameObject.GetComponent<Almah>();

                        generals.Add(almah.general_name);
                    }
                }

            }

            num_generals = generals.Count;
        }

        public void ActivateTactica(Tactica tactica)
        {
            tactica.activated = true;
            StartCoroutine(tactica.TacticaAbility(true));
        }

    }
}
