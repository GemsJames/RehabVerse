using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    GameManager gameManager;
    public GameObject textPrefab;
    public Material particleMaterial;
    public Transform spawnPosition;

    public float excThresh, greatThresh;

    public Color excellentColor;
    public Color greatColor;
    public Color goodColor;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    public void ShowScore(float value)
    {
        GameObject tempText = Instantiate(textPrefab, spawnPosition.transform.position, Quaternion.identity);
        TextMesh txtMesh = tempText.GetComponent<TextMesh>();

        if (value < excThresh)
        {
            txtMesh.text = "Espetacular";
            txtMesh.color = excellentColor;
            particleMaterial.color = excellentColor;
        }
        else if(value < greatThresh)
        {
            txtMesh.text = "Íncrivel";
            txtMesh.color = greatColor;
            particleMaterial.color = greatColor;
        }
        else
        {
            txtMesh.text = "Boa";
            txtMesh.color = goodColor;
            particleMaterial.color = goodColor;
        }
    }
}
