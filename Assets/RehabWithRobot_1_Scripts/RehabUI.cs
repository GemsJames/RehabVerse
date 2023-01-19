using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using System.Threading;
using Random=UnityEngine.Random;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Diagnostics;


public class RehabUI : MonoBehaviour {
    // KUKA CONFIGS/VARS
    private KukaRehabLib iiwa;
    private string iiwa_IP = "172.31.1.147";
    //private string iiwa_IP = "192.168.137.200";
    private int iiwa_port = 30001;
    private static String msg = ""; // tcp msg from robot

    // PANELS
    public GameObject startPanel;
    public GameObject menuPanel;
    public GameObject exerciseHelpPanel;
    public GameObject exerciseFreeModePanel;
    public GameObject exerciseRecordingPanel;
    public GameObject exercisePlayPanel;
    public GameObject wpSelectPanel;
    public GameObject impPanel;
    public GameObject sinePanel;
    public GameObject rotationPanel;
    public GameObject configMovPanel;
    public GameObject statusMovPanel;
    public GameObject saveRobotPathPanel;
    public GameObject loadRobotPathPanel;
    public GameObject loadReportPanel;
    public GameObject saveReportPanel;
    public GameObject reportsPanel;
    public GameObject reportsForcePanel;
    public GameObject forceInfoPanel;
    public GameObject impGamePanel;

    public GameObject setMaxForcePanel;

    public GameObject helpCreateExercisePanel;
    public GameObject helpPlayPanel;

    // BUTTONS
    public Button connectBtn;
    public Button stopRecordingBtn;
    public Button stopPlayPathBtn;
    public GameObject clearWpBtnGO;
    public Button playPathBtn;
    public Button testingPathBtn;
    public Button incStartVel, decStartVel, incReps, decReps;
    public Button showForceReportBtn;
    public Button saveRobotPathBtn;
    public Button loadExerciseBtn;
    public Button saveReportBtn;
    public Button loadReportsBtn;
    public Button newExerciseBtn;
    public Button removeForcesBtn;
    public Button impGameBtn;

    public Button setMaxForceBtn;

    // TEXT
    public Text numRepsText;
    public Text numVelText;
    public Text numImpVelText;
    public Text numSineVelText;
    public Text numRotationVelText;
    public Text numImpGameVelText;
    public Text numRepsFinalText;
    public Text numActualForceText;
    public Text numMaxForceText;
    public Text repStatusText;
    public Text numVelFinalText;
    public Text numProgressText;
    public GameObject pingTxtGO;
    public GameObject errorTxt;
    public GameObject forcePointTxtGO;
    public GameObject waypointInfoTxtGO;
    public Text forceInfoTypeText;
    public Text forceInfoMaxText;
    public Text forceInfoMeanText;
    public Text forceInfoNumPointsText;
    public Text hintTxt;

    public Text mainHintTxt;

    //public GameObject coinsText;
    public GameObject scorePHText;
    public GameObject scoreText;

    // DROPDOWNS
    public Dropdown impDrop;
    public Dropdown DOFDrop;
    public Dropdown AmplitudeDrop;
    public Dropdown FreqDrop; 
    public Dropdown angleDrop; 
    public Dropdown angleSpeedDrop; 
    public Dropdown savedPathsDropDown;
    public Dropdown savedReportsDropDown;
    public Dropdown repsDropdown;
    public Dropdown impGameImpedanceDropdown;
    public Dropdown impGameCoinShiftDropdown;
    public Dropdown impGameCoinPosDropdown;

    // INPUTFIELD
    public InputField savePathInputField;
    public InputField saveReportInputField;
    public InputField redZoneInputField;

    public InputField forceLimitInputField, timeLimitInputField;
    

    // CIRCUIT
    private List<Vector3> path_positions;
    private List<Vector3> path_rotations;
    private Boolean isRepeatingPath = false;
    

    // DRAW CIRCUIT ON SCREEN
    LineRenderer lr; 
    public GameObject pathLine; // gameobject which has the line to draw the circuit


    // WAYPOINTS
    private List<GameObject> pathList; // holder of waypoints (spheres) on the circuit 
    String pathWaypoints = "wp,"; // string holder for configs on waypoints
    int startWaypointIdx = -1, endWaypointIdx = -1;
    GameObject startWaypointObj, endWaypointObj;

    float sc = 6f; // scale of path in Unity
    float scaleSphere = 0.06f; // size of waypoint ball

    GameObject kukaPos; // robot actual position on circuit

    int cRepeat = 0; // last update on drawing position


    // RANDOM
    Boolean isInFreeMode = false;

    string olderButtonState = "no_btn", actualButtonState = "";

    Boolean activateWaypoints = false;

    private List<int> pathIdxs;

    string wpMode = ""; // mode choosed for waypoint


    // FORCE
    double maxForce = -1f; // monitor max force while playing path
    private List<List<Vector3>> path_force; // raw force comming from robot

    private List<Vector3> rep_force; // one rep force

    private List<string> rep_success;

    List<Vector3> force_on_path; // list of force points with the waypoint to the force direction 
    Boolean draw_force = false; // draw force on screen
    Boolean toggleShowForceBtn = false;
    List<LineRenderer> force_lines; // linerenderer holder

    List<float> path_force_mags;

    Boolean onPlayPanel = false;
    Boolean onReportPanel = false;

    private List<Vector3> path_midpoint; 

    Vector3 cameraDefaultPos = new Vector3(5.2f, 0.9f, -2.65f);
    Quaternion cameraDefaultRot = Quaternion.Euler(-3f, -57f, 89f);

    private List<Vector3> all_path_points; 

    string nameOfPath = "";

    List<GameObject> sphereForceList;

    string robot_saved_paths = "Assets/RehabWithRobot_1_Scripts/Paths/";
    string robot_saved_reports = "Assets/RehabWithRobot_1_Scripts/Reports/";

    int CIRCUIT_SIZE = 0;
    
    GameObject centerSphere;
    float centerSphere_x, centerSphere_y, centerSphere_z; // vars to get center point of path

    float defaultForceToDisplayOnScreen = 2.2f; // min force of the mean of 1 rep (forward->reverse) to show on screen
    //float defaultForceToDisplayOnScreen = 10f;

    float minForceToDisplayOnScreen = -1; 

    int samplingDist = 20; // dist between points in milimeters

    Boolean isRobotConnected = false;

    public int tcpTimeout = 3000;

    public class TOE {
        public string toe_type;
        public int toe_start, toe_end;
        public TOE(string t, int i1, int i2) {
            toe_type = t;
            toe_start = i1;
            toe_end = i2;
        }
    }

    private List<TOE> type_of_exercises_list; 

    List<LineRenderer> path_lines; // linerenderer holder

    int oldRep = 1;

    int stopGettingForces = 0;

    String holdGetPathWithSampling = "";

    int isExerciseLoaded = 0;

    Ping ping;
    List<int> pingList;

    List<int> timeRobotEStopList;

    long currentPing, unixTimeUnity;

    string isOnPlayOrTest = "test";

    int diffUnityRobotTime, diffUnityRobotEStop;

    private float currTime;

    long unixTimeSendEStop;

    public Sprite testBtnImage1, testBtnImage2;

    public Button testBtn;

    // PATHS
    string pingsPath = "Assets/RehabWithRobot_1_Scripts/Pings/const_pings_router_stress_1.txt";
    string eStopPingsPath = "Assets/RehabWithRobot_1_Scripts/Pings/estop_pings_router_stress_20.txt";
    string errorPingsPath = "Assets/RehabWithRobot_1_Scripts/Pings/error_pings_router_stress_20.txt";
    
    GameObject testCrash = null;

    int stopBtnPressed = 0;

    [HideInInspector]
    public int totalReps;
    int coinsScore = 0;
    int coinsGot = 0;

    int restartReps = 0;

    int repsDone = 0;

    string waypointsIdxsGUI = "wp_idx_gui,";

    string robotInfoExercise = "";


    string currentActionForPings = "N/A";

    Color _orange = new Color(1.0f, 0.27f, 0.0f);

    public Camera camera2;

    public int renderLayer;

    public UnityEvent newRepEvent;

    [System.Serializable]
    public class Segment{
        public int start;
        public int end;
        public string mode;
        public Segment(int s, int e, string m) {
            start = s;
            end = e;
            mode = m;
        }
    }

    List<Segment> waypointsSegments;


    string robotInfoCurrPath = "";

    //string robot_info_path = "Assets/RehabWithRobot_1_Scripts/RobotInfo/v_";
    //string robot_info_path = "Assets/RehabWithRobot_1_Scripts/TrainingInfo2/robotInfo_";

    string robot_info_path = "Assets/RehabWithRobot_1_Scripts/TI10/ri_";

    string unityDelayPath  = "Assets/RehabWithRobot_1_Scripts/unity_delay.txt";


    List<int> unityDelayArray;

