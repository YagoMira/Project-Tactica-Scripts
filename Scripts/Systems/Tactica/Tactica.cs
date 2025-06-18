using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    abstract public class Tactica : MonoBehaviour
    {
        public string tactica_name;
        public string tactica_description;
        public float duration;
        public float cooldown;

        public bool activated = false;

        //Pass the Tactica as parameter on the Tactica System script's functions
        public virtual IEnumerator TacticaAbility(bool enabled) { yield return null; }
    }
}
