using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsScreen : MonoBehaviour
{
    public GameObject[] panels;
    public GameObject nextButton;
    private int panel = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject obj in panels) {
            obj.SetActive(false);
        }

        panels[0].SetActive(true);
    }

    public void Next() {
        panel++;
        panels[panel - 1].SetActive(false);
        panels[panel].SetActive(true);

        if (panel >= panels.Length - 1) {
            nextButton.SetActive(false);
        }
    }

    // Update is called once per frame
    public void Back()
    {
        StartCoroutine(LoadLevel());
    }

    public IEnumerator LoadLevel() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Screen");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {

            yield return null;
        }
    }
}
