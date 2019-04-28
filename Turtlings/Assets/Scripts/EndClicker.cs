using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndClicker : MonoBehaviour {

    private bool enoughLemmingsSaved = false;
    private bool endingClicked = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        if (enoughLemmingsSaved)
            endingClicked = true;
    }

    public void SetEnoughLemmingSaved()
    {
        enoughLemmingsSaved = true;
    }

    public bool GetEndingClicked()
    {
        return endingClicked;
    }
}
