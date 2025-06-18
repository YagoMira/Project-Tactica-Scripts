using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using VRM;

namespace Project_Tactica
{
    public class Dialogue_Menu : MonoBehaviour
    {
        public DialogueManager dialogue_manager;
        public TMP_Text uiText_classic;
        public TMP_Text uiText_liberate;
        public TMP_Text uiText_liberate_speaker;
        public string[,] dialogueData;
        public string[] textData;

        public float charPrintDelay = 0.1f;
        public bool isPrinting = false;

        public int max_lines = 6;
        public int counted_lines = 0;
        public int global_counted_lines = 0;
        public int readed_lines = 0;

        public int actual_event = 0;
        public bool liberation_event = false; //#TO-DO: Implement getDialogueDataByLiberationEvent

        public Queue<string> sentences;

        string portraits_parent_name = "Character Menu";
        string characters_parent_name = "Characters";
        string character_positions_parent_name = "Characters_Positions";
        string menu_type_parent_classic_name = "Dialogue_Classic";
        string menu_type_parent_liberate_name = "Dialogue_LVRMiberate";

        string cutscene_str = "CUTSCENE";
        string transition_str = "TRANSITION";
        bool isCutscene = false;

        //public GameObject[] characterPortraits;
        public GameObject[] character_positions;
        public string[] character_occupied_positions;

        //OTHER SYSTEMS
        public bool liberate_system_activated = false;
        public GameObject liberate_character_speaker;
        public VRMBlendShapeProxy mouth_manipulator;

        private void Start()
        {
            sentences = new Queue<string>();

            if(dialogue_manager == null)
                this.GetComponentInParent<DialogueManager>();

            //GetCharacterPortraits()
            if(liberate_system_activated == false)
            {
                GetCharacterPositions();
            }
            else
            {
                if(liberate_character_speaker != null)
                    mouth_manipulator = liberate_character_speaker.GetComponent<VRMBlendShapeProxy>();
            }

            StartCoroutine("DelayStart", 0.25f);
        }

        //Activate UI depend on the Liberate System activation or not
        public void activateDialogueType(bool liberate_system_activated)
        {
            if(liberate_system_activated == false)
            {
                this.transform.Find(menu_type_parent_classic_name).gameObject.SetActive(true);
                this.transform.Find(menu_type_parent_liberate_name).gameObject.SetActive(false);
            }
            else
            {
                this.transform.Find(menu_type_parent_liberate_name).gameObject.SetActive(true);
                this.transform.Find(menu_type_parent_classic_name).gameObject.SetActive(false);
                max_lines = 1;
            }
        }

        //Get all character UI Dialogue positions
        public void GetCharacterPositions()
        {
            Transform positions_parent_object = (this.transform.Find(menu_type_parent_classic_name).transform.Find(character_positions_parent_name));

            int childs = positions_parent_object.childCount;
            character_positions = new GameObject[childs];
            character_occupied_positions = new string[childs];

            for (int i = 0; i < childs; ++i)
            {
                character_positions[i] = positions_parent_object.GetChild(i).gameObject;
                character_occupied_positions[i] = "";
            }

        }

        //Get character portrait
        public GameObject GetCharacter(string character_name)
        {
            Transform characters_parent_object = (this.transform.Find(menu_type_parent_classic_name).transform.Find(characters_parent_name));

            int characters = characters_parent_object.childCount;

            GameObject portrait = null;

            if (characters > 0)
            {
                for (int i = 0; i < characters; i++)
                {
                    if (characters_parent_object.GetChild(i).name == character_name)
                        portrait = characters_parent_object.GetChild(i).gameObject;
                }
            }

            return portrait;
        }


        public void PlaySilenceAnimation(GameObject character)
        {
            /*
            Animator animator = character.transform.GetChild(0).gameObject.GetComponent<Animator>();
            bool activate = animator.GetBool("speak");

            //Debug.Log("ANIMATO?: " + animator);
            Debug.Log("ACTIVATE: " + activate);


            if (activate == true)
            {
                animator.SetBool("speak", false);
            }
            else
            {
                animator.SetBool("speak", true);
            }
            */

            //Hide all characters except the actual one
            Transform characters_parent_object = (this.transform.Find(menu_type_parent_classic_name).transform.Find(characters_parent_name));
            
            int characters = characters_parent_object.childCount;

            if (characters > 0)
            {
                for (int i = 0; i < characters; i++)
                {
                    if(characters_parent_object.GetChild(i).gameObject.activeSelf == true)
                    {
                        Animator animator = characters_parent_object.GetChild(i).transform.GetChild(0).gameObject.GetComponent<Animator>();
                        bool activate = animator.GetBool("speak");

                        if (characters_parent_object.GetChild(i).transform.GetChild(0).gameObject.name == character.name) //ACTUAL CHARACTER
                        {
                            if (activate == true)
                            {
                                animator.SetBool("speak", false);
                            }
                            else
                            {
                                animator.SetBool("speak", true);
                            }
                        }
                        else
                        {
                            animator.SetBool("speak", false);
                        }  

                    }
                }
            }
        }

