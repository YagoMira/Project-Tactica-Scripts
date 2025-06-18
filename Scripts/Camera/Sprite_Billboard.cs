using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite_Billboard : MonoBehaviour
{
    public bool freezeXZAxis = false;
    void Update()
    {
        if(freezeXZAxis == true)
        {
            transform.rotation = Quaternion.Euler(0.0f, Camera.main.transform.rotation.eulerAngles.y, 0.0f);
        }
        else
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
