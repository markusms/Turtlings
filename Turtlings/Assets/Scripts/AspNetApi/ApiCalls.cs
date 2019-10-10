using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;
using System;

public class ApiCalls : MonoBehaviour
{
    /*public static string apiUrl = "http://localhost:5000/api/players/";
    public static string apiUrlEnding = "";

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Use this for initialization
    void Start()
    {

    }

    public static IEnumerator GetPlayer()
    {
        string url = apiUrl + apiUrlEnding;
        WWW www = new WWW(url);

        Debug.Log(apiUrl);

        while (!www.isDone)
            yield return null;

        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.text);
        }
        else
            Debug.Log(www.error);
    }

    public static IEnumerator GetUnityWebRequest(string jsonString)
    {
        //Debug.Log(jsonString);
        string url = apiUrl + apiUrlEnding;
        //byte[] byteData = System.Text.Encoding.ASCII.GetBytes(jsonString.ToCharArray());
        //Debug.Log(url);
        //UnityWebRequest unityWebRequest = new UnityWebRequest(url, "Get");
        ////unityWebRequest.uploadHandler = new UploadHandlerRaw(byteData);
        //unityWebRequest.uploadHandler = new UploadHandlerRaw(jsonString);
        //unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        //DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();
        //unityWebRequest.downloadHandler = downloadHandlerBuffer;
        //yield return unityWebRequest.Send();

        //if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        //{
        //    Debug.Log(unityWebRequest.error);
        //}
        //else
        //{
        //    string response = unityWebRequest.downloadHandler.text;
        //    Debug.Log("Form upload complete! Status Code: " + unityWebRequest.responseCode);
        //    Debug.Log(response);
        //}

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        var jsonString2 = "{\"Id\":2,\"Name\":\"Mary\"}";
        byte[] byteData = System.Text.Encoding.ASCII.GetBytes(jsonString.ToCharArray());
        WWW wwwPostRequest = new WWW(url, byteData, headers);
        yield return wwwPostRequest;
        if (string.IsNullOrEmpty(wwwPostRequest.error))
        {
            Debug.Log("Form upload complete!");
        }
        else
        {
            Debug.Log(wwwPostRequest.error);
        }
    }

    public static IEnumerator PostUnityWebRequest()
    {
        ///<summary>
        /// Post using UnityWebRequest class
        /// </summary>
        var jsonString = "{\"Id\":3,\"Name\":\"Roy\"}";
        byte[] byteData = System.Text.Encoding.ASCII.GetBytes(jsonString.ToCharArray());

        UnityWebRequest unityWebRequest = new UnityWebRequest("http://localhost:55376/api/values", "POST");
        unityWebRequest.uploadHandler = new UploadHandlerRaw(byteData);
        unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        yield return unityWebRequest.Send();

        if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            Debug.Log(unityWebRequest.error);
        }
        else
        {
            Debug.Log("Form upload complete! Status Code: " + unityWebRequest.responseCode);
        }
    }

    public static IEnumerator PostApi()
    {
        string url = apiUrl + apiUrlEnding;
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        var jsonString = "{\"Id\":2,\"Name\":\"Mary\"}";
        byte[] byteData = System.Text.Encoding.ASCII.GetBytes(jsonString.ToCharArray());
        WWW wwwPostRequest = new WWW(url, byteData, headers);
        yield return wwwPostRequest;
        if (string.IsNullOrEmpty(wwwPostRequest.error))
        {
            Debug.Log("Form upload complete!");
        }
        else
        {
            Debug.Log(wwwPostRequest.error);
        }
    }*/
}
