using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointController : MonoBehaviour
{
    PathController pathController;
    public TextMeshPro label;

    Vector3 initialScale;
    public float scaleInUI;

    // Start is called before the first frame update
    void Start()
    {
        pathController = FindObjectOfType<PathController>();

        initialScale = gameObject.transform.localScale;
        scaleInUI = 50;
    }

    public void SetActivePoint()
    {
        pathController.SetActivePoint(this);
    }

    public void RemoveActivePoint()
    {
        if (pathController.IsSamePoint(gameObject))
        {
            pathController.SetActivePoint(null);
        }
    }

    public void ChangeScale(float scale, float scalemodifier)
    {
        gameObject.transform.localScale = initialScale * ((scale * scalemodifier)) * 2;

        scaleInUI = scale;
    }
}
