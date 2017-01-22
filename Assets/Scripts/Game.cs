using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public void Start() {
        GameManager.Start();
    }
    public void Update() {
        GameManager.Update();
    }
}

public class BrickKind {
    public Sprite
        NormalSide,
        NormalBody,
        BrokenSide,
        BrokenBody;
    public float WeightPerUnit;
    public float StrengthPerUnit;
    public float GoldPerUnit;
    public float Gravity;
}
public static class GameManager {
    public const int GAMEMODE_WAIT = 0;
    public const int GAMEMODE_BUILD = 1;
    public const int GAMEMODE_BUILD_WAIT = 10;
    public const int GAMEMODE_BUILD_LENGTH = 11;
    public const int GAMEMODE_BUILD_WIDTH = 12;
    public const int GAMEMODE_DESTROY = 2;
    public const int KIND_ICE = 0;
    public const int KIND_WOOD = 1;
    public const int KIND_STONE = 2;
    public static BrickKind[] Kinds = new BrickKind[3] {
        new BrickKind {//ice
            NormalSide = Resources.Load<Sprite>("ice_normal_side"),
            NormalBody = Resources.Load<Sprite>("ice_normal_body"),
            BrokenSide = Resources.Load<Sprite>("ice_normal_side"),
            BrokenBody = Resources.Load<Sprite>("ice_normal_body"),
            WeightPerUnit = 3,
            StrengthPerUnit = 1,//test
            GoldPerUnit = 0.5f,
            Gravity = 1,
        },
        new BrickKind {//wood
            NormalSide = Resources.Load<Sprite>("wood_normal_side"),
            NormalBody = Resources.Load<Sprite>("wood_normal_body"),
            BrokenSide = Resources.Load<Sprite>("wood_normal_side"),
            BrokenBody = Resources.Load<Sprite>("wood_normal_body"),
            WeightPerUnit = 1,
            StrengthPerUnit = 2,
            GoldPerUnit = 2,
            Gravity = 0.5f,
        },
        new BrickKind {//stone
            NormalSide = Resources.Load<Sprite>("stone_normal_side"),
            NormalBody = Resources.Load<Sprite>("stone_normal_body"),
            BrokenSide = Resources.Load<Sprite>("stone_normal_side"),
            BrokenBody = Resources.Load<Sprite>("stone_normal_body"),
            WeightPerUnit = 10,
            StrengthPerUnit = 5,
            GoldPerUnit = 10,
            Gravity = 1.5f,
        },
    };
    public static Catastrophe.Rockfall.RockInfo[] Rocks = new Catastrophe.Rockfall.RockInfo[] {
        new Catastrophe.Rockfall.RockInfo {
            Weight = 100,
            Gravity = 2,
            Speed = 2f,
            Damage = 0.2f,
            Prefab = Resources.Load<GameObject>("Rock1"),
        },
        new Catastrophe.Rockfall.RockInfo {
            Weight = 50,
            Gravity = 2,
            Speed = 1f,
            Damage = 0.1f,
            Prefab = Resources.Load<GameObject>("Rock2"),
        },
        new Catastrophe.Rockfall.RockInfo {
            Weight = 50,
            Gravity = 2,
            Speed = 1f,
            Damage = 0.1f,
            Prefab = Resources.Load<GameObject>("Rock3"),
        },
    };
    public static Catastrophe.MeteorShower.MeteorInfo[] Meteors = new Catastrophe.MeteorShower.MeteorInfo[] {
        new Catastrophe.MeteorShower.MeteorInfo {
            Speed = 12,
            Damage = 0.5f,
            Prefab = Resources.Load<GameObject>("Meteor1"),
        },
        new Catastrophe.MeteorShower.MeteorInfo {
            Speed = 12,
            Damage = 0.5f,
            Prefab = Resources.Load<GameObject>("Meteor2"),
        },
    };
    public static Catastrophe.Flood.WaterInfo[] Waters = new Catastrophe.Flood.WaterInfo[] {
        new Catastrophe.Flood.WaterInfo {
            Speed = 1,
            Damage = 0.3f,
            Force = 1000,
            TimeScale = Mathf.PI * 1.5f,
            Amplitude = 0.1f,
            Prefab = Resources.Load<GameObject>("Water1"),
        },
    };
    public const float MinLength = 1f;
    public const float MinWidth = 0.25f;// 宽度最多是长度的二分之一
    public static Vector2 ScreenOrigin = new Vector2(0, 0);
    public static Vector2 ScreenSize = new Vector2(16, 12);

    public static int CurrentLevel = 1;
    public static Catastrophe[] Catastrophes;
    public static float Gold;
    public static float Volume;
    public static float SuccTime;

    //public static Dictionary<string, byte[]> ImageDataDictionary;
    public static Dictionary<int, BrickInfo> Bricks;
    public static int GameMode;

