using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PathUIController : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    PathController pathController;

    public Slider scaleSlider;
    float scalemodifier;

    public Text xCoordinateText, yCoordinateText, zCoordinateText;
    public Text xCoordinateUI, yCoordinateUI, zCoordinateUI;

    // Start is called before the first frame update
    void Start()
    {
        nameText.text = "Point Name: ";

        xCoordinateUI.text = "...";
        yCoordinateUI.text = "...";
        zCoordinateUI.text = "...";

        xCoordinateText.text = "X: ";
        yCoordinateText.text = "Y: ";
        zCoordinateText.text = "Z: ";

        pathController = FindObjectOfType<PathController>();

        scalemodifier = 1 / scaleSlider.maxValue;
    }

    public void UpdateUI(PointController pointController)
    {
        nameText.text = "Point Name: " + pointController.gameObject.name;

        scaleSlider.value = pointController.scaleInUI;
    }

    public void ChangePointName(string str)
    {
        pathController.selectedPoint.name = str;
        pathController.selectedPoint.label.text = str;
        nameText.text = "Point Name: " + str;
    }

    public void ChandeCoordinates(string x, string y, string z)
    {
        xCoordinateUI.text = x;
        yCoordinateUI.text = y;
        zCoordinateUI.text = z;

        xCoordinateText.text = "X: " + x;
        yCoordinateText.text = "Y: " + y;
        zCoordinateText.text = "Z: " + z;
    }

    public void ScalePoint(Single single)
    {
        pathController.selectedPoint.ChangeScale((float)single, scalemodifier);
    }
}
