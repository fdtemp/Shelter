using UnityEngine;

public abstract class Catastrophe {
    public abstract void Update();

    public class Rockfall : Catastrophe {
        public class RockInfo {
            public float Weight;
            public float Gravity;
            public float Speed;
            public float Damage;
            public GameObject Prefab;
        }

        private float StartTime, Interval, AmountPerRound, Y, XRangeStart,
            XRangeEnd, AngleRangeStart, AngleRangeEnd;
        private int Rounds;
        private RockInfo[] Rocks;
        private int LastRound = -1;

        public Rockfall(float startTime, float interval, int rounds, float amountPerRound,
            float y, float xRangeStart, float xRangeEnd, float angleRangeStart, float angleRangeEnd,
            RockInfo[] rocks) {
            StartTime = startTime;
            Interval = interval;
            Rounds = rounds;
            AmountPerRound = amountPerRound;
            Y = y;
            XRangeStart = xRangeStart;
            XRangeEnd = xRangeEnd;
            AngleRangeStart = angleRangeStart;
            AngleRangeEnd = angleRangeEnd;
            Rocks = rocks;
        }
        public override void Update() {
            float time = GameManager.CurrentTime;
            if (time < StartTime
                || (time > StartTime + Rounds * Interval && LastRound == Rounds - 1))
                return;
            int CurrentRound = Mathf.FloorToInt((GameManager.CurrentTime - StartTime) / Interval);
            while (LastRound < CurrentRound) {
                for (int i = 0; i < AmountPerRound; i++)
                    GenerateRock(
                        new Vector2(Random.Range(XRangeStart, XRangeEnd), Y),
                        Random.Range(AngleRangeStart, AngleRangeEnd),
                        Random.Range(0, Rocks.Length)
                    );
                LastRound++;
                if (LastRound == 0) GameManager.AddWorkToGod();
                if (LastRound == Rounds - 1) GameManager.RemoveWorkForGod();
            }
        }
        private void GenerateRock(Vector2 Position, float AngleAdjust, int Kind) {
            RockInfo info = Rocks[Kind];
            float theta = (AngleAdjust / 180) * Mathf.PI;
            Vector2 Direction = new Vector2(Mathf.Sin(theta), -Mathf.Cos(theta));
            GameObject go = GameObject.Instantiate<GameObject>(info.Prefab);
            go.transform.localPosition = Position;
            Rigidbody2D rb2d = go.GetComponent<Rigidbody2D>();
            rb2d.mass = info.Weight;
            rb2d.gravityScale = info.Gravity;
            rb2d.velocity = Direction * info.Speed;
            rb2d.angularVelocity = 90;
            Script_Rock sr = go.GetComponent<Script_Rock>();
            sr.Damage = info.Damage;
        }
    }
    public class MeteorShower : Catastrophe {
        public class MeteorInfo {
            public float Speed;
            public float Damage;
            public GameObject Prefab;
        }

        private float StartTime, Interval, AmountPerRound, Y, XRangeStart,
            XRangeEnd, AngleRangeStart, AngleRangeEnd;
        private int Rounds;
        private MeteorInfo[] Meteors;
        private int LastRound = -1;

