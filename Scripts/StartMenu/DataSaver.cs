using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public static class DataSaver
    {
        public static string player_name = "PLAYER";

        //OPTIONS
        public static int language_selected = 0; //English == 0 | Spanish == 1
        public static float music_volume = 0.5f; // 0.5 by default (MAX 1.0)
        public static float effects_volume = 0.5f;
        public static float fov = 80.0f;


        public static void SetPlayerName(string name)
        {
            if((name != "") || (name != null))
                player_name = name;
        }

        public static void SetAllValues(Values values)
        {

            language_selected = values.language;
            music_volume = values.music_volume;
            effects_volume = values.effects_volume;
            fov = values.fov;
        }
    }
}
