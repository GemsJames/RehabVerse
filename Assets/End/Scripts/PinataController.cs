using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PinataController : MonoBehaviour
{
    public MeshRenderer mesh;
    public LineRenderer line;

    public Material undamaged;
    public Material stage1;
    public Material stage2;
    public Material stage3;

    public ParticleSystem destroyParticles;
    public ParticleSystem smallParticles;
    public ParticleSystem spawnParticles;

    public GameManager gameManager;

    //public DecoyPinata decoy;
    public float newPinataDelay;
    public GameObject pinataDecoyPrefab;
    public float spawnOffset;
    public GameObject worldParent;
    public GameObject spawnPosition;
    int currentPinata;
    int maxPinatas;
    List<DecoyPinata> pinataDecoys;

    public UnityEvent maxPinataReached;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetPunched()
    {
        switch (gameManager.repetitionNumber % 4)
        {
            case 1:
                Punch1();
                break;
            case 2:
                Punch2();
                break;
            case 3:
                Punch3();
                break;
            case 0:
                Punch4();
                break;
            default:
                break;
        }
    }

    public void SpawnPinatas()
    {
        Debug.Log("Spawn Pinatas");

        pinataDecoys = new List<DecoyPinata>();
        currentPinata = 0;
        maxPinatas = (gameManager.timesToRepeat / 4) - 1;

        for (int i = 0; i < maxPinatas; i++)
        {
            Debug.Log("Spawn Pinatas Cycle: " + i);

            Vector3 spawnposition = new Vector3(spawnPosition.transform.position.x, spawnPosition.transform.position.y + spawnOffset * i, spawnPosition.transform.position.z);
            Vector3 spawnrotation = new Vector3(spawnPosition.transform.rotation.x, Random.Range(0,359) , spawnPosition.transform.rotation.z);

            DecoyPinata newPinata = Instantiate(pinataDecoyPrefab, spawnposition, Quaternion.Euler(spawnrotation), worldParent.transform).GetComponent<DecoyPinata>();

            newPinata.pinataController = this;

            pinataDecoys.Add(newPinata);
        }
    }

    public void EnablePinata()
    {
        mesh.material = undamaged;
        mesh.enabled = true;
        line.enabled = true;
        spawnParticles.Play();
    }

    public void Punch1()
    {
        mesh.material = stage1;
        smallParticles.Play();
    }

    public void Punch2()
    {
        mesh.material = stage2;
        smallParticles.Play();
    }

    public void Punch3()
    {
        mesh.material = stage3;
        smallParticles.Play();
    }

    public void Punch4()
    {
        mesh.enabled = false;
        line.enabled = false;
        destroyParticles.Play();

        gameManager.pinatasDestruidas++;

        Invoke("StartDecoy", newPinataDelay);
    }

    void StartDecoy()
    {
        //decoy.StartAnimation();

        if (currentPinata >= maxPinatas)
        {
            //maxPinataReached.Invoke();
            Debug.Log("maxPinataReached");
        }
        else
        {
            pinataDecoys[currentPinata].StartAnimation();
            currentPinata++;
        }
    }
}
