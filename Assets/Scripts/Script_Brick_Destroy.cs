using UnityEngine;

public class Script_Brick_Destroy : MonoBehaviour {
    public bool Normal;
    public BrickInfo Info;
    public float CurrentStrength;

    public void OnTriggerEnter2D(Collider2D coll) {
        switch (coll.gameObject.tag) {
        case "People":
            GameManager.Failed();
            break;
        }
    }
    public void Start() { Normal = true; }
    public float TryDestroy(float Damage) {
        if (Damage < CurrentStrength + 0.1f) {
            CurrentStrength -= Damage;
            if (Normal && CurrentStrength < (GameManager.Kinds[Info.Kind].StrengthPerUnit * Info.Width) / 2) {
                gameObject.transform.Find("Left").gameObject.GetComponent<SpriteRenderer>().sprite =
                    GameObject.Instantiate<Sprite>(GameManager.Kinds[Info.Kind].BrokenSide);
                gameObject.transform.Find("Mid").gameObject.GetComponent<SpriteRenderer>().sprite =
                    GameObject.Instantiate<Sprite>(GameManager.Kinds[Info.Kind].BrokenBody);
                gameObject.transform.Find("Right").gameObject.GetComponent<SpriteRenderer>().sprite =
                    GameObject.Instantiate<Sprite>(GameManager.Kinds[Info.Kind].BrokenSide);
                Normal = false;
            }
            return 0;
        } else {
            Destroy(gameObject);
            GameManager.UnregistBrick(Info.ID);
            return Damage - CurrentStrength;
        }
    }
}