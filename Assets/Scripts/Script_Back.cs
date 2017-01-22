using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_Back : MonoBehaviour {
    private bool Overlapped = false;
    private bool MouseDown = false;
    private SpriteRenderer srend;
    private Sprite Normal, Click;

    public void OnMouseEnter() {
        Overlapped = true;
        GameManager.IgnoreMouse = true;
    }
    public void OnMouseDown() {
        if (Overlapped) {
            MouseDown = true;
            srend.sprite = Click;
        }
    }
    public void OnMouseUp() {
        if (Overlapped && MouseDown) {
            SceneManager.LoadScene("MainMenu");
        }
    }
    public void OnMouseExit() {
        Overlapped = false;
        MouseDown = false;
        srend.sprite = Normal;
        GameManager.IgnoreMouse = false;
    }
    public void Start() {
        srend = gameObject.GetComponent<SpriteRenderer>();
        Normal = srend.sprite;
        Click = Resources.Load<Sprite>("close_2");
    }
}