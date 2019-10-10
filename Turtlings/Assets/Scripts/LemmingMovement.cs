using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LemmingMovement : Photon.MonoBehaviour
{
    //variables
    private int movement; //movement direction: -1 = left, 0 = stopped, 1 = right
    private int lastMovementBeforeStop;
    private int currentLevel;
    public float stairX; //where the stair is built x-coord
    public float stairY; //where the stair is built y-coord
    private float defaultGravity;
    private float soundFxVolume;
    public float stairsTime;
    public float diggingTime;
    public float movementSpeed;
    public float slopeMovementSpeed;
    public float exitRotationSpeed;
    private bool alreadyClickedOnLemming; // make sure you only click on lemming 1 time for every mouse hover
    private bool buildingStairs;
    private bool digging;
    private bool touchedExit;
    private bool doOnce;
    private bool doOnce2;
    private bool notOnTheFirstLoopIteration;
    private bool multiplayer;
    public bool isStopped;
    public bool grounded; //lemming is touching the ground for objects with mesh renderer
    public bool groundedTooMuch;
    public bool groundedFront;
    public bool groundedBack;
    public bool wallHit;
    public bool slopeHit;
    public bool headHit;
    public bool digDepthHit;
    public bool digDepthHitDeep;
    public bool groundedFrontDig;
    public bool groundedBackDig;

    //game objects
    public GameObject stair;
    public GameObject testRayGround; //raycast check for ground
    public GameObject testRayGroundTooDeep;
    public GameObject testRayGroundFront;
    public GameObject testRayGroundBack;
    public GameObject testRayWall; //raycast check for a wall
    public GameObject testRaySlope; //raycast check for a slope
    public GameObject testRayHead;
    public GameObject testRayDigDepth;
    public GameObject testRayDigDepthDeep;
    public GameObject testRayGroundFrontDig;
    public GameObject testRayGroundBackDig;
    private GameObject exit;
    private GameObject gameController;

    //other
    public Animator animator;
    private AudioSource audioSource;
    public AudioClip build;
    public AudioClip dig;

    //ui
    private Button buttonStop;
    private Button buttonStairs;
    private Button buttonDig;
    private Text textLemmingsFunction;

    WaitForSeconds waitForSeconds = new WaitForSeconds(1f); //coroutine iteration timer
    WaitForSeconds waitForDigging = new WaitForSeconds(0.5f); //coroutine iteration timer for digging
    WaitForSeconds waitForStopping = new WaitForSeconds(0.1f); //coroutine iteration timer for digging

    // Use this for initialization
    void Start()
    {
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        if (Preload.noSoundFx)
            soundFxVolume = 0f;
        else
            soundFxVolume = 1f;

        defaultGravity = 0.5f;
        currentLevel = 1;
        isStopped = false;
        alreadyClickedOnLemming = false;
        buildingStairs = false;
        touchedExit = false;
        doOnce = false;
        doOnce2 = false;
        notOnTheFirstLoopIteration = false;
        if (GameObject.FindGameObjectWithTag("Multiplayer") != null)
            multiplayer = true;

        audioSource = FindObjectOfType<AudioSource>();
        gameController = GameObject.Find("GameController");
        exit = GameObject.Find("Ending");
        buttonStop = GameObject.Find("ButtonStop").GetComponent<Button>();
        buttonStairs = GameObject.Find("ButtonStairs").GetComponent<Button>();
        buttonDig = GameObject.Find("ButtonDig").GetComponent<Button>();
        textLemmingsFunction = GameObject.Find("TextLemmingsFunction").GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!isStopped && !buildingStairs && !digging && !touchedExit) //lemming is not a stopper and it's not building or digging and hasn't touched the exit
        {
            Movement();
        }
        if (digging || isStopped) //lemming is digging/stopped
        {
            DiggingDrop();
        }
        DroppedToDeath();
    }

    private void Movement()
    {
        //raycast different parts of the lemming's body
        slopeHit = gameObject.GetComponent<DrawRays>().DrawRay(testRaySlope.transform.position);
        grounded = gameObject.GetComponent<DrawRays>().DrawRay(testRayGround.transform.position);
        groundedFront = gameObject.GetComponent<DrawRays>().DrawRay(testRayGroundFront.transform.position);
        groundedBack = gameObject.GetComponent<DrawRays>().DrawRay(testRayGroundBack.transform.position);
        groundedTooMuch = gameObject.GetComponent<DrawRays>().DrawRay(testRayGroundTooDeep.transform.position);
        wallHit = gameObject.GetComponent<DrawRays>().DrawRay(testRayWall.transform.position);
        digDepthHit = gameObject.GetComponent<DrawRays>().DrawRay(testRayDigDepth.transform.position);

        if (wallHit) //lemming hit a wall
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        else //didn't hit a wall
        {
            if(slopeHit)
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                gameObject.transform.Translate(transform.localScale.x * slopeMovementSpeed * Time.deltaTime*3, slopeMovementSpeed * Time.deltaTime*0.5f, 0);
                while (groundedTooMuch) //move the object out of the ground
                {
                    gameObject.transform.Translate(0, 0.01f, 0);
                    groundedTooMuch = gameObject.GetComponent<DrawRays>().DrawRay(testRayGroundTooDeep.transform.position);
                }
            }
            else if (groundedFront || groundedBack) //lemming is touching the ground
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                while (groundedTooMuch) //move the object out of the ground
                {
                    gameObject.transform.Translate(0, 0.01f, 0);
                    groundedTooMuch = gameObject.GetComponent<DrawRays>().DrawRay(testRayGroundTooDeep.transform.position);
                }
                gameObject.transform.Translate(transform.localScale.x * movementSpeed * Time.deltaTime, 0, 0);
            }
            else if (grounded) //lemming is touching the ground
            {
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                while(groundedTooMuch) //move the object out of the ground
                {
                    gameObject.transform.Translate(0, 0.01f, 0);
                    groundedTooMuch = gameObject.GetComponent<DrawRays>().DrawRay(testRayGroundTooDeep.transform.position);
                }
                gameObject.transform.Translate(transform.localScale.x * movementSpeed * Time.deltaTime, 0, 0);
            }
            else
            {
                gameObject.GetComponent<Rigidbody2D>().gravityScale = defaultGravity;
            }
        }
    }

    /// <summary>
    /// What to do when lemming is clicked on.
    /// </summary>
    private void OnMouseOver()
    {
        if (Input.GetButtonDown("Fire1") && !alreadyClickedOnLemming)
        {
            gameController.GetComponent<GameController>().SetFunctionTextPanelOff();
            textLemmingsFunction.text = "";
            switch (Preload.lemmingFunction)
            {
                case 1:
                    if(multiplayer)
                        photonView.RPC("SendLemmingCommandOverNetwork", PhotonTargets.Others, gameObject.tag, 1);
                    alreadyClickedOnLemming = true;
                    isStopped = true;
                    animator.SetBool("Digging", false);
                    animator.SetBool("Building", false);
                    gameController.GetComponent<GameController>().SetFunctionTextPanelOff();
                    textLemmingsFunction.text = "";
                    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                    animator.SetBool("Stopping", true);
                    buttonStop.GetComponent<Image>().color = Color.white;
                    Preload.lemmingFunction = 0;
                    break;
                case 2:
                    if (multiplayer)
                        photonView.RPC("SendLemmingCommandOverNetwork", PhotonTargets.Others, gameObject.tag, 2);
                    if(isStopped)
                    {
                        animator.SetBool("Stopping", false);
                        isStopped = false;
                    }
                    animator.SetBool("Digging", false);
                    alreadyClickedOnLemming = true;
                    gameController.GetComponent<GameController>().SetFunctionTextPanelOff();
                    textLemmingsFunction.text = "";
                    buttonStairs.GetComponent<Image>().color = Color.white;
                    Preload.lemmingFunction = 0;
                    stairsTime = Time.time;
                    animator.SetBool("Building", true);
                    StartCoroutine("BuildStairs");
                    break;
                case 3:
                    if (multiplayer)
                        photonView.RPC("SendLemmingCommandOverNetwork", PhotonTargets.Others, gameObject.tag, 3);
                    alreadyClickedOnLemming = true;
                    if (isStopped)
                    {
                        animator.SetBool("Stopping", false);
                        isStopped = false;
                    }
                    animator.SetBool("Building", false);
                    gameController.GetComponent<GameController>().SetFunctionTextPanelOff();
                    textLemmingsFunction.text = "";
                    buttonDig.GetComponent<Image>().color = Color.white;
                    Preload.lemmingFunction = 0;
                    diggingTime = Time.time;
                    Debug.Log("digging");
                    animator.SetBool("Digging", true);
                    StartCoroutine("Dig");
                    break;
                case 0:
                    gameController.GetComponent<GameController>().SetFunctionTextPanelOff();
                    textLemmingsFunction.text = "";
                    buttonDig.GetComponent<Image>().color = Color.white;
                    buttonStairs.GetComponent<Image>().color = Color.white;
                    buttonStop.GetComponent<Image>().color = Color.white;
                    break;
            }
        }
    }

    /// <summary>
    /// We do not have a mouse over the lemming anymore so that lemming can be clicked again.
    /// </summary>
    private void OnMouseExit()
    {
        alreadyClickedOnLemming = false;
    }

    /// <summary>
    /// Lower the lemming back to ground while digging after the lemming has dug a part of the ground.
    /// </summary>
    private void DiggingDrop()
    {
        grounded = gameObject.GetComponent<DrawRays>().DrawRay(testRayGround.transform.position);
        if (grounded) //lemming is touching the ground
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().gravityScale = defaultGravity;
        }
    }

    /// <summary>
    /// Check if the lemming dropped to its death.
    /// </summary>
    private void DroppedToDeath()
    {
        if(gameObject.transform.position.y < -20 && !doOnce)
        {
            gameController.GetComponent<GameController>().SetAmountOfLemmingsLeft(); //1 less lemming left
            doOnce = true;
            Destroy(gameObject, 1);
        }
    }

    /// <summary>
    /// Stair building. The size of the stairs is different depending on the scene.
    /// </summary>
    /// <returns></returns>
    IEnumerator BuildStairs()
    {
        while (true)
        {
            digging = false;
            StopCoroutine("Dig");
            buildingStairs = true;
            wallHit = gameObject.GetComponent<DrawRays>().DrawRay(testRayWall.transform.position);
            headHit = gameObject.GetComponent<DrawRays>().DrawRay(testRayHead.transform.position);

            if (Time.time - stairsTime > 12f)
            {
                Debug.Log("stopping building stairs");
                buildingStairs = false;
                grounded = true;
                animator.SetBool("Building", false);
                StopCoroutine("BuildStairs");
            }
            else if (wallHit || headHit)
            {
                Debug.Log("stopping building stairs because hit a wall/ceiling");
                buildingStairs = false;
                grounded = true;
                animator.SetBool("Building", false);
                StopCoroutine("BuildStairs");
            }

            if (Preload.currentLevel == "level4" || Preload.currentLevel == "level4multiplayer")
            {
                currentLevel = 4;
            }

            //check if we are going left or right
            if (transform.localScale.x < 0)
            {
                gameObject.GetComponent<DrawRays>().Building(testRayGroundBack.transform.position, 5, "left", currentLevel);
                if(currentLevel == 4)
                {
                    gameObject.transform.Translate(-0.06f, 0.04f, 0);
                }
                else
                {
                    gameObject.transform.Translate(-0.08f, 0.05f, 0);
                }
            }
            else
            {
                gameObject.GetComponent<DrawRays>().Building(testRayGroundBack.transform.position, 5, "right", currentLevel);
                if (currentLevel == 4)
                {
                    gameObject.transform.Translate(0.06f, 0.04f, 0);
                }
                else
                {
                    gameObject.transform.Translate(0.08f, 0.05f, 0);
                }
            }
            audioSource.PlayOneShot(build, soundFxVolume);
            yield return waitForSeconds;
        }
    }

    public IEnumerator Dig()
    {
        while (true)
        {
            buildingStairs = false;
            StopCoroutine("BuildStairs");
            bool digged = false;
            int diggingSize = 5;
            digging = true;
            if (Preload.currentLevel == "level4")
            {
                diggingSize = 10;
            }
            if (Time.time - diggingTime > 5f)
            {
                Debug.Log("stopping to dig because of time");
                digging = false;
                animator.SetBool("Digging", false);
                StopCoroutine("Dig");
            }
            if(notOnTheFirstLoopIteration)
            {
                grounded = gameObject.GetComponent<DrawRays>().DrawRay(testRayGround.transform.position);
                groundedTooMuch = gameObject.GetComponent<DrawRays>().DrawRay(testRayGroundTooDeep.transform.position);
            }
            digDepthHit = gameObject.GetComponent<DrawRays>().DrawRay(testRayDigDepth.transform.position);
            digDepthHitDeep = gameObject.GetComponent<DrawRays>().DrawRay(testRayDigDepthDeep.transform.position);
            groundedFrontDig = gameObject.GetComponent<DrawRays>().DrawRay(testRayGroundFrontDig.transform.position);
            groundedBackDig = gameObject.GetComponent<DrawRays>().DrawRay(testRayGroundBackDig.transform.position);
            if (digDepthHit || grounded || groundedTooMuch)
            {
                gameObject.GetComponent<DrawRays>().Digging(testRayDigDepth.transform.position, diggingSize);
                gameObject.GetComponent<DrawRays>().Digging(testRayGround.transform.position, diggingSize);
                gameObject.GetComponent<DrawRays>().Digging(testRayDigDepthDeep.transform.position, diggingSize);
                digged = true;
            }
            if (digDepthHitDeep && !digged)
            {
                gameObject.GetComponent<DrawRays>().Digging(testRayGroundFrontDig.transform.position, diggingSize);
                digged = true;
            }
            if (groundedFrontDig && !digged)
            {
                gameObject.GetComponent<DrawRays>().Digging(testRayGroundFrontDig.transform.position, diggingSize);
                digged = true;
            }
            if (groundedBackDig && !digged)
            {
                gameObject.GetComponent<DrawRays>().Digging(testRayGroundBackDig.transform.position, diggingSize);
                digged = true;
            }
            if(digged == false)
            {
                Debug.Log("stopping to dig");
                digging = false;
                animator.SetBool("Digging", false);
                StopCoroutine("Dig");
            }
            digged = false;
            notOnTheFirstLoopIteration = true;
            audioSource.PlayOneShot(dig, soundFxVolume);
            yield return waitForDigging;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("End") && !doOnce2)
        {
            gameController.GetComponent<GameController>().SetAmountOfLemmingsLeft(); //1 less lemming left
            gameController.GetComponent<GameController>().SetLemmingsExited();
            doOnce2 = true;
            StartCoroutine("LemmingGoesToExit");
        }
        if (collision.gameObject.CompareTag("Lemming") ||
            collision.gameObject.CompareTag("Lemming1") ||
            collision.gameObject.CompareTag("Lemming2") ||
            collision.gameObject.CompareTag("Lemming3") ||
            collision.gameObject.CompareTag("Lemming4") ||
            collision.gameObject.CompareTag("Lemming5") ||
            collision.gameObject.CompareTag("Lemming6") ||
            collision.gameObject.CompareTag("Lemming7") ||
            collision.gameObject.CompareTag("Lemming8") ||
            collision.gameObject.CompareTag("Lemming9") ||
            collision.gameObject.CompareTag("Lemming10") ||
            collision.gameObject.CompareTag("Lemming11") ||
            collision.gameObject.CompareTag("Lemming12"))
        {
            if (collision.gameObject.GetComponent<LemmingMovement>().GetIsStopped())
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    IEnumerator LemmingGoesToExit()
    {
        while (true)
        {
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            gameObject.transform.localScale *= 0.9975f;
            gameObject.transform.RotateAround(exit.transform.position, new Vector3(0, 0, -10f), exitRotationSpeed/2f * Time.deltaTime);
            gameObject.transform.Translate(new Vector3(+0.001f, +0.001f, 0));
            if (gameObject.transform.localScale.x < 0.0125f)
            {
                StopCoroutine("LemmingGoesToExit");
                Debug.Log("Lemming exited");
                Destroy(gameObject);
            }
            yield return null;
        }
    }

    /// <summary>
    /// Coroutine to make sure that if a lemming is stopped in the air it will fall in to ground and then stay on the ground.
    /// </summary>
    /// <returns></returns>
    IEnumerator LemmingStoppedInTheAir()
    {
        while(true)
        {
            grounded = gameObject.GetComponent<DrawRays>().DrawRay(testRayGround.transform.position);
            slopeHit = gameObject.GetComponent<DrawRays>().DrawRay(testRaySlope.transform.position);
            Debug.Log("IEnumerator LemmingStoppedInTheAir(); grounded " + grounded + " slopeHit " + slopeHit);
            if (grounded || slopeHit)
            {
                gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                StopCoroutine("LemmingStoppedInTheAir");
                //break;
            }
            else
            {
                gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.25f;
            }
            yield return waitForStopping;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!buildingStairs)
        {
            if (collision.CompareTag("Stair"))
            {
                gameObject.transform.Translate(transform.localScale.x * movementSpeed * Time.deltaTime, 0, 0);
            }
        }
    }

    public bool GetIsStopped()
    {
        return isStopped;
    }

    public void SetMovementDirection()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    [PunRPC]
    void SendLemmingCommandOverNetwork(string tag, int function)
    {
        GameObject lemm = GameObject.FindGameObjectWithTag(tag);

        switch (function)
        {
            case 1:
                lemm.GetComponent<LemmingMovement>().isStopped = true;
                lemm.GetComponent<LemmingMovement>().animator.SetBool("Digging", false);
                lemm.GetComponent<LemmingMovement>().animator.SetBool("Building", false);
                lemm.transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                lemm.GetComponent<LemmingMovement>().animator.SetBool("Stopping", true);
                break;
            case 2:
                if (lemm.GetComponent<LemmingMovement>().isStopped)
                {
                    lemm.GetComponent<LemmingMovement>().animator.SetBool("Stopping", false);
                    lemm.GetComponent<LemmingMovement>().isStopped = false;
                }
                lemm.GetComponent<LemmingMovement>().animator.SetBool("Digging", false);
                lemm.GetComponent<LemmingMovement>().stairsTime = Time.time;
                lemm.GetComponent<LemmingMovement>().animator.SetBool("Building", true);
                lemm.GetComponent<LemmingMovement>().StartCoroutine("BuildStairs");
                break;
            case 3:
                if (lemm.GetComponent<LemmingMovement>().isStopped)
                {
                    lemm.GetComponent<LemmingMovement>().animator.SetBool("Stopping", false);
                    lemm.GetComponent<LemmingMovement>().isStopped = false;
                }
                lemm.GetComponent<LemmingMovement>().animator.SetBool("Building", false);
                lemm.GetComponent<LemmingMovement>().diggingTime = Time.time;
                lemm.GetComponent<LemmingMovement>().animator.SetBool("Digging", true);
                lemm.GetComponent<LemmingMovement>().StartCoroutine("Dig");
                break;
            case 0:
                break;
        }
    }

}