    public static int BuildMode;
    public static int CurrentKind;
    public static BrickInfo CurrentBrick;
    public static bool IgnoreMouse;

    private static GameObject FailMenu, SuccMenu;
    private static TextMesh GoldText, VolumeText;
    private static SpriteRenderer GodRenderer;
    private static Sprite GodWaiting, GodWorking;
    private static int GodWork;

    private static float StartTime;
    private static int Index;

    public static float CurrentTime {
        get {
            return Time.time - StartTime;
        }
    }
    public static int GetID() {
        return Index++;
    }
    public static Vector2 TransformToWorldPos(Vector3 ScreenPos) {
        return new Vector2(
            ScreenOrigin.x + (ScreenPos.x / Screen.width) * ScreenSize.x,
            ScreenOrigin.y + (ScreenPos.y / Screen.height) * ScreenSize.y
        );
    }

    public static bool BrickCostIsEnough(int Kind, float Length, float Width) {
        float v = Length * Width;
        return Volume > v && Gold > Kinds[Kind].GoldPerUnit * v;
    }
    public static void RegistBrick(BrickInfo info) {
        float v = info.Length * Mathf.Abs(info.Width);
        Volume -= v;
        Gold -= Kinds[info.Kind].GoldPerUnit * v;
        BrickInfo b = new BrickInfo {
            ID = info.ID,
            Kind = info.Kind,
            StartPos = info.StartPos,
            Direction = info.Direction,
            Length = info.Length,
            Width = info.Width,
            Entity = info.Entity,
        };
        Bricks[b.ID] = b;
    }
    public static void UnregistBrick(int id) {
        if (Bricks.ContainsKey(id)) {
            BrickInfo info = Bricks[id];
            float v = info.Length * Mathf.Abs(info.Width);
            Volume += v;
            Gold += Kinds[info.Kind].GoldPerUnit * v;
            Bricks.Remove(id);
        }
    }
    public static void DestroyBrickSafely(GameObject Brick) {
        Script_Brick_Build sbb = Brick.GetComponent<Script_Brick_Build>();
        sbb.Destroy = true;
        sbb.DestroyTime = Time.time;
        Brick.transform.position = new Vector3(-100, -100);
    }

    public static void StartDestroy() {
        GameMode = GAMEMODE_DESTROY;
        StartTime = Time.time;
        GameObject.Destroy(CurrentBrick.Entity);
        foreach(var p in Bricks) {
            BrickInfo info = p.Value;
            info.SwitchForDestroy();
        }
    }
    public static void AddWorkToGod() {
        if (++GodWork == 1) {
            GodRenderer.sprite = GodWorking;
        }
    }
    public static void RemoveWorkForGod() {
        if (--GodWork == 0) {
            GodRenderer.sprite = GodWaiting;
        }
    }
    public static void Failed() {
        Time.timeScale = 0;
        GameMode = GAMEMODE_WAIT;
        GameObject.Instantiate<GameObject>(FailMenu);
    }
    public static void Success() {
        Time.timeScale = 0;
        GameMode = GAMEMODE_WAIT;
        GameObject.Instantiate<GameObject>(SuccMenu);
    }
    public static void RefreshCounter(float value, TextMesh TextMesh) {
        int ceil = Mathf.CeilToInt(value);
        if (ceil - value < 0.1f) {
            TextMesh.text = "  " + ceil.ToString();
        } else {
            TextMesh.text = "< " + ceil.ToString();
        }
    }

