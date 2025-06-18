using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class DialogueManager : MonoBehaviour
    {
        //public string[] dialogue;
        public string[,] dialogue_data;
        public string[] character_names = { "BB", "X", "ALICE", "VARIANT", "ANNA",
            "DRACO", "MALENKI", "NIARBAL", "LUXIAN", "EZILO",
            "JACK", "LUCY", "JULIA", "DEXTER", "KLAUDE",
            "RAINER", "DARDANZA", "NINA", "FINNS", "SACRANTO",
            "AZ", "RYDN", "GRANTT", "MERLIN"}; // 24 CHARACTERS IN TOTAL

   
        //Recover the raw data from CSV to manage in the Dialogue Manager
        public void getDialogueData(string[,] textData)
        {
            dialogue_data = textData;
        }


        //public string getDialogueSpeakerByEvent(int event_id,int line_number)
        //{
        //    int column = 3; //Column 3 equals to the speaker.
        //    string character = ""; //Character 3
        //    string[] dialogue_event = getDialogueDataByEvent(event_id);



        //    return
        //}

        //Get DIALOGUE Data from the CSV file by EVENT ID
        public string[,] getDialogueDataByEvent(int event_id)
        {
            string[,] dialogue_event;
            int lines = 0;

            int first_index = 0; //Index of the first line of the actual event

            //Debug.Log("LENGTH?: " + dialogue_data.GetLength(0) + " , " + dialogue_data.GetLength(1));

            //Debug.Log("PRUEBA: " + dialogue_data[2, 40]);

            //Count the number of lines with the event 0
            for (int r = 0; r < dialogue_data.GetLength(1); r++)
            {
                if (event_id == int.Parse(dialogue_data[0, r]))
                {
                    lines++;

                    if (lines <= 1)
                        first_index = r;
                }
                
            }


            //Set the text into the array with the predefined lines
            dialogue_event = new string[dialogue_data.GetLength(0), lines]; //Lines of dialogue

            //Debug.Log("LINES: " + lines);
            

            int aux_index = first_index;

            //Debug.Log("EVENT: " + event_id + " INDEX: " + first_index);

            for (int c = 0; c < dialogue_data.GetLength(0); c++)
            {
                for (int r = 0; r < lines; r++)
                {
                    if (event_id == int.Parse(dialogue_data[0, first_index]))
                    {
                        //if(c == 2)
                        //    Debug.Log("TEXT-TO-SAVE: " + (string)dialogue_data[c, first_index]);
                        dialogue_event[c, r] = (string)dialogue_data[c, first_index];
                        first_index++;
                    }

                    if(r == lines-1)
                    {
                        first_index = aux_index;
                    }

                        /*
                        if(first_index < (first_index + lines - 1))
                        {

                            if (event_id == int.Parse(dialogue_data[0, r]))
                            {
                               // Debug.Log("COLUMN: " + c + " INDEX: " + aux_index + " TEXT: " + (string)dialogue_data[c, r]);
                                dialogue_event[c, aux_index] = (string)dialogue_data[c, r];
                                aux_index++;
                            }
                        }
                        */

                        //Debug.Log("TEXTO-LEIDO: " + dialogue_event[2, r]);

                    }
            }
           
            return dialogue_event;
        }

        //Get DIALOGUE Data from the CSV file by EVENT - LIBERATION SYSTEM - ID
        public string[,] getDialogueDataByLiberationEvent(int event_id)
        {
            string[,] dialogue_event;
            string l_event_id = "L_" + event_id.ToString();
            int lines = 0;

            int first_index = 0; //Index of the first line of the actual event

            //Count the number of lines with the event 0
            for (int r = 0; r < dialogue_data.GetLength(1); r++)
            {
                if (l_event_id == dialogue_data[0, r])
                {
                    lines++;

                    if (lines <= 1)
                        first_index = r;
                }

            }


            //Set the text into the array with the predefined lines
            dialogue_event = new string[dialogue_data.GetLength(0), lines]; //Lines of dialogue

            int aux_index = first_index;

            for (int c = 0; c < dialogue_data.GetLength(0); c++)
            {
                for (int r = 0; r < lines; r++)
                {
                    if (l_event_id == dialogue_data[0, first_index])
                    {
                        dialogue_event[c, r] = (string)dialogue_data[c, first_index];
                        first_index++;
                    }

                    if (r == lines - 1)
                    {
                        first_index = aux_index;
                    }

                }
            }

            return dialogue_event;
        }

        public int getMaxEvents()
        {
            int max_row = dialogue_data.GetLength(1);
            int max_event = 0;

            max_event = int.Parse(dialogue_data[0, max_row-1]);

            return max_event;
        }


        /*
        //Get DIALOGUE Data from the CSV file by EVENT ID
        public string[] getDialogueDataByEvent(int event_id)
        {
            int column = 2; //Column 2 equals the dialogue text.

            dialogue = new string[dialogue_data.GetLength(1)]; //Lines of dialogue

            for (int r = 0; r < dialogue_data.GetLength(1); r++)
            {
                if (event_id == int.Parse(dialogue_data[0, r]))
                    dialogue[r] = dialogue_data[column, r];
            }
            return dialogue;
        }
        */
    }
}

