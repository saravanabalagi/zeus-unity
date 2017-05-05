using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeText : MonoBehaviour {

    public string url = "https://duckduckgo.com/?q=rose&format=json";

    void Start() {
        StartCoroutine(makeRequest());
    }

    IEnumerator makeRequest() {
        WWW duckDuckGoRequest = new WWW(url);
        yield return duckDuckGoRequest;
        JSONObject responseJson = new JSONObject(duckDuckGoRequest.text);
        Debug.Log(responseJson.GetField("Heading"));
        updateText(responseJson.GetField("Heading").str);
    }

    void updateText(string text) {
        gameObject.GetComponent<Text>().text = text;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
