using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_Tactica; //BECAUSE OF MUTIPLE NAMESPACES USE
using ADAPT; //BECAUSE OF MUTIPLE NAMESPACES USE
using UnityEngine.AI;
using System;

public class Faction_AI : Agent
{

    [Space]
    [Space]

    public string faction_name;

    //Type of Faction in relationship with the world and other Factions
    public enum faction_status { Neutral, Offensive, Deffensive };
    public faction_status status;

   
    [HideInInspector]
    public SquadronController squadron_controller;

    public GameObject squadron_ai_prefab;
    [Space]
    [Space]
    [Space]
    [Space]

    [SerializeField]
    public Squadron[] squadrons;  //Array of arrays of Units (Squadron Creation)
    public Squadron[] auxiliar_squadron;

    public int num_units = 5; //Number of units (GENERAL INCLUDE)
    public int num_squadrons = 8; //Number of squadrons
    public float wave_timer = 10.0f;

    public bool createUnits = false;

    public GameObject ai_prefab;
    public GameObject spawn_point;

    [Header("----------------")]
    [Header("Patrol Properties")]
    public int[] actual_patrol_point;
    public GameObject patrol_points; //CREATE AS MANY CHILDRENS AS SQUADS
    [HideInInspector]
    public Transform[][] points;

    bool initiated_squadrons = false;

    bool false_start = false;

    private void Awake()
    {
        //InitiateSquadrons();
    }


    // Start is called before the first frame update
    new void Start()
    {

        //GOAP_AI();

        ////******SQUADRON MANAGEMENT******//

        //auxiliar_squadron = squadrons;
        //if (createUnits == true)
        //    StartCoroutine(waitToCreate(1.0f));
        ////******SQUADRON MANAGEMENT******//

    }

    //Use this after the first tutorial interaction (The time is stopped should the Faction Squadrons are not created properly)
    public void FalseStart()
    {
        if(false_start == false)
        {
            InitiateSquadrons();

            GOAP_AI();

            //******SQUADRON MANAGEMENT******//

            auxiliar_squadron = squadrons;
            if (createUnits == true)
                StartCoroutine(waitToCreate(1.0f));
            //******SQUADRON MANAGEMENT******//

            false_start = true;
        }
       
    }

    //******A.D.A.P.T. AI******//
    void GOAP_AI()
    {
        //******A.D.A.P.T. AI******//
        AddGoals();
        /************/
        base.Start(); //DON'T DELETE THIS LINE!!!
                      /************/
        ManageStates();
        //******A.D.A.P.T. AI******//
    }

    public void AddGoals()
    {

    }

    public void ManageStates()
    {

    }
    //******A.D.A.P.T. AI******//

    public void InitiateSquadrons()
    {
        if (initiated_squadrons == false)
        {
            if (squadrons.Length <= 0)
            {
                squadrons = new Squadron[num_squadrons];


                //IMPORTANT: USE THIS TO INITIALIZE THE ARRAY OF OBJECTS!
                for (int i = 0; i < num_squadrons; i++)
                {
                    squadrons[i] = new Squadron();
                }
                //
            }
            else
            {
                RefactorSquadrons();
            }

            if (num_squadrons <= 0)
            {
                num_squadrons = squadrons.Length;
            }

            squadron_controller = GameObject.Find("Game Logic").GetComponent<SquadronController>();


            //Patrol points initialization
            actual_patrol_point = new int[patrol_points.transform.childCount]; //CREATE ON THE HIERARCHY THE SAME NUMBER OF SQUADRON POINTS AS FACTIONS!!!
            points = new Transform[patrol_points.transform.childCount][];

            for (int i = 0; i < patrol_points.transform.childCount; i++) //Initialize the Array of Arrays of Patrol Points (FACTION_X_PATROLPOINTS - INDEXes)
            {
                actual_patrol_point[i] = 0; //Initialize patrol point NAVIGATION index

                Transform[] aux; //Aux Trasnform Array for get all childrens of Squadron Parent Points
                aux = new Transform[patrol_points.transform.GetChild(i).childCount];

                for (int j = 0; j < aux.Length; j++) //Store all the patrol points - GETALLCHILDREN
                {
                    aux[j] = patrol_points.transform.GetChild(i).GetChild(j);
                }

                points[i] = aux; //Inside of each row of the 2D Array now each is array of points of the parent (0 -> 0, 1 ,2 ...)
            }
        }

        initiated_squadrons = true;
    }

