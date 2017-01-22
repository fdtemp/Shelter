using UnityEngine;

public class Script_Rock: MonoBehaviour {
    public float Damage;
    public float StartTime;

    public void OnTriggerEnter2D(Collider2D coll) {
        switch (coll.gameObject.tag) {
        case "People":
            GameManager.Failed();
            break;
        }
    }
    public void OnCollisionEnter2D(Collision2D coll) {
        switch (coll.gameObject.tag) {
        case "Brick":
            float rest = coll.gameObject.GetComponent<Script_Brick_Destroy>().TryDestroy(Damage);
            if (rest < 0.1f) {
                Destroy(gameObject);
            } else {
                Damage = rest;
            }
            break;
        }
    }
    public void Start() {
        StartTime = GameManager.CurrentTime;
    }
    public void Update() {
        float DeltaTime = GameManager.CurrentTime - StartTime;
        if (DeltaTime > 10) {
            if (DeltaTime < 20) {
                float rate = ((20 - DeltaTime) / 10f) * 0.8f + 0.2f;
                gameObject.transform.localScale = new Vector3(rate, rate);
            } else {
                Destroy(gameObject);
            }
        }
    }
}