    public static void Start() {
        //ImageDataDictionary = new Dictionary<string, byte[]>();
        Bricks = new Dictionary<int, BrickInfo>();
        GameMode = GAMEMODE_BUILD;
        BuildMode = GAMEMODE_BUILD_WAIT;
        CurrentKind = KIND_ICE;
        CurrentBrick = new BrickInfo();
        FailMenu = Resources.Load<GameObject>("FailMenu");
        SuccMenu = Resources.Load<GameObject>("SuccMenu");
        GoldText = GameObject.Find("GoldText").GetComponent<TextMesh>();
        VolumeText = GameObject.Find("VolumeText").GetComponent<TextMesh>();
        GodRenderer = GameObject.Find("God").GetComponent<SpriteRenderer>();
        GodWaiting = GodRenderer.sprite;
        GodWorking = Resources.Load<Sprite>("god_2");
        StartTime = Time.time;
        Index = 0;

        switch (CurrentLevel) {
        case 1:
            Catastrophes = new Catastrophe[] {
                new Catastrophe.Rockfall(5,1,10,2,13,-1,17,-45,45,Rocks),
                new Catastrophe.Flood(5,1.3f,7,18,1.8f,true,Waters),
                new Catastrophe.Flood(5,1.3f,7,-2,1.8f,false,Waters),
                new Catastrophe.MeteorShower(5,1,10,2,13,-1,17,-45,45,Meteors),
            };
            Gold = 999.99f;
            Volume = 999.99f;
            SuccTime = 20;
            break;
        case 2:
            Catastrophes = new Catastrophe[] {
                new Catastrophe.Rockfall(5,2.5f,4,6,13,-1,17,-45,45,Rocks),
                new Catastrophe.Flood(15,1.3f,7,18,1.8f,true,Waters),
                new Catastrophe.Flood(15,1.3f,7,-2,1.8f,false,Waters),
                new Catastrophe.MeteorShower(25,2.5f,4,6,13,-1,17,-45,45,Meteors),

                new Catastrophe.Rockfall(35,1,10,2,13,-1,17,-45,45,Rocks),
                new Catastrophe.Flood(35,1.3f,7,18,1.8f,true,Waters),
                new Catastrophe.Flood(35,1.3f,7,-2,1.8f,false,Waters),
                new Catastrophe.MeteorShower(35,1,10,2,13,-1,17,-45,45,Meteors),
            };
            Gold = 99.99f;
            Volume = 49.99f;
            SuccTime = 50;
            break;
        }
        
    }
    public static void Update() {
        Vector2 MousePos = TransformToWorldPos(Input.mousePosition);
        RefreshCounter(Gold,GoldText);
        RefreshCounter(Volume, VolumeText);
        switch (GameMode) {
        case GAMEMODE_BUILD:
            if (CurrentBrick.Entity == null) {
                BuildMode = GAMEMODE_BUILD_WAIT;
            } else if (BuildMode != GAMEMODE_BUILD_WAIT && Input.GetMouseButtonDown(1)) {
                DestroyBrickSafely(CurrentBrick.Entity);
                CurrentBrick.Entity = null;
                BuildMode = GAMEMODE_BUILD_WAIT;
            }
            switch (BuildMode) {
            case GAMEMODE_BUILD_WAIT:
                if (Input.GetMouseButtonDown(0) && !IgnoreMouse && BrickCostIsEnough(CurrentKind,MinLength,MinWidth)) {
                    CurrentBrick.ID = GetID();
                    CurrentBrick.Kind = CurrentKind;
                    CurrentBrick.StartPos = MousePos;
                    CurrentBrick.Direction = Vector2.up;
                    CurrentBrick.Length = MinLength;
                    CurrentBrick.Width = MinWidth;
                    CurrentBrick.CreateEntity();
                    BuildMode = GAMEMODE_BUILD_LENGTH;
                }
                break;
            case GAMEMODE_BUILD_LENGTH:
                if (Input.GetMouseButtonDown(0) && !IgnoreMouse) {
                    BuildMode = GAMEMODE_BUILD_WIDTH;
                } else {
                    Vector2 direction = MousePos - CurrentBrick.StartPos;
                    float length = Mathf.Max(direction.magnitude, MinLength);
                    if (BrickCostIsEnough(CurrentBrick.Kind, length, CurrentBrick.Width)) {
                        direction.Normalize();
                        CurrentBrick.Direction = direction;
                        CurrentBrick.Length = length;
                        CurrentBrick.ApplyLength();
                        CurrentBrick.ApplyDirection();
                    }
                }
                break;
            case GAMEMODE_BUILD_WIDTH:
                if (Input.GetMouseButtonDown(0) && !IgnoreMouse && !CurrentBrick.Entity.GetComponent<Script_Brick_Build>().Collided) {
                    RegistBrick(CurrentBrick);
                    CurrentBrick.Entity = null;
                    BuildMode = GAMEMODE_BUILD_WAIT;
                } else {
                    Vector2
                        a = MousePos - CurrentBrick.StartPos,
                        b = CurrentBrick.Direction * CurrentBrick.Length,
                        c = Vector3.Project(a, b);
                    //投影法求点线距离
                    float width = Mathf.Clamp(
                        Mathf.Sqrt(Mathf.Pow(a.magnitude, 2) - Mathf.Pow(c.magnitude, 2)),
                        MinWidth, CurrentBrick.Length / 2
                    );
                    if (BrickCostIsEnough(CurrentBrick.Kind, CurrentBrick.Length, width)) {
                        //叉乘判断左右侧
                        if (a.x * b.y - b.x * a.y > 0) {
                            CurrentBrick.Width = -width;
                        } else {
                            CurrentBrick.Width = width;
                        }
                        CurrentBrick.ApplyWidth();
                    }
                }
                break;
            }
            break;
        case GAMEMODE_DESTROY:
            if (CurrentTime > SuccTime)
                Success();
            for (int i = 0; i < Catastrophes.Length; i++)
                Catastrophes[i].Update();
            break;
        }
    }
}