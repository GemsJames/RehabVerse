using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class UDPClient : MonoBehaviour {

	public UdpClient client;
	public IPAddress serverIp;
	public string hostIp = "172.31.1.147";
    public int hostPort = 30001;
    //public string hostIp = "127.0.0.1";
	//public int hostPort = 9600;
	public IPEndPoint hostEndPoint;

	private int toggleImpedance = 0;
	private Boolean isInImpedance = false;

	private int btnWidth = 200;

	// x,y,z que é posição do robô
	//vector3 pos

	// receber pos cartesianas sempre que user pedir ou de x em x tempo
	// receber força da flange como string e converter para vetor3 e retornar assim
	// getPositions(), getForce(), getForceAndPositions()

	[Range(1, 6)]
    public float integerRange;

	// goTo(vetor3) ir para posiçao cart especifica
	// goTo(vetor3, speed) ir para posiçao cart especifica com speed limitada
	// startRecording() e stopRecording() modo teaching

	// savePath() depois do stopRecording()

	// sendPath(), indica qual o path/exercicio atual

	// startFollowing(float impedanceFreedom) usar o limitador

	// getCurrentLimits() ir buscar os valores de impedancia atuais

	// changeRobotHelpSpeed(float speed) muda a velocidade a que o robo ajuda a puxar pelo caminho

	// setForce(vetor3 force) - dar força para sempre

	// vetor worldPos
	// vetor globalPos 


	void Start() {
		serverIp = IPAddress.Parse(hostIp);
		hostEndPoint = new IPEndPoint(serverIp,hostPort);
        try {
            client = new UdpClient();
            client.Connect(hostEndPoint);
            client.Client.Blocking = false;
        } 
        catch(Exception ex) {
             Debug.Log("Exception thrown");  
             throw ex; 
        }

	}

	public void SendDatagram(string msg){
		byte[] sendPacket = Encoding.UTF8.GetBytes(msg);
        try {
            client.Send(sendPacket,sendPacket.Length);
		    client.BeginReceive(new AsyncCallback(processDatagram),client);
        }
        catch (Exception ex) {
            Debug.Log("Exception thrown: Error on sending packet");  
			throw ex;
		}
	}

	public void processDatagram(IAsyncResult res){
		try {
			byte[] receivedPacket = client.EndReceive(res,ref hostEndPoint);
			Debug.Log(Encoding.UTF8.GetString(receivedPacket));
		} catch (Exception ex) {
            Debug.Log("Exception thrown: Error on receiving packet");  
			throw ex;
		}
	}

	void OnGUI()
	{
		GUI.enabled = !isInImpedance;
        if(GUI.Button (new Rect (10,10,btnWidth,40), "Send hello"))
		{
			SendDatagram("hello from unity client");
			toggleImpedance = 0;
		}

		if(GUI.Button (new Rect (10,60,btnWidth,40), "Send ptp_home"))
		{
			SendDatagram("ptp_home");
			toggleImpedance = 0;
		}

        if(GUI.Button (new Rect (10,110,btnWidth,40), "Send ptp_work"))
		{
			SendDatagram("ptp_work");
			toggleImpedance = 0;
		}

        if(GUI.Button (new Rect (10,160,btnWidth,40), "Close connection"))
		{
			SendDatagram("end");
			toggleImpedance = 0;
		}

		GUI.enabled = true;

		if(GUI.Button (new Rect (10,210,btnWidth,40), "Toggle Impedance control"))
		{
			if(toggleImpedance % 2 == 0) {
				isInImpedance = true;
				//Debug.Log("imp_1");
				SendDatagram("imp_1");
		
			} else {
				isInImpedance = false;
				SendDatagram("imp_0");
			} 
			toggleImpedance++;
		} 

		
	} // end OnGUI() function

}