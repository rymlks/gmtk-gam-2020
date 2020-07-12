using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {
    public Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void UpdateCursor() {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
        transform.position = canvas.transform.TransformPoint(pos);
    }

    // Update is called once per frame
    void Update() {
        UpdateCursor();
    }
}
