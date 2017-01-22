using UnityEngine;

public class Script_Water : MonoBehaviour {
    public float Height;
    public float TimeScale;
    public float Offset;
    public float Amplitude;
    public float Damage;
    public Vector2 Force;

    private float StartTime;
    private bool Shot;
    private float ShotTime;
    private Rigidbody2D rb2d;

    public void OnTriggerEnter2D(Collider2D coll) {
        switch (coll.gameObject.tag) {
        case "Brick":
            coll.gameObject.GetComponent<Rigidbody2D>().AddForce(Force);
            float rest = coll.gameObject.GetComponent<Script_Brick_Destroy>().TryDestroy(Damage);
            if (rest < 0.1f) {
                Shot = true;
                ShotTime = GameManager.CurrentTime;
            } else {
                Damage = rest;
            }
            break;
        case "People":
            GameManager.Failed();
            break;
        }
    }
    public void Start() {
        Shot = false;
        StartTime = GameManager.CurrentTime;
        rb2d = gameObject.GetComponent<Rigidbody2D>();
    }
    public void Update() {
        if (GameManager.CurrentTime - StartTime > 20f) {
            Destroy(gameObject);
        } else {
            if (Shot) {
                transform.Translate(0, -2f * Time.deltaTime, 0);
            } else {
                Vector2 pos = transform.localPosition;
                pos.y = Height + Mathf.Sin(GameManager.CurrentTime * TimeScale + Offset) * Amplitude;
                transform.localPosition = pos;
            }
        }
    }
}