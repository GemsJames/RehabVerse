using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
    public Vector3 PositionMult;
    public Vector3 PositionDirection;
    private Vector3 positionTemp;

    void Start()
    {
        positionTemp = this.transform.position;
    }

    void Update()
    {
        positionTemp += PositionDirection * Time.deltaTime;
        PositionDirection += PositionMult * Time.deltaTime;
        this.transform.position = Vector3.Lerp(this.transform.position, positionTemp, 0.5f);

    }
}
