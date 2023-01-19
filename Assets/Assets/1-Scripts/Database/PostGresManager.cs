using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Npgsql;
using System;
using System.Threading;
using UnityEngine.UI;
using System.Data;

public class PostGresManager : MonoBehaviour
{
    public string host = "localhost";
    public string port = "5432";
    public string dataBase = "test";
    public string user = "postgres";
    public string password = "sh2021";

    NpgsqlConnection connection;

    Thread connThread;

    public List<Patient> newPatients;

    public InputField fntxt;
    public InputField lntxt;
    public InputField gtxt;
    public InputField dobtxt;
    public InputField etxt;

    public Text dataText;
    string dbresult;
    bool updateData = false;

    // Start is called before the first frame update
    void Start()
    {
        string connString = String.Format("Server={0};Username={1};Database={2};Port={3};Password={4};SSLMode=Prefer", host, user, dataBase, port, password);

        ConnectToDatabase(connString);

        Invoke("AddData", 5);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (updateData)
        {
            updateData = false;
            dataText.text = dbresult;
        }
    }

    public void AddData()
    {
        List<Patient> temp = new List<Patient>();

        foreach(Patient p in newPatients)
        {
            temp.Add(p);
        }

        newPatients.Clear();

        AddDataToDataBase(temp);
    }

    public void ConnectToDatabase(string connString)
    {
        new Thread(() =>
        {
            try
            {
                Debug.Log("Connecting to the Database...");
                connection = new NpgsqlConnection(connString);
                connection.Open();
                Debug.Log("Connected to the Database.");
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                throw ex;
            }
        }).Start();
    }

    public void AddDataToDataBase(List<Patient> patList)
    {
        new Thread(() =>
        {
            try
            {
                Debug.Log("Sending data to the Database...");

                string commandString = "insert into Patient (first_name, last_name, gender, date_of_birth, email) values";

                for(int i = 0; i < patList.Count - 1; i++)
                {
                    commandString +=
                    "('" + patList[i].first_name +
                    "', '" + patList[i].last_name +
                    "', '" + patList[i].gender +
                    "', '" + patList[i].date_of_birth +
                    "', '" + patList[i].email + "'), ";
                }

                commandString +=
                    "('" + patList[patList.Count - 1].first_name +
                    "', '" + patList[patList.Count - 1].last_name +
                    "', '" + patList[patList.Count - 1].gender +
                    "', '" + patList[patList.Count - 1].date_of_birth +
                    "', '" + patList[patList.Count - 1].email + "')";

                NpgsqlCommand sendCommand = new NpgsqlCommand(commandString, connection);

                sendCommand.ExecuteNonQuery();

                Debug.Log("Done sending data to the Database.");
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                throw ex;
            }
        }).Start();
    }

    public void CreatePatient()
    {
        Patient patient = ScriptableObject.CreateInstance<Patient>();

        patient.first_name = fntxt.text;
        patient.last_name = lntxt.text;
        patient.gender = gtxt.text;
        patient.date_of_birth = dobtxt.text;
        patient.email = etxt.text;

        newPatients.Add(patient);

        fntxt.text = "";
        lntxt.text = "";
        gtxt.text = "";
        dobtxt.text = "";
        etxt.text = "";
    }

    public void UpdateTable()
    {
        new Thread(() =>
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Patient;", connection);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
            DataTable table = new DataTable();
            DataSet ds = new DataSet();
            da.Fill(ds);
            table = ds.Tables[0];
            dbresult = "";

            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    dbresult += table.Rows[i].ItemArray[j].ToString() + " | ";
                }
                dbresult += "\n";
            }

            updateData = true;

        }).Start();

    }

}