        public MeteorShower(float startTime, float interval, int rounds, float amountPerRound,
            float y, float xRangeStart, float xRangeEnd, float angleRangeStart, float angleRangeEnd,
            MeteorInfo[] meteors) {
            StartTime = startTime;
            Interval = interval;
            Rounds = rounds;
            AmountPerRound = amountPerRound;
            Y = y;
            XRangeStart = xRangeStart;
            XRangeEnd = xRangeEnd;
            AngleRangeStart = angleRangeStart;
            AngleRangeEnd = angleRangeEnd;
            Meteors = meteors;
        }
        public override void Update() {
            float time = GameManager.CurrentTime;
            if (time < StartTime
                || (time > StartTime + Rounds * Interval && LastRound == Rounds - 1))
                return;
            int CurrentRound = Mathf.FloorToInt((GameManager.CurrentTime - StartTime) / Interval);
            while (LastRound < CurrentRound) {
                for (int i = 0; i < AmountPerRound; i++)
                    GenerateMeteor(
                        new Vector2(Random.Range(XRangeStart, XRangeEnd), Y),
                        Random.Range(AngleRangeStart, AngleRangeEnd),
                        Random.Range(0, Meteors.Length)
                    );
                LastRound++;
                if (LastRound == 0) GameManager.AddWorkToGod();
                if (LastRound == Rounds - 1) GameManager.RemoveWorkForGod();
            }
        }
        private void GenerateMeteor(Vector2 Position, float AngleAdjust, int Kind) {
            MeteorInfo info = Meteors[Kind];
            float theta = (AngleAdjust / 180) * Mathf.PI;
            Vector2 Direction = new Vector2(Mathf.Sin(theta), -Mathf.Cos(theta));
            GameObject go = GameObject.Instantiate<GameObject>(info.Prefab);
            go.transform.localRotation = Quaternion.Euler(0, 0, AngleAdjust);
            go.transform.localPosition = Position;
            Rigidbody2D rb2d = go.GetComponent<Rigidbody2D>();
            rb2d.velocity = Direction * info.Speed;
            Script_Meteor sr = go.GetComponent<Script_Meteor>();
            sr.Damage = info.Damage;
        }
    }
    public class Flood : Catastrophe {
        public class WaterInfo {
            public float Speed;
            public float Damage;
            public float Force;
            public float TimeScale;
            public float Amplitude;
            public GameObject Prefab;
        }

        private float StartTime, Interval, X, Y;
        private int Rounds;
        private WaterInfo[] Waters;
        private int LastRound = -1;
        private bool TurnLeft;

        public Flood(float startTime, float interval, int rounds,
            float x, float y, bool turnLeft, WaterInfo[] waters) {
            float time = GameManager.CurrentTime;
            StartTime = startTime;
            Interval = interval;
            Rounds = rounds;
            X = x;
            Y = y;
            TurnLeft = turnLeft;
            Waters = waters;
        }
        public override void Update() {
            float time = GameManager.CurrentTime;
            if (time < StartTime
                || (time > StartTime + Rounds * Interval && LastRound == Rounds - 1))
                return;
            int CurrentRound = Mathf.FloorToInt((GameManager.CurrentTime - StartTime) / Interval);
            while (LastRound < CurrentRound) {
                GenerateWater(
                    new Vector2(X, Y),
                    Random.Range(0, 2 * Mathf.PI),
                    Random.Range(0, Waters.Length),
                    TurnLeft
                );
                LastRound++;
                if (LastRound == 0) GameManager.AddWorkToGod();
                if (LastRound == Rounds - 1) GameManager.RemoveWorkForGod();
            }
        }
        private void GenerateWater(Vector2 Position, float Offset, int Kind, bool TurnLeft) {
            WaterInfo info = Waters[Kind];
            GameObject go = GameObject.Instantiate<GameObject>(info.Prefab);
            go.transform.localPosition = Position;
            if (TurnLeft) {
                Transform ch = go.transform.Find("water");
                Vector2 s = ch.localScale;
                s.x = -s.x;
                ch.localScale = s;
            }
            Rigidbody2D rb2d = go.GetComponent<Rigidbody2D>();
            if (TurnLeft) {
                rb2d.velocity = new Vector2(-info.Speed, 0);
                CapsuleCollider2D cc2d = go.GetComponent<CapsuleCollider2D>();
                Vector2 o = cc2d.offset;
                o.x = -o.x;
                cc2d.offset = o;
            } else {
                rb2d.velocity = new Vector2(info.Speed, 0);
            }
            
            Script_Water sw = go.GetComponent<Script_Water>();
            sw.Height = Position.y;
            sw.Damage = info.Damage;
            sw.Offset = Offset;
            sw.TimeScale = info.TimeScale;
            sw.Amplitude = info.Amplitude;
            if (TurnLeft) {
                sw.Force = Vector2.left * info.Force;
            } else {
                sw.Force = Vector2.right * info.Force;
            }
        }
    }
}