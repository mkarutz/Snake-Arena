using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetDefaultScrollBar : MonoBehaviour {

    public ScrollRect scrollRect;

    // Use this for initialization
    void Awake () {
    }

    void Update()
    {
    }

    void OnEnable()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1.0f;
        Canvas.ForceUpdateCanvases();
        scrollRect.velocity = new Vector2(0f, -1000f);
    }
}
