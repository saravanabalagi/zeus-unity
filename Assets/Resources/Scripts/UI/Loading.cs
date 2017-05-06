using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour {

    public GameObject loadingReticle;
    public float rotateSpeed = 300;
    public Camera mainCam;
    public static bool loading = false;

	// Use this for initialization
	void Start () {
        if (mainCam == null) mainCam = Camera.main;
        transform.SetParent(mainCam.transform, true);
        transform.localPosition = new Vector3(0,0,5);
        transform.localEulerAngles = new Vector3(0,0,0);
	}
	
	// Update is called once per frame
	void Update () {
        loadingReticle.SetActive(loading);
        transform.Rotate(Vector3.back * Time.deltaTime * rotateSpeed);
	}
}