        public void DeactivateAllCharacters()
        {
            Transform characters_parent_object = (this.transform.Find(menu_type_parent_classic_name).transform.Find(characters_parent_name));

            int characters = characters_parent_object.childCount;

            if (characters > 0)
            {
                for (int i = 0; i < characters; i++)
                {
                    if(characters_parent_object.GetChild(i).gameObject.activeSelf == true)
                    {
                        //characters_parent_object.GetChild(i).gameObject.SetActive(false);
                        //Play the hide speaker animation

                        Animator animator = characters_parent_object.GetChild(i).gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>();
                        bool activate = animator.GetBool("speak");

                        animator.SetBool("speak", false);

                    }
                   
                }
            }

        }

        public void HideAllCharacters()
        {
            Transform characters_parent_object = (this.transform.Find(menu_type_parent_classic_name).transform.Find(characters_parent_name));

            int characters = characters_parent_object.childCount;

            if (characters > 0)
            {
                for (int i = 0; i < characters; i++)
                {
                    characters_parent_object.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        /*
        //Get all the portraits from the CHARACTER MENU (The characters will be identified by ID, the id reference can be seen in the csv file)
        public void GetCharacterPortraits()
        {
            Transform portraits_parent_object = (this.transform.Find(menu_type_parent_classic_name).transform.Find(portraits_parent_name));

            int childs = portraits_parent_object.childCount;
            characterPortraits = new GameObject[childs];

            for (int i = 0; i < childs; ++i)
            {
                characterPortraits[i] = portraits_parent_object.GetChild(i).gameObject;
            }
                
        }

        //Get the portrait gameobject by the character id
        public GameObject GetCharacterPortrait(int character_id)
        {
            GameObject portrait = null;

            if(characterPortraits.Length > 0)posi
            {
                for(int i = 0; i < characterPortraits.Length; i++)
                {
                    if (int.Parse((characterPortraits[i].name).Split('_')[1]) == character_id)
                        portrait = characterPortraits[i];
                }
            }

            return portrait;
        }
       

        public void CharacterIsSpeaking(GameObject speaker)
        {
            if(characterPortraits.Length > 0)
            {
                foreach(GameObject character in characterPortraits)
                {
                    if(character.name == speaker.name)
                        character.SetActive(true);
                    else
                        character.SetActive(false);
                }
                
            }
        }
        */

        //UTILS - DELAY START
        public IEnumerator DelayStart(float time)
        {
            yield return new WaitForSeconds(time);

            //EventManager.event_manager_instance.ManageEvents(actual_event, false);
            EventManager.event_manager_instance.ManageEvents(actual_event);

            if (actual_event <= dialogue_manager.getMaxEvents())
            {
                //Debug.Log("EVENTO: " + actual_event);
                StartEvent(actual_event);
                StartDialogue(textData);
            }
        }

        private void Update()
        { 
            if (Input.GetKeyDown(KeyCode.Space)) //KEYCODE.SPACE SHOULD BE CHANGED!!!
            {
                //if (isPrinting == false)
                //    StartDialogue(textData);
                //else
                charPrintDelay = 0.01f;
            }

            if (Input.GetKeyUp(KeyCode.Space)) //KEYCODE.SPACE SHOULD BE CHANGED!!!
            {
                charPrintDelay = 0.05f;
            }
        }

        public void StartEvent(int event_id)
        {
            if(liberate_system_activated == false)
                GetCharacterPositions(); //CLEAR POSITIONS

            //Debug.Log("EVENT.ID: " + event_id + " GLOBAL.C: " + global_counted_lines);

            dialogueData = dialogue_manager.getDialogueDataByEvent(event_id); //Get all the data for dialoge by event_id
            readed_lines = dialogueData.GetLength(1);
            textData = new string[readed_lines];

            for (int i = 0; i < readed_lines; i++)
            {
                textData[i] = dialogueData[2, i]; //GET ONLY THE TEXT
            }


           
            if (liberate_system_activated == false)
            {
                uiText_classic.text = "";
            }
            else
            {
                uiText_liberate.text = "";
            }

            //Debug.Log("TEXT-TEST: " + textData[readed_lines-1]);

        }

        public void StartDialogue(string[] textDialogue)
        {
            sentences.Clear();

            foreach(string sentence in textDialogue)
            {
                if(sentence == cutscene_str)
                {
                    Debug.Log("CUTSCENE - " + actual_event);
                    isCutscene = true;

                    break;
                }


                if ((sentence != "") && (sentence != null) && (sentence.Length > 0))
                {
                    sentences.Enqueue(sentence);
                }
                    
            }

            if(isCutscene == true)
            {
                StartCoroutine(WaitTimeFinishCinematic(actual_event));
                
            }
            else
            {
                isPrinting = true;
                DisplayNextSentence();
            }
        }

        IEnumerator WaitTimeFinishCinematic(int event_id)
        {  
            float video_length = (float) EventManager.event_manager_instance.GetCinematicByEvent(event_id).length;

            yield return new WaitForSeconds(video_length);

            isCutscene = false;

            isPrinting = true;
            DisplayNextSentence();
        }


        public void DisplayNextSentence()
        {
            //Check if the dialogue is finish or not
            if ((sentences.Count == 0) || (global_counted_lines == readed_lines))
            {
                EndDialogue();
                return;
            }

            if (liberate_system_activated == false)
            {
                uiText_classic.text = "";
            }
            else
            {
                uiText_liberate.text = "";
            }


            if(liberate_system_activated == false)
                SetSpeakers(); //THIS FUNCTION MANAGES THE PORTRAITS AND SPEAKER ON THE DIALOGUE


            if(counted_lines == max_lines)
            {
                if (liberate_system_activated == false)
                {
                    uiText_classic.text = "";
                }
                else
                {
                    uiText_liberate.text = "";
                }

                counted_lines = 0;
            }

            string sentence = sentences.Dequeue() + "\n";
            StopAllCoroutines();


            //The counter SHOULD BE AFTER THE CHARACTER_PORTRAIT ACTIVATION!
            global_counted_lines++;
            counted_lines++;

            if (sentence == "TRANSITION" + "\n")
            {
                sentence = "";
            }

            StartCoroutine(TypeSentence(sentence));
        }

        void EndDialogue()
        {
            //Debug.Log("Dialogue Finish");
            isPrinting = false;
            StopAllCoroutines();

            max_lines = 6;
            counted_lines = 0;
            global_counted_lines = 0;
            readed_lines = 0;

            dialogueData = null; //CLEAR
            textData = null; //CLEAR

            //StartCoroutine(ManageFinalEvents(1.0f));

            //Debug.Log("MAX-EVENT: " + dialogue_manager.getMaxEvents());

            if(this.gameObject.activeSelf == true)
            {
                if (actual_event <= dialogue_manager.getMaxEvents())
                {
                    if(liberate_system_activated == false)
                        HideAllCharacters();
                    actual_event++;
                    StartCoroutine("DelayStart", 0.2f);
                }

            }

        }

        /*
        IEnumerator ManageFinalEvents(float seconds)
        {
            EventManager.event_manager_instance.ManageEvents(actual_event, true);

            yield return new WaitForSeconds(seconds);
        }
        */

        //THIS FUNCTION MANAGES THE PORTRAITS AND SPEAKER ON THE DIALOGUE
        public void SetSpeakers()
        {
            //Get the character portrait and enable it when the character speaks
            //GameObject speaker = GetCharacterPortrait(System.Array.IndexOf(dialogue_manager.character_names, dialogueData[3, global_counted_lines]));

            string actual_speaker = dialogueData[3, global_counted_lines];
            string left_speaker = dialogueData[4, global_counted_lines];
            string right_speaker = dialogueData[5, global_counted_lines];

            //Debug.Log("CH-LEFT: " + dialogueData[4, global_counted_lines]);
            //Debug.Log("CH-RIGHT: " + dialogueData[5, global_counted_lines]);
            //Debug.Log("ACTUAL_SPEAKER: " + actual_speaker);

            if (actual_speaker != "X") //Player
            {
                GameObject speaker = GetCharacter(actual_speaker);

                if ((speaker != null) && (liberate_system_activated == false))
                {
                    //global_counted_linesDebug.Log("SPEAKER: " + speaker.name);

                    RectTransform portrait_transform = speaker.GetComponent<RectTransform>();

                    //Set the character on position
                    if (speaker.name == left_speaker) //LEFT
                    {
                        //Check the scale (On left should be positive)
                        if (portrait_transform.localScale.x < 0)
                        {
                            portrait_transform.localScale = new Vector3(portrait_transform.localScale.x * -1.0f,
                                portrait_transform.localScale.y, portrait_transform.localScale.z);
                        }

                        if ((character_occupied_positions[1] == null) || (character_occupied_positions[1] == ""))
                        {
                            portrait_transform.position = new Vector3(
                                character_positions[1].GetComponent<RectTransform>().transform.position.x,
                                portrait_transform.position.y, portrait_transform.position.z);

                            character_occupied_positions[1] = speaker.name;
                        }
                        else
                        {
                            if (character_occupied_positions[1] != speaker.name)
                            {
                                portrait_transform.position = new Vector3(
                                    character_positions[0].GetComponent<RectTransform>().transform.position.x,
                                    portrait_transform.position.y, portrait_transform.position.z);

                                character_occupied_positions[0] = speaker.name;
                            }

                        }
                    }
                    else //RIGHT
                    {
                        //Check the scale (On right should be negative)
                        if (portrait_transform.transform.localScale.x > 0)
                        {
                            portrait_transform.localScale = new Vector3(portrait_transform.localScale.x * -1.0f,
                              portrait_transform.localScale.y, portrait_transform.localScale.z);
                        }

                        if ((character_occupied_positions[2] == null) || (character_occupied_positions[2] == ""))
                        {
                            portrait_transform.position = new Vector3(
                              character_positions[2].GetComponent<RectTransform>().transform.position.x,
                              portrait_transform.position.y, portrait_transform.position.z);


                            character_occupied_positions[2] = speaker.name;
                        }
                        else
                        {
                            if (character_occupied_positions[2] != speaker.name)
                            {
                                portrait_transform.position = new Vector3(
                                  character_positions[3].GetComponent<RectTransform>().transform.position.x,
                                  portrait_transform.position.y, portrait_transform.position.z);

                                character_occupied_positions[3] = speaker.name;
                            }
                        }
                    }

                    if (speaker.activeSelf == false)
                        speaker.SetActive(true);
                    else
                        PlaySilenceAnimation(speaker);

                    // CharacterIsSpeaking(speaker);

                    SetCharacterName(speaker.name);

                }
            }
            else //Player speaking
            {
                DeactivateAllCharacters();

                SetCharacterName((DataSaver.player_name).ToString().ToUpper());
            }
        }


        public void SetCharacterName(string name)
        {
            string character_name_str = "Character_Name";

            Transform character_name = this.transform.Find(menu_type_parent_classic_name).transform.Find(character_name_str);
            character_name.GetChild(0).gameObject.GetComponent<TMP_Text>().text = name;
        }

        IEnumerator TypeSentence(string sentence)
        {
            //uiText.text = "";

            foreach(char character in sentence.ToCharArray())
            {
                if (liberate_system_activated == false)
                    uiText_classic.text += character;
                else
                {
                    string actual_speaker = dialogueData[3, global_counted_lines-1]; //-1 To get the real speaker
                    uiText_liberate.text += character;

                    if(sentence != "")
                        uiText_liberate_speaker.text = actual_speaker;
                    else
                        uiText_liberate_speaker.text = "";

                    if (actual_speaker == "ALICE") //Radio speak
                    {
                        uiText_liberate.fontStyle = FontStyles.Italic;
                    }
                    else
                    {
                        uiText_liberate.fontStyle = FontStyles.Normal;
                    }

                    //MouthMovement(actual_speaker);
                }
                    
                yield return new WaitForSeconds(charPrintDelay);
            }

            DisplayNextSentence();
        }

        //LIBERATE SYSTEM 3D Model Speaks #TO-DO
        public void MouthMovement(string speaker)
        {

            if ((speaker.ToString().ToUpper()) == (liberate_character_speaker.name).ToString().ToUpper())
            {
                //Speak
            }
            else
            {
                //Close mouth
            }
        }

    }
}
