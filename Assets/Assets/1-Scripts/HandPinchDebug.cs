using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HandPinchDebug : MonoBehaviour
{
    OVRHand[] hands;
    bool handsExist;
    int frame;

    public OVRHand rightHand;
    public OVRHand leftHand;

    public TextMeshProUGUI textMeshRight;
    public TextMeshProUGUI textMeshLeft;

    // Start is called before the first frame update
    void Start()
    {
        handsExist = false;
        frame = 0;
    }

    // Update is called once per frame
    void Update()
    {
        frame++;
        if(frame > 60)
        {
            frame = 0;

            hands = FindObjectsOfType<OVRHand>();

            if(hands.Length != 0)
            {
                Debug.Log("Hand 0: " + hands[0].name);
                Debug.Log("Hand 1: " + hands[1].name);

                handsExist = true;
                rightHand = hands[0];
                leftHand = hands[1];
            }
            else
            {
                handsExist = false;
            }
        }

        if (handsExist)
        {
            textMeshRight.text =
            "Thumb Pitch: " + rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb).ToString("0.00")
            + "\nIndex Pitch: " + rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Index).ToString("0.00")
            + "\nMiddle Pitch:" + rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle).ToString("0.00")
            + "\nRing Pitch: " + rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring).ToString("0.00")
            + "\nPinky Pitch: " + rightHand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky).ToString("0.00");

            textMeshLeft.text =
            "Thumb Pitch: " + leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb).ToString("0.00")
            + "\nIndex Pitch: " + leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Index).ToString("0.00")
            + "\nMiddle Pitch:" + leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle).ToString("0.00")
            + "\nRing Pitch: " + leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring).ToString("0.00")
            + "\nPinky Pitch: " + leftHand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky).ToString("0.00");
        }

    }
}
