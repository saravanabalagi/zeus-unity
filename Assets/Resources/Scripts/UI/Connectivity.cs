using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connectivity : MonoBehaviour {

    public Camera mainCam;

	// Use this for initialization
	void Start () {
        if(mainCam==null) mainCam = Camera.main;
        transform.SetParent(mainCam.transform, true);
        transform.localPosition = new Vector3(0.7f, 0, 5);
        transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update () {
        gameObject.SetActive(!Ping.connected);
	}
}
