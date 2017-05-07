using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HashTag : MonoBehaviour {

    public static GameObject tooltipCanvas;
    public static GameObject selectedHashTag;
    public static GameObject[] hashTagCanvases;
    public GameObject text;
    public GameObject worldMap;

	// Use this for initialization
	void Start () {
        normalizeSelection();
    }

    public static void normalizeSelection() {
        if (hashTagCanvases != null)
            foreach (GameObject hashTag in hashTagCanvases)
                if (hashTag != null) hashTag.GetComponentInChildren<CanvasRenderer>().GetComponent<Image>().color = new Color32(0, 0, 0, 93);
        if (selectedHashTag != null) selectedHashTag.GetComponentInChildren<CanvasRenderer>().GetComponent<Image>().color = new Color32(134, 0, 163, 200);
    }

    // Update is called once per frame
    void Update () {
        if (selectedHashTag != null && hashTagCanvases != null && hashTagCanvases.Length > 0) {
            if (Input.GetKeyUp(KeyCode.Joystick1Button1) || Input.GetKeyUp(KeyCode.P)) {
                selectedHashTag.GetComponentInChildren<CanvasRenderer>().GetComponent<Image>().color = new Color32(0,0,0,93);
                do { selectedHashTag = hashTagCanvases[(((System.Array.IndexOf(hashTagCanvases, selectedHashTag) + 1) % GazeInteraction.maxHashTags) + GazeInteraction.maxHashTags) % GazeInteraction.maxHashTags]; }
                while (selectedHashTag == null);
                selectedHashTag.GetComponentInChildren<CanvasRenderer>().GetComponent<Image>().color = new Color32(134, 0, 163, 200);
            }
            if (Input.GetKeyUp(KeyCode.JoystickButton2) || Input.GetKeyUp(KeyCode.O)) {
                selectedHashTag.GetComponentInChildren<CanvasRenderer>().GetComponent<Image>().color = new Color32(0, 0, 0, 93);
                do { selectedHashTag = hashTagCanvases[(((System.Array.IndexOf(hashTagCanvases, selectedHashTag) - 1) % GazeInteraction.maxHashTags) + GazeInteraction.maxHashTags) % GazeInteraction.maxHashTags]; }
                while (selectedHashTag == null);
                selectedHashTag.GetComponentInChildren<CanvasRenderer>().GetComponent<Image>().color = new Color32(134, 0, 163, 200);
            }
        }
        if ((Input.GetKeyUp(KeyCode.Joystick1Button4) || Input.GetKeyUp(KeyCode.X)) && selectedHashTag != null) {

            GameObject vrViewer = GameObject.Find("VR Viewer");
            NewsRoom newsRoom = vrViewer.GetComponent<NewsRoom>();
            string query = selectedHashTag.GetComponentInChildren<Text>().text.Replace("#","");
            newsRoom.startRequest(query);

            VRViewer.shouldBeShown = true;
            worldMap.SetActive(false);
            if (tooltipCanvas != null)
                tooltipCanvas.SetActive(false);
            if (hashTagCanvases != null)
                for (int i = 0; i < hashTagCanvases.Length; i++)
                    if (hashTagCanvases[i] != null) hashTagCanvases[i].SetActive(false);
        }
        if ((Input.GetKeyUp(KeyCode.Joystick1Button5) || Input.GetKeyUp(KeyCode.Z)) && VRViewer.shouldBeShown == true) {
            worldMap.SetActive(true);
            VRViewer.shouldBeShown = false;
            foreach (GameObject card in NewsRoom.cardList)
                card.SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.Joystick1Button3)) {
            GameObject speechText = GameObject.Find("Speech Text");
            SendText sendText = speechText.GetComponent<SendText>();
            sendText.getAndSendText();

            VRViewer.shouldBeShown = true;
            worldMap.SetActive(false);
            if (tooltipCanvas != null)
                tooltipCanvas.SetActive(false);
            if (hashTagCanvases != null)
                for (int i = 0; i < hashTagCanvases.Length; i++)
                    if (hashTagCanvases[i] != null) hashTagCanvases[i].SetActive(false);
        }

        if (Input.GetKeyUp(KeyCode.Joystick1Button0)) {
            GameObject go = GameObject.Find("Speech Text");
            ApiAiModule apiAimodule = go.GetComponent<ApiAiModule>();
            apiAimodule.answerTextField = go.GetComponent<Text>();
            apiAimodule.StartNativeRecognition();
        }


        //foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))) {
        //    if (Input.GetKey(vKey)) {
        //        text.GetComponent<Text>().text = vKey.ToString();
        //    }
        //}


    }

}
