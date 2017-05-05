using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldNews : MonoBehaviour {

    public Camera mainCam;

	// Use this for initialization
	void Start () {
        mainCam = Camera.main;
        transform.SetParent(mainCam.GetComponent<Transform>(), true);
    }
	
	// Update is called once per frame
	void Update () {
	}
}
