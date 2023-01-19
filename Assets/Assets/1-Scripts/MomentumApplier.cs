using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumApplier : MonoBehaviour
{
    Rigidbody rb;

    Vector3 lastPos;
    Quaternion lastRot;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPos = Vector3.zero;
        lastRot = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        lastPos = transform.position;
        lastRot = transform.rotation;
    }

    public void SetMomentum()
    {
        //rb.velocity = (transform.position - lastPos) / Time.fixedDeltaTime;
        //rb.angularVelocity = (transform.eulerAngles - lastRot.eulerAngles) / Time.fixedDeltaTime;
    }

}
