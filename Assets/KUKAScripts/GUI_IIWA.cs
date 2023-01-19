using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Threading;

public class GUI_IIWA : MonoBehaviour
{
    private KUKA_IIWA_LIB2 iiwa;
    public string iiwa_IP = "172.31.1.147";
    public int iiwa_port = 30001;

    private static int toggleConnect = 0;
    private int toggleRecord = 0;

    private Button recordBtn;
    private Button getPositionBtn;
    private Button savePathBtn;
    private Button loadPathBtn;
    private Button playPathBtn;

    private Button emergencyStopBtn;

    public Button clearWpBtn;
    public GameObject clearWpBtnGO;

    // PANELS
    public GameObject loadPathsPanel;
    public GameObject savePathPanel;
    public GameObject waypointsPanel;

    // DROPDOWNS
    public Dropdown impDrop;
    public Dropdown DOFDrop;
    public Dropdown AmplitudeDrop;
    public Dropdown FreqDrop; 
    public Dropdown angleDrop; 
    public Dropdown angleSpeedDrop; 

    public Dropdown followLineImpDrop; 

    // TOGGLES
    public Toggle normal_toggle;
    public Toggle sine_toggle;
    public Toggle rotation_toggle;
    public Toggle follow_line_toggle;

    // INPUT FIELDS
    public InputField savePathInputField;
    //public InputField waypointImpInputField;
    private Text connectBtnText;
    private Text recordBtnText;
    private Text numRepsText;
    private Text numVelText;
    public Text numWaypointVelText;
    private static String msg = "";
    private List<Vector3> path_positions;
    private List<Vector3> path_rotations;

    private Boolean isRepeatingPath = false;

    private GameObject sphere;

    private List<GameObject> pathList;

    String pathWaypoints = "wp,";

    private string nameOfPath = "";

    LineRenderer lr;

    int startWaypointIdx = -1, endWaypointIdx = -1;
    GameObject startWaypointObj, endWaypointObj;

    void Awake() {
        iiwa = new KUKA_IIWA_LIB2(iiwa_IP, iiwa_port);
    }

    // Start is called before the first frame update
    void Start() { 
        // testing create path
        connectBtnText = GameObject.Find("BtnConnect").GetComponentInChildren<Text>();
        recordBtnText = GameObject.Find("BtnRecordPath").GetComponentInChildren<Text>();
        numRepsText = GameObject.Find("NumReps").GetComponent<Text>();
        numVelText = GameObject.Find("NumStartVel").GetComponent<Text>();

        recordBtn = GameObject.Find("BtnRecordPath").GetComponent<Button>();
        savePathBtn = GameObject.Find("BtnSavePath").GetComponent<Button>();
        loadPathBtn = GameObject.Find("BtnLoadPath").GetComponent<Button>();
        playPathBtn = GameObject.Find("BtnPlayPath").GetComponent<Button>();
        emergencyStopBtn = GameObject.Find("EmergencyStopBtn").GetComponent<Button>(); 

        pathList = new List<GameObject>();

        follow_line_toggle.onValueChanged.AddListener(delegate {
            if(follow_line_toggle.isOn) {
                followLineImpDrop.interactable = true;
                impDrop.interactable = false;
            } else {
                followLineImpDrop.interactable = false;
                impDrop.interactable = true;
            }
        });

        
        sine_toggle.onValueChanged.AddListener(delegate {
            if(sine_toggle.isOn) {
                DOFDrop.interactable = true;
                AmplitudeDrop.interactable = true;
                FreqDrop.interactable = true;
                impDrop.interactable = false;
                angleDrop.interactable = false;
                angleSpeedDrop.interactable = false;
                normal_toggle.isOn = false;
                rotation_toggle.isOn = false;
            } 
        });

        normal_toggle.onValueChanged.AddListener(delegate {
            if(normal_toggle.isOn) {
                DOFDrop.interactable = false;
                AmplitudeDrop.interactable = false;
                FreqDrop.interactable = false;
                impDrop.interactable = true;
                angleDrop.interactable = false;
                angleSpeedDrop.interactable = false;
                sine_toggle.isOn = false;
                rotation_toggle.isOn = false;
            } 
        });

        rotation_toggle.onValueChanged.AddListener(delegate {
            if(rotation_toggle.isOn) {
                DOFDrop.interactable = false;
                AmplitudeDrop.interactable = false;
                FreqDrop.interactable = false;
                impDrop.interactable = false;
                angleDrop.interactable = true;
                angleSpeedDrop.interactable = true;
                normal_toggle.isOn = false;
                sine_toggle.isOn = false;
            } 
        });

        lr = new GameObject().AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.SetWidth(.02f, .02f);
        lr.SetColors(Color.red, Color.red);
     }


