using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private Button buttonLoad;
    private Button buttonCloudLoad;
    private Button buttonPlay;
    private Button buttonTutorial;
    private Button buttonQuit;
    private Button buttonMultiplayer;
    public Button buttonLogin;
    public Button buttonDeleteCloudSave;
    public GameObject loginPanel;
    public InputField inputFieldName;
    public InputField inputFieldPassword;
    public InputField inputFieldNameAdmin;
    public InputField inputFieldPasswordAdmin;
    public InputField inputFieldBanPlayerName;
    private string name;
    private string password;
    private string adminName;
    private string adminPass;
    private string banPlayerName;

    // Use this for initialization
    void Start()
    {
        Preload.fromMainMenu = true;
        buttonPlay = GameObject.Find("ButtonPlay").GetComponent<Button>();
        buttonLoad = GameObject.Find("ButtonLoad").GetComponent<Button>();
        buttonCloudLoad = GameObject.Find("ButtonCloudLoad").GetComponent<Button>();
        buttonTutorial = GameObject.Find("ButtonTutorial").GetComponent<Button>();
        buttonQuit = GameObject.Find("ButtonQuit").GetComponent<Button>();
        buttonMultiplayer = GameObject.Find("ButtonMultiplayer").GetComponent<Button>();
        buttonPlay.onClick.AddListener(() => StartGame("play"));
        buttonLoad.onClick.AddListener(() => StartGame("load"));
        buttonLogin.onClick.AddListener(() => StartGame("cloudload"));
        buttonDeleteCloudSave.onClick.AddListener(() => StartGame("delete"));
        buttonTutorial.onClick.AddListener(() => StartGame("tutorial"));
        buttonQuit.onClick.AddListener(() => StartGame("quit"));
        buttonMultiplayer.onClick.AddListener(() => StartGame("multiplayer"));
    }

    // Update is called once per frame
    void Update()
    {
        password = inputFieldPassword.text;
        name = inputFieldName.text;
        adminName = inputFieldNameAdmin.text;
        adminPass = inputFieldPasswordAdmin.text;
        banPlayerName = inputFieldBanPlayerName.text;
    }

    void StartGame(string command)
    {
        if (command == "play")
        {
            SceneManager.LoadScene(Preload.currentLevel);
        }
        else if (command == "load")
        {
            Preload.Load();
            buttonLoad.GetComponentInChildren<Text>().text = "Loaded";
            buttonLoad.GetComponent<Image>().color = Color.red;
        }
        else if (command == "cloudload")
        {
            RestClient.Get("http://localhost:5000/api/players?name=" + name, (exception, helper) => LoadCallback(exception, helper));
        }
        else if (command == "delete")
        {
            string postUrl = "http://localhost:5000/api/players/" + Preload.playerGuid + "/save?delete=delete";
            var newPlayer = new NewPlayer();
            newPlayer.Name = Preload.name;
            newPlayer.Password = Preload.password;
            var jsonRun = Newtonsoft.Json.JsonConvert.SerializeObject(newPlayer);
            RestClient.Put(postUrl, jsonRun, (exception, helper) => DeleteCallback(exception, helper));
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

    private void LoadCallback(RequestException exception, ResponseHelper response)
    {
        var playerInformationHolder = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerArray>(response.Text);
        Debug.Log(response.Text);
        string playerId = playerInformationHolder.InfoArray[0].Id.ToString();

        Preload.name = name;
        Preload.password = password;
        Preload.playerGuid = playerId;

        string postUrl = "http://localhost:5000/api/players/" + playerId + "/save";
        var newPlayer = new NewPlayer();
        newPlayer.Name = name;
        newPlayer.Password = password;
        var jsonPlayer = Newtonsoft.Json.JsonConvert.SerializeObject(newPlayer);
        RestClient.Put(postUrl, jsonPlayer).Then(response2 =>
        {
            EditorUtility.DisplayDialog("Status", "status code: " + response2.StatusCode.ToString(), "Ok");
            var playerSaveData = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(response2.Text);
            if(playerSaveData == null)
            {
                Preload.lemmingsSavedTotal = 0;
                Preload.currentLevel = "level1";
                Preload.playTime = 0;
            }
            else
            {
                Preload.lemmingsSavedTotal = playerSaveData.LemmingsSavedTotal;
                Preload.currentLevel = playerSaveData.CurrentLevel;
                Preload.playTime = playerSaveData.PlayTime;
            }
            Debug.Log(response2.Text);

            buttonCloudLoad.GetComponentInChildren<Text>().text = "Loaded";
            buttonCloudLoad.GetComponent<Image>().color = Color.red;
            loginPanel.SetActive(false);
        });
    }

    private void DeleteCallback(RequestException exception, ResponseHelper response)
    {
        EditorUtility.DisplayDialog("Status", "status code: " + response.StatusCode.ToString(), "Ok");
    }

    public void OnLeaderboardsClicked()
    {
        SceneManager.LoadScene("Leaderboard");
    }
}
