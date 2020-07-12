using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientSky : MonoBehaviour {
    public Gradient gradient;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;

    void Start() {
        gradient = new Gradient();

        // What's the color at the relative time 0.25 (25 %) ?
        Debug.Log(gradient.Evaluate(0.25f));
    }
}
