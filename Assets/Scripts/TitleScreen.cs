using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    public void ContinueToScene() {
        StartCoroutine(LoadLevel());
    }

    public void LoadControlsScreen() {
        StartCoroutine(LoadControlsScreenAsync());
    }

    public IEnumerator LoadLevel() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(GameObject.Find("GameManager").GetComponent<PersistentData>().level);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {

            yield return null;
        }
    }

    public IEnumerator LoadControlsScreenAsync() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Controls Screen");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {

            yield return null;
        }
    }

    public void OpenLink(string link) {
        Application.OpenURL(link);
    }
}
