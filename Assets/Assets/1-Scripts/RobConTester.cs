using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System;
using System.Globalization;
using TMPro;


public class RobConTester : MonoBehaviour
{
    public struct RobConInfo
    {
        public RobConInfo(int iD,int riD, Vector3 upos, Vector3 rpos, Quaternion urot, Quaternion rrot, long uts, long rts)
        {
            id = iD;
            rid = riD;

            unityPosition = upos;
            robotPosition = rpos;

            unityRotation = urot;
            robotRotation = rrot;

            unityTimeStamp = uts;
            robotTimeStamp = rts;
        }

        public int id;
        public int rid;

        public Vector3 unityPosition;
        public Vector3 robotPosition;

        public Quaternion unityRotation;
        public Quaternion robotRotation;

        public long unityTimeStamp;
        public long robotTimeStamp;
    }

    HandManager handManager;
    GUI_IIWA erc;
    List<RobConInfo> robConInfos;
    int currentId;
    DirectoryInfo dInfo;

    public int numberOfCalibrations;
    int calibrationCounter;
    public bool isRightController = true;

    public UnityEvent<float> newRecording;
    public TextMeshProUGUI coordText;

    Vector3 unityPos;
    Vector3 unityRot;

    public GameObject robotWorld;
    public GameObject robotHeadDebugger;
    public Vector3 robotOffset;

    //Média Quadrática

    float mqPosX, mqPosY, mqPosZ;
    float mqRotX, mqRotY, mqRotZ;

    bool firstPoint = true;

    Vector3 posDelta;
    Vector3 rotDelta;

    WorldAligner worldAligner;

    int pointcounter = 0;

    float somatorioDistancePos = 0;
    float somatorioDistanceRot = 0;

    public GameObject diferenca;
    public TextMesh txtMesh;

    public Vector3 mods = new Vector3(1,1,1);

