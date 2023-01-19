using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class KukaRobotLib {

    private TcpClient client;
	private string hostIp = "172.31.1.147";
    private int hostPort = 30001;
    private Boolean isConnected = false;

    StreamReader reader;
    StreamWriter writer;

    private String received = "";

    public void startTCPConnection() {
        new Thread (() => {
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
        new Thread (() => {
            try {
                writer.WriteLine(msg);
                writer.Flush();
                rcv = reader.ReadLine(); 
                PlayerMovement.robotResponseCallback(rcv);
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

    public Boolean getConnectionStatus() {
        return isConnected;
    }

    public void checkRobotConnection() {
        Debug.Log("check is connected");
        sendTCPMsg("is_connected");
    }
    
    public void moveRobot() {
        sendTCPMsg("start");
    }

    public void stopRobot() {
        sendTCPMsg("stop");
    }

    public void getRobotCartPosition() {
        sendTCPMsg("get_cart_pos");
    }


}

