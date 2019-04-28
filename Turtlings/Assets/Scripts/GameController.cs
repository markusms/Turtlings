using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : Photon.MonoBehaviour {

    public int lemmingsExited;
    public int amountOfLemmingsLeft;
    private int amountOfLemmings;
    private int lemmingsRequiredForWin;
    private float currentTime;
    private float progressTimer;
    private float sceneStartTime;
    private float soundFxVolume;
    private bool paused;
    private bool spedUp;
    private bool doOnce;
    private bool multiplayer;

    public GameObject endParticles;
    private GameObject ending;
    private GameObject exitButton;
    private GameObject restartButton;
    private GameObject nextLevelButton;
    private GameObject saveButton;
    private GameObject menuButton;
    private GameObject buttonStop;
    private GameObject buttonStairs;
    private GameObject buttonDig;
    private GameObject buttonPause;
    private GameObject buttonSpeedUp;
    private GameObject panelTextFunction;
    private GameObject panelTextLoss;
    private GameObject spawner;
    
    private Text textLemmingsFunction;
    private Text lossText;
    public Image progressBar;

    private AudioSource audioSource;
    public AudioClip lemmingExit;

    // Use this for initialization
    void Start()
    {
        InitializeVariables();
    }

    // Update is called once per frame
    void Update()
    {
        GameEnd();
        Timer();

        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && currentTime > 0)
        {
            Paused();
        }

        if (Input.GetKeyDown(KeyCode.O) && currentTime > 0)
        {
            SpeedUp();
        }

    }

    private void InitializeVariables()
    {
        Preload.currentLevel = SceneManager.GetActiveScene().name;
        if (Preload.noSoundFx)
            soundFxVolume = 0f;
        else
            soundFxVolume = 1f;

        paused = false;
        spedUp = false;
        doOnce = false;
        currentTime = 150f;
        progressTimer = 150f;
        sceneStartTime = Time.time;
        lemmingsExited = 0;
        lemmingsRequiredForWin = 9;
        amountOfLemmings = 12;
        amountOfLemmingsLeft = 12;
        if (GameObject.FindGameObjectWithTag("Multiplayer") != null)
            multiplayer = true;

        audioSource = FindObjectOfType<AudioSource>();
        ending = GameObject.FindGameObjectWithTag("End");
        spawner = GameObject.FindGameObjectWithTag("Spawner");
        exitButton = GameObject.Find("ButtonExit");
        restartButton = GameObject.Find("ButtonRestart");
        nextLevelButton = GameObject.Find("ButtonNextLevel");
        saveButton = GameObject.Find("ButtonSave");
        menuButton = GameObject.Find("ButtonMenu");
        buttonStop = GameObject.Find("ButtonStop");
        buttonStairs = GameObject.Find("ButtonStairs");
        buttonDig = GameObject.Find("ButtonDig");
        buttonPause = GameObject.Find("ButtonPause");
        buttonSpeedUp = GameObject.Find("ButtonSpeedUp");
        panelTextFunction = GameObject.Find("PanelTextFunction");
        panelTextLoss = GameObject.Find("PanelTextLoss");
        lossText = GameObject.Find("TextLoss").GetComponent<Text>();
        textLemmingsFunction = GameObject.Find("TextLemmingsFunction").GetComponent<Text>();

        exitButton.GetComponent<Button>().onClick.AddListener(() => ButtonClicked("exit"));
        restartButton.GetComponent<Button>().onClick.AddListener(() => ButtonClicked("restart"));
        nextLevelButton.GetComponent<Button>().onClick.AddListener(() => ButtonClicked("next"));
        buttonPause.GetComponent<Button>().onClick.AddListener(() => Paused());
        saveButton.GetComponent<Button>().onClick.AddListener(() => Saved());
        menuButton.GetComponent<Button>().onClick.AddListener(() => MainMenu());
        buttonSpeedUp.GetComponent<Button>().onClick.AddListener(() => SpeedUp());
        buttonStop.GetComponent<Button>().onClick.AddListener(() => LemmingsFunctionClicked(1));
        buttonStairs.GetComponent<Button>().onClick.AddListener(() => LemmingsFunctionClicked(2));
        buttonDig.GetComponent<Button>().onClick.AddListener(() => LemmingsFunctionClicked(3));

        //hide when not active
        exitButton.SetActive(false);
        restartButton.SetActive(false);
        nextLevelButton.SetActive(false);
        saveButton.SetActive(false);
        menuButton.SetActive(false);
        panelTextFunction.SetActive(false);
        panelTextLoss.SetActive(false);
        lossText.text = "";

        Time.timeScale = 1;
    }

    private void Timer()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            progressBar.fillAmount = currentTime / progressTimer;
        }
        //player won but didn't save all the lemmings
        else if (currentTime <= 0 && lemmingsExited >= lemmingsRequiredForWin)
        {
            GameEndTimeEnded();
        }
        else //time is out, show the hidden ui
        {
            GameLost();
        }
    }

    private void ButtonClicked(string command)
    {
        Time.timeScale = 1;
        if (command == "exit")
        {
            SceneManager.LoadScene("mainMenu");
        }
        else if(command == "next")
        {
            Preload.lemmingsSavedTotal += lemmingsExited;
            if (multiplayer)
                photonView.RPC("SendChangeLevelOverNetwork", PhotonTargets.Others, Preload.lemmingsSavedTotal);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }
        else //reload scene
        {
            if (multiplayer)
                photonView.RPC("SendReloadLevelOverNetwork", PhotonTargets.Others);
            SceneManager.LoadScene("reloadBackgroundTexture");
        }
    }

    private void LemmingsFunctionClicked(int i)
    {
        switch (i)
        {
            case 1:
                if (Preload.lemmingFunction == 1)
                {
                    SetFunctionTextPanelOff();
                    textLemmingsFunction.text = "";
                    buttonStop.GetComponent<Image>().color = Color.white;
                    Preload.lemmingFunction = 0;
                }
                else
                {
                    SetFunctionTextPanelOn();
                    textLemmingsFunction.text = "STOP";
                    buttonStop.GetComponent<Image>().color = Color.yellow;
                    buttonStairs.GetComponent<Image>().color = Color.white;
                    buttonDig.GetComponent<Image>().color = Color.white;
                    Preload.lemmingFunction = 1;
                }
                break;
            case 2:
                if (Preload.lemmingFunction == 2)
                {
                    SetFunctionTextPanelOff();
                    textLemmingsFunction.text = "";
                    buttonStairs.GetComponent<Image>().color = Color.white;
                    Preload.lemmingFunction = 0;
                }
                else
                {
                    SetFunctionTextPanelOn();
                    textLemmingsFunction.text = "STAIRS";
                    buttonStairs.GetComponent<Image>().color = Color.yellow;
                    buttonDig.GetComponent<Image>().color = Color.white;
                    buttonStop.GetComponent<Image>().color = Color.white;
                    Preload.lemmingFunction = 2;
                }
                break;
            case 3:
                if (Preload.lemmingFunction == 3)
                {
                    SetFunctionTextPanelOff();
                    textLemmingsFunction.text = "";
                    buttonDig.GetComponent<Image>().color = Color.white;
                    Preload.lemmingFunction = 0;
                }
                else
                {
                    SetFunctionTextPanelOn();
                    textLemmingsFunction.text = "DIG";
                    buttonDig.GetComponent<Image>().color = Color.yellow;
                    buttonStop.GetComponent<Image>().color = Color.white;
                    buttonStairs.GetComponent<Image>().color = Color.white;
                    Preload.lemmingFunction = 3;
                }
                break;
        }
    }

    private void Paused()
    {
        if (multiplayer)
            photonView.RPC("SendPauseOverNetwork", PhotonTargets.Others);
        if (!paused)
        {
            Time.timeScale = 0;
            paused = true;
            buttonPause.GetComponent<Image>().color = Color.red;
            panelTextLoss.SetActive(true);
            lossText.text = "Paused";
            saveButton.SetActive(true);
            menuButton.SetActive(true);
        }
        else
        {
            if(spedUp)
            {
                Time.timeScale = 2;
            }
            else
            {
                Time.timeScale = 1;
            }
            paused = false;
            buttonPause.GetComponent<Image>().color = Color.white;
            panelTextLoss.SetActive(false);
            lossText.text = "";
            saveButton.SetActive(false);
            menuButton.SetActive(false);
        }
    }

    private void Saved()
    {
        Preload.Save();
        panelTextLoss.SetActive(true);
        lossText.text = "Saved";
    }

    private void MainMenu()
    {
        SceneManager.LoadScene("mainMenu");
    }

    private void SpeedUp()
    {
        if (multiplayer)
            photonView.RPC("SendSpeedUpOverNetwork", PhotonTargets.Others);
        if (!spedUp)
        {
            Time.timeScale = 2;
            spedUp = true;
            spawner.GetComponent<SpawnerController>().SetSpawnSpeedOn();
            buttonSpeedUp.GetComponent<Image>().color = Color.red;
            panelTextLoss.SetActive(true);
            lossText.text = ">>";
        }
        else
        {
            Time.timeScale = 1;
            spedUp = false;
            spawner.GetComponent<SpawnerController>().SetSpawnSpeedOff();
            buttonSpeedUp.GetComponent<Image>().color = Color.white;
            panelTextLoss.SetActive(false);
            lossText.text = "";
        }
    }

    public void SetLemmingsExited()
    {
        lemmingsExited += 1;
        audioSource.PlayOneShot(lemmingExit, soundFxVolume);
    }

    public void SetLemmingsRequiredForWin(int amount)
    {
        lemmingsRequiredForWin = amount;
    }

    private void GameEnd()
    {
        if (lemmingsExited >= lemmingsRequiredForWin && !doOnce)
        {
            Debug.Log("End time: " + (Time.time - sceneStartTime));
            Instantiate(endParticles, ending.transform.position, Quaternion.identity); //particles
            ending.GetComponent<EndClicker>().SetEnoughLemmingSaved(); //make ending clickable
            doOnce = true;
        }

        if (lemmingsExited == amountOfLemmings)
        {
            Time.timeScale = 0;
            panelTextLoss.SetActive(true);
            lossText.text = "You saved everyone!";
            exitButton.SetActive(true);
            nextLevelButton.SetActive(true);
            buttonStop.SetActive(false);
            buttonStairs.SetActive(false);
            buttonDig.SetActive(false);
            buttonPause.SetActive(false);
            buttonSpeedUp.SetActive(false);
        }

        if (amountOfLemmingsLeft <= 0 && lemmingsExited < lemmingsRequiredForWin)
            GameLost();

        if (amountOfLemmingsLeft <= 0 && lemmingsExited >= lemmingsRequiredForWin)
            GameEndTimeEnded();

        if (ending.GetComponent<EndClicker>().GetEndingClicked())
            GameEndTimeEnded();
    }

    /// <summary>
    /// Won but didn't save all lemmings.
    /// </summary>
    public void GameEndTimeEnded()
    {
        Time.timeScale = 0;
        panelTextLoss.SetActive(true);
        lossText.text = "Saved "+lemmingsExited+"/"+ amountOfLemmings + " turtles!";
        exitButton.SetActive(true);
        nextLevelButton.SetActive(true);
        buttonStop.SetActive(false);
        buttonStairs.SetActive(false);
        buttonDig.SetActive(false);
        buttonPause.SetActive(false);
        buttonSpeedUp.SetActive(false);
    }

    /// <summary>
    /// Lost because time is out.
    /// </summary>
    public void GameLost()
    {
        Debug.Log("Lost because time is out");
        Time.timeScale = 0;
        panelTextLoss.SetActive(true);
        lossText.text = "You lost!";
        exitButton.SetActive(true);
        restartButton.SetActive(true);
        buttonStop.SetActive(false);
        buttonStairs.SetActive(false);
        buttonDig.SetActive(false);
        buttonPause.SetActive(false);
        buttonSpeedUp.SetActive(false);
    }

    public void SetFunctionTextPanelOn()
    {
        panelTextFunction.SetActive(true);
    }

    public void SetFunctionTextPanelOff()
    {
        panelTextFunction.SetActive(false);
    }

    public void SetAmountOfLemmingsLeft()
    {
        amountOfLemmingsLeft--;
    }
    
    [PunRPC]
    void SendChangeLevelOverNetwork(int saved)
    {
        Preload.lemmingsSavedTotal = saved;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    [PunRPC]
    void SendReloadLevelOverNetwork()
    {
        SceneManager.LoadScene("reloadBackgroundTexture");
    }

    [PunRPC]
    void SendPauseOverNetwork()
    {
        PausedOverNetwork();
    }

    private void PausedOverNetwork()
    {
        if (!paused)
        {
            Time.timeScale = 0;
            paused = true;
            buttonPause.GetComponent<Image>().color = Color.red;
            panelTextLoss.SetActive(true);
            lossText.text = "Paused";
            saveButton.SetActive(true);
            menuButton.SetActive(true);
        }
        else
        {
            if (spedUp)
            {
                Time.timeScale = 2;
            }
            else
            {
                Time.timeScale = 1;
            }
            paused = false;
            buttonPause.GetComponent<Image>().color = Color.white;
            panelTextLoss.SetActive(false);
            lossText.text = "";
            saveButton.SetActive(false);
            menuButton.SetActive(false);
        }
    }

    [PunRPC]
    void SendSpeedUpOverNetwork()
    {
        SpeedUpOverNetwork();
    }

    private void SpeedUpOverNetwork()
    {
        if (!spedUp)
        {
            Time.timeScale = 2;
            spedUp = true;
            spawner.GetComponent<SpawnerController>().SetSpawnSpeedOn();
            buttonSpeedUp.GetComponent<Image>().color = Color.red;
            panelTextLoss.SetActive(true);
            lossText.text = ">>";
        }
        else
        {
            Time.timeScale = 1;
            spedUp = false;
            spawner.GetComponent<SpawnerController>().SetSpawnSpeedOff();
            buttonSpeedUp.GetComponent<Image>().color = Color.white;
            panelTextLoss.SetActive(false);
            lossText.text = "";
        }
    }
}
