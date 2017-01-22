using UnityEngine;

public class Script_Brick_Build : MonoBehaviour {
    public int ID;
    private SpriteRenderer Left, Mid, Right;
    private int CollideCounter = 0;
    public bool Destroy;
    public float DestroyTime;
    public bool Collided {
        get {
            return CollideCounter > 0;
        }
    }
    public void OnMouseOver() {
        if (Input.GetMouseButton(1)) {
            GameManager.DestroyBrickSafely(gameObject);
            GameManager.UnregistBrick(ID);
        }
    }
    public void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == "Brick" || coll.gameObject.tag == "Edge" || coll.gameObject.tag == "People")
            CollideCounter++;
    }
    public void OnTriggerExit2D(Collider2D coll) {
        if (coll.gameObject.tag == "Brick" || coll.gameObject.tag == "Edge" || coll.gameObject.tag == "People")
            CollideCounter--;
    }
    public void Start() {
        Destroy = false;
        Left = transform.Find("Left").gameObject.GetComponent<SpriteRenderer>();
        Mid = transform.Find("Mid").gameObject.GetComponent<SpriteRenderer>();
        Right = transform.Find("Right").gameObject.GetComponent<SpriteRenderer>();
    }
    public void Update() {
        Color col = Left.color;
        if (Collided) {
            col.a = 0.5f;
        } else {
            col.a = 1f;
        }
        Left.color = Mid.color = Right.color = col;
        if (Destroy && Time.time - DestroyTime > 3f)
            Destroy(gameObject);
    }
}