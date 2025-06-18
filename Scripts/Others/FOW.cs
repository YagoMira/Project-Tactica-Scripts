using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_Tactica;

public class FOW : MonoBehaviour
{
    //ELEMENTS - UNITS / BUILDINGS
    public GameObject units;
    public GameObject buildings;

    public GameObject main_camera;

    //FOW PROPERTIES
    public LayerMask fow_layer;
    //float radius { get { return fow_radius * fow_radius; } }
    //public float fow_radius = 10.0f;

    public float unit_fow_radius = 10.0f;
    public float building_fow_radius = 50.0f;

    //PLANE PROPERTIES
    public GameObject fow_plane;
    Mesh mesh;
    Vector3[] vertices;
    Color[] colors;

    //FOW PROPERTIES
    public float refow_seconds = 3.0f; //UPDATE FOW SECONDS
    public float refow_alpha = 0.4f; //REFOW "COLOR"

    private void Start()
    {
        mesh = fow_plane.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        colors = new Color[vertices.Length];

        for(int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.black; //fow_plane.GetComponent<Renderer>().material.GetColor("_Color");
        }

        mesh.colors = colors;

        StartCoroutine(ApplyFow(0.1f));
    }

    private void Update()
    {
        StartCoroutine(ReFOW(refow_seconds));
        StartCoroutine(ApplyFow(refow_seconds));
    }

    IEnumerator ReFOW(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        for (int i = 0; i < vertices.Length; i++)
        {
            if (colors[i].a != 1.0f)
            {
                colors[i].a = refow_alpha;
            }
        }
    }

    IEnumerator ApplyFow(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        for (int i = 0; i < units.transform.childCount; i++)
        {
            UpdateFOW(units.transform.GetChild(i).gameObject, unit_fow_radius);
        }

        for (int j = 0; j < buildings.transform.childCount; j++)
        {
            UpdateFOW(buildings.transform.GetChild(j).gameObject, building_fow_radius);
        }
    }

    private void UpdateFOW(GameObject element, float fow_radius)
    {
        float d = 1000000.0f; //Maximun distance of ray
        float radius = fow_radius * fow_radius;

        Ray ray = new Ray(element.transform.position, (element.transform.up) * d); //TURN ROTATION Z = -180 WHEN USE THIS
        //Ray ray = new Ray(main_camera.transform.position, element.transform.position - main_camera.transform.position); //TURN ROTATION Z = 0 WHEN USE THIS
        RaycastHit hit;

        //Debug RAY
        Debug.DrawRay(element.transform.position, (element.transform.up) * d, Color.red);
        //Debug.DrawRay(main_camera.transform.position, (element.transform.position - main_camera.transform.position) * d, Color.red);

        if (Physics.Raycast(ray, out hit, d, fow_layer, QueryTriggerInteraction.Collide))
        {
            for(int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = fow_plane.transform.TransformPoint(vertices[i]);
                float distance = Vector3.SqrMagnitude(v - hit.point);

                if(distance < radius)
                {
                    float alpha = 0.0f; //Mathf.Min(colors[i].a, distance / radius);
                    colors[i].a = alpha;
                }
            }

            mesh.colors = colors;
        }
    }
}
