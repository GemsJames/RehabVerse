using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;
using System.Collections.Generic;


public class ExampleRobotControl : MonoBehaviour
{
    private KUKA_IIWA_LIB iiwa;

    public string iiwa_IP = "172.31.1.147";
    public int iiwa_port = 30001;

    private int btnWidth1 = 180;
    private int btnWidth2 = 150;
    private int toggleBtn = 0;

    public string cartPos = "20.0,0.0,0.0";

    RobConTester rct;

    public List<string> processes;

    public UnityEvent<string> calibrateEvent;
    public UnityEvent<string> positionEvent;

    bool firstPosGet = true;
    bool recording = false;

    public WorldAligner worldAligner;

    // Start is called before the first frame update
    void Start()
    {
        if (iiwa == null)
        {
            iiwa = new KUKA_IIWA_LIB(iiwa_IP, iiwa_port);
            iiwa.erc = this;
        }

        firstPosGet = true;
        recording = false;
        rct = GetComponent<RobConTester>();
        processes = new List<string>();
    }
    void Update()
    {
        if (processes.Count > 0)
        {
            Decoder(processes);
        }
    }

    void OnGUI()
	{
        GUI.enabled = !iiwa.getConnectionStatus();
        if(GUI.Button (new Rect (10,10,btnWidth1,40), "Connect to KUKA")) 
        {
            iiwa.startTCPConnection();
		}

        GUI.enabled = iiwa.getConnectionStatus();
        if(GUI.Button (new Rect (200,10,btnWidth1,40), "Close KUKA Server")) 
        {
            iiwa.shutdownRobot();
		}

        GUI.enabled = !iiwa.getImpedanceState() && iiwa.getConnectionStatus();

        if(GUI.Button (new Rect (10,80,btnWidth2,40), "Send hello"))
		{
            iiwa.testCommunication();
		}

		if(GUI.Button (new Rect (10,130,btnWidth2,40), "Send robot home"))
		{
            iiwa.sendRobotToHomePosition();
		}

        if(GUI.Button (new Rect (10,180,btnWidth2,40), "Send robot start"))
		{
            iiwa.sendRobotToStartPosition();
		}

        GUI.enabled = iiwa.getConnectionStatus();
		if(GUI.Button (new Rect (10,230,btnWidth2,40), "Record/Stop Path"))
		{
			if(toggleBtn % 2 == 0) {
                Debug.Log(toggleBtn);
                iiwa.recordPath();
			} else {
				Debug.Log(toggleBtn);
                iiwa.stopRecordPath();
			} 
			toggleBtn++;
		} 
        

        GUI.enabled = !iiwa.getImpedanceState() && iiwa.getConnectionStatus();
        if(GUI.Button (new Rect (200,80,btnWidth2,40), "Repeat Movement"))
		{
            iiwa.playbackPath();
		}

         if(GUI.Button (new Rect (200,130,btnWidth2,40), "Get Record Path"))
		{
            iiwa.getRecordPathPositions();
		}

        GUI.enabled = iiwa.getConnectionStatus();
        if(GUI.Button (new Rect (200,180,btnWidth2,40), "Get Cart Position"))
		{
            iiwa.getRobotPosition();
		}

        GUI.enabled = !iiwa.getImpedanceState() && iiwa.getConnectionStatus();
        if(GUI.Button (new Rect (200,230,btnWidth2,40), "Calibrate"))
		{
            iiwa.calibrateRobot();
		}

        cartPos = GUI.TextField(new Rect(390, 80, btnWidth2-30, 40), cartPos, 25);

        if(GUI.Button (new Rect (390 + btnWidth2-30,80 ,btnWidth2/2,40), "Go to"))
		{
            string[] splitArray =  cartPos.Split(char.Parse(","));
            Debug.Log(splitArray[0]);
            Debug.Log(splitArray[1]);
            Debug.Log(splitArray[2]);
            iiwa.sendRobotCartPos(splitArray[0], splitArray[1], splitArray[2]);
		}

        if(GUI.Button (new Rect (390, 130, btnWidth2,40), "Init_1"))
		{
            iiwa.initializeRobot();
		}

        if(GUI.Button (new Rect (390, 180, btnWidth2,40), "Init_2"))
		{
            iiwa.initializeRobot2();
		}

       
		
	} // end OnGUI() function

    public void ConnectToRobot()
    {
        if (!iiwa.getConnectionStatus())
        {
            processes.Add("connect");
        }
    }

    public void DisconnectToRobot()
    {
        if (iiwa.getConnectionStatus())
        {
            processes.Add("disconnect");
        }
    }

    public void SendHome()
    {
        iiwa.sendRobotToHomePosition();
    }

    public void StartRecording()
    {
        if (!recording)
        {
            iiwa.recordPath();
            recording = true;
        }
    }

    public void StopRecording()
    {
        if (recording)
        {
            iiwa.recordPath();
            recording = !true;
        }
    }

    public void RepeatPath()
    {
        iiwa.playbackPath();
    }

    public void robotResponseCallback(String rcv)
    {
        //Debug.Log("CALLBACK");

        processes.Add(rcv);

        //Debug.Log(rcv); 
    }

    void Decoder(List<string> strings)
    {
        List<string> tempList = new List<string>(strings);
        strings.Clear();

        foreach (string str in tempList)
        {
            StringProcessor(str);
        }

    }

    void StringProcessor(string str)
    {
        string[] splitArray = str.Split(char.Parse(","));

        string output = "";

        for (int i = 1; i < splitArray.Length - 1; i++)
        {
            output += splitArray[i] + ",";
        }

        output += splitArray[splitArray.Length - 1];

        switch (splitArray[0])
        {
            //Initiating Sequence

            case "connect":
                iiwa.startTCPConnection();
                break;

            case "connected":
                iiwa.initializeRobot();
                break;

            case "init_1":
                rct.Initialise1(output);
                break;

            case "init_2":
                rct.Initialise2(output);
                break;

            case "disconnect":
                iiwa.shutdownRobot();
                break;

            //Initiating Sequence End

            case "calibrate":
                calibrateEvent.Invoke(output);
                break;

            case "position":
                positionEvent.Invoke(output);
                break;

            case "origin":

                break;


            default:
                Debug.Log("Unrecognized message: " + splitArray[0]);
                break;
        }
    }

    public void Calibrate()
    {
        iiwa.calibrateRobot();
    }

    public void GetPosition()
    {
        iiwa.getRobotPosition();
    }

    public void Init2()
    {
        Debug.Log("Initialize 2");
        iiwa.initializeRobot2();
    }

    // ---------------- //
    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        iiwa.closeTCPConnection();
    }

}
