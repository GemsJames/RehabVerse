using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Threading;

public class PlayerMovement : MonoBehaviour {
    private KukaRobotLib robot;

    public float forwardForce;
    public float sidewaysForce;
    public float jumpForce = 5f;

    public bool isOnGround = true;

    public Rigidbody rb;

    private static string robotMsg = "";

    private Boolean isRobotOn = false;

    private Boolean isRobotMoving = false;

    float robotYPos = 0f;
    float robotStartYPos;

    int countFirst = 0;


    void Awake() {
        robot = new KukaRobotLib();
    }

    // Start is called before the first frame update
    void Start() {
        Debug.Log("start connection!");
        robot.startTCPConnection();
    }

    // Update is called once per frame
    void FixedUpdate() {

        if(robotMsg != "") {
            //Debug.Log(robotMsg);
            string[] dataSplit = robotMsg.Split(',');

            if(dataSplit[0] == "unity_connected") {
                isRobotOn = true;
                robot.moveRobot();

                isRobotMoving = true;
                robot.getRobotCartPosition();
            }

            else if (dataSplit[0] == "position") {
                float x = float.Parse(dataSplit[1], CultureInfo.InvariantCulture);
                float y = float.Parse(dataSplit[2], CultureInfo.InvariantCulture);
                float z = float.Parse(dataSplit[3], CultureInfo.InvariantCulture);
                //float a = float.Parse(dataSplit[4], CultureInfo.InvariantCulture);
                //float b = float.Parse(dataSplit[5], CultureInfo.InvariantCulture);
                //float c = float.Parse(dataSplit[6], CultureInfo.InvariantCulture);

                y = (y / 1000);
                y = (float)((Mathf.Round(y * 100)) / 100.0);

                //Debug.Log("y: " + y);
                //Vector3 currPos = new Vector3(x, y, z);
                if(countFirst == 0) {
                    robotStartYPos = y;
                } else
                    robotYPos = robotStartYPos - y;

                countFirst = 1;

                if(isRobotMoving) {
                    robot.getRobotCartPosition();
                }

            }

            robotMsg = "";
        } // end robotMsg processing
   
        if(isRobotMoving) {
            
            rb.AddForce(0, 0, forwardForce * Time.deltaTime);

            if(Input.GetKey("d")) {
                rb.AddForce(sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
            }
            else if(Input.GetKey("a")) {
                rb.AddForce(-sidewaysForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
            }
            else if(Input.GetKey("w") && isOnGround) {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isOnGround = false;
            }

            Debug.Log("robotYPos: " + (-robotYPos * 2f));
            Vector3 temp = new Vector3(-robotYPos * 2f, 0, 0);
            transform.position += temp;

            //rb.AddForce(-robotYPos * 10, 0, 0, ForceMode.VelocityChange);


            if(rb.position.y < -0.5f) {
                //robot.stopRobot();
                //isRobotMoving = false;
                FindObjectOfType<GameManagerV>().endGame();
                
            }

        }

    }


    private void OnCollisionEnter(Collision other) {
        if(other.collider.tag == "Obstacle") {
            //forwardForce = 0f;
            //robot.stopRobot();
            //isRobotMoving = false;
            Debug.Log("game over : obstacle!");
            FindObjectOfType<GameManagerV>().endGame();
        }

        if(other.collider.tag == "Ground") {
            isOnGround = true;
        }

    }


    public void GetRobotPositionOnLoop() {
        while(isRobotMoving == true) {
            robot.getRobotCartPosition();
            Thread.Sleep(30);
        }  
    }


    public static void robotResponseCallback(String rcv) {
        robotMsg = rcv;
    }

    public void closeConnection() {
        if(robot.getConnectionStatus())
                robot.closeTCPConnection();
    }

    void OnApplicationQuit() {
        try {
            closeConnection();
        } 
        catch(Exception ex) {
            Debug.Log("Error closing tcp connection: " + ex);
        }
        Debug.Log("Application ending after " + Time.time + " seconds");
    }


}
