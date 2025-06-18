using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LookAtCamera : MonoBehaviour
{
    private Transform local;
    public Camera main_camera;


    private void Start()
    {
        main_camera = Camera.main;
    }

    private void Update()
    {
        if (main_camera != null)
        {
            this.gameObject.transform.LookAt(2 * this.gameObject.transform.position - main_camera.transform.position);
        }
    }
}
