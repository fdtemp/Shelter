using UnityEngine;

public class FailMenu_Main : MonoBehaviour {
    private float StartTime;
    private SpriteRenderer srend;
    public void Start() {
        StartTime = Time.realtimeSinceStartup;
        srend = GameObject.Find("Cover").GetComponent<SpriteRenderer>();
    }
    public void Update() {
        srend.color = new Color(
            1, 1, 1,
            Mathf.Clamp01((Time.realtimeSinceStartup - StartTime) / 5f)
        );
    }
}