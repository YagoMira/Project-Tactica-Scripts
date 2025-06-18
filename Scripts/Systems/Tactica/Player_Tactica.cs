using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project_Tactica;

public class Player_Tactica : Tactica
{
    public GameObject units;

    public float speed_percentage = 10.0f; //10% increase

    //The Player Tactica will be: Increase the speed of ALL units
    public override IEnumerator TacticaAbility(bool enabled)
    {
        if(enabled == true)
        {
            GameObject units_aux = units; //Used for restore the units properties to its initial values

            Debug.Log("ACTIVATED-TACTICA");

            if (units.transform.childCount > 0)
            {
                for (int i = 0; i < units.transform.childCount; i++)
                {
                    if (units.transform.GetChild(i).gameObject.GetComponent<Unit>() != null)
                    {
                        if(units.transform.GetChild(i).gameObject.activeSelf == true)
                        {
                            Unit unit = units.transform.GetChild(i).gameObject.GetComponent<Unit>();
                            unit.agent.speed = (unit.agent.speed + (unit.agent.speed * (speed_percentage / 100.0f)));

                            ActivateTacticaUI(unit, true);
                        }
                        
                    }
                }
            }

            yield return new WaitForSeconds(duration);

            activated = false;

            if (units_aux.transform.childCount > 0)
            {
                for (int i = 0; i < units_aux.transform.childCount; i++)
                {
                    if (units_aux.transform.GetChild(i).gameObject.GetComponent<Unit>() != null)
                    {
                        if (units.transform.GetChild(i).gameObject.activeSelf == true)
                        {
                            Unit unit = units_aux.transform.GetChild(i).gameObject.GetComponent<Unit>();
                            unit.agent.speed = unit.unit_stats.speed;

                            ActivateTacticaUI(unit, false);
                        }
                    }
                }
            }
        }   
    }

    public void ActivateTacticaUI(Unit unit, bool enabled)
    {
        GameObject tactica_effect_ui = unit.tactica_effect_ui;

        tactica_effect_ui.SetActive(enabled);
    }

}
