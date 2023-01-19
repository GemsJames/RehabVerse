using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{

    public Transform player;
    public Text scoreText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!FindObjectOfType<GameManagerV>().gameHasEnded) {
            float score = player.position.z / 2;
            scoreText.text = score.ToString("0");
        }
        
    }
}
