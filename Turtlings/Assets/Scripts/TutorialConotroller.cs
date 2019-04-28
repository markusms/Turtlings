using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialConotroller : MonoBehaviour {

    private Button buttonBack;
    private Button buttonVolUp;
    private Button buttonVolDown;
    private Button buttonSounds;
    private GameObject Panel4;

    private AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        audioSource = FindObjectOfType<AudioSource>();

        buttonBack = GameObject.Find("ButtonBack").GetComponent<Button>();
        buttonVolUp = GameObject.Find("ButtonVolumeUp").GetComponent<Button>();
        buttonVolDown = GameObject.Find("ButtonVolumeDown").GetComponent<Button>();
        buttonSounds = GameObject.Find("ButtonSounds").GetComponent<Button>();
        Panel4 = GameObject.Find("Panel4");
        
        ColorBlock cb = buttonVolUp.colors;
        cb.highlightedColor = new Color(0.425f, 0.776f, 0.322f);
        buttonVolUp.colors = cb;
        cb = buttonVolDown.colors;
        cb.highlightedColor = new Color(0.896f, 0.289f, 0.283f);
        buttonVolDown.colors = cb;

        buttonBack.onClick.AddListener(() => Back());
        buttonVolUp.onClick.AddListener(() => Volume(1));
        buttonVolDown.onClick.AddListener(() => Volume(0));
        buttonSounds.onClick.AddListener(() => Volume(2));

        if (Preload.noSoundFx == true)
        {
            buttonSounds.GetComponentInChildren<Text>().text = "Sounds: NO";
            cb = buttonSounds.colors;
            cb.normalColor = new Color(0.896f, 0.289f, 0.283f);
            cb.highlightedColor = new Color(0.896f, 0.289f, 0.283f);
            buttonSounds.colors = cb;
        }
        else
        {
            buttonSounds.GetComponentInChildren<Text>().text = "Sounds: YES";
            cb = buttonSounds.colors;
            cb.normalColor = new Color(0.425f, 0.776f, 0.322f);
            cb.highlightedColor = new Color(0.425f, 0.776f, 0.322f);
            buttonSounds.colors = cb;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float truncated = (float)(System.Math.Truncate((double)audioSource.volume * 1000.0) / 1000.0);
        Panel4.GetComponentInChildren<Text>().text = "Vol: " + truncated;
    }

    void Back()
    {
        SceneManager.LoadScene("mainMenu");
    }

    void Volume(int i)
    {
        if (i == 1)
        {
            if (audioSource.volume < 1 && audioSource.volume < 0.1)
                audioSource.volume += 0.01f;
            else if (audioSource.volume < 1)
                audioSource.volume += 0.05f;
        }
        else if (i == 0)
        {
            if (audioSource.volume > 0 && audioSource.volume <= 0.1)
                audioSource.volume -= 0.01f;
            else if (audioSource.volume > 0)
                audioSource.volume -= 0.05f;
        }
        else
        {
            if(Preload.noSoundFx == false)
            {
                buttonSounds.GetComponentInChildren<Text>().text = "Sounds: NO";
                ColorBlock cb = buttonSounds.colors;
                cb.normalColor = new Color(0.896f,0.289f,0.283f);
                cb.highlightedColor = new Color(0.896f, 0.289f, 0.283f);
                buttonSounds.colors = cb;
                Preload.noSoundFx = true;
            }
            else
            {
                buttonSounds.GetComponentInChildren<Text>().text = "Sounds: YES";
                ColorBlock cb = buttonSounds.colors;
                cb.normalColor = new Color(0.425f, 0.776f, 0.322f);
                cb.highlightedColor = new Color(0.425f, 0.776f, 0.322f);
                buttonSounds.colors = cb;
                Preload.noSoundFx = false;
            }
        }
    }
}
