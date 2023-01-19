using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCreator : MonoBehaviour
{
    Vector3 point1;
    Vector3 point2;
    Vector3 point3;

    bool drawing;
    int currentPoint;

    public Transform indicator;
    public float defaultTableLength;

    public GameObject tablePrefab;
    public GameObject debugSphere;
    GameObject currentTable;

    // Start is called before the first frame update
    void Start()
    {
        drawing = false;
        currentPoint = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (drawing)
        {
            point2 = indicator.position;

            CalculateRectangle();

        }
    }

    public void SetPoint()
    {
        Debug.Log("setpoint");

        switch (currentPoint)
        {
            case 1:
                SetPoint1();
                break;

            case 2:
                SetPoint2();
                break;

            case 3:
                SetPoint3();
                break;

            default:
                break;
        }
    }

    void SetPoint1()
    {
        currentTable = Instantiate(tablePrefab);
        currentPoint = 2;

        point1 = indicator.position;
        drawing = true;

        Instantiate(debugSphere, indicator.position, Quaternion.identity);
    }

    void SetPoint2()
    {
        drawing = false;
        point2 = indicator.position;
        currentPoint = 3;

        Instantiate(debugSphere, indicator.position, Quaternion.identity);
    }

    void SetPoint3()
    {
        currentPoint = 1;
    }

    void CalculateRectangle()
    {
        currentTable.transform.position = new Vector3(
            (point1.x + point2.x) / 2,
            point1.y,
            (point1.z + point2.z) / 2);
    }
}
