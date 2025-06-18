using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script helps to play other animation from other animator with the animation key events
public class PlaySiblingAnim : MonoBehaviour
{
    public Animator sibling_animator;

    public void PlaySAnimation()
    {
        if(sibling_animator != null)
        {
            sibling_animator.SetBool("play", true);
        }
    }
}
