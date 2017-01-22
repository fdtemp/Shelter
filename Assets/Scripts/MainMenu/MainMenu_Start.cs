using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Start : MonoBehaviour {
    private bool Overlapped = false;
    private bool MouseDown = false;
    private bool GameStart = false;
    private float StartTime;
    private float Speed;
    private SpriteRenderer srend;
    private Sprite Normal, Click;

    public void OnMouseEnter() {
        Overlapped = true;
    }
    public void OnMouseDown() {
        if (Overlapped) {
            MouseDown = true;
            srend.sprite = Click;
        }
    }
    public void OnMouseUp() {
        if (Overlapped && MouseDown) {
            GameStart = true;
            StartTime = Time.time;
            Speed = 4f;
        }
    }
    public void OnMouseExit() {
        Overlapped = false;
        MouseDown = false;
        if (!GameStart) {
            srend.sprite = Normal;
        }
    }
    public void Start() {
        srend = gameObject.GetComponent<SpriteRenderer>();
        Normal = srend.sprite;
        Click = Resources.Load<Sprite>("mainmenu_start_2");
    }
    public void Update() {
        if (GameStart) {
            if (Time.time - StartTime < 2f) {
                transform.position+=new Vector3(0, Speed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, (Time.time - StartTime) * 90f);
                Speed -= 16f * Time.deltaTime;
            } else {
                SceneManager.LoadScene("Game");
            }
        }
    }
}