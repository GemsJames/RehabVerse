using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerFollow : MonoBehaviour
{
    HandManager handManager;

    public bool isRightController;

    Vector3 unityPos;
    Vector3 unityRot;

    void Start()
    {
        handManager = FindObjectOfType<HandManager>();
    }

    // Start is called before the first frame update
    void Update()
    {
        UpdateUnityPositionAndRotation();

        transform.position = unityPos;
    }

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
}
