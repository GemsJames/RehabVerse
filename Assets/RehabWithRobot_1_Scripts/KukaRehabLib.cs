using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class KukaRehabLib  {
    public TcpClient client;
	public string hostIp;
    public int hostPort;

    // TODO // 
    // timeout deve ser 2x o delay do tempo de resposta normal usado + pequeno desvio (fazer estudo)
    private int fixedTimeout = 2000; // putting connection and reading response from KUKA timeouts

    private Boolean isConnected = false;

    StreamReader reader;
    StreamWriter writer;

    public String received = "";

    public KukaRehabLib(string IP, int port) {
        hostIp = IP;
        hostPort = port;
    }

    public void startTCPConnection(int t_out) {
        // set timeout of connection using a fixed timeout calculated or a longer timeout set by the user
        int timeout = fixedTimeout;
        if(t_out > fixedTimeout) timeout = t_out;

        new Thread (() => 
        {
            try {
                Debug.Log("Connecting to KUKA...");
                client = new TcpClient();

                if (!client.ConnectAsync(hostIp, hostPort).Wait(timeout)) {
                    // connection failure
                    RehabUI.robotResponseCallback("couldnt_connect");
                    Debug.Log("TCP CLIENT CONNECTION FAILED!");
                } else {
                    reader = new StreamReader(client.GetStream());
                    reader.BaseStream.ReadTimeout = timeout;

                    writer = new StreamWriter(client.GetStream());
                    isConnected = true;
                    checkRobotConnection();
                }

            } 
            catch(Exception ex) {
                Debug.Log("Exception thrown: " + ex); 
                RehabUI.robotResponseCallback("couldnt_connect"); 
                throw ex; 
            }
        }).Start(); 
    }

    void sendTCPMsg(String msg) {
        String rcv = "";
        new Thread (() => 
        {
            try {
                //Debug.Log(msg);
                writer.WriteLine(msg);
                writer.Flush();
                rcv = reader.ReadLine(); 
                //Debug.Log("rcv: " + rcv);
                RehabUI.robotResponseCallback(rcv);
            } 
            catch(Exception ex) {
                if(isConnected == false)
                    RehabUI.robotResponseCallback("couldnt_connect"); 
                    
                Debug.Log("Exception thrown :: " + ex.ToString());  
                throw ex; 
            }
        }).Start(); 
    }

    public void closeTCPConnection() {
        sendTCPMsg("close_TCP_connection");
        //Debug.Log("Closing connection with robot!!");
        isConnected = false;
    }


    public Boolean getConnectionStatus() {
        return isConnected;
    }

    public void checkRobotConnection() {
        //Debug.Log("check is connected");
        sendTCPMsg("is_connected");
    }

    public void pingRobot(long timestamp) {
        sendTCPMsg("check_connection," + timestamp);
    }

    public void createExercise() {
        sendTCPMsg("create_exercise");
    }

    public void stopCreatingExercisePath() {
        sendTCPMsg("stop_exercise");
    }

    public void getButtonState() {
        sendTCPMsg("get_button_pressed");
    }

    public void sendRobotPath(String m) {
        sendTCPMsg("load_path," + m);
    }

    public void setupPath(String wps) {
        sendTCPMsg("setup_path," + wps);
    }

    public void playPath(String reps, String v) {
        sendTCPMsg("play_path," + reps + "," + v);
    }

    public void testPath() {
        sendTCPMsg("test_path");
    }

    public void getPathWithSampling(int dist) {
        sendTCPMsg("get_path_with_sampling," + dist.ToString());
    }

    public void getFullPath() {
        sendTCPMsg("get_full_path");
    }

    public void getRobotCartPosition() {
        sendTCPMsg("get_cart_position");
    }

    public void getRobotPositionForceReps() {
        sendTCPMsg("get_pos_force_reps");
    }

    public void getIdxsPath() {
        sendTCPMsg("get_idxs");
    }

    public void getTypeOfExerciseInPath() {
        sendTCPMsg("get_type_of_exercise");
    }

    public void emergencyStopRobot() {
        sendTCPMsg("emergency_stop");
    }

    public void goToBegin() {
        sendTCPMsg("loaded_go_to_begin");
    }

    public void stopGoToBegin() {
        sendTCPMsg("stop_go_to_begin_done");
    }

    public void getDistToBegin() {
        sendTCPMsg("is_at_begin");
    }


    // TESTES PINGS
    public void getStopTimeErrorConn() {
        sendTCPMsg("get_stop_time_error_conn");
    }

    // ROBOT INFO
    public void getRobotVelExercise() {
        sendTCPMsg("get_robot_vel_exercise");
    }

    public void getJointsPosExercise() {
        sendTCPMsg("get_joints_pos_exercise");
    }

    public void getToolTorqueExercise() {
        sendTCPMsg("get_tool_torque_exercise");
    }

    public void getToolMomentumExercise() {
        sendTCPMsg("get_tool_momentum_exercise");
    }

    public void setMaxForceConfigs(float maxForce, float maxSeconds) {
        sendTCPMsg("max_force_configs," + maxForce + "," + maxSeconds);
    }

    public void getRobotInfo() {
        sendTCPMsg("get_current_robot_info");
    }

}