    // Start is called before the first frame update
    void Start()
    {
        handManager = FindObjectOfType<HandManager>();
        erc = GetComponent<GUI_IIWA>();
        worldAligner = FindObjectOfType<WorldAligner>();

        dInfo = new DirectoryInfo(GetPath());

        calibrationCounter = 1;

        NewRecording();

        //InvokeRepeating("tmpRecord", 0.1f, 0.1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            NewRecording();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveInformation();
        }
        

    }

    //===================================================
    //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
    //Information Saving

    public void NewRecording()
    {
        robConInfos = new List<RobConInfo>();
        currentId = 0;
    }

    public void RobotRecord(string str)
    {
        string[] splitArray = str.Split(char.Parse(","));

        RecordPosition(
            int.Parse(splitArray[0]),
            long.Parse(splitArray[1]),
            PositionFromString(str),
            RotationFromString(str));
    }

    public void RecordPosition(int robotID, long robotTimeStamp , Vector3 robotPos, Vector3 robotRot)
    {
        UpdateUnityPositionAndRotation();

        robotRot = new Vector3(robotRot.z * Mathf.Rad2Deg * mods.x, robotRot.y * Mathf.Rad2Deg * mods.y, robotRot.x * Mathf.Rad2Deg * mods.z);

        robotPos = new Vector3(robotPos.x / 1000, robotPos.y / 1000, robotPos.z / 1000);

        //float distance = Vector3.Distance(unityPos, robotPos);

        robotPos = worldAligner.ConvertPoint(robotPos);
        Quaternion robotRotQuat = worldAligner.ConvertRotation(robotRot);
        Quaternion unityRotQuat = worldAligner.ConvertUnityRotation();

        //diferenca.transform.localRotation = robotRotQuat * Quaternion.Inverse(unityRotQuat);

        //txtMesh.text = ("Diferença Euler:" +
        //    (robotRotQuat.eulerAngles.x - unityRotQuat.eulerAngles.x) + " , " +
        //    (robotRotQuat.eulerAngles.y - unityRotQuat.eulerAngles.y) + " , " +
        //    (robotRotQuat.eulerAngles.z - unityRotQuat.eulerAngles.z) + ";");

        SomaPosMedia(unityPos, robotPos);
        SomaRotMedia(unityRot, robotRot);

        SomatorioSquaredEuclideanDistance(unityPos, robotPos,unityRot, robotRot);
        pointcounter++;

        long timeStamp = GetUnixTimeStamp();

        robConInfos.Add(new RobConInfo(currentId, robotID, unityPos, robotPos, unityRotQuat, robotRotQuat, timeStamp, robotTimeStamp));

        currentId++;


        //newRecording.Invoke(distance);
    }

    public void SaveInformation()
    {
        //Debug.Log("Saving...");

        DateTime dt = System.DateTime.Now;
        string date = "_" + dt.Day.ToString() + "-" + dt.Month.ToString() + "-" + dt.Year.ToString() + "_" + dt.Hour.ToString() + "-" + dt.Minute.ToString() + "-" + dt.Second.ToString();
        int numberFiles = DataCount(dInfo);
        string filename = "Precision_Results_" + numberFiles + date + ".csv";
        StreamWriter sw = File.CreateText(GetPath() + filename);

        string finalstring = 
            "Unity ID,Robot ID," +
            "Unity x Position,Unity y Position,Unity z Position," +
            "Robot x Position,Robot y Position,Robot z Position," +
            "Unity x Rotation,Unity y Rotation,Unity z Rotation,Unity w Rotation," +
            "Robot x Rotation,Robot y Rotation,Robot z Rotation,Robot w Rotation," +
            "Unity Time Stamp,Robot Time Stamp\n";

        for (int i = 0; i < robConInfos.Count; i++)
        {

            //Rotation
            Quaternion uniRot = robConInfos[i].unityRotation;
            Quaternion robRot = robConInfos[i].robotRotation;

            finalstring +=
                //IDs
                robConInfos[i].id.ToString(CultureInfo.InvariantCulture) + "," +
                robConInfos[i].rid.ToString(CultureInfo.InvariantCulture) + "," +

                //Position
                robConInfos[i].unityPosition.x.ToString(CultureInfo.InvariantCulture) + "," +
                robConInfos[i].unityPosition.y.ToString(CultureInfo.InvariantCulture) + "," +
                robConInfos[i].unityPosition.z.ToString(CultureInfo.InvariantCulture) + "," +

                robConInfos[i].robotPosition.x.ToString(CultureInfo.InvariantCulture) + "," +
                robConInfos[i].robotPosition.y.ToString(CultureInfo.InvariantCulture) + "," +
                robConInfos[i].robotPosition.z.ToString(CultureInfo.InvariantCulture) + "," +

                uniRot.x.ToString(CultureInfo.InvariantCulture) + "," +
                uniRot.y.ToString(CultureInfo.InvariantCulture) + "," +
                uniRot.z.ToString(CultureInfo.InvariantCulture) + "," +
                uniRot.w.ToString(CultureInfo.InvariantCulture) + "," +

                robRot.x.ToString(CultureInfo.InvariantCulture) + "," +
                robRot.y.ToString(CultureInfo.InvariantCulture) + "," +
                robRot.z.ToString(CultureInfo.InvariantCulture) + "," +
                robRot.w.ToString(CultureInfo.InvariantCulture) + "," +

                //robConInfos[i].unityRotation.x.ToString(CultureInfo.InvariantCulture) + "," +
                //robConInfos[i].unityRotation.y.ToString(CultureInfo.InvariantCulture) + "," +
                //robConInfos[i].unityRotation.z.ToString(CultureInfo.InvariantCulture) + "," +

                //robConInfos[i].robotRotation.x.ToString(CultureInfo.InvariantCulture) + "," +
                //robConInfos[i].robotRotation.y.ToString(CultureInfo.InvariantCulture) + "," +
                //robConInfos[i].robotRotation.z.ToString(CultureInfo.InvariantCulture) + "," +

                //TimeStamps
                robConInfos[i].unityTimeStamp.ToString(CultureInfo.InvariantCulture) + "," +
                robConInfos[i].robotTimeStamp.ToString(CultureInfo.InvariantCulture) + "\n";
        }

        ErroQuadratico();

        finalstring += "\nErro quadrático médio Posição: ( " + mqPosX.ToString(CultureInfo.InvariantCulture) + " ; " + mqPosY.ToString(CultureInfo.InvariantCulture) + " ; " + mqPosZ.ToString(CultureInfo.InvariantCulture) + " )"
            /*",Erro quadrático médio Rotação: ( " + mqRotX.ToString(CultureInfo.InvariantCulture) + " , " + mqRotY.ToString(CultureInfo.InvariantCulture) + " , " + mqRotZ.ToString(CultureInfo.InvariantCulture) */
            /*",Erro quadrático médio Rotação: ( " + mqRotX.ToString(CultureInfo.InvariantCulture) + " , " + mqRotY.ToString(CultureInfo.InvariantCulture) + " , " + mqRotZ.ToString(CultureInfo.InvariantCulture) */ 
            + " )\n Somatório Distancia Posição: " + somatorioDistancePos + "\n";

        finalstring += "\n  ";

        sw.WriteLine(finalstring);
        sw.Flush();
        sw.Close();

        Debug.Log("Saved");
    }

    string GetPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/Data/";
#elif UNITY_ANDROID
        return Application.persistentDataPath;
#else
        return Application.dataPath +"/";
#endif
    }

    public static int DataCount(DirectoryInfo d)
    {
        int i = 0;
        // Add file sizes.
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            if (fi.Extension.Contains("csv"))
                i++;
        }
        return i;
    }

    long GetUnixTimeStamp()
    {
        return new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
    }
    //\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
    //===================================================

