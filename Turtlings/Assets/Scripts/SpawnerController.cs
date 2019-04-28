using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{

    public GameObject lemming;
    public int amountOfLemmings;
    private int lemmingsIndicator;
    private bool spawnSpeed;

    WaitForSeconds waitForSeconds = new WaitForSeconds(4f);
    WaitForSeconds waitForSecondsQuick = new WaitForSeconds(1f);

    // Use this for initialization
    void Start()
    {
        lemmingsIndicator = 1;
        StartCoroutine("SpawnLemmings");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnLemmings()
    {
        while (amountOfLemmings > 0)
        {
            GameObject lemmingsInstance = Instantiate(lemming, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.2f, gameObject.transform.position.z), Quaternion.identity);
            lemmingsInstance.tag = "Lemming" + lemmingsIndicator;
            lemmingsIndicator++;
            amountOfLemmings -= 1;
            if(spawnSpeed)
            {
                yield return waitForSecondsQuick;
            }
            else
            {
                yield return waitForSeconds;
            }
        }
    }

    public void SetSpawnSpeedOn()
    {
        spawnSpeed = true;
    }

    public void SetSpawnSpeedOff()
    {
        spawnSpeed = false;
    }
}
