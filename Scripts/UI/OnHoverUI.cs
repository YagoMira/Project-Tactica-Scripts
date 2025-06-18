using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnHoverUI : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    //Get the description UI Object and its animator
    Animator description_animator;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject description = this.transform.Find("Description").gameObject;

        if (description.activeSelf == false)
            description.SetActive(true);

        if(description_animator == null)
            description_animator = description.GetComponent<Animator>(); //Find in the General Gameobject Parent!

        description_animator.SetBool("enabled", true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        description_animator.SetBool("enabled", false);
    }

}
