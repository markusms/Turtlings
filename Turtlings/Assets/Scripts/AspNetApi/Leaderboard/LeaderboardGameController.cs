using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardGameController : MonoBehaviour
{
    private float time = 0;
    private InputField inputFieldName;
    private InputField inputFieldName2;
    private InputField inputFieldPassword;
    private Text textScores;
    private Button submitButton;
    private Button menuButton;
    private string name;
    private string name2;
    private string password;

    // Start is called before the first frame update
    void Start()
    {
        submitButton = GameObject.Find("SubmitButton").GetComponent<Button>();
        submitButton.onClick.AddListener(() => Submitted());
        textScores = GameObject.Find("TextScores").GetComponent<Text>();
        inputFieldName = GameObject.Find("InputFieldName").GetComponent<InputField>();
        inputFieldName2 = GameObject.Find("InputFieldName2").GetComponent<InputField>();
        inputFieldPassword = GameObject.Find("InputFieldPassword").GetComponent<InputField>();
        time = Time.time+Preload.playTime;
        if (!Preload.fromMainMenu) //we come to the leaderboard from winning the game
        {
            //time = Time.time;
        }
        else //checking leaderboard from the menu
        {
            inputFieldName.gameObject.SetActive(false);
            inputFieldPassword.gameObject.SetActive(false);
            submitButton.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        password = inputFieldPassword.text;
        name = inputFieldName.text;
        name2 = inputFieldName2.text;
    }

    private void Submitted()
    {
        inputFieldName.gameObject.SetActive(false);
        inputFieldPassword.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);

        string url = "http://localhost:5000/api/players?name=" + name;
        RestClient.Get(url).Then(response =>
        {
            Debug.Log(response.Text);
            var playerInformationHolder = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerArray>(response.Text);
            if(playerInformationHolder.InfoArray.Count() == 0) //Create a new player
            {
                var newPlayer = new NewPlayer();
                newPlayer.Name = name;
                newPlayer.Password = password;
                string postUrl = "http://localhost:5000/api/players";

                var jsonPlayer = Newtonsoft.Json.JsonConvert.SerializeObject(newPlayer);
                RestClient.Post(postUrl, jsonPlayer).Then(response2 => {
                    Debug.Log(response2.Text);
                    var playerInformation = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerInformation>(response2.Text);
                    Preload.playerGuid = playerInformation.Id.ToString();
                    string postUrl2 = "http://localhost:5000/api/players/" + playerInformation.Id.ToString() + "/runs";
                    var newRun = new NewRun();
                    newRun.TimeTaken = time;
                    newRun.TimePosted = DateTime.UtcNow;
                    newRun.Level = "game";
                    var jsonRun = Newtonsoft.Json.JsonConvert.SerializeObject(newRun);
                    RestClient.Post(postUrl2, jsonRun, (exception, helper) => SubmitCallback(exception, helper));
                });
                
            }
            else
            {
                Debug.Log(playerInformationHolder.InfoArray[0]);
                string playerId = playerInformationHolder.InfoArray[0].Id.ToString();
                Preload.playerGuid = playerId;

                string postUrl = "http://localhost:5000/api/players/" + playerId + "/runs";

                var newRun = new NewRun();
                newRun.TimeTaken = time;
                newRun.TimePosted = DateTime.UtcNow;
                newRun.Level = "game";
                var jsonRun = Newtonsoft.Json.JsonConvert.SerializeObject(newRun);
                RestClient.Post(postUrl, jsonRun, (exception, helper) => SubmitCallback(exception, helper));
            }
            
        });

    }

    private void SubmitCallback(RequestException exception, ResponseHelper response)
    {
        EditorUtility.DisplayDialog("Status", "status code: " + response.StatusCode.ToString(), "Ok");

        RestClient.Get("http://localhost:5000/api/players/" + Preload.playerGuid + " /runs").Then(response2 =>
        {
            var runArray = Newtonsoft.Json.JsonConvert.DeserializeObject<Run[]>(response2.Text);
            Debug.Log(response2.Text);

            StringBuilder sb = new StringBuilder();
            sb.Append("Your runs:\n");
            var runList = new List<Run>(runArray);
            runList = runList.OrderBy(x => x.TimeTaken).ToList();

            int i = 1;
            foreach (var run in runList)
            {
                sb.Append(i).Append(". ").Append(run.TimeTaken).Append("s - ").Append(run.TimePosted.ToShortDateString()).Append("\n");
                i++;
                if (i == 11)
                    break;
            }
            textScores.text = sb.ToString();

        });
    }

    public void OnMenuButtonClick()
    {
        SceneManager.LoadScene("mainMenu");
    }

    public void OnPlayersRunsClicked()
    {
        string playerId = "";
        if (name2 != null)
        {
            RestClient.Get("http://localhost:5000/api/players?name=" + name2, (exception, helper) => GetIdCallback(exception, helper));
        }
        else if (Preload.playerGuid != "")
        {
            playerId = Preload.playerGuid;
            if(Preload.name != "")
                PrintPlayersRuns(playerId, Preload.name);
            else
                PrintPlayersRuns(playerId);
        }
    }

    private void GetIdCallback(RequestException exception, ResponseHelper response)
    {
        var playerInformationHolder = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerArray>(response.Text);
        Debug.Log(response.Text);
        if(playerInformationHolder.InfoArray.Count() != 0)
        {
            string playerId = playerInformationHolder.InfoArray[0].Id.ToString();
            PrintPlayersRuns(playerId, playerInformationHolder.InfoArray[0].Name);
        }
        else
        {
            EditorUtility.DisplayDialog("Status", "not found", "Ok");
        }
    }

    private void PrintPlayersRuns(string playerId, string playerName = "Player")
    {
        RestClient.Get("http://localhost:5000/api/players/" + playerId + "/runs").Then(response2 =>
        {
            var runArray = Newtonsoft.Json.JsonConvert.DeserializeObject<Run[]>(response2.Text);
            Debug.Log(response2.Text);

            StringBuilder sb = new StringBuilder();
            sb.Append(playerName).Append("'s runs:\n");
            var runList = new List<Run>(runArray);
            runList = runList.OrderBy(x => x.TimeTaken).ToList();

            int i = 1;
            foreach (var run in runList)
            {
                sb.Append(i).Append(". ").Append(run.TimeTaken).Append("s - ").Append(run.TimePosted.ToShortDateString()).Append("\n");
                i++;
                if (i == 11)
                    break;
            }
            textScores.text = sb.ToString();

        });
    }

    public void OnAllRunsTodayClicked()
    {
        OnAllRunsClicked("today");
    }

    public void OnAllRunsThisWeekClicked()
    {
        OnAllRunsClicked("week");
    }

    public void OnAllRunsClicked(string dateFilter = "")
    {
        string getUrl = "http://localhost:5000/api/players/top?filterDate=" + dateFilter;
        RestClient.Get(getUrl).Then(response =>
        {
            var playerArray = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerArray>(response.Text);
            Debug.Log(response.Text);

            StringBuilder sb = new StringBuilder();
            sb.Append("All runs:\n");

            int i = 1;
            foreach (var player in playerArray.InfoArray)
            {
                sb.Append(i).Append(". ").Append(playerArray.InfoArray[i-1].Name).Append(" - ").Append(playerArray.InfoArray[i - 1].Runs[0].TimeTaken).Append("s - ").Append(playerArray.InfoArray[i - 1].Runs[0].TimePosted.ToShortDateString()).Append("\n");
                i++;
                if (i == 11)
                    break;
            }
            textScores.text = sb.ToString();

        });
    }
}
