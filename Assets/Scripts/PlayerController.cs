using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public GameObject manaBar;
    public GameObject gemMenuPanel;
    public float runSpeed;
    public float maxMana;

    private Rigidbody2D rigidBody;
    private float mana;
    private Vector3 previousPosition;
    private List<string> disabledKeys;

    private bool toJump = false;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        mana = maxMana;
        previousPosition = transform.position;
        gemMenuPanel.SetActive(false);
        disabledKeys = new List<string>();
    }

    // Update is called once per frame
    void Update() {
        if (GetKeyPress(KeyCode.Space)) {
            Jump();
        }

        if (GetKeyPress(KeyCode.E)) {
            ToggleMenu();
        }
        //rigidBody.MovePosition(new Vector2(transform.position.x + runSpeed * Time.deltaTime, transform.position.y));

        mana -= (transform.position.x - previousPosition.x);

        if (mana <= 0) {
            GameOver();
            manaBar.transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
        } else {
            rigidBody.velocity = new Vector2(runSpeed, rigidBody.velocity.y);
            manaBar.transform.localScale = new Vector3(mana / maxMana, 1.0f, 1.0f);
        }
        previousPosition = transform.position;
    }

    void Jump() {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 3);
    }

    void ToggleMenu() {
        gemMenuPanel.SetActive(!gemMenuPanel.activeSelf);
    }

    bool GetKeyPress(KeyCode key) {
        if (disabledKeys.Find(x => x == key.ToString()) != null) {
            return false;
        }
        return Input.GetKeyDown(key);
    }

    void AddMana(float amount) {
        mana = Mathf.Clamp(mana + amount, 0.0f, maxMana);
    }

    void GameOver() {
        Debug.Log("YOU LOSE");
    }

    public void SpendKey(string key) {
        disabledKeys.Add(key);

        switch(key) {
            case "W":
                AddMana(20);
                break;
            case "A":
                AddMana(20);
                break;
            case "S":
                AddMana(20);
                break;
            case "D":
                AddMana(20);
                break;
            case "E":
                AddMana(20);
                break;
            case "Space":
                AddMana(100);
                break;
        }
    }
}
