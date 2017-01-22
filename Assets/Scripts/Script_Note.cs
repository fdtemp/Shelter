using UnityEngine;

public class Script_Note : MonoBehaviour {
    public int BrickKind;

    private float[] Delta = new float[3] { 0, -1f, -1.5f, };
    private Vector3 BasePos;
    private int Target;
    private bool Overlapped = false;
    private bool MouseDown = false;
    private bool Chose = false;
    public void OnMouseEnter() {
        Overlapped = true;
        GameManager.IgnoreMouse = true;
    }
    public void OnMouseDown() {
        if (Overlapped) {
            MouseDown = true;
        }
    }
    public void OnMouseUp() {
        if (Overlapped && MouseDown) {
            GameManager.CurrentKind = BrickKind;
        }
    }
    public void OnMouseExit() {
        Overlapped = false;
        MouseDown = false;
        GameManager.IgnoreMouse = false;
    }
    public void Start() {
        BasePos = transform.position;
    }
    public void Update() {
        if (GameManager.GameMode == GameManager.GAMEMODE_DESTROY) {
            Target = 0;
        } else {
            if (GameManager.CurrentKind == BrickKind || Overlapped) {
                Target = 2;
            } else {
                Target = 1;
            }
        }
        Vector3
            CurrentPos = transform.position,
            TargetPos = BasePos + new Vector3(Delta[Target], 0, 0);
        float len = 4f * Mathf.Min(Time.deltaTime,0.1f);
        if (Mathf.Abs(TargetPos.x - CurrentPos.x) < len) {
            transform.position = TargetPos;
        } else {
            if (TargetPos.x > CurrentPos.x) {
                transform.Translate(len, 0, 0);
            } else {
                transform.Translate(-len, 0, 0);
            }
        }
    }
}