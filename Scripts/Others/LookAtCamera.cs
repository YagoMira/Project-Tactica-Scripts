using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera main_camera;

    public float rotation_speed = 3.0f;

    private void Start()
    {
        main_camera = Camera.main;
    }
    private void Update()
    {
        this.gameObject.transform.rotation = Quaternion.Slerp(this.gameObject.transform.rotation, main_camera.transform.rotation, rotation_speed * Time.deltaTime);
    }
}
