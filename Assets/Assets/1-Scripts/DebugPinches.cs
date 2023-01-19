using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugPinches : MonoBehaviour
{
    HandPinchDebug hpd;

    //0- Thumb, 1- Index, 2- Middle, 3- Ring, 4- Pinky
    List<float> values;
    float average;
    float lowest;
    float highest;
    string valuesString;

    public TextMeshProUGUI textAverage;

    // Start is called before the first frame update
    void Start()
    {
        hpd = GetComponent<HandPinchDebug>();

        values = new List<float>();
        average = lowest = highest = 0;

        UpdateAverage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SavePinch(int finger)
    {
        switch (finger)
        {
            case 1:
                values.Add(hpd.rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index));

                break;
            case 2:
                values.Add(hpd.rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle));
                break;
            case 3:
                values.Add(hpd.rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring));
                break;
            case 4:
                values.Add(hpd.rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky));
                break;
            case 5:
                values.Add(hpd.leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Index));
                break;
            case 6:
                values.Add(hpd.leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle));
                break;
            case 7:
                values.Add(hpd.leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring));
                break;
            case 8:
                values.Add(hpd.leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky));
                break;
        }

        UpdateAverage();
    }

    void AddValue(float f)
    {
        values.Add(f);

        if(values.Count == 1)
        {
            highest = f;
            lowest = f;
        }
        else
        {
            if (f > highest)
                highest = f;
            if (f < lowest)
                lowest = f;
        }

    }

    public void ClearValues()
    {
        values.Clear();
        UpdateAverage();
    }

    void UpdateAverage()
    {
        if(values.Count == 0)
        {
            average = lowest = highest = 0;

            valuesString = "Values: Empty";
        }
        else
        {
            average = values.Average();

            valuesString = "Values: ";

            for(int i = 0; i < values.Count-1; i++)
            {
                valuesString += values[i].ToString("0.00") + ", ";
            }

            valuesString += values[values.Count-1].ToString("0.00");
        }

        textAverage.text = "Average: " + average + "\nNumber of Values: " + values.Count + "\nLowest Value: " + lowest.ToString("0.00") + "\nHighest Value: " + highest.ToString("0.00") + "\n" + valuesString;
    }
}
