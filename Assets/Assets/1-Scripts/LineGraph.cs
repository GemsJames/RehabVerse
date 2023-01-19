using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LineGraph : MonoBehaviour
{
    LineRenderer line;

    public int numberOfPoints;
    float[] values; 

    float lineLength;
    float pointLength;

    public float heightOffset = 1;
    float lineHeight;

    float biggestValue;

    public TextMeshProUGUI currentValueText;
    public TextMeshProUGUI highValueText;
    public TextMeshProUGUI mediumValueText;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();

        lineLength = line.GetPosition(1).x;
        lineHeight = line.GetPosition(1).y;

        line.positionCount = numberOfPoints;

        pointLength = lineLength / numberOfPoints;
        Vector3[] lineValues = new Vector3[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            lineValues[i] = new Vector3(i * pointLength, 0, 0);
        }

        line.SetPositions(lineValues);

        biggestValue = 0;

        values = new float[numberOfPoints];
        ChangeValueText(0);
        ChangeLegenda(1);
    }

    public void NewValue(float valor)
    {
        if (valor > biggestValue)
        {
            biggestValue = valor;

            ChangeLegenda(biggestValue);
        }

        if(values[0] == biggestValue)
        {
            biggestValue = 0;
        }

        for (int i = 0; i < numberOfPoints - 1; i++)
        {
            values[i] = values[i + 1];

            if (values[i] > biggestValue)
                biggestValue = values[i];
        }

        values[numberOfPoints - 1] = valor;

        for (int i = 0; i < numberOfPoints; i++)
        {
            line.SetPosition(i, new Vector3(
                line.GetPosition(i).x,
                values[i] * (lineHeight / biggestValue),
                line.GetPosition(i).z));
        }

        ChangeValueText(valor);
    }


    void ChangeValueText(float valor)
    {
        currentValueText.text = "Distance: " + valor;
    }

    void ChangeLegenda(float value)
    {
        highValueText.text = value.ToString("0.000");
        mediumValueText.text = (value/2).ToString("0.000");
    }
}