using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRViewer : MonoBehaviour {

    public static bool shouldBeShown = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Renderer>().enabled = shouldBeShown;
	}


}
