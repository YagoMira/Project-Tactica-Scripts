using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public enum item_ui_type { Item, Dialogue, UI }

    public class ItemsUIManager : MonoBehaviour
    {
        public string[,] items_ui_data;

        public CSVReader csv_data; //Dialogue CSV DATA
        public bool search_csv_data = false;

        private void Awake()
        {
            Clear();

            //Debug.Log("ITEM-1: " + getItemsUIDataByID(item_ui_type.Item, 1));
        }

        private void Start()
        {
            if(search_csv_data == false)
            {
                GameObject ui_manager_object = GameObject.Find("UI Manager");

                if (ui_manager_object != null)
                {
                    csv_data = GameObject.Find("UI Manager").GetComponent<CSVReader>();
                    getItemsUIData(csv_data.textData_items_ui);
                }
            }
        }

        public void Clear()
        {
            
            items_ui_data = null;

            if(search_csv_data == true)
            {
                csv_data = null;
                csv_data = this.GetComponent<CSVReader>();
            }

            if(csv_data != null)
                getItemsUIData(csv_data.textData_items_ui);
        }

        //Recover the raw data from CSV to manage in the Dialogue Manager
        public void getItemsUIData(string[,] textData)
        {
            items_ui_data = textData;
        }

        public string getItemUIType(item_ui_type type)
        {
            string item_ui_type_str = null;

            if (type == item_ui_type.Item)
            {
                item_ui_type_str = "item";
            }
            else if (type == item_ui_type.Dialogue)
            {
                item_ui_type_str = "dialog";
            }
            else if (type == item_ui_type.UI)
            {
                item_ui_type_str = "ui";
            }
            else
            {
                item_ui_type_str = null;
            }

            return item_ui_type_str;
        }

        //Get Item/UI Data from the CSV file by ID
        public string getItemsUIDataByID(item_ui_type type, int id)
        {
            string item_ui_text = null;
            string item_ui_parser = (getItemUIType(type) + "_" + id).ToString();

            if ((csv_data == null) || (items_ui_data == null)) //Fix any initialization delay/order problem
            {
                //RECACH DATA
                if (csv_data.textData_items_ui == null)
                {
                    csv_data.getDataSize_items_ui(csv_data.dataLines_items_ui);

                    csv_data.textData_items_ui = new string[csv_data.columns_items_ui, csv_data.rows_items_ui]; //Initialize the array which will contain the soldiers names

                    csv_data.getDataText_items_ui(csv_data.dataLines_items_ui);
                }

                csv_data = this.GetComponent<CSVReader>();

                if (csv_data != null)
                {
                    getItemsUIData(csv_data.textData_items_ui);
                }
                else
                {
                    csv_data = GameObject.Find("UI Manager").GetComponent<CSVReader>(); //World scene
                    getItemsUIData(csv_data.textData_items_ui);
                }

            }


            for (int c = 0; c < items_ui_data.GetLength(0); c++)
            {
                for (int r = 0; r < items_ui_data.GetLength(1); r++)
                {
                    if (item_ui_parser == (items_ui_data[0, r]).ToString())
                    {
                        item_ui_text = (string)items_ui_data[c, r];
                        break;
                    }

                }
            }



            if ((item_ui_text == "") || (item_ui_text == null))
            {
                //Debug.LogError("ITEM-UI NOT FOUND!");
            }

            return item_ui_text;
           
        }

    }
}
