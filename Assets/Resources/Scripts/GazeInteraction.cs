using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GazeInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public Material inactiveMaterial;
    public Material gazedAtMaterial;
    public Material selectedMaterial;

    public VRViewer vrViewer;
    public Camera mainCam;

    public static GameObject canvasGameObject;
    public static GameObject textGameObject;
    public static GameObject panelGameObject;

    public static GameObject selectedCountry;


    void Start() {
        SetGazedAt(false);
        mainCam = Camera.main;
        if (GameObject.Find("VR Viewer"))  vrViewer = GameObject.Find("VR Viewer").GetComponent<VRViewer>();
    }

    public void SetGazedAt(bool gazedAt) {
        if (inactiveMaterial != null && gazedAtMaterial != null) {
            GetComponent<Renderer>().material = gazedAt 
                ? gazedAtMaterial 
                : (selectedCountry == gameObject) ? selectedMaterial : inactiveMaterial;
            return;
        }
        GetComponent<Renderer>().material.color = gazedAt ? Color.green : Color.red;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        SetGazedAt(true);

        if (canvasGameObject == null) {
            canvasGameObject = new GameObject();
            canvasGameObject.name = "Tooltip";
            canvasGameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            Canvas canvas = canvasGameObject.AddComponent<Canvas>();

            panelGameObject = new GameObject();
            panelGameObject.name = "Panel";
            panelGameObject.transform.SetParent(canvasGameObject.GetComponent<Transform>(), true);
            panelGameObject.AddComponent<RectTransform>();
            
            panelGameObject.transform.localPosition = new Vector3(0,0,0);
            panelGameObject.transform.localScale = new Vector3(1,1,1);
            panelGameObject.AddComponent<CanvasRenderer>();
            Image panelBg = panelGameObject.AddComponent<Image>();
            panelBg.sprite = Resources.Load<Sprite>("Sprites/Black-Rounded-Minimal");

            textGameObject = new GameObject();
            textGameObject.name = "Text";
            textGameObject.transform.parent = canvasGameObject.transform;
            textGameObject.transform.localPosition = new Vector3(0, 0, 0);
            textGameObject.transform.localScale = new Vector3(1, 1, 1);

            Text t = textGameObject.AddComponent<Text>();
            t.alignment = TextAnchor.MiddleCenter;
            t.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            textGameObject.AddComponent<WorldNewsTransform>();
        }

        Text textComponent = textGameObject.GetComponent<Text>();
        textComponent.text = gameObject.name;

        RectTransform rt = panelGameObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(CalculateLengthOfMessage(textComponent, textComponent.text)+40, 45);

        canvasGameObject.SetActive(true);
        textGameObject.SetActive(true);

 
        canvasGameObject.transform.SetParent(mainCam.GetComponent<Transform>(), true);
        canvasGameObject.transform.localPosition = new Vector3(1, 0, 6);
        //canvasGameObject.transform.position = new Vector3(0, 1, 6);
        //canvasGameObject.transform.RotateAround(new Vector3(0, 1, 0), Vector3.right, mainCam.transform.eulerAngles.x);
        //canvasGameObject.transform.RotateAround(new Vector3(0, 1, 0), Vector3.up, mainCam.transform.eulerAngles.y);
        //canvasGameObject.transform.LookAt(2 * canvasGameObject.transform.position - new Vector3(0, 1, 0));

    }

    int CalculateLengthOfMessage(Text textComponent, string message) {
        int totalLength = 0;
        Font myFont = textComponent.font;  //chatText is my Text component
        CharacterInfo characterInfo = new CharacterInfo();
        char[] arr = message.ToCharArray();
        foreach (char c in arr) {
            myFont.GetCharacterInfo(c, out characterInfo, textComponent.fontSize);
            totalLength += characterInfo.advance;
        }
        return totalLength;
    }

    public void OnPointerExit(PointerEventData eventData) {
        SetGazedAt(false);
        canvasGameObject.SetActive(false);
        textGameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData) {

        if(selectedCountry != null)
            selectedCountry.GetComponent<Renderer>().material = inactiveMaterial;

        if (selectedCountry == gameObject) {
            selectedCountry.GetComponent<Renderer>().material = gazedAtMaterial;
            selectedCountry = null;
        } else {
            selectedCountry = this.gameObject;
            selectedCountry.GetComponent<Renderer>().material = selectedMaterial;
        }


        

    }

    public Vector3 getTransformCoordinatesFromAngle(Vector3 angle) {
        float toRadians = Mathf.PI / 180;
        Vector3 coordinates = new Vector3(0, 0, 0);
        coordinates.x = Mathf.Sin(angle.y * toRadians) * Mathf.Cos(angle.x * toRadians) ;
        coordinates.y =                                - Mathf.Sin(angle.x * toRadians) + 1;
        coordinates.z = Mathf.Cos(angle.y * toRadians) * Mathf.Cos(angle.x * toRadians) ;
        return coordinates;
    }



    // Update is called once per frame
    void Update () {
		
	}

}
