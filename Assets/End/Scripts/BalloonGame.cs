using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonGame : MonoBehaviour
{
    GameManager gameManager;

    [Range(0.0f, 1.0f)]
    public float pumpDistance;
    public bool pumping;

    [Range(0.0f, 1.0f)]
    float resetDistance;
    public float resetSpeed;
    bool resetting;

    public Vector3 pumpInitial, pumpFinal;
    public GameObject pump;
    public GameObject balloon;
    public Material balloonMaterial;

    public float balloonScaleModifier;

    bool particuled;
    public ParticleSystem popParticles;

    int pumpNumber;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void Initiate()
    {
        pumping = true;
        particuled = false;
        pump.transform.position = pumpInitial;
        resetDistance = 0;
        pumpNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (pumping)
        {
            pump.transform.localPosition = Vector3.Lerp(pumpInitial, pumpFinal, pumpDistance);

            Debug.Log("pump" + pumpNumber);

            float scale = (pumpNumber * balloonScaleModifier) + pumpDistance * balloonScaleModifier;

            balloon.transform.localScale = new Vector3(scale, scale, scale);

            if(pumpDistance >= 1f)
            {
                resetting = true;
                pumping = false;

                pumpNumber++;
                if (pumpNumber >= 4)
                {
                    pumpNumber = 0;
                    PopBallon();
                }
            }
        }
        else if (resetting)
        {
            resetDistance += resetSpeed * Time.deltaTime;

            pump.transform.localPosition = Vector3.Lerp(pumpFinal, pumpInitial, resetDistance);

            //float scale = (pumpNumber + 1) * balloonScaleModifier;

            //balloon.transform.localScale = new Vector3(scale, scale, scale);

            if (resetDistance >= 1)
            {
                pump.transform.localPosition = Vector3.Lerp(pumpFinal, pumpInitial, 1);

                resetting = false;
                resetDistance = 1;

                //pumpNumber++;
                //if(pumpNumber >= 4)
                //{
                //    pumpNumber = 0;
                //    PopBallon();
                //}

                //punchedEvent.Invoke();
                //Invoke("DelayedCompletion", punchDelay);
            }
        }
    }

    void PopBallon()
    {
        balloon.transform.localScale = Vector3.zero;

        popParticles.Play();

        balloonMaterial.color = Random.ColorHSV(0, 1);
    }
}
