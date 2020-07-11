using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableOnClick : MonoBehaviour
{
    public Sprite disabledSprite;

    private Button buttonComponent;
    private Sprite enabledSprite;
    private void Start() {
        buttonComponent = GetComponent<Button>();
        buttonComponent.onClick.AddListener(TaskOnClick);
        enabledSprite = GetComponent<Image>().sprite;
    }
    // Start is called before the first frame update
    void TaskOnClick() {
        buttonComponent.enabled = false;

        GetComponent<Image>().color = new Color(1, 1, 1, 0.75f);
        if (disabledSprite != null)
            GetComponent<Image>().sprite = disabledSprite;
    }

    public void ReEnable() {
        GetComponent<Image>().color = new Color(1, 1, 1, 1);
        GetComponent<Image>().sprite = enabledSprite;
        buttonComponent.enabled = true;
    }
}