using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class LineRendering : MonoBehaviour
    {
        public LineRenderer l_renderer;

        void Start()
        {
            l_renderer = this.gameObject.GetComponent<LineRenderer>();
        }

        public void SetRenderer(LineRenderer l_renderer)
        {
            this.l_renderer = l_renderer;
        }

        public void SetLine(Vector3 start_pos, Vector3 final_pos)
        {
            l_renderer.positionCount = 2; //2 Vertex for each line

            //Set the positions of line
            l_renderer.SetPosition(0, start_pos);
            l_renderer.SetPosition(1, final_pos);
        }
    }

}
