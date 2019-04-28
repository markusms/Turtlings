using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private Button buttonLoad;
    private Button buttonPlay;
    private Button buttonTutorial;
    private Button buttonQuit;
    private Button buttonMultiplayer;

    // Use this for initialization
    void Start()
    {
        buttonPlay = GameObject.Find("ButtonPlay").GetComponent<Button>();
        buttonLoad = GameObject.Find("ButtonLoad").GetComponent<Button>();
        buttonTutorial = GameObject.Find("ButtonTutorial").GetComponent<Button>();
        buttonQuit = GameObject.Find("ButtonQuit").GetComponent<Button>();
        buttonMultiplayer = GameObject.Find("ButtonMultiplayer").GetComponent<Button>();
        buttonPlay.onClick.AddListener(() => StartGame("play"));
        buttonLoad.onClick.AddListener(() => StartGame("load"));
        buttonTutorial.onClick.AddListener(() => StartGame("tutorial"));
        buttonQuit.onClick.AddListener(() => StartGame("quit"));
        buttonMultiplayer.onClick.AddListener(() => StartGame("multiplayer"));
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartGame(string command)
    {
        if (command == "play")
        {
            SceneManager.LoadScene(Preload.currentLevel);
        }
        else if(command == "load")
        {
            Preload.Load();
            buttonLoad.GetComponentInChildren<Text>().text = "Loaded";
            buttonLoad.GetComponent<Image>().color = Color.red;
        }
        else if (command == "quit")
        {
            Application.Quit();
        }
        else if (command == "multiplayer")
        {
            //SceneManager.LoadScene(Preload.currentLevel+"multiplayer");
            SceneManager.LoadScene("multiplayerLobby");
        }
        else
        {
            SceneManager.LoadScene("tutorial");
        }
    }
}
