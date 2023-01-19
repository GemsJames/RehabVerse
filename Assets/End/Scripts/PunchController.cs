using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PunchController : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float revDistance;

    public bool reving;

    [Range(0.0f, 1.0f)]
    float punchDistance;
    public float punchSpeed;
    Vector3 tinyArmPosition;
    Vector3 smallArmPosition;
    Vector3 bigArmPosition;
    Vector3 hugeArmPosition;

    bool punch;
    bool particuled;

    public float punchDelay;

    public Vector3 armInitial, armFinal;
    public GameObject hugeArm, bigArm, smallArm, tinyArm;

    public UnityEvent punchedEvent;
    public UnityEvent delayedPunchedEvent;

    public ParticleSystem hugeParticle, bigParticle, smallParticle, tinyParticle,sparklez;
    public AudioSource steamAudio;

    public float gearModifier;
    
    public GameObject gear1;
    public GameObject gear2;
    public GameObject gear3;
    public GameObject gear4;

    // Start is called before the first frame update
    void Start()
    {
       //Initiate();
    }

    public void Initiate()
    {
        reving = true;
        particuled = false;
        tinyArmPosition = armInitial;
        smallArmPosition = armInitial;
        bigArmPosition = armInitial;
        hugeArmPosition = armInitial;
    }

    // Update is called once per frame
    void Update()
    {
        if (reving)
        {
            if (revDistance < 0.25f)
            {
                tinyArmPosition = Vector3.Lerp(armInitial, armFinal, revDistance * 4);
                smallArmPosition = Vector3.Lerp(armInitial, armFinal, 0);
                bigArmPosition = Vector3.Lerp(armInitial, armFinal, 0);
                hugeArmPosition = Vector3.Lerp(armInitial, armFinal, 0);
            }
            else if (revDistance < 0.5f)
            {
                tinyArmPosition = Vector3.Lerp(armInitial, armFinal, 1);
                smallArmPosition = Vector3.Lerp(armInitial, armFinal, (revDistance - .25f) * 4);
                bigArmPosition = Vector3.Lerp(armInitial, armFinal, 0);
                hugeArmPosition = Vector3.Lerp(armInitial, armFinal, 0);
            }
            else if (revDistance < 0.75f)
            {
                tinyArmPosition = Vector3.Lerp(armInitial, armFinal, 1);
                smallArmPosition = Vector3.Lerp(armInitial, armFinal, 1);
                bigArmPosition = Vector3.Lerp(armInitial, armFinal, (revDistance - .5f) * 4);
                hugeArmPosition = Vector3.Lerp(armInitial, armFinal, 0);
            }
            else
            {
                tinyArmPosition = Vector3.Lerp(armInitial, armFinal, 1);
                smallArmPosition = Vector3.Lerp(armInitial, armFinal, 1);
                bigArmPosition = Vector3.Lerp(armInitial, armFinal, 1);
                hugeArmPosition = Vector3.Lerp(armInitial, armFinal, (revDistance - .75f) * 4);
            }

            gear1.transform.localEulerAngles = new Vector3(0, 0, revDistance * gearModifier);
            gear2.transform.localEulerAngles = new Vector3(0, 0, revDistance * gearModifier);
            gear3.transform.localEulerAngles = new Vector3(0, 0, revDistance * gearModifier);
            gear4.transform.localEulerAngles = new Vector3(0, 0, revDistance * gearModifier);

        }
        else if (punch)
        {
            //pode-se trocar isto por uma animação

            punchDistance += punchSpeed * Time.deltaTime;

            if (punchDistance < 0.25f)
            {
                if(particuled == false)
                {
                    particuled = true;
                    hugeParticle.Play();
                }

                hugeArmPosition = Vector3.Lerp(armFinal, armInitial, punchDistance * 4);
            }
            else if (punchDistance < 0.5f)
            {
                if (particuled == true)
                {
                    particuled = false;
                    bigParticle.Play();

                    hugeArmPosition = Vector3.Lerp(armFinal, armInitial, 1);
                }

                bigArmPosition = Vector3.Lerp(armFinal, armInitial, (punchDistance - .25f) * 4);
            }
            else if (punchDistance < 0.75f)
            {
                if (particuled == false)
                {
                    particuled = true;
                    smallParticle.Play();

                    bigArmPosition = Vector3.Lerp(armFinal, armInitial, 1);
                }

                smallArmPosition = Vector3.Lerp(armFinal, armInitial, (punchDistance - .5f) * 4);
            }
            else
            {
                if (particuled == true)
                {
                    particuled = false;
                    tinyParticle.Play();
                    steamAudio.Play();

                    smallArmPosition = Vector3.Lerp(armFinal, armInitial, 1);
                }

                tinyArmPosition = Vector3.Lerp(armFinal, armInitial, (punchDistance - .75f) * 4);
            }

            gear1.transform.localEulerAngles = new Vector3(0, 0, punchDistance * gearModifier);
            gear2.transform.localEulerAngles = new Vector3(0, 0, punchDistance * gearModifier);
            gear3.transform.localEulerAngles = new Vector3(0, 0, punchDistance * gearModifier);
            gear4.transform.localEulerAngles = new Vector3(0, 0, punchDistance * gearModifier);

            if (punchDistance >= 1)
            {
                tinyArmPosition = Vector3.Lerp(armFinal, armInitial, 1);

                sparklez.Stop();
                punch = false;
                punchDistance = 1;
                punchedEvent.Invoke();
                Invoke("DelayedCompletion", punchDelay);
            }


        }

        tinyArm.transform.localPosition = tinyArmPosition;
        smallArm.transform.localPosition = smallArmPosition;
        bigArm.transform.localPosition = bigArmPosition;
        hugeArm.transform.localPosition = hugeArmPosition;

    }

    public void Punch()
    {
        reving = false;
        punch = true;

        sparklez.Play();

        punchDistance = 0;
    }

    public void StopRev()
    {
        reving = false;
        sparklez.Stop();
    }

    public void Rev()
    {
        reving = true;
        sparklez.Play();
    }
    void DelayedCompletion()
    {
        delayedPunchedEvent.Invoke();
    }

    public void TinyParticles()
    {
        tinyParticle.Play();
        steamAudio.Play();
    }

    public void SmallParticles()
    {
        smallParticle.Play();
        steamAudio.Play();
    }

    public void BigParticles()
    {
        bigParticle.Play();
        steamAudio.Play();
    }

    public void HugeParticles()
    {
        hugeParticle.Play();
        steamAudio.Play();
    }

}
