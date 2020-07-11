using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableOnClick : MonoBehaviour
{
    public Sprite disabledSprite;
    private Button buttonComponent;
    private void Start() {
        buttonComponent = GetComponent<Button>();
        buttonComponent.onClick.AddListener(TaskOnClick);
    }
    // Start is called before the first frame update
    void TaskOnClick() {
        buttonComponent.enabled = false;

        GetComponent<Image>().color = new Color(1, 1, 1, 0.75f);
        if (disabledSprite != null)
            GetComponent<Image>().sprite = disabledSprite;
    }
}