    public void ConnectRobot() {
        if(toggleConnect == 0)
            iiwa.startTCPConnection();
        else
            iiwa.closeTCPConnection();
    }

    public void EmergencyStopRobot() {
        iiwa.emergencyStopRobot();
    }

    public void GetRobotPosition() {
        iiwa.getRobotCartPosition();
    }

    public void RecordRobot() {
        if(toggleRecord == 0) 
            iiwa.recordPath();
        else 
            iiwa.stopRecordPath();
    }

    public void SaveRobotPath() {
        nameOfPath = "";
        savePathPanel.SetActive(true);
    }

    public void CloseSaveRobotPath() {
        savePathPanel.SetActive(false);
    }

    public void ReallySaveRobotPath() {
        nameOfPath = savePathInputField.text;
        if(nameOfPath != "") {
            savePathPanel.SetActive(false);
            iiwa.getRecordPathPositions();
        } else {
            Debug.Log("Error: name file of the robot path!");
        }
    }


    public void GetRobotPositionOnLoop() {
        while(isRepeatingPath == true) {
            iiwa.getRobotCartPosition();
            Thread.Sleep(100);
        }  
    }

    public void LoadRobotPath() {
        loadPathsPanel.SetActive(true);
    }

    public void closeLoadPathList() {
        loadPathsPanel.SetActive(false);
    }

    public void increaseRepsBtn() {
        int reps = int.Parse(numRepsText.text);
        reps++;
        numRepsText.text = reps.ToString();
    }

    public void decreaseRepsBtn() {
        int reps = int.Parse(numRepsText.text);
        if(reps > 1) {
            reps--;
            numRepsText.text = reps.ToString();
        }
    }

    public void increaseVelBtn() {
        string v1 = numVelText.text.Replace("%", "");
        int vel = int.Parse(v1);
        if(vel < 100) {
            vel+=5;
            numVelText.text = vel.ToString() + "%";
        }
    }

    public void decreaseVelBtn() {
        string v1 = numVelText.text.Replace("%", "");
        int vel = int.Parse(v1);
        if(vel > 5) {
            vel-=5;
            numVelText.text = vel.ToString() + "%";
        }
    }

    public void increaseWaypointVelBtn() {
        string v1 = numWaypointVelText.text.Replace("%", "");
        int vel = int.Parse(v1);
        if(vel < 100) {
            vel+=5;
            numWaypointVelText.text = vel.ToString() + "%";
        }
    }

    public void decreaseWaypointVelBtn() {
        string v1 = numWaypointVelText.text.Replace("%", "");
        int vel = int.Parse(v1);
        if(vel > 5) {
            vel-=5;
            numWaypointVelText.text = vel.ToString() + "%";
        }
    }

    GameObject kukaPos;
    public void PlayRobotPath() {
        emergencyStopBtn.interactable = true;

        String r = numRepsText.text;
        String v = numVelText.text.Replace("%", "");
        Debug.Log(pathWaypoints);

        iiwa.playbackPath(r, v, pathWaypoints);

        if(kukaPos != null)
            Destroy(kukaPos);
        
        kukaPos = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kukaPos.name = "KUKA_POSITION";
        //kukaPos.transform.position = path_positions[0];
        kukaPos.transform.localScale = new Vector3(scaleSphere, scaleSphere, scaleSphere);
        kukaPos.GetComponent<Renderer>().material.color = Color.green;

        Thread threadGetPosition = new Thread(GetRobotPositionOnLoop);
        isRepeatingPath = true;
        threadGetPosition.Start();
  
        recordBtn.interactable = false;
        loadPathBtn.interactable = false;
    }

