using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class Preload : MonoBehaviour {

    public static int lemmingFunction = 0; //Lemming function used to control lemmings: 0 - nothing, 1 - stop, 2 - stairs, 3 - dig
    public static int lemmingsSavedTotal = 0;
    public static string currentLevel = "level1";
    public static string name = "";
    public static string password = "";
    public static float playTime = 0;
    public static bool noSoundFx = false;
    public static bool fromMainMenu = false;
    public static string playerGuid = ""; //8f616943-f24b-47b8-a8c0-34a118108896
    public int func; //show lemmingFunction in inspector

    //public static Preload preload = this;

    /// <summary>
    /// When the gameobject is created.
    /// </summary>
    void Awake()
    {
        //This gameobject does not get destroyed when a new scene is loaded so the background music keeps playing from scene to another scene.
        //Makes it so that the audio source (background music) attached to this same gameobject won't stop on scene load
        DontDestroyOnLoad(transform.gameObject);
    }

    /// <summary>
    /// Run at the start of the instance.
    /// </summary>
    void Start()
    {
        SceneManager.LoadScene("mainMenu");
    }

    private void Update()
    {
        func = lemmingFunction;
    }

    public static void ReloadLevel()
    {
        SceneManager.LoadScene(currentLevel);
    }

    /// <summary>
    /// Serialize playerdata and save it in a file.
    /// </summary>
    public static void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGame.dat");
        PlayerData data = new PlayerData();
        data.lemmingsSavedTotal = lemmingsSavedTotal;
        data.currentLevel = currentLevel;
        Debug.Log("saving level: " + data.currentLevel);
        bf.Serialize(file, data);
        file.Close();
    }

    /// <summary>
    /// Load the serialized data.
    /// </summary>
    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGame.dat"))
        {
            Debug.Log(Application.persistentDataPath);
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGame.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();
            if(data.currentLevel != "level1")
            {
                lemmingsSavedTotal = data.lemmingsSavedTotal;
                Debug.Log("lemmings saved: " + data.lemmingsSavedTotal);
            }
            currentLevel = data.currentLevel;
            Debug.Log("current level: " + data.currentLevel);
        }
    }
}

[Serializable]
class PlayerData
{
    public int lemmingsSavedTotal;
    public string currentLevel;
}