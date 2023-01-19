using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationTester : MonoBehaviour
{
    public float x, y, z, w;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = new Quaternion(x, y, z, w);
    }
}
