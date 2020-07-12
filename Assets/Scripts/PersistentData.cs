using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentData : MonoBehaviour
{
    public string level;
    // Start is called before the first frame update
    void Awake()
    {
        if (GameObject.Find(gameObject.name) != gameObject) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
