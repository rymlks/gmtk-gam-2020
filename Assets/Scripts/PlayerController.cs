using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public GameObject manaBar;
    public GameObject gemMenuPanel;
    public float runSpeed;
    public float manaDecay;
    public float maxMana;

    public float jumpHeight;

    public Transform groundCheckTop_Left;
    public Transform groundCheckBottomRight;
    public LayerMask groundLayers;

    private Rigidbody2D rigidBody;
    private bool grounded = true;

    private float mana;
    private List<string> disabledKeys;

    private Dictionary<string, GameObject> UIButtons;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        mana = maxMana;
        gemMenuPanel.SetActive(false);
        disabledKeys = new List<string>();
        UIButtons = new Dictionary<string, GameObject>();

        foreach(Transform UIChild in gemMenuPanel.transform) {
            UIButtons[UIChild.name.ToLower()] = UIChild.gameObject;
        }
    }

    void FixedUpdate() {
        grounded = Physics2D.OverlapArea(groundCheckTop_Left.position, groundCheckBottomRight.position, groundLayers);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (disabledKeys.Find(x => x.Equals(collision.tag.ToLower())) != null) {
            ReclaimKey(collision.tag);
            Destroy(collision.gameObject);
        }
    }

    // Update is called once per frame
    void Update() {

        mana -= manaDecay * Time.deltaTime;

        if (mana <= 0) {
            GameOver();
            manaBar.transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
        } else {
            if (GetKeyPress(KeyCode.Space)) {
                Jump();
            }

            if (GetKeyPress(KeyCode.E)) {
                ToggleMenu();
            }

            if (GetKeyDown(KeyCode.D)) {
                rigidBody.velocity = new Vector2(runSpeed, rigidBody.velocity.y);
            }

            if (GetKeyDown(KeyCode.A)) {
                rigidBody.velocity = new Vector2(-runSpeed, rigidBody.velocity.y);
            }

            manaBar.transform.localScale = new Vector3(mana / maxMana, 1.0f, 1.0f);
        }
    }

    void Jump() {
        if (grounded)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpHeight);
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

        switch(key) {
            case "w":
                AddMana(20);
                break;
            case "a":
                AddMana(20);
                break;
            case "s":
                AddMana(20);
                break;
            case "d":
                AddMana(20);
                break;
            case "e":
                AddMana(20);
                break;
            case "space":
                AddMana(100);
                break;
        }
    }

    public void ReclaimKey(string key) {
        key = key.ToLower();
        disabledKeys.Remove(key);
        UIButtons[key].GetComponent<DisableOnClick>().ReEnable();
    }

    void AddMana(float amount) {
        mana = Mathf.Clamp(mana + amount, 0.0f, maxMana);
    }

    bool DisabledKeyEffect(KeyCode key) {
        switch (key) {
            case KeyCode.W:
                return false;
            case KeyCode.A:
                return (int)Time.time % disabledKeys.Count == 0;
            case KeyCode.S:
                return false;
            case KeyCode.D:
                runSpeed = disabledKeys.Count;
                return true;
            case KeyCode.E:
                return false;
            case KeyCode.Space:
                jumpHeight = disabledKeys.Count;
                return true;
            default:
                return false;
        }
    }

    void GameOver() {
        Debug.Log("YOU LOSE");
    }
}
