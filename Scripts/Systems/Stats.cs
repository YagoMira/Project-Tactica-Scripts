using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class Stats : MonoBehaviour
    {
        public float damage; //Damage to the enemies
        public float extra_health; //Percentage of extra health (use it on final calculation)
        public float health_recovery; //Health Recovery speed
        public float speed; //Affects to unit movement (Agent)
        public float resistance; //Affects to the amount of received damage (Depends on the unit type, country, ...)
        public float range; //Affects to unit range to shoot (Agent)
        public float reload_time; //Affects to bullet shoot timer
    }
}
