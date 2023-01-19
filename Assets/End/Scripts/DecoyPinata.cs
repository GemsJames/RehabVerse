using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DecoyPinata : MonoBehaviour
{
    public Animator animator;

    public PinataController pinataController;

    MeshRenderer mesh; 
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.enabled = true;
        animator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAnimation()
    {
        mesh.enabled = true;
        animator.enabled = true;
        animator.SetTrigger("Move");
    }

    public void PlaySound()
    {
        audioSource.Play();
    }

    public void AnimationEnded()
    {
        mesh.enabled = false;

        pinataController.EnablePinata();
        Destroy(this);
    }
}
