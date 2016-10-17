using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIGenerateSkinShopItems : MonoBehaviour {

    public GameObject shopItemPrefab;
    public float yOffset;
    public float ySpacing;

    private int numSnakeSkins = GameConfig.NUM_SNAKE_SKINS;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < this.numSnakeSkins; i++)
        {
            CreateSnakeSkinItem(i, yOffset - (i * ySpacing));
        }
        this.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(this.GetComponent<RectTransform>().sizeDelta.x, -(yOffset - (this.numSnakeSkins * ySpacing)));

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private void CreateSnakeSkinItem(int snakeSkinID, float yOffset)
    {
        GameObject item = Instantiate<GameObject>(shopItemPrefab);
        item.GetComponentInChildren<SnakeState>().snakeSkinID = snakeSkinID;
        RectTransform itemRectTransform = item.GetComponent<RectTransform>();
        itemRectTransform.SetParent(this.GetComponent<RectTransform>(), false);
        itemRectTransform.anchoredPosition = new Vector3(0.0f, yOffset, 0.0f);
        itemRectTransform.localScale = shopItemPrefab.transform.localScale;
    }
}
