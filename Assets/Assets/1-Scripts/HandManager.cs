using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class HandManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject leftController;
    [HideInInspector]
    public GameObject rightController;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 60 == 0)
        {
            foreach (IMixedRealityController controller in InputSystem.DetectedControllers)
            {
                if (controller.Visualizer?.GameObjectProxy != null)
                {
                    if (controller.ControllerHandedness.IsLeft())
                    {
                        leftController = controller.Visualizer.GameObjectProxy;
                    }
                    else
                    {
                        rightController = controller.Visualizer.GameObjectProxy;
                    }
                }
                else
                {
                    Debug.Log("Controller has no visualizer!");
                }
            }
        }
    }
}