    void Awake() {
        iiwa = new KukaRehabLib(iiwa_IP, iiwa_port);
   
        EventTrigger trigger = testingPathBtn.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((data) => { OnPointerUpDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry);

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerDown;
        entry2.callback.AddListener((data) => { OnPointerDownDelegate((PointerEventData)data); });
        trigger.triggers.Add(entry2);

        oldTime = Time.time;
    }


    public void OnPointerDownDelegate(PointerEventData data){
        if(testingPathBtn.interactable) {
            //Debug.Log("On");
            TestRobotPath();
        }
    }


    public void OnPointerUpDelegate(PointerEventData data){
        //Debug.Log("Off");
        if((isExerciseLoaded == 1 || stopBtnPressed == 1) && isRepeatingPath) {
            //Debug.Log("STOPPPPPPPPPPPPPPPPPPP!!!");
            currentActionForPings = "idle";
            iiwa.stopGoToBegin();
            isRepeatingPath = false;
        }

    }

    
    void getConnectionState() {
        while(isRobotConnected && toggle) {
            DateTime foo = DateTime.Now;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeMilliseconds();

            unixTimeUnity = unixTime;
            iiwa.pingRobot(unixTime);
            Thread.Sleep(270);
        }
    }


    // Start is called before the first frame update
    void Start() {
        //connectBtn = GameObject.Find("BtnConnect").GetComponent<Button>();

        pathList = new List<GameObject>();
        path_force = new List<List<Vector3>>();
        rep_force  = new List<Vector3>();
        force_on_path = new List<Vector3>();
        path_force_mags = new List<float>();
        path_midpoint = new List<Vector3>();
        all_path_points = new List<Vector3>();
        sphereForceList = new List<GameObject>();
        pathIdxs = new List<int>();
        timeRobotEStopList = new List<int>();
        waypointsSegments = new List<Segment>();

        unityDelayArray = new List<int>();

        rep_success = new List<string>();

        pathLine = GameObject.Find("PathLine");

        lr = pathLine.GetComponent<LineRenderer>();
        lr.gameObject.layer = renderLayer;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.SetWidth(.02f, .02f);
        lr.SetColors(Color.blue, Color.blue);

        force_lines = new List<LineRenderer>();

        path_lines = new List<LineRenderer>();

        type_of_exercises_list = new List<TOE>();


        // check if can load reports
        DirectoryInfo dir_reports = new DirectoryInfo(robot_saved_reports);
        FileInfo[] info_reports = dir_reports.GetFiles("*.txt");
        if(info_reports.Length > 0) {
            loadReportsBtn.interactable = true;
        } else loadReportsBtn.interactable = false;

    }

    int toggleCoin = 0;
    int counter = 0;

    double actualForce = 0;
    int toggleDisplayPercent = 0;
    public float displayPercent = 0;

    [HideInInspector]
    public float elapsedTime;
    float oldTime;
    public float completion;
    public int distance;
    int oldpoint;
    public float desvioDoCaminho = -1;
    //int currScore = 0;

    // Update is called once per frame

    public System.DateTime startTime;
    private float fire_start_time;



    void FixedUpdate() {

        startTime = System.DateTime.UtcNow;
        fire_start_time = Time.time;


        counter++;
        if(counter > 9999) counter = 0;

        /*
        if(toggle) {
            Debug.Log("wait");
            Thread.Sleep(2000);
            Debug.Log("done");
            toggle = false;
        }
        */

        /*
        // rotate coin
        foreach(GameObject c in gameCoins) {
            float xRotation = c.transform.eulerAngles.x;
            
            if(xRotation > 88) toggleCoin = 1;
            else if(xRotation < 1) toggleCoin = 0;

            if(toggleCoin == 1) xRotation-=0.1f;
            if(toggleCoin == 0) xRotation+=0.1f;

            c.transform.eulerAngles = new Vector3(xRotation, c.transform.eulerAngles.y, transform.eulerAngles.z);
        }
        */

        // GET COINS
        if (isRepeatingPath && coinCount > 0) {
            
            Vector3 kuka = kukaPos.transform.position;
            foreach(GameObject fooObj in GameObject.FindGameObjectsWithTag("coin")) {
                //Debug.Log("fooObj.name: " + fooObj.name);
                if(fooObj.name.Contains("Coin_")) {
                    double dist = euclidianDistance(kuka, fooObj.transform.position) * 1000;

                    double distY = Math.Round(Math.Abs(kuka.y - fooObj.transform.position.y) * 1000);
                    double distX = Math.Round(Math.Abs(kuka.x - fooObj.transform.position.x)* 1000);

                    //Debug.Log(fooObj.name + " -> " + dist + ",  x " + distX + ", y " + distY);

                    if(dist < 200) {
                        Debug.Log(fooObj.name + " -> " + dist + ",  x " + distX + ", y " + distY);
                    }
                    /*
                    // TODO - COINS
                    int score = 0;
                    if(dist < 200)
                        score = mapScore(dist);

                    coinsScore = score;
                    */

                    
                    //if(counter % 25 == 0)
                    //    scoreText.GetComponentInChildren<Text>().text = "" + coinsScore;
                    
                    if(dist < 120 || (distX < 60 && distY < 60)) {
                        coinsGot++;
                        scoreText.GetComponentInChildren<Text>().text = coinsGot + "/" + coinCount;
                        fooObj.SetActive(false);
                        break;
                    }
                    
 
                } // single coin
            } // loop coins
        } // while robot is moving



        //Check for mouse click on sphere (waypoint)
        if (Input.GetMouseButtonDown(0) && !isRepeatingPath && activateWaypoints) {
             RaycastHit raycastHit;
             Ray ray = camera2.ScreenPointToRay(Input.mousePosition);
             if (Physics.Raycast(ray, out raycastHit, 100f)) {
                 if (raycastHit.transform != null) {
                    if(raycastHit.transform.name.Contains("Sphere")) {
                        GameObject auxGO = raycastHit.transform.gameObject;

                        Color objColor = auxGO.GetComponent<Renderer>().material.color;
                        Color testColor = _orange;

                        string[] dataSplit = raycastHit.transform.name.Split('_');
                        int _idx = int.Parse(dataSplit[1]);

                        int allow = 1;
                        for(int i = 0; i < waypointsSegments.Count; i++) {
                            // point is between a segment do not allow to write!!
                            if(_idx >= waypointsSegments[i].start && _idx <= waypointsSegments[i].end) {
                                allow = 0;
                                Debug.Log("POINT IS ON THE INSIDE OF ANOTHER SEGMENT!");
                                break;
                            }
                            // point is overlapping a smaller segment
                            if(_idx <= waypointsSegments[i].start && _idx >= waypointsSegments[i].end) {
                                allow = 0;
                                Debug.Log("POINT IS ON THE INSIDE OF ANOTHER SEGMENT!");
                                break;
                            }

                            int s = startWaypointIdx,e = _idx;
                            if(startWaypointIdx != -1) {
                                if(startWaypointIdx > _idx){
                                    s = _idx;
                                    e = startWaypointIdx;
                                }

                                if(s <= waypointsSegments[i].start && e >= waypointsSegments[i].end) {
                                    allow = 0;
                                    Debug.Log("SEGMENT CHOOSED IS OVERLAPPING ANOTHER SEGMENT!");
                                    break;
                                }

                            }
                        }

                        // already clicked!
                        if(objColor.Equals(testColor) && allow == 1) {
                            auxGO.GetComponent<Renderer>().material.color = Color.blue;
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

                        } else if((startWaypointIdx == -1 || endWaypointIdx == -1) && allow == 1) {
                            auxGO.GetComponent<Renderer>().material.color = _orange;
                            auxGO.transform.localScale = new Vector3(scaleSphere + 0.02f, 
                                scaleSphere + 0.02f, scaleSphere + 0.02f);
                        
                            if(startWaypointIdx == -1) {
                                startWaypointIdx = _idx;
                                startWaypointObj = auxGO;
                                //Debug.Log("startWaypointIdx:" + _idx);
                            } else if (endWaypointIdx == -1) {
                                endWaypointIdx = _idx;
                                endWaypointObj = auxGO;
                                //Debug.Log("endWaypointIdx:" + _idx);
                                wpSelectPanel.SetActive(true);

                                if(Math.Abs(endWaypointIdx - startWaypointIdx) == 2)
                                    impGameBtn.interactable = true;
                                else {
                                    impGameBtn.interactable = false;
                                }    
                            }
         
                        } 
                    } // if detected sphere
                }
            }
        } // end mouse click


        // show type of waypoint
        if (activateWaypoints || isExerciseLoaded == 1) {
             RaycastHit raycastHit;
             Ray ray = camera2.ScreenPointToRay(Input.mousePosition);
             if (Physics.Raycast(ray, out raycastHit, 100f)) {
                 if (raycastHit.transform != null) {
                    if(raycastHit.transform.name.Contains("Sphere")) {
                        GameObject auxGO = raycastHit.transform.gameObject;

                        string[] dataSplit = raycastHit.transform.name.Split('_');
                        int _idx = int.Parse(dataSplit[1]);

                        string m_mode = "";
                        for(int i = 0; i < waypointsSegments.Count; i++) {
                            Debug.Log(waypointsSegments[i].start + "," + waypointsSegments[i].end + "," + waypointsSegments[i].mode);
                            if(_idx == waypointsSegments[i].start || _idx == waypointsSegments[i].end) {
                                m_mode = waypointsSegments[i].mode;
                                break;
                            }
                        }

                        if(m_mode != "") {
                            string mode_res = "";
                            if(m_mode == "sine") mode_res = "Sine Overlay";
                            else if(m_mode == "imp") mode_res = "Impedance";
                            else if(m_mode == "rotation") mode_res = "Rotation Overlay";
                            else if(m_mode == "imp_game") mode_res = "Impedance Game";

                            waypointInfoTxtGO.GetComponent<Text>().text = mode_res;
                            waypointInfoTxtGO.SetActive(true);
                        } 
                        else waypointInfoTxtGO.SetActive(false);
                        

                    } // end if raycast is not force sphere
                } // end if raycast is null
            } // end raycast cast 
            else waypointInfoTxtGO.SetActive(false);

        } // final end


        // move circuit with arrow keys
        if(Input.GetKey(KeyCode.UpArrow) && (onPlayPanel || onReportPanel)) {
            Debug.Log("arrow up");
            Vector3 newPos = new Vector3(camera2.transform.position.x + 0.01f, 
                camera2.transform.position.y, camera2.transform.position.z);

            camera2.transform.position = newPos;
        }
        else if(Input.GetKey(KeyCode.DownArrow) && (onPlayPanel || onReportPanel)) {
            Debug.Log("arrow down");
            Vector3 newPos = new Vector3(camera2.transform.position.x - 0.01f, 
                camera2.transform.position.y, camera2.transform.position.z);

            camera2.transform.position = newPos;
        }
        else if(Input.GetKey(KeyCode.LeftArrow) && (onPlayPanel || onReportPanel)) {
            Debug.Log("arrow left");
            Vector3 newPos = new Vector3(camera2.transform.position.x, 
                camera2.transform.position.y + 0.01f, camera2.transform.position.z);

            camera2.transform.position = newPos;
        }
        else if(Input.GetKey(KeyCode.RightArrow) && (onPlayPanel || onReportPanel)) {
            Debug.Log("arrow right");
            Vector3 newPos = new Vector3(camera2.transform.position.x, 
                camera2.transform.position.y - 0.01f, camera2.transform.position.z);

            camera2.transform.position = newPos;
        }


        // look at circuit
        if(Input.GetMouseButton(1) && (onPlayPanel || onReportPanel)) {
            camera2.transform.RotateAround(GameObject.Find("CenterSphere").transform.position, 
                                    camera2.transform.up,
                                    -Input.GetAxis("Mouse X") * 1f);

            camera2.transform.RotateAround(GameObject.Find("CenterSphere").transform.position, 
                                    camera2.transform.right,
                                    -Input.GetAxis("Mouse Y") * 1f); 
        }

      
        // mouse wheel btn - move circuit
        if(Input.GetMouseButton(2) && (onPlayPanel || onReportPanel)) {
            Vector3 NewPosition = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));

            NewPosition.x *= .02f;
            float d = 1.5f;

            if(camera2.transform.position.x > cameraDefaultPos.x + d)
                NewPosition = new Vector3(0, NewPosition.y, NewPosition.z);

             if(camera2.transform.position.x < -cameraDefaultPos.x - d)
                NewPosition = new Vector3(0, NewPosition.y, NewPosition.z);


            if(camera2.transform.position.y > cameraDefaultPos.y + d)
                NewPosition = new Vector3(NewPosition.x, 0, NewPosition.z);

            if(camera2.transform.position.y < -cameraDefaultPos.y - d)
                NewPosition = new Vector3(NewPosition.x, 0, NewPosition.z);


            if(camera2.transform.position.z > cameraDefaultPos.z + d)
                NewPosition = new Vector3(NewPosition.x, NewPosition.y, 0);

             if(camera2.transform.position.z < -cameraDefaultPos.z - d)
                NewPosition = new Vector3(NewPosition.x, NewPosition.y, 0);

            camera2.transform.position += NewPosition;
        }
   

        // zoom on circuit
        float scrollFactor = Input.GetAxis("Mouse ScrollWheel");
        if (scrollFactor != 0) {
            float fov = camera2.fieldOfView;
            fov += scrollFactor * 10f; 
            fov = Mathf.Clamp(fov, 10, 120);
            camera2.fieldOfView = fov;
        }

        // draw line between waypoints
        if(pathList != null) {
            if(pathList.Count != 0 && path_lines.Count != 0) {
                for(int i = 0; i < pathList.Count-1; i++) {
                    path_lines[i].positionCount = 2;
                    path_lines[i].SetPosition(0, pathList[i].transform.position);
                    path_lines[i].SetPosition(1, pathList[i+1].transform.position);
  
                } // end loop draw
            } 
        }

        // draw force between waypoints
        if(force_on_path != null && draw_force && force_lines.Count > 0) {
            if(force_on_path.Count != 0) {
                for(int i = 0; i < force_lines.Count; i++) {
                    if(path_force_mags[i] > minForceToDisplayOnScreen) {
                        force_lines[i].positionCount = 2;

                        if(isExerciseLoaded == 1) {
                            force_lines[i].SetPosition(0, circuit_full[i]);
                        } else {
                            force_lines[i].SetPosition(0, all_path_points[i]);
                        }

                        force_lines[i].SetPosition(1, force_on_path[i]);
                    } 
                    else {
                        force_lines[i].positionCount = 0;
                    }
                } // end loop draw
            }
        } 
        else if(draw_force == false && force_on_path.Count > 1 && force_lines.Count > 0) {
            for(int i = 0; i < force_lines.Count; i++) {
                force_lines[i].positionCount = 0;
            }
        }

    
        if (draw_force) {
             RaycastHit raycastHit;
             Ray ray = camera2.ScreenPointToRay(Input.mousePosition);
             if (Physics.Raycast(ray, out raycastHit, 100f)) {
                 if (raycastHit.transform != null) {
                    if(raycastHit.transform.name.Contains("ForceSphere")) {
                        GameObject auxGO = raycastHit.transform.gameObject;

                        //auxGO.transform.localScale = new Vector3(scaleSphere, scaleSphere, scaleSphere);

                        string[] dataSplit = raycastHit.transform.name.Split('_');
                        int _idx = int.Parse(dataSplit[1]);

                        //Debug.Log("Mouse is over " + auxGO.name + " with " + path_force_mags[_idx] + "N");
                        
                        double force = Math.Round(path_force_mags[_idx]*10.0)/10.0;

                        forcePointTxtGO.SetActive(true);
                        forcePointTxtGO.GetComponent<Text>().text = "Force Point " + _idx + 
                                " has " + force + "N";

                    } // end if raycast is not force sphere
                } // end if raycast is null
            } // end raycast cast 
            else forcePointTxtGO.SetActive(false);

        } // final end


        // KUKA MSG PROCESSER //
        if(msg != "") {
            //Debug.Log(msg);
            string[] dataSplit = msg.Split(',');

            if(dataSplit[0] == "unity_connected") {

                toggle = true;

                //Debug.Log("CONNECTED!");

                currentActionForPings = "idle";

                //iiwa.getStopTimeErrorConn();

                // start constant ping to robot
                isRobotConnected = true;
                Thread threadGetConnectionState = new Thread(getConnectionState);
                threadGetConnectionState.Start();

                pingList = new List<int>();
                //ping = new Ping(iiwa_IP);
                //StartCoroutine(PingUpdate());

                StartCoroutine(PingUpdateDisplay());

                pingTxtGO.SetActive(true);

                newExerciseBtn.interactable = true;

                // check if can load path
                DirectoryInfo dir = new DirectoryInfo(robot_saved_paths);
                FileInfo[] info = dir.GetFiles("*.txt");
                if(info.Length > 0) {
                    loadExerciseBtn.interactable = true;
                } else loadExerciseBtn.interactable = false;

            }

            else if (dataSplit[0] == "robot_still_connected") {
                DateTime foo = DateTime.Now;
                long unixTimeRobot = ((DateTimeOffset)foo).ToUnixTimeMilliseconds();
                //Debug.Log("unity: " + unixTimeUnity + ", robot: " + unixTimeRobot + 
                //    ", diff: " + (unixTimeRobot-unixTimeUnity));

                diffUnityRobotTime = (int)(unixTimeRobot-unixTimeUnity);

                DateTime dt = DateTime.Now;
                string date = dt.ToString("dd/MM/yyyy HH:mm:ss:fff");

                String res = date + ", " + diffUnityRobotTime + ", " + currentActionForPings;
                
                if (!File.Exists(pingsPath)) {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(pingsPath)) {
                        sw.WriteLine(DateTime.Now.ToString() + ", " + "File Created!");
                        sw.WriteLine(res);
                    }   
                } else {
                    // This text is always added, making the file longer over time if it is not deleted.
                    using (StreamWriter sw = File.AppendText(pingsPath)) {
                        sw.WriteLine(res);
                    }   
                }

                pingList.Add(diffUnityRobotTime);
            }

            else if (dataSplit[0] == "couldnt_connect") {
                Debug.Log("communication is not working. Robot stopped!");

                currentActionForPings = "N/A";

                isRobotConnected = false;

                exitButtonPressed();
                connectBtn.interactable = true;
                newExerciseBtn.interactable = false;
                loadExerciseBtn.interactable = false;

                errorTxt.GetComponentInChildren<Text>().text = "Communication error. Please Reconnect!";

                pingTxtGO.SetActive(false);
            }

