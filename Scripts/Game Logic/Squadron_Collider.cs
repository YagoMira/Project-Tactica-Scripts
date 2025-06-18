using Project_Tactica;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squadron_Collider : MonoBehaviour
{
    public float repositionate_speed = 0.1f;
    public bool onCollision = false;

    Vector3 original_position;
    

    private void Start()
    {
        original_position = this.gameObject.transform.localPosition;
    }

    private void Update()
    {
        if(onCollision == false)
        {
            if(gameObject.transform.localPosition.Equals(original_position) == false)
            {
                TransitionToPosition(true); //Return to Local Origin (Formation Squad Parent)
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string ignore_tag = "IGNORE";

        if (!other.gameObject.transform.CompareTag(ignore_tag))
        {
            if (other.gameObject.TryGetComponent<Enemy_Checker>(out Enemy_Checker component) == false)
            {
                if (other.gameObject.TryGetComponent<Unit>(out Unit component_2) == false)
                {
                    
                    //Debug.Log("NAME: " + this.gameObject.name + "Collision with: " + other.gameObject.name);
                    onCollision = true;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(onCollision == true)
        {
            TransitionToPosition(false); //Repositionate (Captain Squad Parent)
        }

        if(other.gameObject.layer == 6) //TERRAIN LAYER
        {
            repositionate_speed = 0.8f;
            onCollision = true;
            TransitionToPosition(false); //Repositionate (Captain Squad Parent)
        }
        else
        {
            repositionate_speed = 0.8f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        onCollision = false;
    }

    public void TransitionToPosition(bool returnOriginal)
    {
        if(returnOriginal == true)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, original_position, repositionate_speed);
        }
        else //ReAdapt position to the main parent (Captain Squad)
        {
            Vector3 captain_origin = new Vector3(0.0f, 0.0f, 0.0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, captain_origin, repositionate_speed);
        }
    }
}
