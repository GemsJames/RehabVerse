using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;


public class TCPClient : MonoBehaviour
{
    public TcpClient client;
	public string hostIp = "172.31.1.147";
    public int hostPort = 30005;

	private int toggleImpedance = 0;
	private Boolean isInImpedance = false;

	private int btnWidth1 = 180;
    private int btnWidth2 = 150;

    StreamReader reader;
    StreamWriter writer;

	void Start() {
        
	}

    void startTCPConnection() {
        try {
            Debug.Log("Connecting to KUKA...");
            client = new TcpClient(hostIp, hostPort);
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
        } 
        catch(Exception ex) {
            Debug.Log("Exception thrown");  
            throw ex; 
        }
    }


    String sendTCPMsg(String msg) {
        writer.WriteLine(msg);
        writer.Flush();
        return reader.ReadLine();
    }


    void closeTCPConnection() {
        reader.Close();
        writer.Close();
        client.Close();
    }


	void OnGUI()
	{
		GUI.enabled = !isInImpedance;

        if(GUI.Button (new Rect (10,10,btnWidth1,40), "Connect to KUKA")) {
            startTCPConnection();
		}
        if(GUI.Button (new Rect (200,10,btnWidth1,40), "Close KUKA Server")) {
            String received = sendTCPMsg("end");
            Debug.Log(received);
            closeTCPConnection();
            toggleImpedance = 0;
		}

       
        if(GUI.Button (new Rect (10,80,btnWidth2,40), "Send hello"))
		{
            String received = sendTCPMsg("hello");
            Debug.Log(received);
			toggleImpedance = 0;
		}

		if(GUI.Button (new Rect (10,130,btnWidth2,40), "Send ptp_home"))
		{
            String received = sendTCPMsg("ptp_home");
            Debug.Log(received);
			toggleImpedance = 0;
		}

        if(GUI.Button (new Rect (10,180,btnWidth2,40), "Send ptp_work"))
		{
            String received = sendTCPMsg("ptp_work");
            Debug.Log(received);
			toggleImpedance = 0;
		}

         
		GUI.enabled = true;
		if(GUI.Button (new Rect (10,230,btnWidth2,40), "Record/Stop Path"))
		{
			if(toggleImpedance % 2 == 0) {
                
				isInImpedance = true;
                Debug.Log(toggleImpedance);
                String received = sendTCPMsg("record_path");
                Debug.Log(received);
		
			} else {
				isInImpedance = false;
                Debug.Log(toggleImpedance);
                String received = sendTCPMsg("stop_path");
                Debug.Log(received);
			} 
			toggleImpedance++;
		} 
        

        GUI.enabled = !isInImpedance;
        if(GUI.Button (new Rect (200,80,btnWidth2,40), "Repeat Movement"))
		{
            String received = sendTCPMsg("repeat_path");
            Debug.Log(received);
			toggleImpedance = 0;
		}

         if(GUI.Button (new Rect (200,130,btnWidth2,40), "Get Record Path"))
		{
            String received = sendTCPMsg("get_path");
            Debug.Log(received);
			toggleImpedance = 0;
		}


        if(GUI.Button (new Rect (200,180,btnWidth2,40), "Get Cart Position"))
		{
            String received = sendTCPMsg("get_cart_position");
            Debug.Log(received);
			toggleImpedance = 0;
		}

       
		
	} // end OnGUI() function


    void OnDestroy() {
        String received = sendTCPMsg("end");
        Debug.Log(received);
        closeTCPConnection();
    }

}