            // only for TEST MODE
            else if (dataSplit[0] == "position") {
                // dataSplit[1] -> is unix timestamp
                float x = float.Parse(dataSplit[2], CultureInfo.InvariantCulture);
                float y = float.Parse(dataSplit[3], CultureInfo.InvariantCulture);
                float z = float.Parse(dataSplit[4], CultureInfo.InvariantCulture);
                float a = float.Parse(dataSplit[5], CultureInfo.InvariantCulture);
                float b = float.Parse(dataSplit[6], CultureInfo.InvariantCulture);
                float c = float.Parse(dataSplit[7], CultureInfo.InvariantCulture);

                Vector3 currPos = new Vector3(x/1000 * sc, y/1000 * sc, -z/1000 * sc);
                Vector3 currRot = new Vector3(a, b, c);

                if(isRepeatingPath) {
                    kukaPos.transform.position = currPos;
                    cRepeat++;
                } else {
                    if(cRepeat > 0) {
                        //Debug.Log("last update!!!");
                        if(kukaPos != null)
                            kukaPos.transform.position = currPos;
                        cRepeat = 0;
                    }
                    //Debug.Log("getting cart pos");
                }
            }
 
            // for PLAY MODE
            else if (dataSplit[0] == "pos_force_reps") {
                // get the actual rep from robot and update UI

                int actualRep = int.Parse(dataSplit[11]);

                repsDone = actualRep;

                //Debug.Log("actualRep: " + actualRep);

                numRepsFinalText.text = actualRep + " / " + numRepsText.text;
                
                float x = float.Parse(dataSplit[2], CultureInfo.InvariantCulture);
                float y = float.Parse(dataSplit[3], CultureInfo.InvariantCulture);
                float z = float.Parse(dataSplit[4], CultureInfo.InvariantCulture);
                float a = float.Parse(dataSplit[5], CultureInfo.InvariantCulture);
                float b = float.Parse(dataSplit[6], CultureInfo.InvariantCulture);
                float c = float.Parse(dataSplit[7], CultureInfo.InvariantCulture);

                float fx = float.Parse(dataSplit[8], CultureInfo.InvariantCulture);
                float fy = float.Parse(dataSplit[9], CultureInfo.InvariantCulture);
                float fz = float.Parse(dataSplit[10], CultureInfo.InvariantCulture);

                int actualPoint = int.Parse(dataSplit[12]);
                int totalNumPoints = int.Parse(dataSplit[13]);

                elapsedTime = Time.time - oldTime;

                oldTime = Time.time;

                float percentConcluded = ((float)actualPoint/(totalNumPoints-20))*100.0f;

                if(percentConcluded > 0 && percentConcluded < 100 && toggleDisplayPercent == 0) {
                    displayPercent = percentConcluded;
                } 
                else if (percentConcluded >= 100  && toggleDisplayPercent == 0) {
                    displayPercent = 100;
                    toggleDisplayPercent = 1;
                } 
                else if(percentConcluded < 1 && toggleDisplayPercent == 1) {
                    toggleDisplayPercent = 0;
                }

                numProgressText.text = Math.Round(displayPercent) + " %";

                if(actualPoint != oldpoint)
                {
                    distance = actualPoint - oldpoint;

                    oldpoint = actualPoint;
                }

                //Debug.Log("actual point:" + actualPoint);
                //Debug.Log("display Percent:" + displayPercent);

                //Debug.Log("actualPoint: " + actualPoint + " -> totalNumPoints: " + totalNumPoints);
                totalReps = int.Parse(numRepsText.text);

                if(actualRep != oldRep && actualRep <= totalReps) {

                    //comecou novo rep

                    newRepEvent.Invoke();

                    // respawn coins
                    foreach (GameObject gO in gameCoins){
                        gO.SetActive(true);
                    }

                    //Debug.Log("new rep " + actualRep);
                    stopGettingForces = 0;
                    oldRep = actualRep;
                } 
                else {
                    // qd chegar ao fim do circuito e voltar para trás não se recolhe mais forças
                    if(actualPoint >= totalNumPoints-2 && stopGettingForces == 0) {
                        //Debug.Log("stop getting forces for this rep!!!");
                        stopGettingForces = 1;
                        //Debug.Log("rep_force count: "  + rep_force.Count);
                        path_force.Add(new List<Vector3>(rep_force));
                        rep_force.Clear(); 

                        rep_success.Add("ok");

                        // clear coins
                        foreach(GameObject gO in gameCoins){
                            gO.SetActive(false);
                        }
                    }
    
                }

                if (stopGettingForces == 0) {
                    rep_force.Add(new Vector3(fx, fy, fz));
                }

                // force magnitude
                double force = Math.Sqrt(Math.Pow(fx,2) + Math.Pow(fy,2) + Math.Pow(fz,2));

                actualForce = Math.Round(force*10.0)/10.0;

                numActualForceText.text = actualForce + " N";

                // update max force on UI
                if(force > maxForce) {
                    maxForce = force;
                    numMaxForceText.text = numActualForceText.text;
                }

                Vector3 currPos = new Vector3(x/1000 * sc, y/1000 * sc, -z/1000 * sc);
                Vector3 currRot = new Vector3(a, b, c);

                all_path_points.Add(currPos);

                if(isRepeatingPath && kukaPos != null) {
                    kukaPos.transform.position = currPos;
                    cRepeat++;
                } else {
                    if(cRepeat > 0) {
                        //Debug.Log("last update 2!!!");
                        if(kukaPos != null)
                            kukaPos.transform.position = currPos;
                        cRepeat = 0;
                    }
                    //Debug.Log("getting cart pos");
                }

                /*
                // check if robot is at the end of circuit
                double dist = euclidianDistance(currPos, path_positions[path_positions.Count-1]) * 1000;
                if(dist < 9) {
                    Debug.Log("end DIST: " + dist);
                }
                */
            }

            else if(dataSplit[0] == "creating_exercise") {
                //Debug.Log("Robot creating_exercise!");

                Thread threadGetButtonState = new Thread(GetButtonStateOnLoop);
                isInFreeMode = true;
                threadGetButtonState.Start();
            }

            else if(dataSplit[0] == "stop_creating_exercise") {
                Debug.Log("Robot Stop recording!");

                currentActionForPings = "loading_exercise";

                // retrieve simplified path to show on screen
                iiwa.getPathWithSampling(samplingDist);
            }

            else if(dataSplit[0] == "loaded_points_robot") {
                //Debug.Log("Robot LOAD PATH!");

                if(isExerciseLoaded == 1) {
                    testingPathBtn.GetComponentInChildren<Text>().text = "Reposition";
                    testingPathBtn.GetComponentsInChildren<Image>()[1].sprite = testBtnImage2;
                } else {
                    testingPathBtn.GetComponentInChildren<Text>().text = "Evaluate";
                    testingPathBtn.GetComponentsInChildren<Image>()[1].sprite = testBtnImage1;
                }

                currentActionForPings = "loading_exercise";

                iiwa.getPathWithSampling(samplingDist);

                // TODO - REMOVE ?
                /*
                exercisePlayPanel.SetActive(true);
                menuPanel.SetActive(false);
                currentActionForPings = "idle";  
                onPlayPanel = true;
                */
            }

            else if(dataSplit[0] == "get_path_with_sampling") {
                // save to file
                //Debug.Log("getting path");
                String res = msg.Replace("get_path_with_sampling,", "");

                holdGetPathWithSampling = res;

                iiwa.getIdxsPath();
            }

            else if(dataSplit[0] == "is_at_begin") {
                float d = float.Parse(dataSplit[1], CultureInfo.InvariantCulture);
                //Debug.Log("D :: " + d);
                if(d < 1) {
                    doneTest();
                }
                    
            }

            else if(dataSplit[0] == "robot_done_testing_path") { 
                doneTest();

                /*
                Debug.Log("saving report to file");
                // TODO
                DateTime foo = DateTime.Now;
                long unixTimeNow = ((DateTimeOffset)foo).ToUnixTimeMilliseconds();

                //DateTime dt = DateTime.Now;
                //string date = dt.ToString("dd/MM/yyyy HH:mm:ss:fff");
                string nameOfReport = "" + unixTimeNow;

                string path = robot_saved_reports + nameOfReport + ".txt";
                saveRobotPath(path);
                */
            }

            else if(dataSplit[0] == "finished_setup_path") {
                String r = numRepsText.text;
                String v = numVelText.text.Replace("%", "");
                iiwa.playPath(r, v);

                //Debug.Log("iiwa playing path!!");

                DateTime dt = DateTime.Now;
                string filename = "" + dt.ToString("yyyy-MM-dd HH_mm_ss");

                robotInfoCurrPath = robot_info_path + filename + ".txt";

                Thread threadGetPosition = new Thread(GetRobotPositionForceRepsOnLoop);
                Thread threadGetRobotInfo = new Thread(GetRobotInfoOnLoop);
                isRepeatingPath = true;
                threadGetPosition.Start();
                threadGetRobotInfo.Start();


                //scoreFunc();
            }

            else if(dataSplit[0] == "robot_done_repeating_path") {
                Debug.Log("robot_done_repeating_path robot_done_repeating_path robot_done_repeating_path!!");
                currentActionForPings = "idle";

                mainHintTxt.text = "Done! You can now save the exercise report.";

                isRepeatingPath = false;
                stopPlayPathBtn.interactable = false;
                playPathBtn.interactable = true;
                configMovPanel.SetActive(true);
                statusMovPanel.SetActive(false);
                showForceReportBtn.interactable = true;
                saveRobotPathBtn.interactable = true;
                saveReportBtn.interactable = true;

                foreach(GameObject c in gameCoins) {
                    c.SetActive(true);
                }

                playPathBtn.GetComponentInChildren<Text>().text = "Play";

                // if there is waypoints configured, then button to clear is active
                if(pathWaypoints.Length > 10)
                    clearWpBtnGO.SetActive(true);
            }

            else if(dataSplit[0] == "idxs") {
                //Debug.Log(msg);
                pathIdxs.Clear();
                for(int i = 1; i < dataSplit.Length - 1; i++) {
                    //Debug.Log(dataSplit[i]);
                    string idx2 = dataSplit[i].Replace(",", "");
                    int x = int.Parse(idx2);
                    pathIdxs.Add(x);
                }
                // get list of types of exercise
                iiwa.getTypeOfExerciseInPath();
            }

            else if(dataSplit[0] == "no_btn") {
                actualButtonState = "no_btn";
                currentActionForPings = "idle";
            }
            else if(dataSplit[0] == "red_btn") {
                actualButtonState = "red_btn";
                currentActionForPings = "creating_exercise";
            }
            else if(dataSplit[0] == "green_btn") {
                actualButtonState = "green_btn";
                currentActionForPings = "creating_exercise";
            }
            else if(dataSplit[0] == "orange_btn") {
                actualButtonState = "orange_btn";
                currentActionForPings = "creating_exercise";
            }

            // save path to file
            else if(dataSplit[0] == "get_full_path") { 
                //Debug.Log("saving path to file");

                string path = robot_saved_paths + nameOfPath + ".txt";
                StreamWriter writer = new StreamWriter(path, true);

                String res = msg.Replace("get_full_path,", "full_path,");

                res += "all_waypoints," + pathWaypoints;

                res += waypointsIdxsGUI;

                writer.WriteLine(res);
                writer.Close();
            }

            else if(dataSplit[0] == "type_of_exercise") {
                Debug.Log(msg);
                type_of_exercises_list.Clear();
                if(dataSplit.Length > 3) {
                    for(int i = 1; i < dataSplit.Length - 1; i+=3) {
                        // parse ex: "passive,0,10,"
                        string toe_type = dataSplit[i].Replace(",", "");
                        string toe_1 = dataSplit[i+1].Replace(",", "");
                        string toe_2 = dataSplit[i+2].Replace(",", "");
                        int toe_start = int.Parse(toe_1);
                        int toe_end = int.Parse(toe_2);

                        Debug.Log("toe_type: " + toe_type + ", toe_start: " + toe_start + 
                            ", toe_end: " + toe_end);

                        type_of_exercises_list.Add(new TOE(toe_type, toe_start, toe_end));
                    }
                }

                camera2.fieldOfView = 90;

                exercisePlayPanel.SetActive(true);
                exerciseRecordingPanel.SetActive(false);
                menuPanel.SetActive(false);

                showPathOnScreen(holdGetPathWithSampling, null);

                if(waypoints_msg.Length > 8 && isExerciseLoaded == 1) {
                    //Debug.Log("SETTING ALL WAYPOINTS!!");
                    setAllWaypoints(waypoints_msg);
                }
                    
                currentActionForPings = "idle";    

                onPlayPanel = true;
            }

            else if(dataSplit[0] == "robot_stopped_due_to_force_exceeded" || 
                dataSplit[0] == "robot_stopped_weight_diff_high") 
            {
                currentActionForPings = "idle";
                Debug.Log("!! Robot stopped due to force exceeded ot tool weight diff high !!");
                setMaxForceBtn.interactable = true;
                
                path_force.Add(new List<Vector3>(rep_force));
                rep_force.Clear(); 

                restartReps = 1;

                rep_success.Add("nok");

                isRepeatingPath = false;
                stopPlayPathBtn.interactable = false;

                stopBtnPressed = 1;
                testingPathBtn.interactable = true;
                playPathBtn.interactable = false;

                //playPathBtn.interactable = true;
                playPathBtn.GetComponentInChildren<Text>().text = "Continue";

                //configMovPanel.SetActive(true);
                //statusMovPanel.SetActive(false);
                showForceReportBtn.interactable = true;
                saveRobotPathBtn.interactable = true;
                saveReportBtn.interactable = true;

                // if there is waypoints configured, then button to clear is active
                if(pathWaypoints.Length > 10)
                    clearWpBtnGO.SetActive(true);
            }

