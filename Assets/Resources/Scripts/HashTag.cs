using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HashTag : MonoBehaviour {

    public static GameObject selectedHashTag;
    public static GameObject[] hashTagCanvases;
    public GameObject text;

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
            if (Input.GetKeyUp(KeyCode.Joystick1Button1)) {
                selectedHashTag.GetComponentInChildren<CanvasRenderer>().GetComponent<Image>().color = new Color32(0,0,0,93);
                do { selectedHashTag = hashTagCanvases[(((System.Array.IndexOf(hashTagCanvases, selectedHashTag) + 1) % GazeInteraction.maxHashTags) + GazeInteraction.maxHashTags) % GazeInteraction.maxHashTags]; }
                while (selectedHashTag == null);
                selectedHashTag.GetComponentInChildren<CanvasRenderer>().GetComponent<Image>().color = new Color32(134, 0, 163, 200);
            }
            if (Input.GetKeyUp(KeyCode.JoystickButton2)) {
                selectedHashTag.GetComponentInChildren<CanvasRenderer>().GetComponent<Image>().color = new Color32(0, 0, 0, 93);
                do { selectedHashTag = hashTagCanvases[(((System.Array.IndexOf(hashTagCanvases, selectedHashTag) - 1) % GazeInteraction.maxHashTags) + GazeInteraction.maxHashTags) % GazeInteraction.maxHashTags]; }
                while (selectedHashTag == null);
                selectedHashTag.GetComponentInChildren<CanvasRenderer>().GetComponent<Image>().color = new Color32(134, 0, 163, 200);
            }
        }
        if (Input.GetKeyUp(KeyCode.Joystick1Button4) && selectedHashTag != null) VRViewer.shouldBeShown = true;
        if (Input.GetKeyUp(KeyCode.Joystick1Button5) && VRViewer.shouldBeShown == true) VRViewer.shouldBeShown = false;
        
        
        
        //foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode))) {
        //    if (Input.GetKey(vKey)) {
        //        text.GetComponent<Text>().text = vKey.ToString();
        //    }
        //}


    }

}
