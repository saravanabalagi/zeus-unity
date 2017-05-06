using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connectivity : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.SetActive(!Ping.connected);
	}
}
