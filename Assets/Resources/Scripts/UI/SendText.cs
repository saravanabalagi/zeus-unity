using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendText : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public void getAndSendText() {
        GameObject vrViewer = GameObject.Find("VR Viewer");
        NewsRoom newsRoom = vrViewer.GetComponent<NewsRoom>();
        string query = gameObject.GetComponent<Text>().text;
        JSONObject jsonObject = new JSONObject(query);
        JSONObject resultJson = jsonObject.GetField("result");
        newsRoom.startApiRequest(resultJson.GetField("resolvedQuery").str);
    }
   
	
	// Update is called once per frame
	void Update () {
		
	}
}
