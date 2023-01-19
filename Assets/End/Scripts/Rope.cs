using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public Transform hinge;
    public Transform pinata;

    LineRenderer lineRenderer;

    Vector3[] positions;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        positions = new Vector3[2];
    }

    // Update is called once per frame
    void Update()
    {
        positions[0] = hinge.position;
        positions[1] = pinata.position;

        lineRenderer.SetPositions(positions);
    }
}
