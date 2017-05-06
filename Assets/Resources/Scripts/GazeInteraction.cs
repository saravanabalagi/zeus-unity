using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GazeInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public Material inactiveMaterial;
    public Material gazedAtMaterial;
    public Material selectedMaterial;

    public static GameObject selectedCountry;

    public VRViewer vrViewer;
    public Camera mainCam;

    public static GameObject tooltipCanvas;
    public static GameObject tooltipText;
    public static GameObject panelGameObject;

    //private static bool fetched = false;
    private const int maxHashTags = 20;
    private static GameObject[] hashTagCanvases;
    private static List<string> hashTags = new List<string>();

    void Start() {
        SetGazedAt(false);
        mainCam = Camera.main;
        if (GameObject.Find("VR Viewer")) vrViewer = GameObject.Find("VR Viewer").GetComponent<VRViewer>();
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
        StartCoroutine(getHashTagsForCountry(gameObject.name));
        ShowToolTip();
    }

    private IEnumerator getHashTagsForCountry(string country) {
        WWW hashTagRequest = new WWW(Ping.rootUrl + "/trends/" + country + "?count=20");
        yield return hashTagRequest;
        JSONObject countryJson = new JSONObject(hashTagRequest.text);
        if (countryJson.type == JSONObject.Type.ARRAY) {
            hashTags.Clear();
            foreach (JSONObject trendJson in countryJson.list) {
                if(hashTags.Count<maxHashTags-1) hashTags.Add(trendJson.GetField("name").str);
            }
            ShowHashTags();
        }
    }

    void ShowHashTags() {
        if(hashTagCanvases==null) hashTagCanvases = new GameObject[maxHashTags];
        for(int i=0; i< hashTags.Count; i++) {
            GameObject hashTagCanvas;
            if (hashTagCanvases[i]==null) {
                hashTagCanvas = new GameObject();
                hashTagCanvas.name = "HashTag Tooltip";
                hashTagCanvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                hashTagCanvas.AddComponent<Canvas>();

                GameObject panelGameObject = new GameObject();
                panelGameObject.name = "Panel";
                panelGameObject.transform.SetParent(hashTagCanvas.GetComponent<Transform>(), true);

                panelGameObject.transform.localPosition = new Vector3(0, 0, 0);
                panelGameObject.transform.localScale = new Vector3(1, 1, 1);
                panelGameObject.AddComponent<CanvasRenderer>();
                Image panelBg = panelGameObject.AddComponent<Image>();
                panelBg.sprite = Resources.Load<Sprite>("Sprites/square_rounded_less");
                panelBg.color = new Color32(0, 0, 0, 93);

                GameObject tooltipText = new GameObject();
                tooltipText.name = "Text";
                tooltipText.transform.parent = hashTagCanvas.transform;
                panelGameObject.GetComponent<RectTransform>();
                tooltipText.transform.localPosition = new Vector3(0, 0, 0);
                tooltipText.transform.localScale = new Vector3(1, 1, 1);
                Text t = tooltipText.AddComponent<Text>();
                t.alignment = TextAnchor.MiddleCenter;
                t.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

                t.text = hashTags[i];
                RectTransform rt = panelGameObject.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(CalculateLengthOfMessage(t, t.text) + 40, 45);

                hashTagCanvases[i] = hashTagCanvas;
            }

            hashTagCanvas = hashTagCanvases[i];
            hashTagCanvas.transform.SetParent(mainCam.GetComponent<Transform>(), true);

            Random.InitState(System.DateTime.Now.Millisecond - Random.Range(10001,99999));
            float positionX = Random.Range(-2f, 2f);
            Random.InitState(System.DateTime.Now.Millisecond - Random.Range(10001,99999));
            float positionY = Random.Range(-2f, 2f);

            hashTagCanvas.transform.localPosition = new Vector3(positionX, positionY, 6);
            hashTagCanvas.transform.localEulerAngles = new Vector3(0, 0, 0);

            hashTagCanvas.SetActive(true);
        }
    }

    void ShowToolTip() {
        if (tooltipCanvas == null) {
            tooltipCanvas = new GameObject();
            tooltipCanvas.name = "Tooltip";
            tooltipCanvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            tooltipCanvas.AddComponent<Canvas>();

            panelGameObject = new GameObject();
            panelGameObject.name = "Panel";
            panelGameObject.transform.SetParent(tooltipCanvas.GetComponent<Transform>(), true);
            panelGameObject.AddComponent<RectTransform>();

            panelGameObject.transform.localPosition = new Vector3(0, 0, 0);
            panelGameObject.transform.localScale = new Vector3(1, 1, 1);
            panelGameObject.AddComponent<CanvasRenderer>();
            Image panelBg = panelGameObject.AddComponent<Image>();
            panelBg.sprite = Resources.Load<Sprite>("Sprites/square_rounded_less");
            panelBg.color = new Color32(0, 37, 0, 255);

            tooltipText = new GameObject();
            tooltipText.name = "Text";
            tooltipText.transform.parent = tooltipCanvas.transform;
            tooltipText.transform.localPosition = new Vector3(0, 0, 0);
            tooltipText.transform.localScale = new Vector3(1, 1, 1);
    
            Text t = tooltipText.AddComponent<Text>();
            t.alignment = TextAnchor.MiddleCenter;
            t.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            tooltipText.AddComponent<WorldNewsTransform>();
        }

        Text textComponent = tooltipText.GetComponent<Text>();
        textComponent.text = gameObject.name.Contains("Curve") ? "Unknown" : gameObject.name;

        RectTransform rt = panelGameObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(CalculateLengthOfMessage(textComponent, textComponent.text) + 40, 45);

        tooltipCanvas.SetActive(true);

        tooltipCanvas.transform.SetParent(mainCam.GetComponent<Transform>(), true);
        tooltipCanvas.transform.localPosition = new Vector3(1, 0, 6);
        tooltipCanvas.transform.localEulerAngles = new Vector3(0, 0, 0);
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
        if(tooltipCanvas!=null) tooltipCanvas.SetActive(false);
        if(hashTagCanvases!=null)
            for (int i= 0; i<hashTagCanvases.Length; i++) 
                if (hashTagCanvases[i] != null) hashTagCanvases[i].SetActive(false);
        if (hashTags != null) hashTags.Clear();
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
