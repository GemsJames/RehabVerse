using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OldHandToRobotPositionTesting : MonoBehaviour
{

    HandManager handManager;

    [Range(0,0.5f)]
    public float recordDelay;
    bool recording = false;

    List<Vector3> controllerPositions;
    List<Vector3> robotPositions;
    List<float> distancePositions;

    public UnityEvent<float> newDistance;

    // Start is called before the first frame update
    void Start()
    {
        handManager = FindObjectOfType<HandManager>();
        recording = false;

        controllerPositions = new List<Vector3>();
        robotPositions = new List<Vector3>();
        distancePositions = new List<float>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Record()
    {
        Debug.Log("Record");

        if (!recording)
        {
            controllerPositions = new List<Vector3>();
            robotPositions = new List<Vector3>();

            InvokeRepeating("SavePositions", 0, recordDelay);
            recording = true;
        }
        else
        {
            recording = false;
            CancelInvoke("SavePositions");
        }
    }

    void SavePositions()
    {
        Vector3 handPos = handManager.rightController.transform.position;
        Vector3 robotPos = Vector3.zero;

        float distance = Vector3.Distance(handPos, robotPos);

        controllerPositions.Add(handPos);
        robotPositions.Add(robotPos);
        distancePositions.Add(distance);

        newDistance.Invoke(distance);
    }
}
