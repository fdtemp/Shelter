using UnityEngine;

public class Script_Meteor : MonoBehaviour {
    public float Damage;
    public float StartTime;

    public void OnTriggerEnter2D(Collider2D coll) {
        switch (coll.gameObject.tag) {
        case "Brick":
            float rest = coll.gameObject.GetComponent<Script_Brick_Destroy>().TryDestroy(Damage);
            if (rest < 0.1f) {
                Destroy(gameObject);
            } else {
                Damage = rest;
            }
            break;
        case "Edge":
            Destroy(gameObject);
            break;
        case "People":
            GameManager.Failed();
            break;
        }
    }
    public void Start() {
        StartTime = GameManager.CurrentTime;
    }
    public void Update() {
        float DeltaTime = GameManager.CurrentTime - StartTime;
        if (DeltaTime > 10) {
            Destroy(gameObject);
        }
    }
}