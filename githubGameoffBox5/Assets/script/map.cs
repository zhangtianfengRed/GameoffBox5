using Assets.script;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class map : MonoBehaviour {
    public int width;
    public int high;
    public GameObject Ground;
    public GameObject BoxGroup;
    public GameObject TargetGroup;
    MapData mapData;
    BoxLevel[,] LevelData;
    Action action; 
    MoveAction moveAction;

    private BoxLevel boxLevel;

    // Use this for initialization
    void Start () {
        mapData = new MapData(Ground)
        {
            width = width,
            high = high,
        };
        mapData.Startmap();
        LevelData = new BoxLevel[width, high];
        StartBoxlevel(LevelData);
        ForeachBox(BoxGroup);
        ForeachBox(TargetGroup);
        moveAction = new MoveAction(LevelData, _play(LevelData));
        action = new Action(moveAction);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.W))
        {
            Invoking invoking = new Invoking(new MoveUp(action, _play(LevelData)));
            invoking.Exction();
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            Invoking invoking = new Invoking(new MoveLeft(action, _play(LevelData)));
            invoking.Exction();
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            Invoking invoking = new Invoking(new Moveright(action, _play(LevelData)));
            invoking.Exction();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            Invoking invoking = new Invoking(new Movedown(action,_play(LevelData)));
            invoking.Exction();
        }
    }
    void ForeachBox(GameObject T)
    {
        foreach (Transform i in T.GetComponentInChildren<Transform>())
        {
            if (i != null)
            {
                BoxTag(i.gameObject);
            }
        }
    }


    void BoxTag(GameObject T)
    {
        Vector2Int vector2Int = new Vector2Int((int)T.transform.position.x, (int)T.transform.position.y);
        switch (T.tag)
        {
            case ("box"):
                
                 LevelData[vector2Int.x, vector2Int.y].Box = T;             
                break;
            case ("target"):
               
                LevelData[vector2Int.x, vector2Int.y].Target = T;
                break;
            case ("play"):
                
                LevelData[vector2Int.x, vector2Int.y].play = T;
                break;
        }
    }
    public BoxLevel _play(BoxLevel[,] boxLevel)
    {
        foreach (BoxLevel i in boxLevel)
        {
            if (i != null && i.play != null)
            {
                return i;
            }
        }
        throw new Exception("不存在play对象");
    }

    public void StartBoxlevel(BoxLevel[,] boxLevel)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < high; y++)
            {
                boxLevel[x, y] = new BoxLevel();
            }
        }
    }

    public class BoxLevel : LevelData
    {
    }
    public class MoveAction
    {
        public BoxLevel[,] _map { get; set; }
        public BoxLevel _play { get; set; }

        public MoveAction(BoxLevel[,] _map,BoxLevel _box)
        {
            this._map = _map;
            this._play = _box;
        }
        
        enum moveAxis
        {
            x,
            y,
        }

        bool add(Vector2Int vector2Int,moveAxis axis)
        {
            switch (axis)
            {
                case (moveAxis.x):
                    if (vector2Int.x + 1<_map.GetLength(0)&& _map[vector2Int.x + 1, vector2Int.y].Box == null)
                    {
                        return true;
                    }
                    break;
                case (moveAxis.y):
                    if (vector2Int.y + 1<_map.GetLength(1)&&_map[vector2Int.x, vector2Int.y + 1].Box == null)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }
        bool mun(Vector2Int vector2Int, moveAxis axis)
        {
            switch (axis)
            {
                case (moveAxis.x):
                    if (vector2Int.x - 1>-1&&_map[vector2Int.x - 1, vector2Int.y].Box == null)
                    {
                        return true;
                    }
                    break;
                case (moveAxis.y):
                    if (vector2Int.y - 1>-1&&_map[vector2Int.x, vector2Int.y - 1].Box == null)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        public void moveup(BoxLevel _play)
        {
            if (add(vector2int(_play.play.transform.position), moveAxis.y)&& 
                vector2int(_play.play.transform.position).y< _map.GetLength(1))
            {
                Vector2Int T = vector2int(_play.play.transform.position);
                _map[T.x, T.y + 1].play = _play.play;
                _play.play.transform.position = new Vector3Int(T.x, T.y + 1, -1);
                _map[T.x, T.y].play = null;

            }
        }

        public void movedown(BoxLevel _play)
        {
            if (mun(vector2int(_play.play.transform.position), moveAxis.y)&&
                vector2int(_play.play.transform.position).y-1>-1)
            {
                Vector2Int T = vector2int(_play.play.transform.position);
                _map[T.x, T.y - 1].play = _play.play;
                _play.play.transform.position = new Vector3Int(T.x, T.y - 1,-1);
                _map[T.x, T.y].play = null;
            }
        }
        public void moveleft(BoxLevel _play)
        {
            if (mun(vector2int(_play.play.transform.position), moveAxis.x) &&
                vector2int(_play.play.transform.position).x-1 > -1)
            {
                Vector2Int T = vector2int(_play.play.transform.position);
                _map[T.x - 1, T.y].play = _play.play;
                _play.play.transform.position = new Vector3Int(T.x - 1, T.y, -1);
                _map[T.x, T.y].play = null;
            }
        }
        public void moveright(BoxLevel _play)
        {
            if (add(vector2int(_play.play.transform.position), moveAxis.x) &&
                vector2int(_play.play.transform.position).x <_map.GetLength(0))
            {
                Vector2Int T = vector2int(_play.play.transform.position);
                _map[T.x + 1, T.y].play = _play.play;
                _play.play.transform.position = new Vector3Int(T.x + 1, T.y, -1);
                _map[T.x, T.y].play = null;
            }
        }
        Vector2Int vector2int(Vector3 vector3)
        {
            return new Vector2Int((int)vector3.x, (int)vector3.y);
        }       

    }

    //调用者
    public class Invoking
    {
        public command command;
        public Invoking(command command)
        {
            this.command = command;
        }
        public void Exction()
        {
            command.move();
        }
    }

    //命令
    public abstract class command
    {
        public Action action;
        public BoxLevel Box= new BoxLevel();
        public command(Action action, BoxLevel box)
        {
            this.action = action;
            Box = box;
        }

        public virtual void move() { }
    }
    //具体行为
    public class Action
    {
        public MoveAction moveAction { get; set; }
        public void Up(BoxLevel box)
        {
            moveAction.moveup(box);
        }
        public void Down(BoxLevel box)
        {
            moveAction.movedown(box);
        }
        public void Left(BoxLevel box)
        {
            moveAction.moveleft(box);
        }
        public void right(BoxLevel box)
        {
            moveAction.moveright(box);
        }

        public Action(MoveAction moveAction)
        {
            this.moveAction = moveAction;
        }
    }

    #region 行为类化
    public class MoveUp:command
    {
        public MoveUp(Action action,BoxLevel box) : base(action, box)
        {
        }
        public override void move()
        {
            action.Up(Box);
        }
    }
    public class Movedown : command
    {
        public Movedown(Action action, BoxLevel box) : base(action, box)
        {
        }
        public override void move()
        {
            action.Down(Box);
        }
    }
    public class MoveLeft : command
    {
        public MoveLeft(Action action, BoxLevel box) : base(action, box)
        {
        }
        public override void move()
        {
            action.Left(Box);
        }
    }
    public class Moveright : command
    {
        public Moveright(Action action, BoxLevel box) : base(action, box)
        {
        }
        public override void move()
        {
            action.right(Box);
        }
    }
    #endregion
}
