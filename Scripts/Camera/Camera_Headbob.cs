using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_Tactica;

public class Camera_Headbob : MonoBehaviour
{
    [Range(0.001f, 0.01f)]
    public float amount = 0.002f;

    [Range(1f, 30f)]
    public float frequency = 10.0f;

    [Range(10f, 100f)]
    public float smooth = 10.0f;

    Vector3 start_position;

    //Other systems
    Input_Logic input;

    // Start is called before the first frame update
    void Start()
    {
        input = (this.transform.parent).transform.parent.gameObject.GetComponent<Input_Logic>();

        start_position = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        DoHeadbob();
    }

    //Apply Headbob when the conditions are attached
    public void DoHeadbob()
    {
        float magnitude = new Vector3(this.input.horizontal, 0, this.input.vertical).magnitude;

        if(magnitude > 0) //Movement is happening
        {
            //Start Headbob
            Vector3 position = Vector3.zero;
            position.x += Mathf.Lerp(position.x, Mathf.Cos(Time.time * frequency / 2f) * amount * 1.6f, smooth * Time.deltaTime);
            position.y += Mathf.Lerp(position.y, Mathf.Sin(Time.time * frequency) * amount * 1.4f, smooth * Time.deltaTime);
            transform.localPosition += position;
        }

        //Stop Headbob
        if(transform.localPosition == start_position)
        {
            return;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, start_position, Time.deltaTime);
    }
}
