using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSpawner : MonoBehaviour
{
    GameManager gameManager;
    public GameObject textPrefab;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();


    }

    public void SpawnText()
    {
        GameObject tempText = Instantiate(textPrefab, transform.position, Quaternion.identity);
    }

}
