using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldNewsTransform : MonoBehaviour {

    public Camera mainCam;

    private float lastY;
    private float totalY = 0;

    // Use this for initialization
    void Start () {
        mainCam = Camera.main;
        lastY = mainCam.transform.eulerAngles.y;
    }
	
	// Update is called once per frame
	void Update () {
        //transform.position = getTransformCoordinatesFromAngle(mainCam.transform.eulerAngles);

        float currentY = mainCam.transform.eulerAngles.y;
        float ang = currentY - lastY;           // measure the angle rotated since last frame:
        if ((ang > 0.001 || ang < -0.001)) {    // if rotated a significant angle...
            if (!(ang > 300 || ang < -300)) totalY += ang; // accumulate in curAngleX...
            lastY = currentY; // and update lastFwd
        }

        //print(totalY + " " + currentY);

        //transform.position = new Vector3(0, 1, 0);
        //transform.RotateAround(new Vector3(0, 1, 0), Vector3.right, mainCam.transform.eulerAngles.x);
        //transform.RotateAround(new Vector3(0, 1, 0), Vector3.up, totalY);
        //transform.LookAt(2 * transform.position - new Vector3(0, 1, 0));
    }

    public Vector3 getTransformCoordinatesFromAngle(Vector3 angle) {
        float toRadians = Mathf.PI / 180;
        Vector3 coordinates = new Vector3(0, 0, 0);
        coordinates.x = Mathf.Sin(angle.y * toRadians) * Mathf.Cos(angle.x * toRadians);
        coordinates.y = -Mathf.Sin(angle.x * toRadians) + 1;
        coordinates.z = Mathf.Cos(angle.y * toRadians) * Mathf.Cos(angle.x * toRadians);
        return coordinates;
    }
}
