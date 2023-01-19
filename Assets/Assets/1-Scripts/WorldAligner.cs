using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAligner : MonoBehaviour
{
    public GameObject robotWorld;
    HandManager handManager;
    GUI_IIWA gui_iiwa;
    public bool isRightController = true;
    Vector3 unityPos;
    public Quaternion unityRot;

    Vector3 origin;
    Vector3 forward;
    Vector3 up;

    public Vector3 robotOrigin;
    public Quaternion rotationOrigin;
    public Quaternion unityROrigin;

    public GameObject calculator;
    Vector3 scale;

    //REWORK

    public List<Vector3> origins;
    public List<Vector3> forwards;
    public List<Vector3> ups;

    int state;

    //public float distanceStartRecording = 0.02f;

    public GameObject robotRotationDebugger;
    public GameObject unityRotationDebugger;
    public GameObject mixedTester;

    public Quaternion offset1, offset2;

    public bool firstCall = true;
    public Quaternion desvio;
    public Quaternion diferenca;
 

    // Start is called before the first frame update
    void Start()
    {
        handManager = FindObjectOfType<HandManager>();
        gui_iiwa = FindObjectOfType<GUI_IIWA>();

        ResetCalibration();
    }

    public void ResetCalibration()
    {
        //Debug.Log("Resetting Calibration");

        origins = new List<Vector3>();
        forwards = new List<Vector3>();
        ups  = new List<Vector3>();

        state = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUnityPositionAndRotation();



    }

    public void SetOrigin(Vector3 input, Vector3 rotation)
    {
        UpdateUnityPositionAndRotation();

        robotOrigin = new Vector3(-input.x, input.y, input.z);
        rotationOrigin = Quaternion.Euler(rotation);
        //unityROrigin = new Vector3(unityRot.x, unityRot.y, unityRot.z);
        unityROrigin = unityRot;

        //Debug.Log("initial rot: " + unityROrigin);

        //REWORK
        origin = unityPos;

        //origins.Add(unityPos);
        //origin = MediaVetores(origins);

        MessageController.Message("Robot's origin set to: " + origin);

        //if(state == 0)
        //{
        //    state = 1;
        //    gui_iiwa.GoToForward();
        //}
        //else if(state == 2)
        //{
        //    state = 3;
        //    gui_iiwa.GoToUp();
        //}
        //else if(state == 4)
        //{
        //    SetRobotWorld();
        //    state = 0;
        //}
    }
    public void SetForward()
    {
        UpdateUnityPositionAndRotation();
        forward = (unityPos - origin).normalized;

        //REWORK
        //forwards.Add(forward);
        //

        MessageController.Message("Robot's forward set to: " + forward);

        //(0,0,1)
    }
    public void SetUp()
    {
        UpdateUnityPositionAndRotation();
        up = (unityPos - origin).normalized;

        //REWORK
        //ups.Add(up);
        //

        MessageController.Message("Robot's up set to: " + up);

        //(0,1,0)
    }
    public void SetRobotWorld() 
    {
        //forward = MediaVetores(forwards).normalized;
        //Debug.Log("Média valores forward: " + forward);
        //up = MediaVetores(ups).normalized;
        //Debug.Log("Média valores up: " + up);

        Quaternion orientation = Quaternion.LookRotation(forward, up);

        robotWorld.transform.position = origin;

        robotWorld.transform.rotation = orientation;
    }

    public void UpdateUnityPositionAndRotation()
    {
        if (isRightController)
        {
            unityPos = handManager.rightController.transform.position;
            unityRot = handManager.rightController.transform.rotation;
        }
        else
        {
            unityPos = handManager.leftController.transform.position;
            unityRot = handManager.leftController.transform.rotation;
        }
    }

    public Vector3 ConvertPoint(Vector3 input)
    {
        Vector3 temp = new Vector3(-input.x, input.y, input.z);

        Vector3 result = temp - robotOrigin;

        calculator.transform.localPosition = new Vector3(result.x, result.y, result.z);

        result = calculator.transform.position;

        return result;
    }

    public Quaternion ConvertRotation(Vector3 input)
    {
        Quaternion result = Quaternion.Euler(input);

        calculator.transform.localRotation = result;

        //calculator.transform.localRotation *= offset1;

        //calculator.transform.localRotation *= offset2;

        //if (firstCall)
        //{
        //    firstCall = false;

        //    UpdateUnityPositionAndRotation();

        //    desvio = unityRot * Quaternion.Inverse(calculator.transform.localRotation);

        //    Debug.Log("UnityRot Orientate: " + unityRot);
        //}

        //Real
        calculator.transform.localRotation = desvio * calculator.transform.localRotation;

        calculator.transform.localRotation *= offset1;

        calculator.transform.localRotation *= offset2;

        result = calculator.transform.rotation;

        //DEBUG 
        robotRotationDebugger.transform.localRotation = result;
        diferenca = result * Quaternion.Inverse(unityRot);
        mixedTester.transform.rotation = diferenca;
        Debug.Log(mixedTester.transform.rotation);

        return result;
    }


    public Quaternion ConvertUnityRotation()
    {
        UpdateUnityPositionAndRotation();

        Quaternion result = /*Quaternion.Inverse(unityROrigin) * */ unityRot;

        //DEBUG
        unityRotationDebugger.transform.localRotation = result;

        return result;
    }

    //Vector3 MediaVetores(List<Vector3> lista)
    //{

    //    //OLD
    //    Vector3 result = Vector3.zero;

    //    //MAYBE NEW?
    //    for (int i = 0; i < lista.Count; i++)
    //    {
    //        result = Vector3.Lerp(result, lista[i], 0.5f);
    //    }

    //    return result;
    //}

    public void StartCalibration()
    {
        //gui_iiwa.GoToOrigin();

        //Debug.Log("Started Calibration");
    }

    public void ReachedForward()
    {
        //state = 2;
        //gui_iiwa.GoToOrigin();
        SetForward();
    }

    public void ReachedUp()
    {
        //state = 4;
        //gui_iiwa.GoToOrigin();
        SetUp();
    }

}
