using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_Tactica;

public class PlayAudio_Collider : MonoBehaviour
{
    public Item item;
    public FirstPerson_Contoller fps_controller;

    public float destroy_time = 1.0f;

    private void Start()
    {
        item = this.gameObject.GetComponent<Item>();
    }

    private void OnTriggerEnter(Collider other)
    {
        string player_str = "Player";
        if(other.gameObject.CompareTag(player_str))
        {
            if(item.audio_clip != null)
            {
                //fps_controller.ManageItem(item);
                //Destroy(this.gameObject);
                item.Interact();
                AudioManager.instance.PlayClip(item.audio_clip);

                StartCoroutine(DestroyAfter(destroy_time));
            }
            
        }
    }

    // Update is called once per frame
    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        Destroy(this.gameObject);

    }
}
