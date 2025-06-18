using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_Tactica;
using static Project_Tactica.Element;

namespace Project_Tactica
{
    /*  
         --FACTIONS-- (11)
        VIRION
        LORENGARD
        BALCANIA
        AMARANTO WEST
        AMARANTO EAST
        NEA
        PRIMOHRA
        CORPORATION AEGEA
        SHIMA EMPIRE
        KINGDOM OF AVALON
        WHITE WOLVES
    */


    /*   --RELATIONS-- (TAKE CARE, THE RELATION SHOULD BE THE SAME (0, 10) OR (10, 0) "Referencing the positions of the Factions on array...") !!!!!!

                                    VIRION          LORENGARD           BALCANIA            ...
         VIRION                        X             NEUTRAL              ENEMY
        LORENGARD                   NEUTRAL             X                 ALLY
        BALCANIA                     ENEMY            ALLY                  X
        ...


     */

    public enum Factions : int
    {
        VIRION = 0, LORENGARD = 1, BALCANIA = 2, AMARANTO_WEST = 3, AMARANTO_EAST = 4, NEA = 5,
        PRIMOHRA = 6, CORPORATION_AEGEA = 7, SHIMA_EMPIRE = 8, KINGDOM_OF_AVALON = 9, WHITE_WOLVES = 10
    } //(int)

    public class SocialSystem : MonoBehaviour
    {
        public Factions player_faction = Factions.VIRION;  //!!<3 WHITE_WOLVES <3 //SET THE START FACTION OF THE PLAYER

        public int num_factions = -1; //11 FACTIONS
        public elem_status[,] relations; //2D Array of 11 x 11

        public int general_deaths = 3; //GENERAL DEATHS (SOCIAL/CSV RELATIONSHIPs)


        private void Awake()
        {
            //Initialization
            num_factions = System.Enum.GetValues(typeof(Factions)).Length;

            if(num_factions != -1)
            {
                relations = new elem_status[num_factions, num_factions]; 
            }

            //Initialize array as all neutral (Except same Factions => Ally of itself)
            for(int i = 0; i < num_factions; i++)
            {
                for (int j = 0; j < num_factions; j++)
                {
                    if(i == j) //SAME FACTION => ALLY
                    {
                        relations[i, j] = elem_status.Ally;
                    }
                    else //DIFERENT FACTIONS
                    {
                        relations[i, j] = elem_status.Neutral;
                    }
                    
                }
            }


            SetRelationships();
        }


        //Set all the desired Relationships of the Factions AT THE START
        public void SetRelationships()
        {
            ModifyRelationBetween((int)Factions.WHITE_WOLVES, (int)Factions.VIRION, elem_status.Enemy);
        }

        //Get the relationship between two factions
        public elem_status GetRelationBetween(int faction_A, int faction_B)
        {
            elem_status actual_relation = elem_status.NO_VALID;

            if (faction_A != faction_B) //NOT THE SAME FACTION
            {
                if(relations[faction_A, faction_B] == relations[faction_B, faction_A])
                {
                    actual_relation = relations[faction_A, faction_B];
                }
                else
                {
                    Debug.LogError("The relationship is NOT bilateral: " + ((Factions)faction_A).ToString());
                }
            }
            //DON'T NEED IT AT THE MOMENT: Continous update on "onMove" (Click Button Action)
            //else //Otherwise
            //{
            //    Debug.LogError("You're trying to GET a relation on the same Faction: " + ((Factions)faction_A).ToString());
            //}

            return actual_relation;
        }

        //Modify the relationship between two factions
        public void ModifyRelationBetween(int faction_A, int faction_B, elem_status new_relation)
        {
            if(faction_A != faction_B) //NOT THE SAME FACTION
            {
                relations[faction_A, faction_B] = new_relation; 
                relations[faction_B, faction_A] = new_relation; //Should be done in both ways
            }
            else //Otherwise
            {
                Debug.LogError("You're trying to assing a relation on the same Faction: " + ((Factions)faction_A).ToString());
            }
        }

        //Modify the relation of ONE faction with ALL the others
        public void ModifyRelationAll(int faction, elem_status new_relation)
        {
            for (int i = 0; i < num_factions; i++)
            {
                for (int j = 0; j < num_factions; j++)
                {
                    if (i != j) //If not the same faction
                    {
                        relations[i, j] = new_relation;
                    }
                }
            }
        }

    }
}
