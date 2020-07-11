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
    public float floatManaCost;
    public PhysicsMaterial2D bouncyMaterial;

    public Transform groundCheckTop_Left;
    public Transform groundCheckBottomRight;
    public LayerMask groundLayers;

    private Rigidbody2D rigidBody;
    private bool grounded = true;
    private bool prevPWM = false;

    private float mana;
    private List<string> disabledKeys;

    private Dictionary<string, GameObject> UIButtons;
    private Dictionary<string, System.Action> restoreFunctions;

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
        if (grounded)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpHeight);
    }

    void Slam() {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, -jumpHeight);
    }

    void Float() {
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

        switch(key) {
            case "w":
                AddMana(20);
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
                PhysicsMaterial2D prevMat = rigidBody.sharedMaterial;
                void SRestore() {
                    rigidBody.sharedMaterial = prevMat;
                }
                rigidBody.sharedMaterial = bouncyMaterial;
                restoreFunctions[key] = SRestore;
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
        switch (key) {
            case KeyCode.W:
                rigidBody.gravityScale = 0.5f/disabledKeys.Count;
                return false;
            case KeyCode.A:
                return (int)Time.time % disabledKeys.Count == 0;
            case KeyCode.S:
                rigidBody.sharedMaterial.bounciness = disabledKeys.Count;
                bool inCycle = (int)(Time.time * 2) % (disabledKeys.Count * 2) == 0;
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
                return false;
            case KeyCode.Space:
                jumpHeight = disabledKeys.Count;
                return true;
            default:
                return false;
        }
    }

    void AddMana(float amount) {
        mana = Mathf.Clamp(mana + amount, 0.0f, maxMana);
    }

    void GameOver() {
        Debug.Log("YOU LOSE");
    }
}
