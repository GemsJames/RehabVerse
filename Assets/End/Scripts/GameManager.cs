using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GameManager : MonoBehaviour
{
    public RehabUI rehabUI;

    public PunchController punchController;
    public PinataController pinataController;

    public BalloonGame balloonController;

    public TextMeshProUGUI panelText;

    public float debugCompletionSpeed;

    [Range(0.0f, 1.0f)]
    public float completionPercentage = 0;

    public int timesToRepeat;
    public int repetitionNumber;

    public float delay;

    public UnityEvent firstQuarterEvent;

    public UnityEvent secondQuarterEvent;

    public UnityEvent thirdQuarterEvent;

    public UnityEvent completionEvent;
    public UnityEvent delayedCompletion;

    public UnityEvent gameStarted;
    public UnityEvent gameEnded;

    public UnityEvent<float> desvioEvent;

    bool first, second, third, complete;
    public bool playing;

    //unused
    //float velocity = 0; 

    //Desvio
    float oldDesvio = 0;
    float addDesvio = 0;
    int addCounter = 0;
    float mediaDesvio = 0;

    [HideInInspector]
    public int pinatasDestruidas;

    public enum Locations
    {
        spawn,
        pinata,
        balloon
    }

    [HideInInspector]
    public Locations location;

    // Start is called before the first frame update
    void Start()
    {
        StartPinataGame();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playing)
        {         
            //if(completionPercentage > 0.99f)
            //{
            //    completionPercentage = 1;
            //}
            //else
            //    completionPercentage = Mathf.SmoothStep(completionPercentage, rehabUI.displayPercent / 100.0f, rehabUI.elapsedTime);

            completionPercentage = rehabUI.displayPercent / 100.0f;

            completionPercentage += (debugCompletionSpeed * Time.deltaTime);

            //Lógica
            if (completionPercentage >= 0.25f)
            {
                if (first)
                {
                    CalculateDesvioMedio();

                    if(location == Locations.pinata)
                        firstQuarterEvent.Invoke();
                    first = false;
                }
                else if (completionPercentage >= 0.50f)
                {
                    if (second)
                    {
                        CalculateDesvioMedio();

                        if (location == Locations.pinata)
                            secondQuarterEvent.Invoke();
                        second = false;
                    }
                    else if (completionPercentage >= 0.75f)
                    {
                        if (third)
                        {
                            CalculateDesvioMedio();

                            if (location == Locations.pinata)
                                thirdQuarterEvent.Invoke();
                            third = false;
                        }
                        else if (completionPercentage >= 1f)
                        {
                            //ainda nao acabou as repetiçoes?
                            if (!complete)
                            {
                                CalculateDesvioMedio();

                                completionPercentage = 1f;
                                repetitionNumber++;

                                
                                if (location == Locations.pinata)
                                {
                                    Invoke("DelayedCompletion", delay);
                                    completionEvent.Invoke();
                                }                                
                                complete = true;
                            }
                        }
                    }
                }
            }

            if (location == Locations.pinata)
                punchController.revDistance = completionPercentage;
            else if (location == Locations.balloon)
                balloonController.pumpDistance = completionPercentage;

            if (rehabUI.desvioDoCaminho > -1)
            {
                if(rehabUI.desvioDoCaminho != oldDesvio)
                {
                    oldDesvio = rehabUI.desvioDoCaminho;

                    addCounter++;
                    addDesvio += rehabUI.desvioDoCaminho;
                }
            }

            if(repetitionNumber >= timesToRepeat)
            {
                playing = false;

                gameEnded.Invoke();
            }
        }
        else
        {
             
        }

        if(location == Locations.pinata)
        {
            panelText.text = "Repetição: " + repetitionNumber + "/" + timesToRepeat +
                "Pinhatas destruidas: " + pinatasDestruidas;
        }
        else if(location == Locations.balloon)
        {
            panelText.text = "Repetição: " + repetitionNumber + "/" + timesToRepeat +
                "Balões estourados: " + pinatasDestruidas;
        }

    }

    void CalculateDesvioMedio()
    {
        mediaDesvio = addDesvio / addCounter;

        addDesvio = 0;
        addCounter = 0;

        desvioEvent.Invoke(mediaDesvio);
    }

    public void StartGame()
    {
        if (location == Locations.pinata)
        {
            StartPinataGame();
        }
        else if (location == Locations.balloon)
        {
            StartBalloon();
        }
    }

    public void StartPinataGame()
    {
        timesToRepeat = rehabUI.totalReps;

        pinataController.SpawnPinatas();

        first = second = third = true;
        complete = false;

        repetitionNumber = 0;

        punchController.Initiate();

        gameStarted.Invoke();
        playing = true;

        pinatasDestruidas = 0;
    }

    public void StartBalloon()
    {
        timesToRepeat = rehabUI.totalReps;

        //Ballon Leftover visuals
        //pinataController.SpawnPinatas();

        first = second = third = true;
        complete = false;

        repetitionNumber = 0;

        balloonController.Initiate();

        gameStarted.Invoke();
        playing = true;

        pinatasDestruidas = 0;
    }

    void DelayedCompletion()
    {
        delayedCompletion.Invoke();
    }

    public void ResetCompletion()
    {
        completionPercentage = 0;
        complete = false;
        first = second = third = true;
    }
}