//    public void DebugPosition(string str)
//    {
//        Debug.Log("Started debug position");
        
//        string[] splitArray = str.Split(char.Parse(","));

//        UpdateUnityPositionAndRotation();

//        //coordText.text =
//        //    "Unity Position─┬─X: " + unityPos.x +
//        //    "\n                       ├─Y: " + unityPos.y +
//        //    "\n                       └─Z: " + unityPos.z +
//        //    "\nRobot Position─┬─X: " + splitArray[1] +
//        //    "\n                        ├─Y: " + splitArray[2] +
//        //    "\n                        └─Z: " + splitArray[3] +
//        //    "\nUnity Rotation─┬─X: " + unityRot.x +
//        //    "\n                       ├─Y: " + unityRot.y +
//        //    "\n                       └─Z: " + unityRot.z +
//        //    "\nRobot Rotation─┬─X: " + splitArray[4] +
//        //    "\n                        ├─Y: " + splitArray[5] +
//        //    "\n                        └─Z: " + splitArray[6];

//        Vector3 robotPosInUnity = PositionFromString(str) - robotOffset;

//        robotHeadDebugger.transform.localPosition = robotPosInUnity;


//        coordText.text =
//            "Unity Position─┬─X: " + unityPos.x +
//            "\n                       ├─Y: " + unityPos.y +
//            "\n                       └─Z: " + unityPos.z;

//        Invoke("GetPosition", 0.25f);
//    }

//#pragma warning disable IDE0051
//    void GetPosition()
//#pragma warning restore IDE0051 // Remover membros privados não utilizados
//    {
//        erc.GetPosition();
//    }

    void UpdateUnityPositionAndRotation()
    {
        if (isRightController)
        {
            unityPos = handManager.rightController.transform.position;
            unityRot = handManager.rightController.transform.eulerAngles;
        }
        else
        {
            unityPos = handManager.leftController.transform.position;
            unityRot = handManager.leftController.transform.eulerAngles;
        }
    }

    //public void InitialPositionCalibration(string str)
    //{
    //    //string[] splitArray = str.Split(char.Parse(","));

    //    UpdateUnityPositionAndRotation();

    //    robotWorld.transform.Rotate(RotationFromString(str));
    //    robotOffset = PositionFromString(str);

    //    robotWorld.transform.position = unityPos;

    //}

    public void Initialise1(string str)
    {
        UpdateUnityPositionAndRotation();

        robotWorld.transform.position = unityPos;
        robotOffset = PositionFromString(str);

        //erc.Init2();

    }

    public void Initialise2(string str)
    {
        UpdateUnityPositionAndRotation();

        robotWorld.transform.LookAt(PositionFromString(str));

    }

    float FloatParse(string str)
    {
        return float.Parse(str, CultureInfo.InvariantCulture);
    }

    Vector3 PositionFromString(string str)
    {
        string[] splitArray = str.Split(char.Parse(","));

        return new Vector3(FloatParse(splitArray[2]), FloatParse(splitArray[3]), FloatParse(splitArray[4]));
    }

    Vector3 RotationFromString(string str)
    {
        string[] splitArray = str.Split(char.Parse(","));

        return new Vector3(FloatParse(splitArray[5]), FloatParse(splitArray[6]), FloatParse(splitArray[7]));
    }

    void SomaPosMedia(Vector3 unity, Vector3 kuka)
    {
        mqPosX += Mathf.Pow(unity.x - kuka.x, 2);
        mqPosY += Mathf.Pow(unity.y - kuka.y, 2);
        mqPosZ += Mathf.Pow(unity.z - kuka.z, 2);
    }
    void SomaRotMedia(Vector3 unity, Vector3 kuka)
    {
        mqRotX += Mathf.Pow(unity.x - kuka.x, 2);
        mqRotY += Mathf.Pow(unity.y - kuka.y, 2);
        mqRotZ += Mathf.Pow(unity.z - kuka.z, 2);
    }

    void ErroQuadratico()
    {
        //posiçao
        mqPosX = mqPosX / pointcounter;
        mqPosY = mqPosY / pointcounter;
        mqPosZ = mqPosZ / pointcounter;

        //rotyaçao
        mqRotX = mqRotX / pointcounter;
        mqRotY = mqRotY / pointcounter;
        mqRotZ = mqRotZ / pointcounter;
    }

    void SomatorioSquaredEuclideanDistance(Vector3 unityPos, Vector3 robotPos, Vector3 unityRot, Vector3 robotRot)
    {
        somatorioDistancePos += Vector3.Distance(unityPos, robotPos);
        somatorioDistanceRot += Vector3.Distance(unityRot, robotRot);
    }

    //Vector3 Delta(Vector3 unity, Vector3 robot)
    //{
    //    Vector3 result;

    //    result = new Vector3(unity.x - robot.x, unity.y - robot.y, unity.z - robot.z);

    //    return result;
    //}
} 
