using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    public Material preto;

    public Color pretoColor;
    public Color transColor;

    public float fadeDuration;

    public Transform spawnPosition;
    public Transform pinataPosition;
    public Transform balloonPosition;

    public GameObject player;

    public GameManager gameManager;
    //public enum Locations
    //{
    //    spawn,
    //    pinata,
    //    balloon
    //}

    //[HideInInspector]
    //public Locations location;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.location = GameManager.Locations.spawn;

        StartCoroutine(FadeIntoTrans());
        player.transform.parent = spawnPosition;
        player.transform.localPosition = Vector3.zero;
        //TeleportToSpawn();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TeleportToSpawn()
    {
        gameManager.location = GameManager.Locations.spawn;
        StartCoroutine(FadeIntoBlack());
    }

    public void TeleportToPinata()
    {
        gameManager.location = GameManager.Locations.pinata;
        StartCoroutine(FadeIntoBlack());
    }

    public void TeleportToBalloon()
    {
        gameManager.location = GameManager.Locations.balloon;
        StartCoroutine(FadeIntoBlack());
    }


    void TeleportToLocation(Transform target)
    {
        player.transform.parent = target;
        player.transform.localPosition = Vector3.zero;
        StartCoroutine(FadeIntoTrans());
    }

    private IEnumerator FadeIntoBlack()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            preto.color = Color.Lerp(transColor, pretoColor, elapsedTime / fadeDuration);
            yield return null;
        }

        FadedToBlack();
    }

    void FadedToBlack()
    {
        Debug.Log("blacked");
        switch (gameManager.location)
        {
            case GameManager.Locations.spawn:
                TeleportToLocation(spawnPosition);
                break;
            case GameManager.Locations.pinata:
                TeleportToLocation(pinataPosition);
                break;
            case GameManager.Locations.balloon:
                TeleportToLocation(balloonPosition);
                break;
            default:
                break;
        }
    }

    private IEnumerator FadeIntoTrans()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            preto.color = Color.Lerp(pretoColor, transColor, elapsedTime / fadeDuration);
            yield return null;
        }
        FadedToTrans();
    }

    void FadedToTrans()
    {
        Debug.Log("transed");
    }
}
