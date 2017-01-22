using UnityEngine;

public class BrickInfo {
    public int ID;
    public int Kind;
    public Vector2 StartPos;
    public Vector2 Direction;
    public float Length;
    public float Width;
    public GameObject Entity;

    public void CreateEntity() {
        Entity = new GameObject("Brick");
        Entity.tag = "Brick";
        GameObject
            left = new GameObject("Left"),
            mid = new GameObject("Mid"),
            right = new GameObject("Right");

        left.AddComponent<SpriteRenderer>().sprite =
            GameObject.Instantiate<Sprite>(GameManager.Kinds[Kind].NormalSide);
        mid.AddComponent<SpriteRenderer>().sprite =
            GameObject.Instantiate<Sprite>(GameManager.Kinds[Kind].NormalBody);
        right.AddComponent<SpriteRenderer>().sprite =
            GameObject.Instantiate<Sprite>(GameManager.Kinds[Kind].NormalSide);

        left.transform.parent = Entity.transform;
        mid.transform.parent = Entity.transform;
        right.transform.parent = Entity.transform;
        float len = Length / Mathf.Abs(Width);
        mid.transform.localScale = new Vector3((len - 2) / 3, 1);
        right.transform.localScale = new Vector3(-1, 1, 1);
        left.transform.localPosition = Vector3.zero;
        mid.transform.localPosition = new Vector3(1, 0);
        right.transform.localPosition = new Vector3(len, 0);

        BoxCollider2D bc2d = Entity.AddComponent<BoxCollider2D>();
        bc2d.isTrigger = true;
        bc2d.offset = new Vector2(len / 2, 0.5f);
        bc2d.size = new Vector2(len, 1);
        Rigidbody2D rb2d = Entity.AddComponent<Rigidbody2D>();
        rb2d.gravityScale = 0;
        Entity.transform.localPosition = new Vector3(StartPos.x, StartPos.y, -1);
        Entity.transform.localScale = new Vector3(Mathf.Abs(Width), Width);
        Entity.transform.localRotation = Quaternion.FromToRotation(Vector2.right, Direction);
        Script_Brick_Build sbb = Entity.AddComponent<Script_Brick_Build>();
        sbb.ID = ID;
    }
    public void ApplyLength() {
        GameObject
            mid = Entity.transform.Find("Mid").gameObject,
            right = Entity.transform.Find("Right").gameObject;
        float len = Length / Mathf.Abs(Width);
        mid.transform.localScale = new Vector3((len - 2) / 3, 1);
        right.transform.localPosition = new Vector3(len, 0);
        BoxCollider2D bc2d = Entity.GetComponent<BoxCollider2D>();
        bc2d.offset = new Vector2(len / 2, 0.5f);
        bc2d.size = new Vector2(len, 1);
    }
    public void ApplyDirection() {
        Entity.transform.localRotation = Quaternion.FromToRotation(Vector2.right, Direction);
    }
    public void ApplyWidth() {
        GameObject
            left = Entity.transform.Find("Left").gameObject,
            mid = Entity.transform.Find("Mid").gameObject,
            right = Entity.transform.Find("Right").gameObject;
        float len = Length / Mathf.Abs(Width);
        mid.transform.localScale = new Vector3((len - 2) / 3, 1);
        right.transform.localPosition = new Vector3(len, 0);
        Entity.transform.localScale = new Vector3(Mathf.Abs(Width), Width);
        BoxCollider2D bc2d = Entity.GetComponent<BoxCollider2D>();
        bc2d.offset = new Vector2(len / 2, 0.5f);
        bc2d.size = new Vector2(len, 1);
    }
    public void SwitchForDestroy() {
        Entity.transform.Find("Left").gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        Entity.transform.Find("Mid").gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        Entity.transform.Find("Right").gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        float
            volume = Length * Mathf.Abs(Width),
            weight = volume * GameManager.Kinds[Kind].WeightPerUnit,
            strength = Mathf.Abs(Width) * GameManager.Kinds[Kind].StrengthPerUnit,
            gold = volume * GameManager.Kinds[Kind].GoldPerUnit;
        Rigidbody2D rb2d = Entity.GetComponent<Rigidbody2D>();
        rb2d.gravityScale = GameManager.Kinds[Kind].Gravity;
        rb2d.angularDrag = 0.02f;
        rb2d.mass = weight;
        BoxCollider2D bc2d = Entity.GetComponent<BoxCollider2D>();
        bc2d.isTrigger = false;
        GameObject.Destroy(Entity.GetComponent<Script_Brick_Build>());
        Script_Brick_Destroy sbd = Entity.AddComponent<Script_Brick_Destroy>();
        sbd.CurrentStrength = strength;
        sbd.Info = this;
    }
}