    float sc = 8f; // scale of path in Unity
    void showPathOnScreen(String p) {
        string[] s = p.Split(',');

        path_positions = new List<Vector3>();
        path_rotations = new List<Vector3>();

        for(int i = 0; i < s.Length - 1; i+=6) {
            float x = float.Parse(s[i], CultureInfo.InvariantCulture);
            float y = float.Parse(s[i+1], CultureInfo.InvariantCulture);
            float z = float.Parse(s[i+2], CultureInfo.InvariantCulture);
            float a = float.Parse(s[i+3], CultureInfo.InvariantCulture);
            float b = float.Parse(s[i+4], CultureInfo.InvariantCulture);
            float c = float.Parse(s[i+5], CultureInfo.InvariantCulture);
            //Debug.Log(x + "," + y + "," + z + "," + a + "," + b + "," + c);

            path_positions.Add(new Vector3(x/1000 * sc, y/1000 * sc, z/1000 * sc));
            path_rotations.Add(new Vector3(a, b, c));
        }
        
        //Reseta a lista
        foreach(GameObject gO in pathList){
            Destroy(gO);
        }

        pathList.Clear();

        // draw path
        int ii = 0;
        foreach(Vector3 pos in path_positions) {
            CreatePoint(pos, ii);
            ii++;
        }
    }

    float scaleSphere = 0.07f; // size of waypoint ball
    void CreatePoint(Vector3 pos, int ii) {
        GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tmp.name = "Sphere_" + ii;
        tmp.transform.position = pos;
        tmp.transform.localScale = new Vector3(scaleSphere, scaleSphere, scaleSphere);

        pathList.Add(tmp);
    }

    public void loadPathCallback(String m) {
        Debug.Log("clicked:" + m);

        string path = "Assets/ROBOT_PATHS/" + m;
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path); 
    
        String msg = reader.ReadToEnd();
        // load on screen
        showPathOnScreen(msg);

        iiwa.sendRobotPath(msg);

        loadPathsPanel.SetActive(false);
        // start play button
        playPathBtn.interactable = true;

