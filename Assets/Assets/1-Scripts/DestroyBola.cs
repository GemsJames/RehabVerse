using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBola : MonoBehaviour
{
    public ParticleSystem ps;
    public AudioSource ass;
    public AudioClip ac;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("Collided");
            ps.Play();
            ass.clip = ac;
            ass.Play();
        }
    }
}
