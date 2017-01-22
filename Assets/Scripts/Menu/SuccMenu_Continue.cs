using UnityEngine;
using UnityEngine.SceneManagement;

public class SuccMenu_Continue : MonoBehaviour {
    private bool Overlapped = false;
    private bool MouseDown = false;
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
            Time.timeScale = 1;
            GameManager.CurrentLevel++;
            SceneManager.LoadScene("Game");
        }
    }
    public void OnMouseExit() {
        Overlapped = false;
        MouseDown = false;
        srend.sprite = Normal;
    }
    public void Start() {
        srend = gameObject.GetComponent<SpriteRenderer>();
        Normal = srend.sprite;
        Click = Resources.Load<Sprite>("succmenu_continue_2");
    }
}