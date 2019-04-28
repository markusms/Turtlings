using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingMultiplayerController : MonoBehaviour
{

    private Button buttonBack;
    private GameObject Panel1;

    // Use this for initialization
    void Start()
    {
        buttonBack = GameObject.Find("ButtonBack").GetComponent<Button>();
        Panel1 = GameObject.Find("Panel1");
        float savedLemmings = (float)Preload.lemmingsSavedTotal / 60 * 100;
        Panel1.GetComponentInChildren<Text>().text = "Thank you for playing!\nYou guys saved " + (int)savedLemmings + "% of the turtles!";
        buttonBack.onClick.AddListener(() => Back());
    }

    // Update is called once per frame
    void Update()
    {
        float savedLemmings = (float)Preload.lemmingsSavedTotal / 60 * 100;
        Panel1.GetComponentInChildren<Text>().text = "Thank you for playing!\nYou guys saved " + (int)savedLemmings + "% of the turtles!";
    }

    void Back()
    {
        SceneManager.LoadScene("mainMenu");
    }
}
