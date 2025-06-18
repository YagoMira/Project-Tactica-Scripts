using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Simple FPS Counter
public class FPSCounter : MonoBehaviour
{
    private int last_frame;
    private float[] frames_storage;
    public Text ui_fps_text;

    private void Awake()
    {
        //Init the array of frames
        frames_storage = new float[50];
    }

    private void Update()
    {
        frames_storage[last_frame] = Time.unscaledDeltaTime;
        last_frame = (last_frame + 1) % frames_storage.Length;

        ui_fps_text.text = Mathf.RoundToInt(CalculateFPS()).ToString();
    }

    //Average of frames
    private float CalculateFPS()
    {
        float total = 0.0f;

        foreach(float time in frames_storage)
        {
            total += time;
        }

        return frames_storage.Length / total;
    }
}
