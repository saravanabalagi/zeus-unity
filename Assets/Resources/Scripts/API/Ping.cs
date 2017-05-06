using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ping : MonoBehaviour {

    public static string rootUrl = "https://zeus-news.herokuapp.com";
    public static bool connected = false;

	// Use this for initialization
	IEnumerator Start () {
        WWW www = new WWW(rootUrl+"/ping");
        yield return www;
        if (www.responseHeaders["STATUS"].Contains("OK")) {
            connected = true;
            gameObject.GetComponent<Text>().text = "Connected";
        } else {
            connected = false;
            gameObject.GetComponent<Text>().text = "Connection Failed";
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
