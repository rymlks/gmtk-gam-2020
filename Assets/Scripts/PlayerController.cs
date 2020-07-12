using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public string nextLevelString;

    public AudioSource jump;
    public AudioSource floating;
    public AudioSource slam;
    public AudioSource skull;
    public AudioSource smash;
    public AudioSource died;

    public Canvas canvas;
    public GameObject manaBar;
    public GameObject gemMenuPanel;
    public GameObject fadeToBlackPanel;
    public float runSpeed;
    public float manaDecay;
    public float maxMana;

    public float jumpHeight;
    public float floatManaCost;

    public Transform groundCheckTop_Left;
    public Transform groundCheckBottomRight;
    public LayerMask groundLayers;

    private Rigidbody2D rigidBody;
    private bool grounded = true;
    private bool prevPWM = false;

    private bool noUpdate = false;

    private float mana;
    private List<string> disabledKeys;

    private Dictionary<string, GameObject> UIButtons;
    private Dictionary<string, System.Action> restoreFunctions;

    private float fadeFrameCount = 0.0f;
    private float fadeFramesTotal = 60.0f;
    private bool fadingOut = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        mana = maxMana;
        gemMenuPanel.SetActive(false);
        disabledKeys = new List<string>();
        UIButtons = new Dictionary<string, GameObject>();
        restoreFunctions = new Dictionary<string, System.Action>();
        void Noop() { }

        foreach (Transform UIChild in gemMenuPanel.transform) {
            UIButtons[UIChild.name.ToLower()] = UIChild.gameObject;
            restoreFunctions[UIChild.name.ToLower()] = Noop;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        canvas.gameObject.SetActive(true);

        GameObject.Find("GameManager").GetComponent<PersistentData>().level = SceneManager.GetActiveScene().name;
    }

    void FixedUpdate() {
        if (!fadingOut) {
            fadeToBlackPanel.GetComponent<Image>().color = new Color(0, 0, 0, (fadeFramesTotal - Mathf.Min(fadeFrameCount, fadeFramesTotal)) / fadeFramesTotal);
            fadeFrameCount++;
        } else {
            fadeToBlackPanel.GetComponent<Image>().color = new Color(0, 0, 0, (fadeFramesTotal - Mathf.Max(fadeFrameCount, 0)) / fadeFramesTotal);
            fadeFrameCount--;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (disabledKeys.Find(x => x.Equals(collision.tag.ToLower())) != null) {
            ReclaimKey(collision.tag);
            Destroy(collision.gameObject);
        } else if (collision.CompareTag("Skull")) {
            Destroy(collision.gameObject);
            mana -= 10;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("EndZone") && !noUpdate) {
            NextLevel();
        }
    }

        // Update is called once per frame
    void Update() {
        grounded = Physics2D.OverlapArea(groundCheckTop_Left.position, groundCheckBottomRight.position, groundLayers);
        if (GetKeyPress(KeyCode.Escape)) {
            Quit();
        }
        if (noUpdate) {
            return;
        }

        mana -= manaDecay * Time.deltaTime / (disabledKeys.Count + 1);

        if (mana <= 0 || transform.position.y < -1.0f) {
            GameOver();
            manaBar.transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
        } else {
            if (GetKeyPress(KeyCode.Space)) {
                Jump();
            }

            if (GetKeyPress(KeyCode.E)) {
                ToggleMenu();
            }

            if (GetKeyDown(KeyCode.W)) {
                Float();
            } else {
                floating.Stop();
            }

            if (GetKeyPress(KeyCode.S)) {
                Slam();
            }

            if (GetKeyDown(KeyCode.D)) {
                MoveForward();
            }

            if (GetKeyDown(KeyCode.A)) {
                MoveBackward();
            }

            manaBar.transform.localScale = new Vector3(mana / maxMana, 1.0f, 1.0f);
        }
    }

    void MoveForward() {
        rigidBody.velocity = new Vector2(runSpeed, rigidBody.velocity.y);
    }

    void MoveBackward() {
        rigidBody.velocity = new Vector2(-runSpeed, rigidBody.velocity.y);
    }

    void Jump() {
        if (grounded) {
            jump.Play();
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpHeight);
        }
    }

    void Slam() {
        slam.Play();
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, -jumpHeight);
    }

    void Float() {
        if (!floating.isPlaying) {
            floating.Play();
        }
        mana -= floatManaCost;
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 3.0f);
    }

    void ToggleMenu() {
        gemMenuPanel.SetActive(!gemMenuPanel.activeSelf);
    }

    bool GetKeyPress(KeyCode key) {
        if (disabledKeys.Find(x => x.Equals(key.ToString().ToLower())) != null) {
            return DisabledKeyEffect(key);
        }
        return Input.GetKeyDown(key);
    }

    bool GetKeyDown(KeyCode key) {
        if (disabledKeys.Find(x => x.Equals(key.ToString().ToLower())) != null) {
            return DisabledKeyEffect(key);
        }
        return Input.GetKey(key);
    }

    public void SpendKey(string key) {
        key = key.ToLower();
        disabledKeys.Add(key);

        smash.pitch = Random.Range(1.0f, 1.2f);
        smash.Play();

        switch(key) {
            case "w":
                AddMana(50);
                float gscale = rigidBody.gravityScale;
                void WRestore() {
                    rigidBody.gravityScale = gscale;
                }
                restoreFunctions[key] = WRestore;
                break;
            case "a":
                AddMana(20);
                break;
            case "s":
                AddMana(20);
                break;
            case "d":
                float prevSpeed = runSpeed;
                void DRestore() {
                    runSpeed = prevSpeed;
                }
                restoreFunctions[key] = DRestore;
                AddMana(20);
                break;
            case "e":
                AddMana(20);
                break;
            case "space":
                float prevJumpHeight = jumpHeight;
                void SpaceRestore() {
                    jumpHeight = prevJumpHeight;
                }
                restoreFunctions[key] = SpaceRestore;
                AddMana(20);
                break;
            case "mouse":
                void MouseRestore() {
                    Cursor.lockState = CursorLockMode.None;
                }
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                restoreFunctions[key] = MouseRestore;
                AddMana(100);
                break;
            case "escape":
                AddMana(100);
                break;
            default:
                AddMana(20);
                break;
        }
    }

    public void ReclaimKey(string key) {
        key = key.ToLower();
        disabledKeys.Remove(key);
        UIButtons[key].GetComponent<DisableOnClick>().ReEnable();
        restoreFunctions[key]();
    }

    bool DisabledKeyEffect(KeyCode key) {
        bool inCycle;
        switch (key) {
            case KeyCode.W:
                rigidBody.gravityScale = 0.5f/disabledKeys.Count;
                return false;
            case KeyCode.A:
                return (int)Time.time % disabledKeys.Count == 0;
            case KeyCode.S:
                inCycle = (int)(Time.time * 2) % (disabledKeys.Count * 2) == 0;
                if (!prevPWM) {
                    prevPWM = inCycle;
                    return inCycle;
                }
                prevPWM = inCycle;
                return false;
            case KeyCode.D:
                runSpeed = disabledKeys.Count + 2;
                return true;
            case KeyCode.E:
                inCycle = (int)(Time.time * 2) % (disabledKeys.Count * 2) == 0;
                if (!prevPWM) {
                    prevPWM = inCycle;
                    return inCycle;
                }
                prevPWM = inCycle;
                return false;
            case KeyCode.Space:
                jumpHeight = disabledKeys.Count + 2;
                return true;
            case KeyCode.Escape:
                return Random.value < 0.001f;
            default:
                return false;
        }
    }

    void AddMana(float amount) {
        mana = Mathf.Clamp(mana + amount, 0.0f, maxMana);
    }

    void GameOver() {
        Debug.Log("YOU LOSE");
        noUpdate = true;
        died.Play();
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().name));
    }

    void Quit() {
        StartCoroutine(LoadLevelImmediate("Main Screen"));
    }

    void NextLevel() {
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel(string level) {
        noUpdate = true;
        fadingOut = true;
        fadeFrameCount = fadeFramesTotal;
        yield return new WaitForSeconds(1);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {

            yield return null;
        }
    }

    IEnumerator LoadLevel() {
        noUpdate = true;
        fadingOut = true;
        fadeFrameCount = fadeFramesTotal;
        yield return new WaitForSeconds(1);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextLevelString);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {

            yield return null;
        }
    }

    IEnumerator LoadLevelImmediate(string level) {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {

            yield return null;
        }
    }
}
