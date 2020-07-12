using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseController : MonoBehaviour {
    public Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void UpdateCursor() {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
        transform.position = canvas.transform.TransformPoint(pos) + new Vector3(GetComponent<Image>().sprite.rect.width*0.5f, -GetComponent<Image>().sprite.rect.height *0.5f, 0.0f);
    }

    // Update is called once per frame
    void Update() {
        UpdateCursor();
    }
}