        reader.Close();
    }

    /*
    private void OnDrawGizmos() {
        // draw path
        var PathLineColor = Color.red;
        var PathPointsColor = Color.green;
        if(path_positions != null) { 
            for (int i = 0; i < path_positions.Count - 1; i++) {
                if(path_positions[i] != null) { 
                    Gizmos.color = PathLineColor;
                    Gizmos.DrawLine(path_positions[i], path_positions[i + 1]);

                    //Gizmos.color = PathPointsColor;
                    //Gizmos.DrawSphere(path_positions[i], 0.023f);
                }
            }

            //Gizmos.color = PathPointsColor;
            //Gizmos.DrawSphere(path_positions[path_positions.Count - 1], 0.023f);
        }
    }
    */


    public void clearWaypoints() {
        clearWpBtnGO.SetActive(false);

        pathWaypoints = "wp,";

        for(int i = 0; i < pathList.Count; i++) {
            GameObject go = pathList[i];
            go.GetComponent<Renderer>().material.color = Color.white;
            go.transform.localScale = new Vector3(scaleSphere, scaleSphere, scaleSphere);
        }
    }


    public void openWaypointsPanel() {
        // set waypoint velocity equal to overall velocity
        string v1 = numVelText.text.Replace("%", "");
        int vel = int.Parse(v1);
        numWaypointVelText.text = vel.ToString() + "%";

        waypointsPanel.SetActive(true);
    }

    public void closeWaypointsPanel() {
        waypointsPanel.SetActive(false);

        startWaypointIdx = -1; 
        endWaypointIdx = -1;

        startWaypointObj.GetComponent<Renderer>().material.color = Color.white;
        startWaypointObj.transform.localScale = new Vector3(scaleSphere, scaleSphere, scaleSphere);

        endWaypointObj.GetComponent<Renderer>().material.color = Color.white;
        endWaypointObj.transform.localScale = new Vector3(scaleSphere, scaleSphere, scaleSphere);
    }

    
    
    public void setWaypoint() {
        int start = startWaypointIdx, end = endWaypointIdx;
        if(startWaypointIdx > endWaypointIdx) {
            end = startWaypointIdx;
            start = endWaypointIdx;
        }

        string vWaypoint = numWaypointVelText.text.Replace("%", "");
        
        string imp = "3000", type_imp = "", dof = "_", freq = "-1", 
            amp = "-1", angle = "0", angleSpeed = "-1", followImp = "-1";

        if(sine_toggle.isOn) {
            type_imp = "sine";
            dof = DOFDrop.options[DOFDrop.value].text;
            dof = dof.Split(char.Parse(" "))[0];

            freq = FreqDrop.options[FreqDrop.value].text;
            freq = freq.Split(char.Parse(" "))[0];

            amp = AmplitudeDrop.options[AmplitudeDrop.value].text;
            amp = amp.Split(char.Parse(" "))[0];

        } else if(normal_toggle.isOn) {
            imp = impDrop.options[impDrop.value].text;
            imp = imp.Split(char.Parse(" "))[0];

            type_imp = "normal";

            if(follow_line_toggle.isOn) {
                type_imp = "follow_line";
                followImp = followLineImpDrop.options[followLineImpDrop.value].text;
                followImp = followImp.Split(char.Parse(" "))[0];
            }

        } else if(rotation_toggle.isOn) {
            type_imp = "rotation";

            angle = angleDrop.options[angleDrop.value].text;
            angle = angle.Split(char.Parse(" "))[0];

            angleSpeed = angleSpeedDrop.options[angleSpeedDrop.value].text;
            angleSpeed = angleSpeed.Split(char.Parse(" "))[0];
        }  

        Debug.Log("Wp " + start + " -> " + end + ", imp:" + imp + ", vel:" + 
                 vWaypoint + "%" + ", type:" + type_imp +  ", DOF: " + dof + ", amp: " + amp + ", freq: " + 
                 freq + ", angle: " + angle + ", angleSpeed: " + angleSpeed + ", followImp: " + followImp);

        pathWaypoints += start + "," + end + "," + vWaypoint + "," + type_imp + "," + imp + "," + 
                dof + "," + freq + "," + amp + "," + angle + "," + angleSpeed + "," + followImp + ",";

        // close window and mark the path
        startWaypointIdx = -1; 
        endWaypointIdx = -1;

        startWaypointObj.GetComponent<Renderer>().material.color = Color.green;
        startWaypointObj.transform.localScale = new Vector3(scaleSphere+0.01f, 
            scaleSphere+0.01f, scaleSphere+0.01f);

        endWaypointObj.GetComponent<Renderer>().material.color = Color.green;
        endWaypointObj.transform.localScale = new Vector3(scaleSphere+0.01f, 
            scaleSphere+0.01f, scaleSphere+0.01f);

        waypointsPanel.SetActive(false);

        clearWpBtnGO.SetActive(true);
    }


    int cRepeat = 0;
    // Update is called once per frame
    void Update() { 

         //Check for mouse click on sphere (waypoint)
        if (Input.GetMouseButtonDown(0) && !isRepeatingPath) {
             RaycastHit raycastHit;
             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
             if (Physics.Raycast(ray, out raycastHit, 100f)) {
                 if (raycastHit.transform != null) {
                    if(raycastHit.transform.name.Contains("Sphere")) {
                        GameObject auxGO = raycastHit.transform.gameObject;

                        Color objColor = auxGO.GetComponent<Renderer>().material.color;
                        Color testColor = Color.blue;

                        string[] dataSplit = raycastHit.transform.name.Split('_');
                        int _idx = int.Parse(dataSplit[1]);

                        // already clicked!
                        if(objColor.Equals(testColor)) {
                            auxGO.GetComponent<Renderer>().material.color = Color.white;
                            auxGO.transform.localScale = new Vector3(scaleSphere, scaleSphere, scaleSphere);

                            // clear
                            if(_idx == startWaypointIdx) {
                                startWaypointIdx = -1;
                                startWaypointObj = null;
                            } 
                            else if(_idx == endWaypointIdx) {
                                endWaypointIdx = -1;
                                endWaypointObj = null;
                            }

                        } else if(startWaypointIdx == -1 || endWaypointIdx == -1) {
                            auxGO.GetComponent<Renderer>().material.color = Color.blue;
                            auxGO.transform.localScale = new Vector3(scaleSphere + 0.02f, 
                                scaleSphere + 0.02f, scaleSphere + 0.02f);
                            
                            if(startWaypointIdx == -1) {
                                startWaypointIdx = _idx;
                                startWaypointObj = auxGO;
                                Debug.Log("startWaypointIdx:" + _idx);
                            } else if (endWaypointIdx == -1) {
                                endWaypointIdx = _idx;
                                endWaypointObj = auxGO;
                                Debug.Log("endWaypointIdx:" + _idx);
                                openWaypointsPanel();
                            }

                        } 
                    } // if detected sphere
                }
            }
        } // end mouse click


        // draw line between waypoints
        if(pathList != null) {
            if(pathList.Count != 0) {
                lr.positionCount = 0;
                lr.positionCount = pathList.Count;
                lr.SetPosition(0, pathList[0].transform.position);
                for(int i = 0; i < pathList.Count; i++) {
                    if (pathList[i] != null) {
                        lr.SetPosition(i, pathList[i].transform.position);
                    } 
                }
            }
        }
        


        if(msg != "") {
            //Debug.Log(msg);
            string[] dataSplit = msg.Split(',');

            if(dataSplit[0] == "unity_connected") {
                toggleConnect = 1;
                connectBtnText.text = "Disconnect";
                recordBtn.interactable = true;
                playPathBtn.interactable = true;
                
            }

            else if (dataSplit[0] == "unity_not_connected") {
                toggleConnect = 0;
                connectBtnText.text = "Connect";
                recordBtn.interactable = false;
            }

            else if (dataSplit[0] == "position") {
                float x = float.Parse(dataSplit[2], CultureInfo.InvariantCulture);
                float y = float.Parse(dataSplit[3], CultureInfo.InvariantCulture);
                float z = float.Parse(dataSplit[4], CultureInfo.InvariantCulture);
                float a = float.Parse(dataSplit[5], CultureInfo.InvariantCulture);
                float b = float.Parse(dataSplit[6], CultureInfo.InvariantCulture);
                float c = float.Parse(dataSplit[7], CultureInfo.InvariantCulture);

                Vector3 currPos = new Vector3(x/1000 * sc, y/1000 * sc, z/1000 * sc);
                Vector3 currRot = new Vector3(a, b, c);

                if(isRepeatingPath) {
                    kukaPos.transform.position = currPos;
                    cRepeat++;
                } else {
                    if(cRepeat > 0) {
                        Debug.Log("last update!!!");
                        kukaPos.transform.position = currPos;
                        cRepeat = 0;
                    }
                    Debug.Log("getting cart pos");
                }
            }

            else if(dataSplit[0] == "robot_recording") {
                Debug.Log("Robot recording!");
                toggleRecord = 1;
                recordBtnText.text = "Stop";
                savePathBtn.interactable = false;
            }

            else if(dataSplit[0] == "robot_stop_recording") {
                Debug.Log("Robot Stop recording!");
                toggleRecord = 0;
                recordBtnText.text = "Record";
                savePathBtn.interactable = true;
            }

            else if(dataSplit[0] == "get_path") {
                // save to file
                Debug.Log("saving path to file");

                //DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                //int currentEpochTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;
                //string path = "Assets/ROBOT_PATHS/path_" + currentEpochTime.ToString() + ".txt";

                string path = "Assets/ROBOT_PATHS/" + nameOfPath + ".txt";

                StreamWriter writer = new StreamWriter(path, true);

                String res = msg.Replace("get_path,", "");

                writer.WriteLine(res);
                writer.Close();
            }

            else if(dataSplit[0] == "robot_done_repeating_path") { 
                isRepeatingPath = false;
                recordBtn.interactable = true;
                loadPathBtn.interactable = true;
                emergencyStopBtn.interactable = false;
            }

            msg = "";
        }


     }

    public static void robotResponseCallback(String rcv) {
        msg = rcv;
    }

    void OnApplicationQuit() {
        Debug.Log("Application ending after " + Time.time + " seconds");
        if(iiwa.getConnectionStatus())
            iiwa.closeTCPConnection();
    }

}