            else if(dataSplit[0].Contains("kuka_error")) {
                Debug.Log("!!!  KUKA ERROR   !!!");
                Debug.Log("Unity message sent to robot is in incorrect format!!!");
            }

            
            // TESTE PINGS
            else if(dataSplit[0] == "stop_time_error_conn") {
                Debug.Log("stop_time_error_conn");
                Debug.Log(msg);
                List<int> errorConns = new List<int>();

                for(int i = 1; i < dataSplit.Length - 1; i++) {
                    //Debug.Log(dataSplit[i]);
                    string idx2 = dataSplit[i].Replace(",", "");
                    int x = int.Parse(idx2);
                    errorConns.Add(x);
                }

                /*
                string full_path = "";
                
                if(typeErrorMade == "cable") {
                    string path = "Assets/RehabWithRobot_1_Scripts/saved_error_conn/";
                    DateTime dt = DateTime.Now;
                    string fileName = "stopTimeErrorConn_" + dt.ToString("yyyy-MM-dd HH_mm_ss");
                    full_path = path + fileName + ".txt";
                } 
                else if(typeErrorMade == "app") {
                    string path = "Assets/RehabWithRobot_1_Scripts/saved_close_app/";
                    DateTime dt = DateTime.Now;
                    string fileName = "stopTimeCloseApp_" + dt.ToString("yyyy-MM-dd HH_mm_ss");
                    full_path = path + fileName + ".txt";
                }

                StreamWriter writer = new StreamWriter(full_path, true);
                //writer.WriteLine(res);
                //writer.Close();
                */

                String res = "";
                for(int i = 0; i < errorConns.Count; i++) {
                    res += errorConns[i].ToString().Replace(",", ".") + ","; 
                }

                if (!File.Exists(errorPingsPath)) {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(errorPingsPath)) {
                        sw.WriteLine(DateTime.Now.ToString() + ": " + "File Created!");
                        if(res != "")
                            sw.Write(res);
                    }   
                } else {
                    // This text is always added, making the file longer over time if it is not deleted.
                    using (StreamWriter sw = File.AppendText(errorPingsPath)) {
                        if(res != "")
                            sw.Write(res);
                    }   
                }
            }


            else if(dataSplit[0] == "robot_emergency_stop") {
                currentActionForPings = "idle";
                Debug.Log("robot_emergency_stop");

                DateTime foo = DateTime.Now;
                long unixTimeRobotEStopped = ((DateTimeOffset)foo).ToUnixTimeMilliseconds();

                Debug.Log("unity: " + unixTimeSendEStop + ", robot: " + unixTimeRobotEStopped + 
                    ", EStop diff: " + (unixTimeRobotEStopped-unixTimeSendEStop));

                diffUnityRobotEStop = (int)(unixTimeRobotEStopped-unixTimeSendEStop);

                // return coins
                foreach(GameObject c in gameCoins) {
                    c.SetActive(true);
                }

                DateTime dt = DateTime.Now;
                string date = dt.ToString("dd/MM/yyyy HH:mm:ss:fff");
                String res = date + ", " + diffUnityRobotEStop;
                if (!File.Exists(eStopPingsPath)) {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(eStopPingsPath)) {
                        sw.WriteLine(DateTime.Now.ToString() + ", " + "File Created!");
                        sw.WriteLine(res);
                    }   
                } else {
                    // This text is always added, making the file longer over time if it is not deleted.
                    using (StreamWriter sw = File.AppendText(eStopPingsPath)) {
                        sw.WriteLine(res);
                    }   
                }

                timeRobotEStopList.Add(diffUnityRobotEStop);

                // if there is waypoints configured, then button to clear is active
                if(pathWaypoints.Length > 10)
                    clearWpBtnGO.SetActive(true);
            }


            else if(dataSplit[0] == "robot_vel_exercise") {
                robotInfoExercise = "";
                robotInfoExercise += msg;

                //Debug.Log("robot_vel_exercise :: " + robotInfoExercise);
                iiwa.getJointsPosExercise();
            }

            else if(dataSplit[0] == "joints_pos_exercise") {
                robotInfoExercise += msg;
                //Debug.Log("joints_pos_exercise :: " + robotInfoExercise);
                iiwa.getToolTorqueExercise();
            }

            else if(dataSplit[0] == "tool_torque_exercise") {
                robotInfoExercise += msg;
                //Debug.Log("tool_torque_exercise :: " + robotInfoExercise);
                iiwa.getToolMomentumExercise();
            }

            else if(dataSplit[0] == "tool_momentum_exercise") {
                robotInfoExercise += msg;
                // save all
                //Debug.Log("robotInfoExercise :: " + robotInfoExercise);

                string path = "Assets/RehabWithRobot_1_Scripts/RobotInfo/";

                DateTime dt = DateTime.Now;
                string filename = "robot_info_" + dt.ToString("yyyy-MM-dd HH_mm_ss");

                string full_path = path + filename + ".txt";
                StreamWriter writer = new StreamWriter(full_path, true);

                writer.WriteLine(robotInfoExercise);
                writer.Close();
            }


            else if(dataSplit[0] == "current_robot_info") {
                //Debug.Log("current_robot_info");
                //Debug.Log(msg);

                String buf = "";
                for(int i = 1; i < dataSplit.Length; i++) {
                    buf += dataSplit[i] + ", "; 
                }

                DateTime dt = DateTime.Now;
                string date = dt.ToString("dd/MM/yyyy HH:mm:ss:fff");

                String res = date + ", " + buf;

                if (!File.Exists(robotInfoCurrPath)) {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(robotInfoCurrPath)) {
                        //sw.WriteLine(DateTime.Now.ToString() + ": " + "File Created!");
                        sw.WriteLine(res);
                    }   
                } 
                else {
                    // This text is always added, making the file longer over time if it is not deleted.
                    using (StreamWriter sw = File.AppendText(robotInfoCurrPath)) {
                        sw.WriteLine(res);
                    }   
                }

                //Arranjar Força

                float posx = float.Parse(dataSplit[3], CultureInfo.InvariantCulture);
                float posy = float.Parse(dataSplit[4], CultureInfo.InvariantCulture);
                float posz = float.Parse(dataSplit[5], CultureInfo.InvariantCulture);


                float pathx = float.Parse(dataSplit[10], CultureInfo.InvariantCulture);
                float pathy = float.Parse(dataSplit[11], CultureInfo.InvariantCulture);
                float pathz = float.Parse(dataSplit[12], CultureInfo.InvariantCulture);

                Vector3 pos = new Vector3(posx, posy, posz);
                Vector3 path = new Vector3(pathx, pathy, pathz);

                desvioDoCaminho = Vector3.Distance(pos, path);  

            }
        
            msg = "";
        }

        // only switch to another panel if the next button is different from the previous
        if(actualButtonState != olderButtonState) {
            if(actualButtonState == "no_btn" && (!(olderButtonState == "green_btn") &&
                !(olderButtonState == "orange_btn"))) {

                exerciseHelpPanel.SetActive(true);
                exerciseRecordingPanel.SetActive(false);
                exerciseFreeModePanel.SetActive(false);

            } else if (actualButtonState == "red_btn") {
                exerciseHelpPanel.SetActive(false);
                exerciseRecordingPanel.SetActive(false);
                exerciseFreeModePanel.SetActive(true);

            } else if (actualButtonState == "green_btn" || actualButtonState == "orange_btn") {
                exerciseHelpPanel.SetActive(false);
                exerciseRecordingPanel.SetActive(true);
                exerciseFreeModePanel.SetActive(false);

            }
            olderButtonState = actualButtonState;
        }


        // TODO
        System.TimeSpan ts = System.DateTime.UtcNow - startTime;

        unityDelayArray.Add(ts.Milliseconds);

        if(counter % 100 == 0 && counter > 10) {
            float m = 0;
            for(int i = 0; i < unityDelayArray.Count; i++) {
                m += unityDelayArray[i];
            }

            m /= unityDelayArray.Count;
            Debug.Log("t mean :: " + m + " ms");
            unityDelayArray.Clear();

            // write to file
            DateTime dt = DateTime.Now;
            string date = dt.ToString("dd/MM/yyyy HH:mm:ss:fff");

            string mD = m.ToString();
            mD = mD.Replace(",", ".");

            String res = date + ", " + mD + ", " + currentActionForPings;
            
            if (!File.Exists(unityDelayPath)) {
                using (StreamWriter sw = File.CreateText(unityDelayPath)) {
                    sw.WriteLine(res);
                }   
            } else {
                using (StreamWriter sw = File.AppendText(unityDelayPath)) {
                    sw.WriteLine(res);
                }   
            }
        }


    } // end Update()

    /*
    IEnumerator scoreFunc() {
        yield return new WaitForSeconds (1f);
        scoreText.GetComponentInChildren<Text>().text = "" + coinsScore;
        if(isRepeatingPath)
            StartCoroutine(scoreFunc());
    }
    */


    public void getErrorPings() {
        iiwa.getStopTimeErrorConn();
    }

    public int mapScore(double s) {
        float A = 200; 
        float B = 100; // in max
        float C = 0; 
        float D = 5; // out max
        float res = ((float)s-A)/(B-A) * (D-C) + C;

        if(res < C) res = C;
        if(res > D) res = D;
        //currScore = (int)res;
        return (int)res;
    }

    public void openHelpCreateExerciseBtnPressed() {
        helpCreateExercisePanel.SetActive(true);
    }

    public void closeHelpCreateExerciseBtnPressed() {
        helpCreateExercisePanel.SetActive(false);
    }

    public void openHelpPlayBtnPressed() {
        helpPlayPanel.SetActive(true);
    }

    public void closeHelpPlayBtnPressed() {
        helpPlayPanel.SetActive(false);
    }

    public void openMaxForcePanelBtnPressed() {
        setMaxForcePanel.SetActive(true);
    }

    public void closeMaxForcePanelBtnPressed() {
        setMaxForcePanel.SetActive(false);
    }

    public void setMaxForceBtnPressed() {
        string forceLimitString = forceLimitInputField.text;
        float forceLimit = -1;

        string timeLimitString = timeLimitInputField.text;
        float timeLimit = -1;

        bool success = float.TryParse(forceLimitString, out forceLimit);
        bool success2 = float.TryParse(timeLimitString, out timeLimit);

        if (success && forceLimit >= 10 && success2 && timeLimit >= 1) {
            Debug.Log("setMaxForceConfigs ::  FORCE :: " + forceLimit + ", TIME :: " + timeLimit);
            setMaxForcePanel.SetActive(false);
            iiwa.setMaxForceConfigs(forceLimit, timeLimit);
        }
        else if (forceLimit < 10) {
            Debug.Log("Force Limit value must be >= 10 N");
        } 
        else if (timeLimit < 1) {
            Debug.Log("Time Limit value must be > 1 sec");
        } 
        else if (!success) {
            Debug.Log($"Force Limit Attempted conversion of '{forceLimitString ?? "<null>"}' failed.");
        } 
        else if (!success2) {
            Debug.Log($"Time Limit Attempted conversion of '{forceLimitString ?? "<null>"}' failed.");
        } 
    }


    public void saveRobotInfoBtnPressed() {   
        iiwa.getRobotVelExercise();
    }

    public bool crash() { return crash(); }

    Boolean toggle = true;
    public void testCrashBtnPressed() {   
        //Utils.ForceCrash(ForcedCrashCategory.Abort);
        //crash();
        toggle = false;
        Debug.Log("crash!");

        currentActionForPings = "N/A";

        isRobotConnected = false;

        exitButtonPressed();
        connectBtn.interactable = true;
        newExerciseBtn.interactable = false;
        loadExerciseBtn.interactable = false;

        errorTxt.GetComponentInChildren<Text>().text = "Communication error. Please Reconnect!";

        pingTxtGO.SetActive(false);

        //yield return new WaitForSeconds(3);

        //testCrash.transform.position = new Vector3(0,0,0);
    }

    public void doneTest() {
        mainHintTxt.text = "Configurate the exercise and hit Play!";
        currentActionForPings = "idle";

        configMovPanel.SetActive(true);

        isRepeatingPath = false;
        activateWaypoints = true;
        playPathBtn.interactable = true;
        testingPathBtn.interactable = false;
        stopPlayPathBtn.interactable = false;
        incStartVel.interactable = true;
        decStartVel.interactable = true;
        incReps.interactable = true;
        decReps.interactable = true;
        saveRobotPathBtn.interactable = true;

        
        if(stopBtnPressed == 0)
            StartCoroutine(showHint());
        
        stopBtnPressed = 0;
    }

    bool isOnGame = false;
    public void impGameBtnPressed() {
        wpMode = "imp_game";
        impGamePanel.SetActive(true);
        wpSelectPanel.SetActive(false);
        isOnGame = true;
    }

    public void impGameClosePanel() {
        impGamePanel.SetActive(false);
        wpSelectPanel.SetActive(true);
        isOnGame = false;
    }

    List<GameObject> gameCoins = new List<GameObject>();
    int coinCount = 0;
    string impFromImpGame;
    public void confirmImpGameBtnPressed() {
        if(isOnGame) {
            string m_imp = impGameImpedanceDropdown.options[impGameImpedanceDropdown.value].text;
            m_imp = m_imp.Split(char.Parse(" "))[0];

            impFromImpGame = m_imp;

            string shift = impGameCoinShiftDropdown.options[impGameCoinShiftDropdown.value].text;
            string coinPos = impGameCoinPosDropdown.options[impGameCoinPosDropdown.value].text;

            //Debug.Log("shift :: " + shift);
            //Debug.Log("coinPos :: " + coinPos);

            // generate coins and display on path
            spawnCoin(startWaypointIdx, coinCount, shift, coinPos);
            coinCount++;
        
            // get idxs and send as waypoints to robot
            setWaypoint();
        }
    }

    public GameObject coinPrefab;

    void spawnCoin(int idx, int count, string shift, string coinPos) {
        Debug.Log("idx :: " + idx);
        int start = startWaypointIdx, end = endWaypointIdx;
        if(startWaypointIdx > endWaypointIdx) {
            start = endWaypointIdx;
            end = startWaypointIdx;
        }

        GameObject startWp = GameObject.Find("Sphere_" + (start));
        GameObject endWp = GameObject.Find("Sphere_" + (end));
        GameObject currWp = GameObject.Find("Sphere_" + (start + 1));

        Vector3 pos = currWp.transform.position;

        float rShift = 0.12f;
        //rShift = Random.Range(0.08f, 0.15f);
        if(shift == "EASY") rShift = 0.11f;
        else if(shift == "MEDIUM") rShift = 0.14f;
        else if(shift == "HARD") rShift = 0.18f;

        // TODO - Adjust vector

        Vector3 dir = endWp.transform.position - startWp.transform.position;
        Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;
        Vector3 right = -left;

        if(coinPos == "LEFT") {
            //pos = pos + left * rShift;
            pos.x += rShift; 
        } else if(coinPos == "RIGHT") {
            //pos = pos + right * rShift;
            pos.x -= rShift;    
        }
      
        GameObject tmp = Instantiate(coinPrefab, pos, Quaternion.identity);
        tmp.layer = renderLayer;
        tmp.name = "Coin_" + count;
        //tmp.transform.localScale = new Vector3(11, 11, 11);
        //tmp.transform.localRotation = Quaternion.Euler(16, -57, 0);

        tmp.transform.localScale = new Vector3(0.0027f, 0.0027f, 0.0027f);
        tmp.transform.localRotation = Quaternion.Euler(0, -57, -17);

        gameCoins.Add(tmp);
    }

    IEnumerator showHint() {
        //Debug.Log("show hint");
        yield return new WaitForSeconds (4f);
        if(isRepeatingPath == false) {
            //hintTxt.text = "Hint: Click on 2 points on circuit to change mode!";
            StartCoroutine(closeHint());
        }
    }

    IEnumerator closeHint() {
        //Debug.Log("close hint");
        yield return new WaitForSeconds (5f);
        hintTxt.text = "";
    }

    /*
    IEnumerator PingUpdate() {
        yield return new WaitForSeconds (1.5f);
        if (ping.isDone) {

            pingList.Add(ping.time);

            pingTxtGO.GetComponent<Text>().text = "Ping: " + ping.time + " ms";

            if(ping.time > 150) {
                pingTxtGO.GetComponent<Text>().color = Color.red;
            } else {
                pingTxtGO.GetComponent<Text>().color = Color.green;
            }

            ping = new Ping(iiwa_IP);
        }
        StartCoroutine(PingUpdate());
    }
    */
    IEnumerator PingUpdateDisplay() {
        yield return new WaitForSeconds (1.5f);

        pingTxtGO.GetComponent<Text>().text = "" + diffUnityRobotTime + " ms";
        //pingTxtGO.GetComponent<Text>().text = "Ping: " + diffUnityRobotTime + " ms";

        if(diffUnityRobotTime > 180) {
            pingTxtGO.GetComponent<Text>().color = Color.red;
        } 
        else if(diffUnityRobotTime > 100) {
            pingTxtGO.GetComponent<Text>().color = Color.yellow;
        } 
        else {
            pingTxtGO.GetComponent<Text>().color = Color.green;
        }
     
        StartCoroutine(PingUpdateDisplay());
    }

    string typeErrorMade;
    public void GetStopTimeErrorConnBtnPressed() {
        iiwa.getStopTimeErrorConn();
        typeErrorMade = "cable";
    }

    public void GetStopTimeStopAppBtnPressed() {
        iiwa.getStopTimeErrorConn();
        typeErrorMade = "app";
    }

    public void GetPingListBtnPressed() {
        //Debug.Log("GetPingListBtnPressed");
        
        string robot_saved_pings = "Assets/RehabWithRobot_1_Scripts/SinglePings/";

        DateTime dt = DateTime.Now;
        string nameOfReport = "pings_" + dt.ToString("yyyy-MM-dd HH_mm_ss");

        string path = robot_saved_pings + nameOfReport + ".txt";
        StreamWriter writer = new StreamWriter(path, true);

        String res = "";

        for(int i = 0; i < pingList.Count; i++) {
            res += pingList[i].ToString().Replace(",", ".") + ","; 
        }

        writer.WriteLine(res);
        writer.Close();

        pingList.Clear();
    }

    public void GetEStopListBtnPressed() {
        //Debug.Log("GetEStopListBtnPressed");
        
        string path = "Assets/RehabWithRobot_1_Scripts/saved_estop/";
        DateTime dt = DateTime.Now;
        string filename = "estops_" + dt.ToString("yyyy-MM-dd HH_mm_ss");
        string fullPath = path + filename + ".txt";
        StreamWriter writer = new StreamWriter(fullPath, true);

        String res = "";

        for(int i = 0; i < timeRobotEStopList.Count; i++) {
            res += timeRobotEStopList[i].ToString().Replace(",", ".") + ","; 
        }

        writer.WriteLine(res);
        writer.Close();

        timeRobotEStopList.Clear();
    }

    double euclidianDistance(Vector3 A, Vector3 B) {
        return Math.Sqrt(Math.Pow(B.x - A.x, 2) + Math.Pow(B.y - A.y, 2) + Math.Pow(B.z - A.z, 2));
    }

    public void ConnectRobot() {
        connectBtn.interactable = false;
        newExerciseBtn.interactable = false;
        iiwa.startTCPConnection(tcpTimeout);

        errorTxt.GetComponentInChildren<Text>().text = "";
    }

    public void creatingNewExerciseButtonPressed() {
        menuPanel.SetActive(false);
        exerciseHelpPanel.SetActive(true);

        iiwa.createExercise();
    }

    public void GetButtonStateOnLoop() {
        while(isInFreeMode == true) {
            iiwa.getButtonState();
            Thread.Sleep(500);
        } 
    }

    public void stopRecordingButtonPressed() {
        isInFreeMode = false;
        iiwa.stopCreatingExercisePath();
    }

    public void emergencyStopButtonPressed() {
        stopPlayPathBtn.interactable = false;
        setMaxForceBtn.interactable = true;

        stopBtnPressed = 1;
        testingPathBtn.GetComponentInChildren<Text>().text = "Reposition";
        testingPathBtn.GetComponentsInChildren<Image>()[1].sprite = testBtnImage2;

        configMovPanel.SetActive(true);
        statusMovPanel.SetActive(false);
        
        // clear lists
        path_force.Clear();
        rep_force.Clear(); 
        all_path_points.Clear();

        testingPathBtn.interactable = true;
        playPathBtn.interactable = false;

        /*
        if(isOnPlayOrTest == "play") {
            playPathBtn.GetComponentInChildren<Text>().text = "Restart";
            playPathBtn.interactable = true;
        } else if(isOnPlayOrTest == "test") {
            testingPathBtn.interactable = true;
        }
        */

        isInFreeMode = false;
        isRepeatingPath = false;

        DateTime foo = DateTime.Now;
        unixTimeSendEStop = ((DateTimeOffset)foo).ToUnixTimeMilliseconds();

        iiwa.emergencyStopRobot();
    }

    public void GetRobotDistToBeginning() {
        while(isRepeatingPath == true) {
            iiwa.getDistToBegin();
            Thread.Sleep(250);
        }  
    }

    public void GetRobotPositionOnLoop() {
        while(isRepeatingPath == true) {
            iiwa.getRobotCartPosition();
            Thread.Sleep(100);
        }  
    }

    public void GetRobotPositionForceRepsOnLoop() {
        while(isRepeatingPath == true) {
            iiwa.getRobotPositionForceReps();
            Thread.Sleep(100);
        }  
    }

    public void GetRobotInfoOnLoop() {
        while(isRepeatingPath == true) {
            iiwa.getRobotInfo();
            Thread.Sleep(150);
        } 
    }

    public void returnWaypointsButtonPressed() {
        impPanel.SetActive(false);
        sinePanel.SetActive(false);
        rotationPanel.SetActive(false);
        wpSelectPanel.SetActive(true);
    }
    
    public void wpImpButtonPressed() {
        impPanel.SetActive(true);
        wpSelectPanel.SetActive(false);
        wpMode = "imp";
    }

    public void wpSineButtonPressed() {
        sinePanel.SetActive(true);
        wpSelectPanel.SetActive(false);
        wpMode = "sine";
    }

    public void wpRotationButtonPressed() {
        rotationPanel.SetActive(true);
        wpSelectPanel.SetActive(false);
        wpMode = "rotation";
    }

    public void showForceReportBtnPressed() {

        if(toggleShowForceBtn == false) {
            toggleShowForceBtn = true;

            showForce(path_force, all_path_points, -1, 1, -1);

        } else if(toggleShowForceBtn == true) {
            toggleShowForceBtn = false;
            draw_force = false;

            //Reseta a lista de force points
            foreach(GameObject gO in sphereForceList){
                Destroy(gO);
            }
            sphereForceList.Clear();
        }

    }

    void showForce(List<List<Vector3>> forces, List<Vector3> all_points, int rep, int mean, float redZoneVal) {
        Debug.Log("forces count: " + forces.Count);

        int xx = 0;
        foreach(List<Vector3> l in forces) {
            Debug.Log("forces[" + xx + "] count: " + l.Count);
            xx++;
        }

        path_force_mags.Clear();
        path_midpoint.Clear();

        float maxValForceList = -999;
        double meanValForceList = 0;
        double numPoints = 0;
        int countMean = 0;
        List<Vector3> final_force = new List<Vector3>();

        if(mean == 1) {
            foreach(List<Vector3> l in forces) {
                // get max force executed on path
                for(int i = 0; i < l.Count; i++) {
                    float actualMaxVal = Mathf.Max(Mathf.Max(Math.Abs(l[i].x), 
                            Math.Abs(l[i].y)), Math.Abs(l[i].z));
                    if(actualMaxVal > maxValForceList) maxValForceList = actualMaxVal;

                    meanValForceList += Math.Sqrt(Math.Pow(Math.Abs(l[i].x),2) + 
                            Math.Pow(Math.Abs(l[i].y),2) + Math.Pow(Math.Abs(l[i].z),2));
                    countMean++;
                }

                numPoints += l.Count;

            }

            numPoints /= forces.Count;

            // get mean force 
            final_force = calculateMeanForcePath(forces);

        } else if (rep != -1) {
            final_force = new List<Vector3>(forces[rep]);

            numPoints = forces[rep].Count;

            // get max force executed on path
            for(int i = 0; i < final_force.Count; i++) {
                float actualMaxVal = Mathf.Max(Mathf.Max(Math.Abs(final_force[i].x), 
                        Math.Abs(final_force[i].y)), Math.Abs(final_force[i].z));
                if(actualMaxVal > maxValForceList) maxValForceList = actualMaxVal;

                meanValForceList += Math.Sqrt(Math.Pow(Math.Abs(final_force[i].x),2) + 
                        Math.Pow(Math.Abs(final_force[i].y),2) + Math.Pow(Math.Abs(final_force[i].z),2));
                countMean++;
                
            }

        }

        //Debug.Log("maxValForceList :: " + maxValForceList);

        // put on text
        forceInfoMaxText.text = "Max Force: " + Math.Round(maxValForceList*10.00)/10.00 + " N";
        forceInfoMeanText.text = "Mean Force: " + Math.Round((meanValForceList/countMean)*10.00)/10.00 + " N";
        forceInfoNumPointsText.text = "Total Force Points: " + Math.Round(numPoints);

        //if(maxValForceList < defaultForceToDisplayOnScreen) 
        //    maxValForceList = defaultForceToDisplayOnScreen;
         
        if(maxValForceList < 15) {
            maxValForceList = 15;
        }

        force_on_path.Clear();
        force_lines.Clear();

        // get magnitude between waypoints
        float maxMagWps = (path_positions[1] - path_positions[0]).magnitude;
        

        // get force magnitudes on rep
        for(int i = 0; i < final_force.Count; i++) {
            float force_mag = final_force[i].magnitude;
            path_force_mags.Add(force_mag);
        }

        //Debug.Log("final_force.Count :: " + final_force.Count);
        //Debug.Log("all_points.Count :: " + all_points.Count);
  
        // get force point to draw on circuit
        for(int j = 0; j < final_force.Count; j++) {
            // normalize force dir
            final_force[j] /= maxValForceList;

            // map force (normalize)
            float A = 0; 
            float B = Mathf.Max(path_force_mags.ToArray()); // in max
            float C = 0; 
            float D = maxMagWps * 4.5f; // out max
            float res = (path_force_mags[j]-A)/(B-A) * (D-C) + C;

            //Debug.Log("res " + j + ": " + res);

            // ponto da força
            //Debug.Log("calc force point :: " + j);
            Vector3 next_force = all_points[j] + (final_force[j] * res);

            //Debug.Log("done calc force point");

            // ponto da força que está com offset ao ponto waypoint
            force_on_path.Add(next_force);

            // line for each force
            LineRenderer lr_aux = new GameObject().AddComponent<LineRenderer>();
            lr.gameObject.layer = renderLayer;
            lr_aux.material = new Material(Shader.Find("Sprites/Default"));
            lr_aux.SetWidth(.01f, .01f);
            lr_aux.SetColors(Color.green, Color.green);
            lr_aux.positionCount = 0;

            lr_aux.transform.parent = GameObject.Find("HoldForceLines").transform; 

            force_lines.Add(lr_aux);

            // add sphere on tip of the force if the force executed is high enough
            minForceToDisplayOnScreen = defaultForceToDisplayOnScreen;
            if(redZoneVal >= 1) {
                minForceToDisplayOnScreen = redZoneVal;
            } 

            if(minForceToDisplayOnScreen < (meanValForceList/countMean)) {
                minForceToDisplayOnScreen = (float)((meanValForceList/countMean) + 1);
            }

            if(path_force_mags[j] >= minForceToDisplayOnScreen) {
                //Debug.Log("create force point");
                CreateForcePoint(next_force, j);
            }

        } // end loop calc force point to draw on circuit

        draw_force = true;
    }

    List<Vector3> calculateMeanForcePath(List<List<Vector3>> all_forces) {
        List<Vector3> mean = new List<Vector3>();

        int minSamples = 9999;
        foreach(List<Vector3> aux in all_forces) {
            if(aux.Count < minSamples) minSamples = aux.Count;
        }

        for(int i = 0; i < minSamples; i++) {
            Vector3 sum_aux = new Vector3(0, 0, 0);

            for(int j = 0; j < all_forces.Count; j++) {
                sum_aux += all_forces[j][i];
            }

            mean.Add(sum_aux / all_forces.Count);
        }
        return mean;
    }

    float forcePointScaleSphere = 1.5f;
    void CreateForcePoint(Vector3 pos, int cc) {
        GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        tmp.layer = renderLayer;
        tmp.transform.parent = GameObject.Find("HoldForceSpheres").transform; 
        tmp.name = "ForceSphere_" + cc;
        tmp.transform.position = pos;
        tmp.transform.localScale = new Vector3(scaleSphere/forcePointScaleSphere, 
                    scaleSphere/forcePointScaleSphere, scaleSphere/forcePointScaleSphere);

        tmp.GetComponent<Renderer>().material.color = Color.green;

        sphereForceList.Add(tmp);
    }

    public void loadReportsBtnPressed() {
        loadReportPanel.SetActive(true);

           // set dropdown with all paths
        savedReportsDropDown.ClearOptions();

        List<string> m_savedReportsFiles = new List<string>();

        DirectoryInfo dir = new DirectoryInfo(robot_saved_reports);
        FileInfo[] info = dir.GetFiles("*.txt");
        foreach (FileInfo f in info) {
            string p = f.FullName;
            string filename = Path.GetFileName(p);
            filename = filename.Replace(".txt", "");
            m_savedReportsFiles.Add(filename);
         }
        
        savedReportsDropDown.AddOptions(m_savedReportsFiles);
    }

    public Text reportNameTxt, reportDateTxt, reportTotalReps, reportRobotVel, reportStopsTxt, reportCoinsGot;

    List<List<Vector3>> circuit_forces;
    List<Vector3> circuit_full;

    List<string> rep_status;

    public async void loadChoosedReportBtnPressed() {
        int m_DropdownValue = savedReportsDropDown.value;

        string reportChoosed = savedReportsDropDown.options[m_DropdownValue].text;
        //Debug.Log(reportChoosed);
    
        string path = robot_saved_reports + reportChoosed + ".txt";
        StreamReader reader = new StreamReader(path); 
    
        String msg = reader.ReadToEnd();

        //Debug.Log(msg);

        reader.Close();

        isExerciseLoaded = 1;
        onReportPanel = true;

        loadReportPanel.SetActive(false);
        menuPanel.SetActive(false);
        reportsPanel.SetActive(true);

        // load de tudo do report
        string[] dataSplit = msg.Split(',');

        string date = "", total_reps = "", vel = "", score = "";

        for(int i = 0; i < dataSplit.Length; i++) {
            if(dataSplit[i].Contains("date")) {
                date = dataSplit[i+1];
            }

            if(dataSplit[i].Contains("total_reps")) {
                total_reps = dataSplit[i+1];
            }

            if(dataSplit[i].Contains("vel")) {
                vel = dataSplit[i+1];
            }

            if(dataSplit[i].Contains("score")) {
                score = dataSplit[i+1];
            }

        }

        reportNameTxt.text = "" + reportChoosed;
        reportDateTxt.text = date;

        int actualReps = int.Parse(total_reps.Split('/')[0]);
        int totalReps = int.Parse(total_reps.Split('/')[1]);

        if(actualReps > totalReps) {
            actualReps = totalReps; 
        }

        reportTotalReps.text = actualReps + "/" + totalReps;
        reportRobotVel.text = vel + "%";
        reportCoinsGot.text = score;

        List<Vector3> circuit = new List<Vector3>();
    
        for(int i = 0; i < dataSplit.Length; i++) {
            if(dataSplit[i].Contains("path_positions")) {

                for(int j = i; j < dataSplit.Length; j+=3) {

                    if(!dataSplit[j+1].Contains("all_path_points")) {
                        float x = float.Parse(dataSplit[j+1], CultureInfo.InvariantCulture);
                        float y = float.Parse(dataSplit[j+2], CultureInfo.InvariantCulture);
                        float z = float.Parse(dataSplit[j+3], CultureInfo.InvariantCulture);

                        circuit.Add(new Vector3(x, y, z));

                    } else break;

                }

            }
        }

        showPathOnScreen("", circuit);

        circuit_full = new List<Vector3>();

        for(int i = 0; i < dataSplit.Length; i++) {
            if(dataSplit[i].Contains("all_path_points")) {
                //Debug.Log("all_path_points :: ");

                for(int j = i; j < dataSplit.Length; j+=3) {

                    if(!dataSplit[j+1].Contains("path_force")) {
                        float x = float.Parse(dataSplit[j+1], CultureInfo.InvariantCulture);
                        float y = float.Parse(dataSplit[j+2], CultureInfo.InvariantCulture);
                        float z = float.Parse(dataSplit[j+3], CultureInfo.InvariantCulture);

                        //Debug.Log("v: " + new Vector3(x, y, z));
                        circuit_full.Add(new Vector3(x, y, z));

                    } else break;

                }

            }
        }

        circuit_forces = new List<List<Vector3>>();
        List<Vector3> rep_forces = new List<Vector3>();

        rep_status = new List<string>();
        
        int c = 0;
        List<float> holdF = new List<float>();

        for(int i = 0; i < dataSplit.Length; i++) {
            if(dataSplit[i].Contains("path_force")) {
                
                for(int j = i+2; j < dataSplit.Length; j++) {

                    if (dataSplit[j].Contains("rep_")) {
                        string[] rep = dataSplit[j].Split('_');
                        //Debug.Log("rep: " + rep[1]);

                        circuit_forces.Add(new List<Vector3>(rep_forces));
                        //Debug.Log("rep_forces count :: " + rep_forces.Count);
                        rep_forces.Clear();

                        //j-=2;

                    } 
                    else if (dataSplit[j].Contains("ok")) {
                        Debug.Log("rep is " + dataSplit[j]);
                        rep_status.Add(dataSplit[j]);
                        //j-=1;
                    } 
                    // end of forces
                    else if(dataSplit[j].Contains("total_reps")) {
                        circuit_forces.Add(new List<Vector3>(rep_forces));
                        rep_forces.Clear();
                        Debug.Log("all reps done !!!");
                        break;
                    } 
                    /*
                    else {
                        Debug.Log("dataSplit[j]: " + dataSplit[j] + ", dataSplit[j+1]: " + dataSplit[j+1] + 
                               "dataSplit[j+2]: " + dataSplit[j+2]);

                        float x = float.Parse(dataSplit[j], CultureInfo.InvariantCulture);
                        float y = float.Parse(dataSplit[j+1], CultureInfo.InvariantCulture);
                        float z = float.Parse(dataSplit[j+2], CultureInfo.InvariantCulture);

                        rep_forces.Add(new Vector3(x, y, z));
                    }
                    */
                    else {
                        float v = float.Parse(dataSplit[j], CultureInfo.InvariantCulture);

                        holdF.Add(v);
                        c++;
                        if(c == 3) {
                            c = 0;
                            rep_forces.Add(new Vector3(holdF[0], holdF[1], holdF[2]));
                            holdF.Clear();
                        }
                        
                    }

                }

            }
        }

        int stopCount = 0;
        foreach(string str in rep_status) {
            if(str.Contains("nok")) stopCount++;
        }

        reportStopsTxt.text = "" + stopCount;


        /*
        Debug.Log("circuit_forces.Count :: " + circuit_forces.Count);
        int c = 0;
        foreach(List<Vector3> l in circuit_forces) {
            Debug.Log("l[" + c + "] :: " + circuit_forces[c].Count);
            c++;
        }
        */
    }

    public void openForcesPanelBtnPressed() {
        // set reps dropdown
        repsDropdown.ClearOptions();

        List<string> m_reps = new List<string>();

        string totReps = reportTotalReps.text.Split('/')[1];

        int totalReps = int.Parse(totReps);

        for(int i = 1; i <= totalReps; i++) {
            m_reps.Add((i).ToString());
        }
        
        repsDropdown.AddOptions(m_reps);

        reportsForcePanel.SetActive(true);
    }

    public void closeForcesPanelBtnPressed() {
        reportsForcePanel.SetActive(false);
    }

    public void removeForcesBtnPressed() {
        draw_force = false;

        forceInfoPanel.SetActive(false);

        //Reseta a lista de force points
        foreach(GameObject gO in sphereForceList){
            Destroy(gO);
        }
        sphereForceList.Clear();

        removeForcesBtn.interactable = false;
    }

    public void reportMeanForceDisplayBtnPressed() {
        draw_force = false;

        foreach(LineRenderer l in force_lines){
            Destroy(l);
        }

        //Reseta a lista de force points
        foreach(GameObject gO in sphereForceList){
            Destroy(gO);
        }
        sphereForceList.Clear();

        removeForcesBtn.interactable = true;

        showForce(circuit_forces, circuit_full, -1, 1, -1);

        reportsForcePanel.SetActive(false);

        //repStatusText.GetComponent<Text>().color = Color.white;
        repStatusText.text = "Rep Status: NaN";

        forceInfoTypeText.text = "Type: Mean";
        forceInfoPanel.SetActive(true);
    }

    public void reportRepForceDisplayBtnPressed() {
        draw_force = false;

        foreach(LineRenderer l in force_lines){
            Destroy(l);
        }

        //Reseta a lista de force points
        foreach(GameObject gO in sphereForceList){
            Destroy(gO);
        }
        sphereForceList.Clear();

        removeForcesBtn.interactable = true;

        int m_DropdownValue = repsDropdown.value;
        string repChoosed = repsDropdown.options[m_DropdownValue].text;
        int rep =  int.Parse(repChoosed);
        rep = rep - 1;
        //Debug.Log("rep : " + (rep+1) + ", but in the array is ::" + rep);

        string stat = "COMPLETED";
        if(rep_status[rep].Contains("nok")) {
            stat = "FAILED";
            //repStatusText.GetComponent<Text>().color = Color.red;
        } else {
            //repStatusText.GetComponent<Text>().color = Color.white;
        }

        repStatusText.text = "Rep Status: " + stat;

        showForce(circuit_forces, circuit_full, rep, -1, -1);

        reportsForcePanel.SetActive(false);

        forceInfoTypeText.text = "Type: Repetition " + (rep+1);
        forceInfoPanel.SetActive(true);
    }

    public void reportRedZonesDisplayBtnPressed() {
        
        string redZoneString = redZoneInputField.text;
        float redZoneVal = -1;

        bool success = float.TryParse(redZoneString, out redZoneVal);
        if (success && redZoneVal > 0) {
            //Debug.Log($"Red zone Converted : '{redZoneVal}.");

            draw_force = false;

            foreach(LineRenderer l in force_lines){
                Destroy(l);
            }

            //Reseta a lista de force points
            foreach(GameObject gO in sphereForceList){
                Destroy(gO);
            }
            sphereForceList.Clear();

            removeForcesBtn.interactable = true;

            showForce(circuit_forces, circuit_full, -1, 1, redZoneVal);

            reportsForcePanel.SetActive(false);

            //repStatusText.GetComponent<Text>().color = Color.white;
            repStatusText.text = "Rep Status: NaN";

            forceInfoTypeText.text = "Type: Zones above " + redZoneVal + " N";
            forceInfoPanel.SetActive(true);
        }
        else if (redZoneVal <= 0) {
            Debug.Log("Red zone value must be above 0 N");
        } 
        else {
            Debug.Log($"Red zone Attempted conversion of '{redZoneString ?? "<null>"}' failed.");
        } 
    }

    public void closeLoadReportsBtnPressed() {
        loadReportPanel.SetActive(false);
    }

    public void openSaveReportPanelBtnPressed() {
        saveReportPanel.SetActive(true);
    }

    public void closeSaveReportPanelBtnPressed() {
        saveReportPanel.SetActive(false);
    }

    public void savingReportBtnPressed() {
        saveReportPanel.SetActive(false);
        Debug.Log("saving report to file");

        string nameOfReport = saveReportInputField.text;

        string path = robot_saved_reports + nameOfReport + ".txt";
        saveRobotPath(path);
    }

    public void saveRobotPath(string path) {
        StreamWriter writer = new StreamWriter(path, true);

        String res = "";

        // colocar path simplified
        res += "path_positions,";
        for(int i = 0; i < path_positions.Count; i++) {
            res += path_positions[i].x.ToString().Replace(",", ".") + "," + 
                   path_positions[i].y.ToString().Replace(",", ".") + "," +
                   path_positions[i].z.ToString().Replace(",", ".") + ",";
        }

        // colocar path full
        res += "all_path_points,";
        for(int i = 0; i < all_path_points.Count; i++) {
            res += all_path_points[i].x.ToString().Replace(",", ".") + "," + 
                   all_path_points[i].y.ToString().Replace(",", ".") + "," + 
                   all_path_points[i].z.ToString().Replace(",", ".") + ",";
        }

        // colocar forças todas
        int numOfStops = 0;
        res += "path_force,";
        for(int i = 0; i < path_force.Count; i++) {
            res += "rep_" + (i+1) + ",";
            for(int j = 0; j < path_force[i].Count; j++) {
                res += path_force[i][j].x.ToString().Replace(",", ".") + "," + 
                       path_force[i][j].y.ToString().Replace(",", ".") + "," + 
                       path_force[i][j].z.ToString().Replace(",", ".") + ",";
            }
            res += rep_success[i] + ",";
            Debug.Log(rep_success[i]);

            if(rep_success[i].Contains("nok")) {
                numOfStops++;
            }
        }

        // colocar reps
        if(repsDone > totalReps) repsDone = totalReps;
        res += "total_reps," + repsDone + "/" + numRepsText.text + ",";

        // colocar velocidade de execução
        res += "vel," + numVelText.text.Replace("%", "") + ",";

        // colocar data
        System.DateTime dt = System.DateTime.Now;
        string dateString = dt.ToString("yyyy-MM-dd HH:mm");
        //Debug.Log("date: " + dateString);
        res += "date," + dateString + ",";

        // colocar tempo de execução?
        res += "stops," + numOfStops + ",";


        res += "score," + coinsGot + "/" + coinCount;

        writer.WriteLine(res);
        writer.Close();
    }

    /*
    private void OnDrawGizmos() {
        // draw force between waypoints
        var PathLineColor = Color.green;
        var PathPointsColor = Color.green;
        if(force_on_path != null && draw_force) { 

            for (int i = 0; i < force_on_path.Count; i++) {
                if(force_on_path[i] != null && path_force_mags[i] > 1.5f) { 

                    //Debug.Log(i + " :" + path_positions[i] + ", force:" + force_on_path[i]);
                    Gizmos.color = PathLineColor;
                    Gizmos.DrawLine(path_positions[i], force_on_path[i]);
                    Gizmos.color = PathPointsColor;
                    Gizmos.DrawSphere(force_on_path[i], 0.015f);
                }
            }
        }
    }
    */

    public void exitButtonPressed() {
        emergencyStopButtonPressed();

        testingPathBtn.GetComponentInChildren<Text>().text = "Evaluate";
        testingPathBtn.GetComponentsInChildren<Image>()[1].sprite = testBtnImage1;

        stopBtnPressed = 0;

        coinCount = 0;
        waypointsSegments.Clear();
        isExerciseLoaded = 0;
        hintTxt.text = "";
        menuPanel.SetActive(true);
        saveRobotPathPanel.SetActive(false);
        exercisePlayPanel.SetActive(false);
        loadRobotPathPanel.SetActive(false);
        saveReportPanel.SetActive(false);
        reportsPanel.SetActive(false);
        exerciseHelpPanel.SetActive(false);
        forceInfoPanel.SetActive(false);
        setMaxForceBtn.interactable = true;
        configMovPanel.SetActive(false);

        statusMovPanel.SetActive(false);

        onPlayPanel = false;
        draw_force = false;

        foreach(GameObject gO in gameCoins){
            Destroy(gO);
        }
        gameCoins.Clear();

        playPathBtn.GetComponentInChildren<Text>().text = "Play";

        onReportPanel = false;

        //Reseta a lista
       foreach(GameObject gO in pathList){
            Destroy(gO);
        } 

        foreach(LineRenderer l in path_lines){
            Destroy(l);
        }

        // remove center sphere
        if(centerSphere != null) Destroy(centerSphere);

        pathList.Clear();

        // reseta linha do circuito
        lr.positionCount = 0;

        // limpa posiçao atual do robo
        if(kukaPos != null)
            Destroy(kukaPos);

        
        //Reseta a lista de force points
        if(sphereForceList.Count != 0) {
            foreach(GameObject gO in sphereForceList){
            Destroy(gO);
            }
            sphereForceList.Clear();
        } 
        
        // clear btns
        activateWaypoints = false;
        playPathBtn.interactable = false;
        testingPathBtn.interactable = true;
        incStartVel.interactable = false;
        decStartVel.interactable = false;
        incReps.interactable = false;
        decReps.interactable = false;
        stopPlayPathBtn.interactable = false;
        showForceReportBtn.interactable = false;
        saveRobotPathBtn.interactable = true;
        saveReportBtn.interactable = false;
        
        // clear wp
        clearWaypoints();
    }

    public GameObject robotPosPrefab;

    public void TestRobotPath() {
        stopPlayPathBtn.interactable = true;
        
        saveRobotPathBtn.interactable = false;

        isOnPlayOrTest = "test";

        isRepeatingPath = true;

        currentActionForPings = "repeating_exercise";

        if(isExerciseLoaded == 1 || stopBtnPressed == 1) {
            //Debug.Log("==== iiwa.goToBegin() ====");
            iiwa.goToBegin();
            Thread threadGetIsDone = new Thread(GetRobotDistToBeginning);
            threadGetIsDone.Start();
        } else {
            iiwa.testPath();
            testingPathBtn.interactable = false;
        }
        

        if(kukaPos != null)
            Destroy(kukaPos);
        
        //kukaPos = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector3 pos = new Vector3(0,0,0);
        kukaPos = Instantiate(robotPosPrefab);
        kukaPos.layer = renderLayer;
        kukaPos.transform.parent = GameObject.Find("HoldPath").transform;
        kukaPos.name = "KUKA_POSITION";
        //kukaPos.transform.position = path_positions[0];
        //kukaPos.transform.localScale = new Vector3(scaleSphere, scaleSphere, scaleSphere);
        kukaPos.transform.localScale = new Vector3(0.0065f, 0.0065f, 0.0065f);
        kukaPos.transform.localRotation = Quaternion.Euler(0, 0, 0);
        //kukaPos.GetComponent<Renderer>().material.color = Color.green;

        Thread threadGetPosition = new Thread(GetRobotPositionOnLoop);
        threadGetPosition.Start();
    }

    public void PlayRobotPath() {
        hintTxt.text = "";
        mainHintTxt.text = "Training in progress...";
        stopPlayPathBtn.interactable = true;
        playPathBtn.interactable = false;
        clearWpBtnGO.SetActive(false);
        showForceReportBtn.interactable = false;
        saveRobotPathBtn.interactable = false;
        setMaxForceBtn.interactable = false;
        configMovPanel.SetActive(false);

        isOnPlayOrTest = "play";

        // status panel
        numVelFinalText.text = numVelText.text;

        //scoreText.SetActive(false);
        //scorePHText.SetActive(false);
        if(coinCount > 0) {
            coinsGot = 0;
            scoreText.GetComponentInChildren<Text>().text = coinsGot + "/" + coinCount;

            //scoreText.SetActive(true);
            //scorePHText.SetActive(true);
        } else {
            scoreText.GetComponentInChildren<Text>().text = "N/A";
        }
            
        statusMovPanel.SetActive(true);

        stopGettingForces = 0;
        oldRep = 1;

        totalReps = int.Parse(numRepsText.text);

        draw_force = false;
        if(restartReps == 0) {
            path_force.Clear();
        }

        //Debug.Log("path_force.Count :: " + path_force.Count);

        rep_force.Clear();

        toggleShowForceBtn = false;

        //Reseta a lista de force points
        foreach(GameObject gO in sphereForceList){
            Destroy(gO);
        }
        sphereForceList.Clear();

        if(restartReps == 0) {
            all_path_points.Clear();
        }

        restartReps = 0;

        Debug.Log(pathWaypoints);

        currentActionForPings = "repeating_exercise";

        iiwa.setupPath(pathWaypoints);

        if(kukaPos != null)
            Destroy(kukaPos);
        
        //kukaPos = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector3 pos = new Vector3(0,0,0);
        kukaPos = Instantiate(robotPosPrefab);
        kukaPos.layer = renderLayer;
        kukaPos.transform.parent = GameObject.Find("HoldPath").transform;
        kukaPos.name = "KUKA_POSITION";
        //kukaPos.transform.localScale = new Vector3(scaleSphere, scaleSphere, scaleSphere);
        kukaPos.transform.localScale = new Vector3(0.0065f, 0.0065f, 0.0065f);
        kukaPos.transform.localRotation = Quaternion.Euler(0, 0, 0);
        //kukaPos.GetComponent<Renderer>().material.color = Color.green;
    }

    int cTOE = 0;

    void showPathOnScreen(String p, List<Vector3> circuit) {

        if(p != "") {
            path_positions = new List<Vector3>();
            string[] s = p.Split(',');
            
            CIRCUIT_SIZE = int.Parse(s[0]);
            //Debug.Log("CIRCUIT SIZE: " + CIRCUIT_SIZE);

            path_rotations = new List<Vector3>();

            for(int i = 1; i < s.Length - 1; i+=6) {
                float x = float.Parse(s[i], CultureInfo.InvariantCulture);
                float y = float.Parse(s[i+1], CultureInfo.InvariantCulture);
                float z = float.Parse(s[i+2], CultureInfo.InvariantCulture);
                float a = float.Parse(s[i+3], CultureInfo.InvariantCulture);
                float b = float.Parse(s[i+4], CultureInfo.InvariantCulture);
                float c = float.Parse(s[i+5], CultureInfo.InvariantCulture);
                //Debug.Log(x + "," + y + "," + z + "," + a + "," + b + "," + c);

                path_positions.Add(new Vector3(x/1000 * sc, y/1000 * sc, -z/1000 * sc));
                path_rotations.Add(new Vector3(a, b, c));
            }
        } else {
            path_positions = new List<Vector3>(circuit);
        }

        
        //Reseta a lista
        foreach(GameObject gO in pathList){
            Destroy(gO);
        }

        pathList.Clear();

        // draw path
        int ii = 0;
        cTOE = 0;
        path_lines.Clear();

        foreach(Vector3 pos in path_positions) {
            CreatePoint(pos, ii);
            LineRenderer lr_aux = null;

            if(isExerciseLoaded == 0) {
                //Debug.Log("pathIdxs COUNT ::" + pathIdxs.Count);
                
                //Debug.Log("IDX: " + pathIdxs[ii]);
                bool isFinished = false;
                Color lastColor = Color.blue;
                if(ii < pathIdxs.Count) {
                    while(!isFinished) {
                        TOE toe = type_of_exercises_list[cTOE]; 
                        Debug.Log("toe.toe_start :" + toe.toe_start + ",  toe.toe_end" + toe.toe_end);

                        if(pathIdxs[ii] <= toe.toe_end) {
                            // line for each point
                            lr_aux = new GameObject().AddComponent<LineRenderer>();
                            lr_aux.gameObject.layer = renderLayer;
                            lr_aux.material = new Material(Shader.Find("Sprites/Default"));
                            lr_aux.SetWidth(.02f, .02f);
                            
                            if(toe.toe_type == "passive") {
                                lastColor = Color.blue;
                                lr_aux.SetColors(lastColor, lastColor);
                            } else if (toe.toe_type == "active") {
                                //lastColor = new Color(1f, 0.65f, 0f);
                                lastColor = Color.cyan;
                                lr_aux.SetColors(lastColor, lastColor);
                            } 

                            lr_aux.positionCount = 0;
                            lr_aux.transform.parent = GameObject.Find("HoldPathLines").transform; 

                            isFinished = true;
                        } else if (cTOE < type_of_exercises_list.Count) {
                            cTOE++;
                        } else {
                            //lr_aux.SetColors(lastColor, lastColor);
                            break;
                        } 
                    }
                } 
                
            } else {
                lr_aux = new GameObject().AddComponent<LineRenderer>();
                lr_aux.gameObject.layer = renderLayer;
                lr_aux.material = new Material(Shader.Find("Sprites/Default"));
                lr_aux.SetWidth(.02f, .02f);
                lr_aux.SetColors(Color.blue, Color.blue);
                lr_aux.positionCount = 0;
                lr_aux.transform.parent = GameObject.Find("HoldPathLines").transform; 
            }

            if(lr_aux != null)
                path_lines.Add(lr_aux);

            ii++;
        }

        // get center point of circuit
        centerSphere_x = 0; centerSphere_y = 0; centerSphere_z = 0;
        for(int i = 0; i < path_positions.Count; i++) {
            centerSphere_x+=path_positions[i].x;
            centerSphere_y+=path_positions[i].y;
            centerSphere_z+=path_positions[i].z;
        }

        centerSphere_x /= path_positions.Count;
        centerSphere_y /= path_positions.Count;
        centerSphere_z /= path_positions.Count;

        if(centerSphere != null) Destroy(centerSphere);

        centerSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        centerSphere.layer = renderLayer;
        centerSphere.transform.parent = GameObject.Find("HoldPath").transform; 
        centerSphere.name = "CenterSphere";
        centerSphere.transform.position = new Vector3(centerSphere_x, centerSphere_y, centerSphere_z);
        centerSphere.transform.localScale = new Vector3(scaleSphere*0.5f, scaleSphere*0.5f, scaleSphere*0.5f);

        // set camera to "see"/point to the center point
        camera2.transform.LookAt(GameObject.Find("CenterSphere").transform);

        // TODO
        if(onReportPanel) {
            Vector3 cameraReportPos = new Vector3(5.3f, 0.86f, -2.9f);
            camera2.transform.position = cameraReportPos;
        } 
        else
            camera2.transform.position = cameraDefaultPos;

        camera2.transform.rotation = cameraDefaultRot;
    }

    void CreatePoint(Vector3 pos, int ii) {
        GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
 
        tmp.transform.parent = GameObject.Find("HoldPath").transform;
        tmp.layer = renderLayer;
        tmp.name = "Sphere_" + ii;
        tmp.transform.position = pos;
        tmp.transform.localScale = new Vector3(scaleSphere, scaleSphere, scaleSphere);
        tmp.GetComponent<Renderer>().material.color = Color.blue;

        float scStartEnd = 1.3f;
        if(ii == 0) {
            tmp.GetComponent<Renderer>().material.color = Color.green;
            //tmp.transform.localScale = new Vector3(scaleSphere*scStartEnd, scaleSphere*scStartEnd, scaleSphere*scStartEnd);
        } else if (ii == path_positions.Count - 1) {
            tmp.GetComponent<Renderer>().material.color = Color.red;
            //tmp.transform.localScale = new Vector3(scaleSphere*scStartEnd, scaleSphere*scStartEnd, scaleSphere*scStartEnd);
        }


        pathList.Add(tmp);
    }


     public void closeWaypointsPanel() {
        wpSelectPanel.SetActive(false);

        startWaypointIdx = -1; 
        endWaypointIdx = -1;

        startWaypointObj.GetComponent<Renderer>().material.color = Color.blue;
        startWaypointObj.transform.localScale = new Vector3(scaleSphere, scaleSphere, scaleSphere);

        endWaypointObj.GetComponent<Renderer>().material.color = Color.blue;
        endWaypointObj.transform.localScale = new Vector3(scaleSphere, scaleSphere, scaleSphere);
    }


    public void setAllWaypoints(string msg) {
        
        pathWaypoints += msg;

        clearWpBtnGO.SetActive(true);

        // mark all waypoints on circuit
        string[] dataSplit = guiWaypoints.Split(',');


        string[] wpsSplit = waypoints_msg.Split(',');

        List<string> modes = new List<string>(); 
        for(int i = 3; i < wpsSplit.Length; i+=11) {
            modes.Add(wpsSplit[i]);
            Debug.Log("MODE ::: " + wpsSplit[i]);
        }

        int countModes = 0;
        int idxStart = 0, idxEnd = 0;
        for(int i = 0; i < dataSplit.Length; i+=2) {
            Debug.Log("idxStart :: " + dataSplit[i] + ", idxEnd :: " + dataSplit[i+1]);
            
            bool success = int.TryParse(dataSplit[i], out idxStart);
            if (success) {
            }
            else {
                Debug.Log($"Red zone Attempted conversion of '{dataSplit[i] ?? "<null>"}' failed.");
                break;
            } 

            bool success2 = int.TryParse(dataSplit[i+1], out idxEnd);
            if (success2) {
            }
            else {
                Debug.Log($"Red zone Attempted conversion of '{dataSplit[i+1] ?? "<null>"}' failed.");
                break;
            } 

            if(countModes < modes.Count) {
                //Debug.Log("aaa::: " + idxStart + "," + idxEnd + "," + modes[countModes]);
                waypointsSegments.Add(new Segment(idxStart, idxEnd, modes[countModes]));
                countModes++;
            }

            if(modes[i] == "normal") {
                Debug.Log("COIN NOW! ");
                
            }
   
            GameObject startGO = GameObject.Find("Sphere_" + idxStart);
            GameObject endGO = GameObject.Find("Sphere_" + idxEnd);

            startGO.GetComponent<Renderer>().material.color = _orange;
            startGO.transform.localScale = new Vector3(scaleSphere+0.01f, 
                scaleSphere+0.01f, scaleSphere+0.01f);

            endGO.GetComponent<Renderer>().material.color = _orange;
            endGO.transform.localScale = new Vector3(scaleSphere+0.01f, 
                scaleSphere+0.01f, scaleSphere+0.01f);
            
        }

    }


    public void setWaypoint() {
        int start = pathIdxs[startWaypointIdx], end = pathIdxs[endWaypointIdx];
        if(startWaypointIdx > endWaypointIdx) {
            end = pathIdxs[startWaypointIdx];
            start = pathIdxs[endWaypointIdx];
        }

        waypointsIdxsGUI += startWaypointIdx + "," + endWaypointIdx + ",";

        if(startWaypointIdx > endWaypointIdx) {
            waypointsSegments.Add(new Segment(endWaypointIdx, startWaypointIdx, wpMode));
        } else
            waypointsSegments.Add(new Segment(startWaypointIdx, endWaypointIdx, wpMode));
        
        string vWaypoint = "";
        string imp = "3000", type_imp = "", dof = "_", freq = "-1", 
            amp = "-1", angle = "0", angleSpeed = "-1", followImp = "-1";

        if(wpMode == "sine") {
            vWaypoint = numSineVelText.text.Replace("%", "");
            type_imp = "sine";
            dof = DOFDrop.options[DOFDrop.value].text;
            dof = dof.Split(char.Parse(" "))[0];

            freq = FreqDrop.options[FreqDrop.value].text;
            freq = freq.Split(char.Parse(" "))[0];

            amp = AmplitudeDrop.options[AmplitudeDrop.value].text;
            amp = amp.Split(char.Parse(" "))[0];

        } 
        else if(wpMode == "imp") {
            vWaypoint = numImpVelText.text.Replace("%", "");
            imp = impDrop.options[impDrop.value].text;
            imp = imp.Split(char.Parse(" "))[0];

            type_imp = "normal";
        } 
        else if(wpMode == "rotation") {
            vWaypoint = numRotationVelText.text.Replace("%", "");
            type_imp = "rotation";

            angle = angleDrop.options[angleDrop.value].text;
            angle = angle.Split(char.Parse(" "))[0];

            angleSpeed = angleSpeedDrop.options[angleSpeedDrop.value].text;
            angleSpeed = angleSpeed.Split(char.Parse(" "))[0];
        }  
        else if(wpMode == "imp_game") {
            vWaypoint = numImpGameVelText.text.Replace("%", "");
            imp = impFromImpGame;

            type_imp = "normal";
        }

        Debug.Log("Wp " + start + " -> " + end + ", imp:" + imp + ", vel:" + 
                 vWaypoint + "%" + ", type:" + type_imp +  ", DOF: " + dof + ", amp: " + amp + ", freq: " + 
                 freq + ", angle: " + angle + ", angleSpeed: " + angleSpeed + ", followImp: " + followImp);

        pathWaypoints += start + "," + end + "," + vWaypoint + "," + type_imp + "," + imp + "," + 
                dof + "," + freq + "," + amp + "," + angle + "," + angleSpeed + "," + followImp + ",";

        // close window and mark the path
        startWaypointIdx = -1; 
        endWaypointIdx = -1;

        startWaypointObj.GetComponent<Renderer>().material.color = _orange;
        startWaypointObj.transform.localScale = new Vector3(scaleSphere+0.01f, 
            scaleSphere+0.01f, scaleSphere+0.01f);

        endWaypointObj.GetComponent<Renderer>().material.color = _orange;
        endWaypointObj.transform.localScale = new Vector3(scaleSphere+0.01f, 
            scaleSphere+0.01f, scaleSphere+0.01f);

        wpSelectPanel.SetActive(false);

        impPanel.SetActive(false);
        sinePanel.SetActive(false);
        rotationPanel.SetActive(false);
        impGamePanel.SetActive(false);
        clearWpBtnGO.SetActive(true);
        isOnGame = false;

    }

    public void clearWaypoints() {
        clearWpBtnGO.SetActive(false);

        coinCount = 0;

        waypointsSegments.Clear();

        pathWaypoints = "wp,";

        waypointsIdxsGUI = "wp_idx_gui,";

        for(int i = 0; i < pathList.Count; i++) {
            GameObject go = pathList[i];
            if(i == 0) {
                go.GetComponent<Renderer>().material.color = Color.green;
                //tmp.transform.localScale = new Vector3(scaleSphere*scStartEnd, scaleSphere*scStartEnd, scaleSphere*scStartEnd);
            } else if (i == path_positions.Count - 1) {
                go.GetComponent<Renderer>().material.color = Color.red;
                //tmp.transform.localScale = new Vector3(scaleSphere*scStartEnd, scaleSphere*scStartEnd, scaleSphere*scStartEnd);
            }
            else 
                go.GetComponent<Renderer>().material.color = Color.blue;

            go.transform.localScale = new Vector3(scaleSphere, scaleSphere, scaleSphere);
        }

        foreach(GameObject gO in gameCoins){
            Destroy(gO);
        }
        gameCoins.Clear();
    }

    // set waypoints velocity for all modes in dialog
    public void increaseWaypointVelBtn() {
        if(wpMode == "imp") {
            string v1 = numImpVelText.text.Replace("%", "");
            int vel = int.Parse(v1);
            if(vel < 100) {
                vel+=5;
                numImpVelText.text = vel.ToString() + "%";
            }
        } else if(wpMode == "sine") {
            string v1 = numSineVelText.text.Replace("%", "");
            int vel = int.Parse(v1);
            if(vel < 100) {
                vel+=5;
                numSineVelText.text = vel.ToString() + "%";
            }
        }
        else if(wpMode == "rotation") {
            string v1 = numRotationVelText.text.Replace("%", "");
            int vel = int.Parse(v1);
            if(vel < 100) {
                vel+=5;
                numRotationVelText.text = vel.ToString() + "%";
            }
        }
        else if(wpMode == "imp_game") {
            string v1 = numImpGameVelText.text.Replace("%", "");
            int vel = int.Parse(v1);
            if(vel < 100) {
                vel+=5;
                numImpGameVelText.text = vel.ToString() + "%";
            }
        }
    }

    public void decreaseWaypointVelBtn() {
        if(wpMode == "imp") {
            string v1 = numImpVelText.text.Replace("%", "");
            int vel = int.Parse(v1);
            if(vel > 5) {
                vel-=5;
                numImpVelText.text = vel.ToString() + "%";
            }
        } else if(wpMode == "sine") {
            string v1 = numSineVelText.text.Replace("%", "");
            int vel = int.Parse(v1);
            if(vel > 5) {
                vel-=5;
                numSineVelText.text = vel.ToString() + "%";
            }
        }
        else if(wpMode == "rotation") {
            string v1 = numRotationVelText.text.Replace("%", "");
            int vel = int.Parse(v1);
            if(vel > 5) {
                vel-=5;
                numRotationVelText.text = vel.ToString() + "%";
            }
        }
        else if(wpMode == "imp_game") {
            string v1 = numImpGameVelText.text.Replace("%", "");
            int vel = int.Parse(v1);
            if(vel > 5) {
                vel-=5;
                numImpGameVelText.text = vel.ToString() + "%";
            }
        }
        
    }

    public void saveRobotPathBtnPressed() {
        nameOfPath = "";
        saveRobotPathPanel.SetActive(true);
    }

    public void closeSaveRobotPathBtnPressed() {
        saveRobotPathPanel.SetActive(false);
    }

    public void SaveRobotPathInsidePanelBtnPressed() {
        nameOfPath = savePathInputField.text;
        if(nameOfPath != "") {
            saveRobotPathPanel.SetActive(false);
            iiwa.getFullPath();
        } else {
            Debug.Log("Error: name file of the robot path!");
        }
    }

    public void loadExerciseBtnPressed() {
        loadRobotPathPanel.SetActive(true);

        // set dropdown with all paths
        savedPathsDropDown.ClearOptions();

        List<string> m_savedPathsFiles = new List<string>();

        DirectoryInfo dir = new DirectoryInfo(robot_saved_paths);
        FileInfo[] info = dir.GetFiles("*.txt");
        foreach (FileInfo f in info) {
            string p = f.FullName;
            string filename = Path.GetFileName(p);
            filename = filename.Replace(".txt", "");
            m_savedPathsFiles.Add(filename);
         }
        
        savedPathsDropDown.AddOptions(m_savedPathsFiles);
    }

    public void closeLoadExerciseBtnPressed() {
        loadRobotPathPanel.SetActive(false);
    }


    string waypoints_msg = "", guiWaypoints = "";
    public async void loadChoosedExerciseBtnPressed() {
        int m_DropdownValue = savedPathsDropDown.value;

        string pathChoosed = savedPathsDropDown.options[m_DropdownValue].text;
        //Debug.Log(pathChoosed);
    
        string path = robot_saved_paths + pathChoosed + ".txt";
        StreamReader reader = new StreamReader(path); 
    
        String msg = reader.ReadToEnd();

        // get circuit and waypoints
        string[] dataSplit = msg.Split(',');

        string path_msg = "";
        int saveWpsIdx = 0;
        for(int i = 1; i < dataSplit.Length; i++) {
            if(dataSplit[i].Contains("all_waypoints")) {
                saveWpsIdx = i;
                break;
            } 
            else {
                path_msg += dataSplit[i] + ",";
            }
        }

        Debug.Log("path_msg :: " + path_msg);

        waypoints_msg = "";
        int guiWpsIdx = 0;
        for(int i = saveWpsIdx + 2; i < dataSplit.Length; i++) {
            if(dataSplit[i].Contains("wp_idx_gui")) {
                guiWpsIdx = i;
                break;
            } 
            else {
                waypoints_msg += dataSplit[i] + ",";
            }
            
        }

        Debug.Log("waypoints_msg :: " + waypoints_msg);

        guiWaypoints = "";
        for(int i = guiWpsIdx + 1; i < dataSplit.Length; i++) {
            guiWaypoints += dataSplit[i] + ","; 
        }

        Debug.Log("guiWaypoints_msg :: " + guiWaypoints);

        isExerciseLoaded = 1;

        iiwa.sendRobotPath(path_msg);

        reader.Close();

        loadRobotPathPanel.SetActive(false);
    }

    public void removeChoosedExerciseBtnPressed() {
        int m_DropdownValue = savedPathsDropDown.value;

        string pathChoosed = savedPathsDropDown.options[m_DropdownValue].text;
        Debug.Log(pathChoosed);
    
        string path = robot_saved_paths + pathChoosed + ".txt";
        if ( !File.Exists( path ) ) {
            Debug.Log("Error file does not exist to delete!");
        } else {
            UnityEditor.FileUtil.DeleteFileOrDirectory(path);
            #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
            #endif
        }
        
        loadRobotPathPanel.SetActive(false);

        // check if can load path
        DirectoryInfo dir = new DirectoryInfo(robot_saved_paths);
        FileInfo[] info = dir.GetFiles("*.txt");
        if(info.Length > 0) {
            loadExerciseBtn.interactable = true;
        } else loadExerciseBtn.interactable = false;
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

    public void resetCameraPositionBtnPressed() {
        //Vector3 camPos = new Vector3(5.7f, 1.3f, -1.5f);
        camera2.transform.position = cameraDefaultPos;
        
        //Quaternion camRot = Quaternion.Euler(5f, -80f, 90f);
        camera2.transform.rotation = cameraDefaultRot;
    }

    public static void robotResponseCallback(String rcv) {
        msg = rcv;
    }

    void OnApplicationQuit() {
        Debug.Log("Application ending after " + Time.time + " seconds");
        isRobotConnected = false;
        if(iiwa.getConnectionStatus())
            iiwa.closeTCPConnection();
    }

}
