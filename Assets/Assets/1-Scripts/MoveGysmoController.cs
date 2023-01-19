using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGysmoController : MonoBehaviour
{
    [HideInInspector]
    public List<MoveGysmo> gysmos;

    public Camera targetCamera;
    public GameObject controlledObject;

    public bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("ChangeCamera", 0.1f);
    }

    public void ChangeCamera()
    {
        foreach(MoveGysmo gysmo in gysmos)
        {
            gysmo.interactionCamera = targetCamera;
        }
    }

    public void ChangeControlledObjectPosition()
    {
        if(controlledObject)
            controlledObject.transform.position = transform.position;
    }

    public void HideTool()
    {
        transform.position = new Vector3(10000, 1, 1);
    }

    void Update()
    {
        //Select Point

        if (Input.GetMouseButtonDown(0) && canMove)
        {
            Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.collider.gameObject.tag == "MouseGrabbable")
                {
                    controlledObject = hitInfo.collider.gameObject;
                    transform.position = hitInfo.collider.gameObject.transform.position;
                }
                else if (hitInfo.collider.gameObject.tag != "Gizmo")
                {
                    controlledObject = null;
                    transform.position = hitInfo.collider.gameObject.transform.position;
                }
            }
        }
    }
}
