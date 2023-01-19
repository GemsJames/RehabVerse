using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class KUKA_IIWA_LIB
{
    public TcpClient client;
	public string hostIp;
    public int hostPort;
    private bool isConnected = false;
	private bool isInImpedance = false;

    StreamReader reader;
    StreamWriter writer;

    public String received = "";

    public ExampleRobotControl erc;

    public KUKA_IIWA_LIB(string IP, int port)
    {
        hostIp = IP;
        hostPort = port;
    }


    public void startTCPConnection() {
        new Thread (() => 
        {
            try {
                Debug.Log("Connecting to KUKA...");
                client = new TcpClient(hostIp, hostPort);
                reader = new StreamReader(client.GetStream());
                writer = new StreamWriter(client.GetStream());
                isConnected = true;

                erc.processes.Add("connected");
            } 
            catch(Exception ex) {
                Debug.Log("Exception thrown");  
                throw ex; 
            }
        }).Start(); 
    }

    void sendTCPMsg(String msg) {
        String rcv = "";
        new Thread (() => 
        {
            try {
                writer.WriteLine(msg);
                writer.Flush();
                rcv = reader.ReadLine();
                erc.robotResponseCallback(rcv); 
            } 
            catch(Exception ex) {
                Debug.Log("Exception thrown");  
                throw ex; 
            }
        }).Start(); 
    }

    public void closeTCPConnection() {
        sendTCPMsg("close");
        if(received != "") {
            reader.Close();
            writer.Close();
            client.Close();
            isConnected = false;
        } else {
            Debug.Log("Error on closing client!");
        }
    }

    public void shutdownRobot() {
        sendTCPMsg("end");
        if(received != "") {
            reader.Close();
            writer.Close();
            client.Close();
            isConnected = false;
        } else {
            Debug.Log("Error on shutdown robot!");
        }
    }


    public bool getImpedanceState() {
        return isInImpedance;
    }

    public void setImpedanceState(Boolean state) {
        isInImpedance = state;
    }

    public bool getConnectionStatus() {
        return isConnected;
    }

    public void testCommunication() {
        sendTCPMsg("hello");
    }

    public void sendRobotToHomePosition() {
        sendTCPMsg("ptp_home");
    }

    public void sendRobotToStartPosition() {
        sendTCPMsg("ptp_work");
    }

    public void recordPath() {
        isInImpedance = true;
        sendTCPMsg("record_path");
    }

    public void stopRecordPath() {
        isInImpedance = false;
        sendTCPMsg("stop_path");
    }

    public void playbackPath() {
        sendTCPMsg("repeat_path");
    }

    public void getRecordPathPositions() {
        sendTCPMsg("get_path");
    }

    public void getRobotPosition() {
        sendTCPMsg("get_cart_position");
    }

    public void calibrateRobot() {
        sendTCPMsg("calibrate_exercise");
    }

    public void sendRobotCartPos(string x, string y, string z) {
        String cmd = "goto_" + x + "_" + y + "_" + z;
        sendTCPMsg(cmd);
    }

    public void initializeRobot() {
        sendTCPMsg("init_1"); 
    }

    public void initializeRobot2() {
        sendTCPMsg("init_2"); 
    }

}

