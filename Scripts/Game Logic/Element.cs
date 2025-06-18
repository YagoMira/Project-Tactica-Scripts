using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class Element : MonoBehaviour
    {
        public enum elem_type { Edification, Unit };

        //Type of Element in relationship with the player units
        public enum elem_status { Ally, Neutral, Enemy, Player, NO_VALID } //Use the "Player" for the AI.

        public string name; //Name of the Element
        public string description; //Descript of the Element

        [Space]
        public Factions faction = 0; //ELEMENT Faction (BY DEFAULT == 0)
        public elem_type type;
        public float health = 100.0f;

        public Element(string name, string description, elem_type type, float health)
        {
            this.name = name;
            this.description = description;
            this.type = type;
            this.health = health;
        }

    }
}
