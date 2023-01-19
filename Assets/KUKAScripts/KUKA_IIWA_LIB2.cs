using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class KUKA_IIWA_LIB2
{
    public TcpClient client;
	public string hostIp;
    public int hostPort;
    private Boolean isConnected = false;
	private Boolean isInImpedance = false;

    StreamReader reader;
    StreamWriter writer;

    public String received = "";

    public KUKA_IIWA_LIB2(string IP, int port)
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
                checkRobotConnection();
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
                GUI_IIWA.robotResponseCallback(rcv);
            } 
            catch(Exception ex) {
                Debug.Log("Exception thrown");  
                throw ex; 
            }
        }).Start(); 
    }

    public void closeTCPConnection() {
        sendTCPMsg("close");
        Debug.Log("Closing connection with robot!!");
        isConnected = false;
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


    public Boolean getImpedanceState() {
        return isInImpedance;
    }

    public void setImpedanceState(Boolean state) {
        isInImpedance = state;
    }

    public Boolean getConnectionStatus() {
        return isConnected;
    }

    public void checkRobotConnection() {
        Debug.Log("check is connected");
        sendTCPMsg("is_connected");
    }

    public void sendRobotToHomePosition() {
        sendTCPMsg("ptp_home");
    }

    public void recordPath() {
        isInImpedance = true;
        sendTCPMsg("record_path");
    }

    public void stopRecordPath() {
        isInImpedance = false;
        sendTCPMsg("stop_path");
    }

    public void playbackPath(String reps, String v, String wps) {
        sendTCPMsg("repeat_path," + reps + "," + v + "," + wps);
    }

    public void getRecordPathPositions() {
        sendTCPMsg("get_path");
    }

    public void getRobotCartPosition() {
        sendTCPMsg("get_cart_position");
    }

    public void sendRobotPath(String m) {
        sendTCPMsg("load_path," + m);
    }

    public void emergencyStopRobot() {
        sendTCPMsg("emergency_stop");
    }

}

