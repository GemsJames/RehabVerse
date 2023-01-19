using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class PathController : MonoBehaviour
{
    private IMixedRealityInputSystem inputSystem = null;
    protected IMixedRealityInputSystem InputSystem
    {
        get
        {
            if (inputSystem == null)
            {
                MixedRealityServiceRegistry.TryGetService<IMixedRealityInputSystem>(out inputSystem);
            }
            return inputSystem;
        }
    }

    GameObject rightController;
    GameObject leftController;

    public GameObject linePrefab;
    LineRenderer line;
    public float linePointDistance;
    bool recordingLine, playing;

    public GameObject pointPrefab;
    public int intersectionsPerPoint;
    int counter;

    public List<GameObject> paths;
    GameObject currentPath;

    CapsuleCollider ccollider;

    PointController hoveredPoint;
    public PointController selectedPoint;

    public UnityEvent<PointController> pointSelected;

    public GameObject debugPlane;

    private void Start()
    {
        counter = 0;
        recordingLine = false;

        paths = new List<GameObject>();

        hoveredPoint = null;
    }

    // Update is called once per frame
    void Update()
    {
        DetectControllers();

        if (recordingLine)
        {

            if ((rightController.transform.GetChild(0).transform.position - line.GetPosition(line.positionCount - 2)).magnitude >= linePointDistance)
            {
                line.positionCount++;
                line.SetPosition(line.positionCount - 1, rightController.transform.GetChild(0).transform.position - currentPath.transform.position);

                counter++;
                if(counter > intersectionsPerPoint)
                {
                    Instantiate(pointPrefab, rightController.transform.GetChild(0).transform.position, Quaternion.identity, currentPath.transform);

                    counter -= intersectionsPerPoint;
                }

            } 
        }

    }

    void DetectControllers()
    {
        // Log something every 60 frames.
        if (Time.frameCount % 60 == 0)
        {
            foreach (IMixedRealityController controller in InputSystem.DetectedControllers)
            {
                if (controller.Visualizer?.GameObjectProxy != null)
                {
                    //Debug.Log("Visualizer Game Object: " + controller.Visualizer.GameObjectProxy);

                    if (controller.ControllerHandedness.IsLeft())
                    {
                        leftController = controller.Visualizer.GameObjectProxy;
                    }
                    else/* if (controller.ControllerHandedness.IsRight())*/
                    {
                        rightController = controller.Visualizer.GameObjectProxy;
                        ccollider = rightController.transform.GetChild(0).GetComponent<CapsuleCollider>();

                    }

                }
                else
                {
                    //Debug.Log("Controller has no visualizer!");
                }
            }
        }
    }

    public void StartRecordingLine()
    {
        currentPath = new GameObject("New Path");
        currentPath.transform.position = rightController.transform.GetChild(0).transform.position;

        line = Instantiate(linePrefab, currentPath.transform).GetComponent<LineRenderer>();

        counter = 1;
        line.positionCount = 1;
        line.SetPosition(0, rightController.transform.GetChild(0).transform.position - currentPath.transform.position);

        Instantiate(pointPrefab, rightController.transform.GetChild(0).transform.position, Quaternion.identity, currentPath.transform);

        recordingLine = true;
        playing = false;
        ccollider.enabled = false;


        //Instantiate(debugPlane, rightController.transform.GetChild(0).transform.position, rightController.transform.GetChild(0).transform.rotation);

    }

    public void StopRecordingLine()
    {
        line.positionCount++;
        line.SetPosition(line.positionCount -1, rightController.transform.GetChild(0).transform.position - currentPath.transform.position);

        paths.Add(currentPath);

        recordingLine = false;
        playing = true;
        ccollider.enabled = true;
    }

    public void SetActivePoint(PointController gO)
    {
        hoveredPoint = gO;
    }

    public bool IsSamePoint(GameObject gO)
    {
        if (gO == hoveredPoint)
            return true;
        else
            return false;
    }

    public void SelectPoint()
    {
        if(hoveredPoint != null)
        {
            selectedPoint = hoveredPoint;
            Debug.Log("Selected: " + selectedPoint.name);

            pointSelected.Invoke(selectedPoint);
        }
    }
}