    public void RefactorSquadrons()
    {
        if (num_squadrons > 0)
        {
            //At the moment there is existence of squadrons, ADD and NOT REPLACE
            Squadron[] aux_squadrons = squadrons;
            int aux_length = squadrons.Length;

            //Reinitiate the squadrons original array
            squadrons = null;
            squadrons = new Squadron[num_squadrons + aux_length];

            //Aux squadrons
            for (int i = 0; i < aux_length; i++)
            {
                squadrons[i] = aux_squadrons[i];
            }

            //New squadrons
            for (int i = num_squadrons; i < aux_length; i++)
            {
                squadrons[i] = new Squadron();
            }

           
            num_squadrons = num_squadrons + aux_length; //SAVE THE NUM OF SQUADRONS OF THE ONES CREATED PREVIOUSLY AND THE INSTANTIATED ONES
        }
    }


    IEnumerator waitToCreate(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        faction_CreateSquadrons(num_squadrons, num_units);
    }

    void faction_CreateSquadrons(int num_squadrons, int num_units)
    {
        int squad_num = 0;

        for(int i = 0; i < num_squadrons; i++)
        {
            if(auxiliar_squadron[i] != null)
            {
                if(auxiliar_squadron[i].squadron != null) //CHECK IF A SQUADRON ITS EMPTY OR NOT
                    squad_num++;
            }            
        }

        StartCoroutine(CreateWave(wave_timer, squad_num, num_units)); //The other ones
    }

    IEnumerator CreateWave(float seconds, int squad_num, int num_units)
    {
        float inner_seconds = 0.0f;

        for (int num_squad = squad_num; num_squad < num_squadrons; num_squad++)
        {
            //Debug.Log("NUM_SQUAD: " + num_squad + " NUM_SQUADRONS " + num_squadrons);
            yield return new WaitForSeconds(inner_seconds);
            faction_CreateNewSquad(num_squad, num_units);
            inner_seconds += seconds;
        }
       
    }

    //Add new squadrons GameObject to the actual Faction GameObject
    GameObject addNewSquadron(int num_squad)
    {
        GameObject new_squadron = Instantiate(squadron_ai_prefab);  // new GameObject(("SQUADRON_" + num_squad).ToString());
        new_squadron.name = (("SQUADRON_" + num_squad).ToString());
        new_squadron.transform.position = spawn_point.transform.position; //SPAWN THE NEW SQUADRON (ALL OF IT) ON THE DESIRED POSITION !!!

        Squad_AI squad_ai = new_squadron.GetComponent<Squad_AI>(); //GET THE SQUAD_AI COMPONENT OF EACH ONE OF THE SQUADRONS
        squad_ai.num_squad = num_squad;
        squad_ai.squadron_controller = squadron_controller;

        return new_squadron;
    }

    void faction_CreateNewSquad(int squad_num, int num_units) 
    {
        if(auxiliar_squadron[squad_num] == null)
        {
            auxiliar_squadron[squad_num] = new Squadron();
        }

        if (auxiliar_squadron[squad_num] != null)
        {
            auxiliar_squadron[squad_num].squadron = new List<GameObject>(new GameObject[num_units]); //Initialize the Squadron

            GameObject new_squad = addNewSquadron(squad_num);
            new_squad.transform.parent = this.gameObject.transform; //Set the actual faction as the parent of the new squadrons gameobjects

            for (int i = 0; i < num_units; i++)
            {
                GameObject new_unit = Instantiate(ai_prefab, new_squad.transform); //Transform.position doesn't work as planned because the "SQUADRON" parent

                if (i != 0) //If is not he captain, increase the speed of the other units
                {
                    NavMeshAgent agent = new_unit.GetComponent<NavMeshAgent>();
                    agent.speed = agent.speed + 5.0f;
                }

                auxiliar_squadron[squad_num].squadron[i] = new_unit;
            }

            //Add the captain to the squadron - BY DEFAULT: INDEX 0
            auxiliar_squadron[squad_num].squadron_captain = auxiliar_squadron[squad_num].squadron[0];

            //Add squadron UI to the Captain of the NEW squadron
            squadron_controller.addUISquadron(auxiliar_squadron, squad_num, 0);
        }
    }


}

