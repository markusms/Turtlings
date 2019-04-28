using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkLevelChanger : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeLevel()
    {
        photonView.RPC("SendChangeLevelOverNetwork", PhotonTargets.Others);
    }

    [PunRPC]
    void SendChangeLevelOverNetwork()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
