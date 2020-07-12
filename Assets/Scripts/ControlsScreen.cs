using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlsScreen : MonoBehaviour
{
    public GameObject[] panels;
    public GameObject nextButton;
    private int panel = 0;
    public Sprite broken;

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
            nextButton.GetComponent<Image>().sprite = broken;
            nextButton.GetComponent<Button>().enabled = false;
            nextButton.GetComponentInChildren<Text>().enabled = false;
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
