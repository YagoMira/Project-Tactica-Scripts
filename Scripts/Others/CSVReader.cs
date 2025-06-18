using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project_Tactica
{
    public class CSVReader : MonoBehaviour
    {
        //*TO-DO: SELECT THE SPECIFIC CSV FILE IN CASE OF THE DESIRED LANGUAGE
        string selected_language = "eng";

        public string[,] textData;
        public int columns;
        public int rows;

        public string[,] textData_soldiers; //TO-DO: ADD NAMES FOR ALL TYPES OF UNITS? (TANKS, ...) AND DIFERENTIATE IT IN THE CSV?
        public int columns_soldiers;
        public int rows_soldiers;

        public string[,] textData_items_ui;
        public int columns_items_ui;
        public int rows_items_ui;
        public string[] dataLines_items_ui;

        private void Awake()
        {
            InitiateReader();
        }

        public void Clear()
        {
            textData = null;
            columns = 0;
            rows = 0;

            textData_soldiers = null;
            columns_soldiers = 0;
            rows_soldiers = 0;

            textData_items_ui = null;
            columns_items_ui = 0;
            rows_items_ui = 0;
            dataLines_items_ui = null;
        }

        public void InitiateReader()
        {
            Clear();
            getLanguage();

            //----------------------CSV DATA - DIALOGUES----------------------//
            TextAsset dataset = Resources.Load<TextAsset>("lang_" + selected_language);

            string[] dataLines = dataset.text.Split('\n'); //Get the lines of the csv


            getDataSize(dataLines);

            textData = new string[columns, rows]; //Initialize the array which will contain the characters texts

            //Debug.Log("COLUMNS " + columns);
            // Debug.Log("ROWS " + rows);

            getDataText(dataLines);
            //----------------------CSV DATA - DIALOGUES----------------------//

            //----------------------CSV DATA - SOLDIER NAMES----------------------//
            TextAsset dataset_soldiers = Resources.Load<TextAsset>(("soldiers_names").ToString());
            string[] dataLines_soldiers = dataset_soldiers.text.Split('\n'); //Get the lines of the csv

            getDataSize_soldiers(dataLines_soldiers);

            textData_soldiers = new string[columns_soldiers, rows_soldiers]; //Initialize the array which will contain the soldiers names

            getDataText_soldiers(dataLines_soldiers);
            //----------------------CSV DATA - SOLDIER NAMES----------------------//

            //----------------------CSV DATA - INITIAL ITEMS/DIALOGUES & UI----------------------//

            TextAsset dataset_items_ui = Resources.Load<TextAsset>(("ui_lang_" + selected_language).ToString());
            if (dataset_items_ui != null)
                dataLines_items_ui = dataset_items_ui.text.Split('\n'); //Get the lines of the csv

            getDataSize_items_ui(dataLines_items_ui);

            textData_items_ui = new string[columns_items_ui, rows_items_ui]; //Initialize the array which will contain the soldiers names

            getDataText_items_ui(dataLines_items_ui);

            //----------------------CSV DATA - INITIAL ITEMS/DIALOGUES & UI----------------------//
        }


        //Get the desired language for the game after load the option for the start menu
        void getLanguage() //#TO-DO: SHOULD BE CHANGED TO SWITCH WHEN EXISTS MORE LANGUAGES IN THE GAME!
        {
            //THIS CODE IS USED WITH A DONTDESTROYONLOAD START MENU SCRIPT!
            //if(StartMenu_Values.instance != null) //Prevent possible NullReferences
            //{
            //    if (StartMenu_Values.instance.language_selected == 0) //ENGLISH
            //    {
            //        selected_language = "eng";
            //    }
            //    else if (StartMenu_Values.instance.language_selected == 1) //SPANISH
            //    {
            //        selected_language = "es";
            //    }
            //}
            //

            if (DataSaver.language_selected == 0) //ENGLISH
            {
                selected_language = "eng";
            }
            else if (DataSaver.language_selected == 1) //SPANISH
            {
                selected_language = "es";
            }

        }

        //Get the size of columns and rows of the CSV file
        void getDataSize(string[] dataLines)
        {
            columns = 0;
            rows = 0;

            string[] data = new string[0];

            //GET THE SIZE OF THE ROWS
            if (dataLines.Length > 3)
                rows = dataLines.Length - 3; //Minus two to evade the bad parsing of white spaces and ";" and Minus three to evade the first row of identifiers

            //GET THE SIZE OF THE COLUMNS
            for (int i = 0; i < dataLines.Length; i++) // i == 1 for evade the first line of the identifiers
            {
                //FIRST LINE WILL BE: " ID;EVENT;DESCRIPTION;;TEXT;;;;;;;;;CHARACTER;;CHARACTER LEFT;;;CHARACTER RIGHT;;;LANGUAGE; "
                //Debug.Log(dataLines[i]); //Print Lines (LEFT TO RIGHT)

                data = dataLines[i].Split(';'); //Split by ';'

                for (int c = 1; c < data.Length; c++) // c == 1 for evade the first column (ID COLUMN)
                {
                    if ((data[c].Length > 1) || ((data[c].Length > 0) && int.TryParse(data[c], out _))) //Evade empty strings
                    {
                        if (data[c] == "LANGUAGE") //Evade the last column - "LANGUAGE" IS THE DELIMITATOR
                        {
                            return;
                        }

                        columns = columns + 1;
                        //PRINT DATA OF ROWS: Debug.Log(data[c]);
                    }

                }

            }
        }

        //Get the data from the CSV file
        void getDataText(string[] dataLines)
        {
            int real_row_counter = 0;
            int real_col_counter = 0;

            string[] data = new string[0];

            for (int r = 1; r < rows + 1; r++) // i == 1 for evade the first line of the identifiers and +1 to not evade the last line
            {
                //FIRST LINE WILL BE: " ID;EVENT;DESCRIPTION;;TEXT;;;;;;;;;CHARACTER;;CHARACTER LEFT;;;CHARACTER RIGHT;;;LANGUAGE; "
                //Debug.Log(dataLines[i]); //Print Lines (LEFT TO RIGHT)

                data = dataLines[r].Split(';'); //Split by ';'

                for (int c = 1; c < data.Length - 2; c++) // c == 1 for evade the first column (ID COLUMN) and -2 to evade the "LANGUAGE" column and whitespaces, the use of data.Length and not the "column" variable is because the parsing
                {

                    if ((data[c].Length > 0 && (data[c] != null || data[c] != "")) || ((data[c].Length > 0) && int.TryParse(data[c], out _))) //Evade empty strings
                    {
                        //Debug.Log("PRINT DATA: " + " [" + real_col_counter + ", " + real_row_counter + "] =  " + data[c]);

                        textData[real_col_counter, real_row_counter] = data[c]; //Minus one to start with 0 in the Data Text array --- !!TAKE CARE WITH THE EMPTY SPACES OF THE CSV WHEN SOME LINE HAVE ID AND NOT TEXT (For Example...)

                        //Parse the data for every column with a mod of it (Otherwise, the data will be raw, like if we have 50 x 6 columns of data, the counter of columns there will match 300 columns of data)
                        real_col_counter++;

                        if (real_col_counter == columns)
                            real_col_counter = 0;
                        //Parse the data for every column with a mod of it (Otherwise, the data will be raw, like if we have 50 x 6 columns of data, the counter of columns there will match 300 columns of data)

                    }


                }
                real_row_counter++;
            }
        }

        //Get the size of columns and rows of the CSV file
        void getDataSize_soldiers(string[] dataLines)
        {
            columns_soldiers = 0;
            rows_soldiers = 0;

            string[] data = new string[0];

            //GET THE SIZE OF THE ROWS
            if (dataLines.Length > 3)
                rows_soldiers = dataLines.Length - 3; //Minus two to evade the bad parsing of white spaces and ";" and Minus three to evade the first row of identifiers

            //GET THE SIZE OF THE COLUMNS
            for (int i = 0; i < dataLines.Length; i++) // i == 1 for evade the first line of the identifiers
            {
                //FIRST LINE WILL BE: " NAME;GENDER "

                data = dataLines[i].Split(';'); //Split by ';'

                for (int c = 0; c < data.Length; c++)
                {
                    if ((data[c].Length > 1) || ((data[c].Length > 0) && int.TryParse(data[c], out _))) //Evade empty strings
                    {
                        if (data[c] == "LANGUAGE") //Evade the last column - "LANGUAGE" IS THE DELIMITATOR
                        {
                            return;
                        }

                        columns_soldiers = columns_soldiers + 1;
                        //PRINT DATA OF ROWS: Debug.Log(data[c]);
                    }
                }

            }

            Debug.Log("COLUMNS: " + columns_soldiers + " ROWS: " + rows_soldiers);
        }

        //Get the data from the CSV FILE - SOLDIER NAMES
        void getDataText_soldiers(string[] dataLines)
        {
            int real_row_counter = 0;
            int real_col_counter = 0;

            string[] data = new string[0];

            for (int r = 1; r < rows_soldiers + 1; r++) // i == 1 for evade the first line of the identifiers and +1 to not evade the last line
            {
                //FIRST LINE WILL BE: " NAME;GENDER "

                data = dataLines[r].Split(';'); //Split by ';'

                for (int c = 0; c < data.Length - 1; c++) // c == 1 for evade the first column (NAME, GENDER, ...)  | IN THIS CASE c == 0 FOR TAKE THE SOLDIER NAME!!!
                {

                    if ((data[c].Length > 0 && (data[c] != null || data[c] != "")) || ((data[c].Length > 0) && int.TryParse(data[c], out _))) //Evade empty strings
                    {
                        //Debug.Log("PRINT DATA: " + " [" + real_col_counter + ", " + real_row_counter + "] =  " + data[c]);

                        textData_soldiers[real_col_counter, real_row_counter] = data[c]; //Minus one to start with 0 in the Data Text array --- !!TAKE CARE WITH THE EMPTY SPACES OF THE CSV WHEN SOME LINE HAVE ID AND NOT TEXT (For Example...)

                        //Parse the data for every column with a mod of it (Otherwise, the data will be raw, like if we have 50 x 6 columns of data, the counter of columns there will match 300 columns of data)
                        real_col_counter++;

                        if (real_col_counter == columns_soldiers)
                            real_col_counter = 0;
                        //Parse the data for every column with a mod of it (Otherwise, the data will be raw, like if we have 50 x 6 columns of data, the counter of columns there will match 300 columns of data)

                    }


                }
                real_row_counter++;
            }
        }

        //Get the size of columns and rows of the CSV file - ITEMS & UI
        public void getDataSize_items_ui(string[] dataLines)
        {
            columns_items_ui = 0;
            rows_items_ui = 0;

            string[] data = new string[0];

            //GET THE SIZE OF THE ROWS
            if (dataLines.Length > 3)
                rows_items_ui = dataLines.Length - 2; //Minus two to evade the bad parsing of white spaces and ";" and Minus three to evade the first row of identifiers

            //GET THE SIZE OF THE COLUMNS
            for (int i = 0; i < dataLines.Length; i++) // i == 1 for evade the first line of the identifiers
            {
                //FIRST LINE WILL BE: " NAME;GENDER "

                data = dataLines[i].Split(';'); //Split by ';'

                for (int c = 0; c < data.Length; c++)
                {
                    if ((data[c].Length > 1) || ((data[c].Length > 0) && int.TryParse(data[c], out _))) //Evade empty strings
                    {
                        if (data[c] == "LANGUAGE") //Evade the last column - "LANGUAGE" IS THE DELIMITATOR
                        {
                            return;
                        }

                        columns_items_ui = columns_items_ui + 1;
                        //PRINT DATA OF ROWS: Debug.Log(data[c]);
                    }
                }

            }

            //Debug.Log("COLUMNS: " + rows_items_ui + " ROWS: " + rows_items_ui);
        }

        //Get the data from the CSV FILE  - ITEMS & UI
        public void getDataText_items_ui(string[] dataLines)
        {
            int real_row_counter = 0;
            int real_col_counter = 0;

            string[] data = new string[0];

            for (int r = 1; r < rows_items_ui + 1; r++) // i == 1 for evade the first line of the identifiers and +1 to not evade the last line
            {
                //FIRST LINE WILL BE: " NAME;GENDER "

                data = dataLines[r].Split(';'); //Split by ';'

                for (int c = 0; c < data.Length - 1; c++) // c == 1 for evade the first column (NAME, GENDER, ...)  | IN THIS CASE c == 0 FOR TAKE THE SOLDIER NAME!!!
                {

                    if ((data[c].Length > 0 && (data[c] != null || data[c] != "")) || ((data[c].Length > 0) && int.TryParse(data[c], out _))) //Evade empty strings
                    {
                        //Debug.Log("PRINT DATA: " + " [" + real_col_counter + ", " + real_row_counter + "] =  " + data[c]);

                        textData_items_ui[real_col_counter, real_row_counter] = data[c]; //Minus one to start with 0 in the Data Text array --- !!TAKE CARE WITH THE EMPTY SPACES OF THE CSV WHEN SOME LINE HAVE ID AND NOT TEXT (For Example...)

                        //Parse the data for every column with a mod of it (Otherwise, the data will be raw, like if we have 50 x 6 columns of data, the counter of columns there will match 300 columns of data)
                        real_col_counter++;

                        if (real_col_counter == columns_items_ui)
                            real_col_counter = 0;
                        //Parse the data for every column with a mod of it (Otherwise, the data will be raw, like if we have 50 x 6 columns of data, the counter of columns there will match 300 columns of data)

                    }


                }
                real_row_counter++;
            }
        }
    }
}
