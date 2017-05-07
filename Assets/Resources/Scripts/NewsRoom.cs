using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewsRoom : MonoBehaviour {

    private string query;

    public static GameObject[] cardList = new GameObject[5];
    private List<string> titleList = new List<string>();
    private List<string> imageUrlList = new List<string>();
    private List<string> providerList = new List<string>();
    private List<string> descList = new List<string>();

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void startRequest(string query) {
        this.query = query;
        StartCoroutine(makeNewsRequest());
    }


    IEnumerator makeNewsRequest() {
        WWW newsRequest = new WWW(Ping.rootUrl + "/news/" + query.Replace(" ","%20") + "?count=5");
        yield return newsRequest; 
        JSONObject newsJson = new JSONObject(newsRequest.text);
        print("Response:" + newsJson);
        if (newsJson.type == JSONObject.Type.ARRAY) {
            foreach (JSONObject newsArticleJson in newsJson.list) {
                titleList.Add(newsArticleJson.GetField("title").str);
                descList.Add(newsArticleJson.GetField("description").str);
                imageUrlList.Add(newsArticleJson.GetField("urlToImage").str);
                providerList.Add(newsArticleJson.GetField("provider").str);
            }
            createCards(newsJson.list.Count);
        }
    }

    IEnumerator getTexture(Image image, string url) {
        WWW www = new WWW(url);
        yield return www;
        image.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }

    void createCards(int count) {
        for (int i=0; i<5; i++) {
            GameObject cardCanvas;
            if (cardList[i]==null) {
                cardCanvas = new GameObject();
                cardCanvas.name = "Card";
                cardCanvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                cardCanvas.transform.position = new Vector3(0, 1, 4);
                cardCanvas.transform.RotateAround(new Vector3(0, 1, 0), Vector3.up, (i - 2) * 60);
                cardCanvas.AddComponent<Canvas>();

                GameObject newsImage = new GameObject();
                newsImage.name = "Image";
                newsImage.transform.SetParent(cardCanvas.GetComponent<Transform>(), true);
                RectTransform rtImage = newsImage.AddComponent<RectTransform>();
                rtImage.sizeDelta = new Vector2(230, 150);
                newsImage.transform.localPosition = new Vector3(0, 100, 0);
                newsImage.transform.localScale = new Vector3(1, 1, 1);
                Image newsImageComponent = newsImage.AddComponent<Image>();

                GameObject cardPanel = new GameObject();
                cardPanel.name = "Panel";
                cardPanel.transform.SetParent(cardCanvas.GetComponent<Transform>(), true);
                RectTransform rt = cardPanel.AddComponent<RectTransform>();
                rt.sizeDelta = new Vector2(300, 400);

                cardPanel.transform.localPosition = new Vector3(0, 0, 0);
                cardPanel.transform.localScale = new Vector3(1, 1, 1);
                cardPanel.AddComponent<CanvasRenderer>();
                Image panelBg = cardPanel.AddComponent<Image>();
                panelBg.sprite = Resources.Load<Sprite>("Sprites/square_rounded_minimal");
                panelBg.color = new Color32(0, 0, 0, 200);

                GameObject titleText = new GameObject();
                titleText.name = "Title";
                titleText.transform.parent = cardCanvas.transform;
                titleText.transform.localPosition = new Vector3(0, -20, 0);
                titleText.transform.localScale = new Vector3(1, 1, 1);
                RectTransform rtTitle = titleText.AddComponent<RectTransform>();
                rtTitle.sizeDelta = new Vector2(250, 100);

                Text titleTextComponent = titleText.AddComponent<Text>();
                titleTextComponent.alignment = TextAnchor.MiddleCenter;
                titleTextComponent.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                titleTextComponent.fontSize = 20;

                GameObject providerText = new GameObject();
                providerText.name = "Provider";
                providerText.transform.parent = cardCanvas.transform;
                providerText.transform.localPosition = new Vector3(0, -70, 0);
                providerText.transform.localScale = new Vector3(1, 1, 1);
                RectTransform rtProvider = providerText.AddComponent<RectTransform>();
                rtProvider.sizeDelta = new Vector2(250, 100);

                Text providerTextComponent = providerText.AddComponent<Text>();
                providerTextComponent.alignment = TextAnchor.MiddleCenter;
                providerTextComponent.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                providerTextComponent.fontSize = 12;

                GameObject descText = new GameObject();
                descText.name = "Desc";
                descText.transform.parent = cardCanvas.transform;
                descText.transform.localPosition = new Vector3(0, -130, 0);
                descText.transform.localScale = new Vector3(1, 1, 1);
                RectTransform rtDesc = descText.AddComponent<RectTransform>();
                rtDesc.sizeDelta = new Vector2(250, 100);

                Text descTextComponent = descText.AddComponent<Text>();
                descTextComponent.alignment = TextAnchor.MiddleCenter;
                descTextComponent.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                descTextComponent.fontSize = 16;

                cardCanvas.SetActive(true);
                cardCanvas.transform.localEulerAngles = new Vector3(0, ((i - 2) % 2 != 0) ? (i - 2) * 120 : (i - 2) * 60 * -1, 0);
                cardList[i] = cardCanvas;

            }

            cardCanvas = cardList[i];
            if (i >= count) cardCanvas.SetActive(false);

            Image newsImageComponent1 = cardCanvas.GetComponentInChildren<Image>();
            StartCoroutine(getTexture(newsImageComponent1, imageUrlList[i]));

            Text[] textComponents = cardCanvas.GetComponentsInChildren<Text>();
            textComponents[0].text = titleList[i];  //titleTextComponent.text = titleList[i];
            textComponents[1].text = providerList[i]; //providerTextComponent.text = providerList[i];
            textComponents[2].text = descList[i]; //descTextComponent.text = descList[i];


        }
    }
}
