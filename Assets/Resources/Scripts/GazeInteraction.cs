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
    public static GameObject focusedCountry;

    public VRViewer vrViewer;
    public Camera mainCam;

    public static GameObject tooltipCanvas;
    public static GameObject tooltipText;
    public static GameObject toolTipPanel;

    //private static bool fetched = false;
    public const int maxHashTags = 20;
    private static GameObject[] hashTagCanvases;
    private static List<string> hashTags = new List<string>();
    private static List<int> hashHits = new List<int>();

    private HashTagBounds countryAvoidHashTagBounds = new HashTagBounds(0, 1, -0.2f, 0.2f);
    private List<HashTagBounds> hashTagBoundsList = new List<HashTagBounds>();

    private class HashTagBounds {
        public float startX;
        public float startY;
        public float endX;
        public float endY;
        public HashTagBounds(float startX,
                                float endX,
                                float startY,
                                float endY) {
            this.startX = startX;
            this.startY = startY;
            this.endX = endX;
            this.endY = endY;
        }

        public static bool isWithinBounds(HashTagBounds current, HashTagBounds old) {
            if (current.startX >= old.startX && current.startX <= old.endX) return false;
            else if (current.startY >= old.startY && current.startY <= old.endY) return false;
            else if (current.endX >= old.startX && current.endX <= old.endX) return false;
            else if (current.endY >= old.startY && current.endY <= old.endY) return false;
            return true;
        }

        public static bool isPerfectToBePlaced(HashTagBounds hashTagBounds, List<HashTagBounds> hashTagBoundsList) {
            foreach (HashTagBounds hashTagBoundsIter in hashTagBoundsList)
                if (!HashTagBounds.isWithinBounds(hashTagBounds, hashTagBoundsIter))
                    return false;
            return true;
        }

        override public string ToString() {
            return this.startX + " " + this.endX + " " + this.startY + " " + this.endY;
        }
    }

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
        Loading.loading = true;
        focusedCountry = gameObject;
        StartCoroutine(getHashTagsForCountry(gameObject.name));
        ShowToolTip();
    }

    private IEnumerator getHashTagsForCountry(string country) {
        WWW hashTagRequest = new WWW(Ping.rootUrl + "/trends/" + country + "?count=20");
        yield return hashTagRequest;
        Loading.loading = false;
        JSONObject countryJson = new JSONObject(hashTagRequest.text);
        if (countryJson.type == JSONObject.Type.ARRAY && focusedCountry == gameObject) {
            hashTags.Clear(); hashHits.Clear();
            foreach (JSONObject trendJson in countryJson.list) {
                if(hashTags.Count<maxHashTags-1) hashTags.Add(trendJson.GetField("name").str);
                if(hashHits.Count<maxHashTags-1) hashHits.Add((int)trendJson.GetField("tweet_volume").n);
            }
            ShowHashTags();
        }
    }

    void ShowHashTags() {
        if(hashTagCanvases==null) hashTagCanvases = new GameObject[maxHashTags];
        hashTagBoundsList.Clear();
        hashTagBoundsList.Add(countryAvoidHashTagBounds);
        for (int i=0; i< hashTags.Count; i++) {
            GameObject hashTagCanvas;
            GameObject hashTagText;
            if (hashTagCanvases[i]==null) {
                hashTagCanvas = new GameObject();
                hashTagCanvas.name = "HashTag Tooltip";
                hashTagCanvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                hashTagCanvas.AddComponent<Canvas>();

                GameObject hashTagPanel = new GameObject();
                hashTagPanel.name = "Panel";
                hashTagPanel.transform.SetParent(hashTagCanvas.GetComponent<Transform>(), true);

                hashTagPanel.transform.localPosition = new Vector3(0, 0, 0);
                hashTagPanel.transform.localScale = new Vector3(1, 1, 1);
                hashTagPanel.AddComponent<CanvasRenderer>();
                Image panelBg = hashTagPanel.AddComponent<Image>();
                panelBg.sprite = Resources.Load<Sprite>("Sprites/square_rounded_less");
                panelBg.color = new Color32(0, 0, 0, 93);

                hashTagText = new GameObject();
                hashTagText.name = "Text";
                hashTagText.transform.parent = hashTagCanvas.transform;
                hashTagText.transform.localPosition = new Vector3(0, 0, 0);
                hashTagText.transform.localScale = new Vector3(1, 1, 1);
                Text t = hashTagText.AddComponent<Text>();
                t.alignment = TextAnchor.MiddleCenter;
                t.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

                hashTagCanvases[i] = hashTagCanvas;
            }

            hashTagCanvas = hashTagCanvases[i];
            hashTagCanvas.transform.SetParent(mainCam.GetComponent<Transform>(), true);

            Text hashTagTextComponent = hashTagCanvas.GetComponentInChildren<Text>().GetComponent<Text>();
            hashTagTextComponent.text = hashTags[i];

            RectTransform rt = hashTagCanvas.GetComponentInChildren<CanvasRenderer>().GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(CalculateLengthOfMessage(hashTagTextComponent, hashTagTextComponent.text) + 40, 45);

            HashTagBounds hashTagBounds = null;
            do {
                Random.InitState(System.DateTime.Now.Millisecond - Random.Range(10001, 99999));
                float positionX = Random.Range(-3f, 3f);
                Random.InitState(System.DateTime.Now.Millisecond - Random.Range(10001, 99999));
                float positionY = Random.Range(-3f, 3f);

                float finalPositionX = positionX + 0.1f;
                float finalPositionY = positionY + 0.2f;
                hashTagBounds = new HashTagBounds(positionX, finalPositionX, positionY, finalPositionY);
            } while (!HashTagBounds.isPerfectToBePlaced(hashTagBounds, hashTagBoundsList));

            hashTagBoundsList.Add(hashTagBounds);
            hashTagCanvas.transform.localPosition = new Vector3(hashTagBounds.startX, hashTagBounds.startY, 6);
            hashTagCanvas.transform.localEulerAngles = new Vector3(0, 0, 0);

            hashTagCanvas.SetActive(true);
            if (i == 0) HashTag.selectedHashTag = hashTagCanvas;
        }
        HashTag.hashTagCanvases = hashTagCanvases;
        HashTag.normalizeSelection();
    }

    void ShowToolTip() {
        if (tooltipCanvas == null) {
            tooltipCanvas = new GameObject();
            tooltipCanvas.name = "Tooltip";
            tooltipCanvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            tooltipCanvas.AddComponent<Canvas>();

            toolTipPanel = new GameObject();
            toolTipPanel.name = "Panel";
            toolTipPanel.transform.SetParent(tooltipCanvas.GetComponent<Transform>(), true);
            toolTipPanel.AddComponent<RectTransform>();

            toolTipPanel.transform.localPosition = new Vector3(0, 0, 0);
            toolTipPanel.transform.localScale = new Vector3(1, 1, 1);
            toolTipPanel.AddComponent<CanvasRenderer>();
            Image panelBg = toolTipPanel.AddComponent<Image>();
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

        RectTransform rt = toolTipPanel.GetComponent<RectTransform>();
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
        focusedCountry = null;
        if (tooltipCanvas!=null) tooltipCanvas.SetActive(false);
        if(hashTagCanvases!=null)
            for (int i= 0; i<hashTagCanvases.Length; i++) 
                if (hashTagCanvases[i] != null) hashTagCanvases[i].SetActive(false);
        if (hashTags != null) hashTags.Clear();
        if (hashHits != null) hashHits.Clear();
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
