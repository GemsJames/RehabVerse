using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    public CinemachineFreeLook cfl;

    float topRigH;
    float midRigH, midRigR;
    float botRigH, botRigR;

    float scale;
    float lastScale;

    public float initialSpeedX;
    public float initialSpeedY;

    // Start is called before the first frame update
    void Start()
    {
        CameraMoving(false);

        topRigH = cfl.m_Orbits[0].m_Height;
        midRigH = cfl.m_Orbits[1].m_Height;
        botRigH = cfl.m_Orbits[2].m_Height;

        midRigR = cfl.m_Orbits[1].m_Radius;
        botRigR = cfl.m_Orbits[2].m_Radius;

        scale = 1;
        lastScale = 1;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            CameraMoving(true);
        }
        if (Input.GetMouseButtonUp(1))
        {
            CameraMoving(false);
        }

        scale -= Input.GetAxis("Mouse ScrollWheel");

        if (scale < 0)
            scale = 0;

        if(lastScale != scale)
            ZoomCamera(scale);

        lastScale = scale;
    }

    public void CameraMoving(bool b)
    {
        if(b == false)
        {
            cfl.m_XAxis.m_MaxSpeed = 0;
            cfl.m_YAxis.m_MaxSpeed = 0;
        }
        else
        {
            cfl.m_XAxis.m_MaxSpeed = initialSpeedX;
            cfl.m_YAxis.m_MaxSpeed = initialSpeedY;
        }
    }

    public void ZoomCamera(float f)
    {
        cfl.m_Orbits[0].m_Height = topRigH * f;
        cfl.m_Orbits[1].m_Height = midRigH * f;
        cfl.m_Orbits[2].m_Height = botRigH * f;

        cfl.m_Orbits[1].m_Radius = midRigR * f;
        cfl.m_Orbits[2].m_Radius = botRigR * f;
    }
}
