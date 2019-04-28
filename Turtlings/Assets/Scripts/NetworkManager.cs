using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : Photon.PunBehaviour
{
    private bool anotherPlayerJoined = false;
    private bool weJoined = false;
    private const string roomName = "RoomName";
    private RoomInfo[] roomsList;
    public List<PhotonPlayer> currentPlayersInRoom = new List<PhotonPlayer>();

    /// <summary>
    /// When the gameobject is created.
    /// </summary>
    void Awake()
    {
        //This gameobject does not get destroyed when a new scene is loaded so the background music keeps playing from scene to another scene.
        //Makes it so that the audio source (background music) attached to this same gameobject won't stop on scene load
        DontDestroyOnLoad(transform.gameObject);
    }

    // Use this for initialization
    void Start()
    {
        PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
        PhotonNetwork.ConnectUsingSettings("0.1");


    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGUI()
    {
        if (SceneManager.GetActiveScene().name == "multiplayerLobby" || SceneManager.GetActiveScene().buildIndex > 10)
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

            if (PhotonNetwork.room == null)
            {
                //luodaan huone
                int w = 350;
                int h = 100;
                Rect rect = new Rect((Screen.width - w) / 2, (Screen.height - h) / 4, w, h);
                if (GUI.Button(rect, "Start Server"))
                {
                    //PhotonNetwork.CreateRoom(roomName + System.Guid.NewGuid().ToString("N"));
                    PhotonNetwork.CreateRoom(System.Guid.NewGuid().ToString("N"));
                }

                //liitytään huoneeseen
                if (roomsList != null)
                {
                    for (int i = 1; i <= roomsList.Length; i++)
                    {
                        Rect rect2 = new Rect((Screen.width - w) / 2, (Screen.height - h) / 4 + (110 * i), w, h);
                        if (GUI.Button(rect2, "Join: " + roomsList[i-1].Name))
                        {
                            PhotonNetwork.JoinRoom(roomsList[i-1].Name);
                        }
                    }
                }

            }
        }
        if (!anotherPlayerJoined && weJoined)
        {
            //int w = 350;
            //int h = 100;
            //Rect rect = new Rect((Screen.width - w) / 2, (Screen.height - h) / 4, w, h);
            //if (GUI.Button(rect, "Waiting for another player"))
            //{
            //    //wait
            //}
        }
    }

    public override void OnReceivedRoomListUpdate()
    {
        roomsList = PhotonNetwork.GetRoomList();
    }

    private void OnConnectedToServer()
    {
        Debug.Log("Yhdistettiin serveriin.");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Tultiin lobbyyn.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Mentiin huoneeseen pelaamaan.");
        weJoined = true;
        if (PhotonNetwork.playerList.Length == 2)
        {
            SceneManager.LoadScene(Preload.currentLevel + "multiplayer");
        }
        //Instansioidaan pelaaja. Huom! Network pelaaja
        //GameObject player = PhotonNetwork.Instantiate("Turtle", new Vector3(0, 0.5f, 0), Quaternion.identity, 0);
        //SceneManager.LoadScene(Preload.currentLevel + "multiplayer");
    }

    //private void OnPlayerDisconnected(Network player)
    //{
    //    //jos halutaan tehdä jotain kun lähdetään pois pelistä
    //}

    private void OnPlayerDisconnected(NetworkCharacter player)
    {
        Debug.Log("player disconnected");
        anotherPlayerJoined = false;
        //jos halutaan tehdä jotain kun lähdetään pois pelistä
    }

    public override void OnCreatedRoom()
    {

    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("OnPhotonPlayerConnected() " + newPlayer.NickName); // not seen if you're the player connecting
        
        anotherPlayerJoined = true;
        if (PhotonNetwork.playerList.Length == 2)
        {
            SceneManager.LoadScene(Preload.currentLevel + "multiplayer");
        }
    }


    //void OnPhotonPlayerDisconnected()
    //{

    //}

}
