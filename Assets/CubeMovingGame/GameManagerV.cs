using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerV : MonoBehaviour
{
    public GameObject player;
    public GameObject spawnPoint;

    public Text scoreText;

    public bool gameHasEnded = false;

    public float restartDelay = 2f; 

    public void endGame() {
        if(!gameHasEnded) {
            gameHasEnded = true;
            scoreText.text = "GAME OVER!";
            Invoke("restartGame", restartDelay);
        }
    }


    void restartGame() {
        //scoreText.text = "0";
        //player.transform.position = spawnPoint.transform.position;
        //player.transform.rotation = spawnPoint.transform.rotation;
        //FindObjectOfType<PlayerMovement>().startMoving();
        //Debug.Log("reset pos");
        //gameHasEnded = false;
        FindObjectOfType<PlayerMovement>().closeConnection();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
