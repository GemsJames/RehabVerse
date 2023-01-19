using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGysmo : MonoBehaviour
{
    Vector3 mOffset;
    float mZCoord;

    MoveGysmoController mgc;
    GameObject parent;

    public bool moveInX;
    public bool moveInY;
    public bool moveInZ;

    [HideInInspector]
    public Camera interactionCamera;

    // Start is called before the first frame update
    void Start()
    {
        mgc = GetComponentInParent<MoveGysmoController>();

        mgc.gysmos.Add(this);

        parent = mgc.gameObject;
    }

    private void OnMouseDown()
    {
        mZCoord = interactionCamera.WorldToScreenPoint(parent.transform.position).z;

        mOffset = parent.transform.position - GetModifiedPos();
    }

    private Vector3 GetModifiedPos()
    {
        //pixel coordinates
        Vector3 mousePoint = Input.mousePosition;

        //z coordinate of game object
        mousePoint.z = mZCoord;

        Vector3 target = interactionCamera.ScreenToWorldPoint(mousePoint);

        if (!moveInX)
            target.x = parent.transform.position.x;
        if (!moveInY)
            target.y = parent.transform.position.y;
        if (!moveInZ)
            target.z = parent.transform.position.z;

        return target;
    }

    private void OnMouseDrag()
    {
        parent.transform.position = GetModifiedPos() + mOffset;

        mgc.ChangeControlledObjectPosition();
    }
}
