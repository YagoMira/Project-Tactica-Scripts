using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public GameObject credits;
    public RectTransform credits_rect;
    Vector2 initial_position;

    public float credits_speed = 28.0f; //BABA <3

    //Timer
    float timer = 0.0f;
    public float max_time = 28.0f;

    public Button button_start;
    public Button button_options;
    public Button button_credits;
    public Button button_exit;

    private void Start()
    {
        credits_rect = credits.GetComponent<RectTransform>();
        initial_position = credits_rect.anchoredPosition;
    }

    private void Update()
    {
        if(timer < max_time)
        {
            //Scroll vertically the credtis (Like Star Wars)
            credits_rect.anchoredPosition += new Vector2(0.0f, credits_speed * Time.deltaTime);

            button_start.interactable = false;
            button_options.interactable = false;
            button_credits.interactable = false;
            button_exit.interactable = false;

        }
        else
        {
            timer = 0.0f;
            credits_rect.anchoredPosition = initial_position;
            this.gameObject.SetActive(false);

            //Set the other buttons interactable
            button_start.interactable = true;
            button_options.interactable = true;
            button_credits.interactable = true;
            button_exit.interactable = true;

        }

        CountTime();


    }

    public void CountTime()
    {
        timer += Time.deltaTime;
    }

}
