using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerMenuController : MonoBehaviour {

    private Button buttonBack;

    // Use this for initialization
    void Start () {
        buttonBack = GameObject.Find("ButtonBack").GetComponent<Button>();
        buttonBack.onClick.AddListener(() => Back());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Back()
    {
        SceneManager.LoadScene("mainMenu");
    }
}
