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
    private State MoveState=State._Box;
    Anim anim;


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
        moveAction = new MoveAction(LevelData);
        
        action = new Action(moveAction);
        anim = new Anim(_play(LevelData).play);
        moveAction.Moveanim = anim;
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
    void FixedUpdate()
    {
        anim.Anim_Move();
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
                Debug.Log(i.play.name);
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
        public Anim Moveanim { get; set; }
        public BoxLevel[,] _map { get; set; }
        public BoxLevel Play { get; set; }

        public MoveAction(BoxLevel[,] _map)
        {
            this._map = _map;
        }
        
        enum moveAxis
        {
            x,
            y,
        }

        Vector2Int add(Vector2Int vector2Int,moveAxis axis)
        {
            switch (axis)
            {
                case (moveAxis.x):
                    if (vector2Int.x<_map.GetLength(0))
                    {
                        return Forward(vector2Int, axis);
                    }
                    break;
                case (moveAxis.y):
                    if (vector2Int.y<_map.GetLength(1))
                    {
                        return Forward(vector2Int, axis);
                    }
                    break;
            }
            return vector2Int;
        }
        Vector2Int mun(Vector2Int vector2Int, moveAxis axis)
        {
            switch (axis)
            {
                case (moveAxis.x):
                    if (vector2Int.x >0)
                    {
                        return Negative(vector2Int, axis);
                    }
                    break;
                case (moveAxis.y):
                    if (vector2Int.y >0)
                    {
                        return Negative(vector2Int, axis);
                    }
                    break;
            }
            return vector2Int;
        }

        //Ghost寻找坐标
        Vector2Int Ghostadd(Vector2Int vector2Int, moveAxis axis)
        {
            switch (axis)
            {
                case (moveAxis.x):
                    if (vector2Int.x + 1 < _map.GetLength(0))
                    {
                        return GhostForward(vector2Int, axis);
                    }
                    break;
                case (moveAxis.y):
                    if (vector2Int.y + 1 < _map.GetLength(1))
                    {
                        return GhostForward(vector2Int, axis);
                    }
                    break;
            }
            return vector2Int;
        }
        Vector2Int Ghostmun(Vector2Int vector2Int, moveAxis axis)
        {
            switch (axis)
            {
                case (moveAxis.x):
                    if (vector2Int.x - 1 > -1 && _map[vector2Int.x - 1, vector2Int.y].Box == null)
                    {
                        return GhostNegative(vector2Int, axis);
                    }
                    break;
                case (moveAxis.y):
                    if (vector2Int.y - 1 > -1 && _map[vector2Int.x, vector2Int.y - 1].Box == null)
                    {
                        return GhostNegative(vector2Int, axis);
                    }
                    break;
            }
            throw new Exception("没有合适的位置");
        }

        public void moveup(BoxLevel _play)
        {
            if(GhostBox()== State._Box)
            {
                if (Floorvector2int(_play.play.transform.position).y < _map.GetLength(1))
                {
                    Vector2Int T = Floorvector2int(_play.play.transform.position);
                    Vector2Int target = add(Floorvector2int(_play.play.transform.position), moveAxis.y);
                    Debug.Log(target);
                    _map[target.x, target.y].play = _play.play;
                    Play = _map[target.x, target.y];                    
                    FloorMove_animtest(target);
                    //_play.play.transform.position = new Vector3Int(target.x, target.y, -1);
                    _map[T.x, T.y].play = null;
                }
            }
            if (GhostBox() == State._Ghost)
            {
                Debug.Log("Error");
            }
        }

        public void movedown(BoxLevel _play)
        {
            if (GhostBox() == State._Box)
            {
                if (Ceilingvector2int(_play.play.transform.position).y - 1 > -1)
                {
                    Vector2Int T = Ceilingvector2int(_play.play.transform.position);
                    Vector2Int target = mun(Ceilingvector2int(_play.play.transform.position), moveAxis.y);
                    _map[target.x, target.y].play = _play.play;
                    Play = _map[target.x, target.y];
                    Debug.Log(target);
                    CeilMove_animtest(target);
                    //_play.play.transform.position = new Vector3Int(target.x,target.y, -1);
                    _map[T.x, T.y].play = null;
                }
            }
            if (GhostBox() == State._Ghost)
            {

            }

        }
        public void moveleft(BoxLevel _play)
        {
            if (GhostBox() == State._Box)
            {
                if (Ceilingvector2int(_play.play.transform.position).x > -1)
                {
                    Vector2Int T = Ceilingvector2int(_play.play.transform.position);
                    Vector2Int target = mun(Ceilingvector2int(_play.play.transform.position), moveAxis.x);
                    _map[target.x, target.y].play = _play.play;
                    Play = _map[target.x, target.y];
                    //_play.play.transform.position = new Vector3Int(target.x, target.y, -1);
                    Debug.Log(target);
                    CeilMove_animtest(target);
                    _map[T.x, T.y].play = null;
                }
            }
            if (GhostBox() == State._Ghost)
            {

            }
        }
        public void moveright(BoxLevel _play)
        {
            if (GhostBox() == State._Box)
            {
                if (Floorvector2int(_play.play.transform.position).x < _map.GetLength(0))
                {
                    Vector2Int T = Floorvector2int(_play.play.transform.position);
                    Vector2Int target = add(Floorvector2int(_play.play.transform.position), moveAxis.x);
                    _map[target.x, target.y].play = _play.play;
                    Play = _map[target.x, target.y];
                    //_play.play.transform.position = new Vector3Int(target.x, target.y, -1);
                    Debug.Log(target);
                    FloorMove_animtest(target);
                    _map[T.x, T.y].play = null;
                }
            }
            if (GhostBox() == State._Ghost)
            {                                
                Vector2Int target = Ghostadd(Floorvector2int(_play.play.transform.position), moveAxis.x);
                GameObject Z = _map[target.x, target.y].Box;

                _map[target.x, target.y].play = Z;
                _map[target.x, target.y].play.tag = "play";
                _map[target.x, target.y].Box = null;


                GameObject V = Play.play;
                Play .Box= V;
                _play.Box.tag = "box";
                Play.play = null;
                Play = _map[target.x, target.y];
            }

        }
        private State GhostBox()
        {
            switch (Moveanim.playState)
            {
                case (Anim.Play._Stay):
                    return State._Box;
                case (Anim.Play._MoveCeil):
                    return State._Ghost;
                case (Anim.Play._MoveFloor):
                    return State._Ghost;
            }
            throw new Exception("返回状态出错！");
        }

        private void CeilMove_animtest(Vector2Int target)
        {
            Moveanim.playobject = _map[target.x, target.y].play;
            Moveanim.Target = new Vector3(target.x, target.y, -1);
            Moveanim.playState = Anim.Play._MoveCeil;
            Moveanim.smoothTime = 0f;
        }
        private void FloorMove_animtest(Vector2Int target)
        {
            Moveanim.playobject = _map[target.x, target.y].play;
            Moveanim.Target = new Vector3(target.x, target.y, -1);
            Moveanim.playState = Anim.Play._MoveFloor;
            Moveanim.smoothTime = 0f;
        }
        //向上取整
        Vector2Int Ceilingvector2int(Vector3 vector3)
        {
            return new Vector2Int(Mathf.CeilToInt( vector3.x), Mathf.CeilToInt(vector3.y));
        }
        //向下取整
        Vector2Int Floorvector2int(Vector3 vector3)
        {
            return new Vector2Int(Mathf.FloorToInt(vector3.x), Mathf.FloorToInt(vector3.y));
        }

        Vector2Int Forward(Vector2Int ghostvector, moveAxis axis)
        {
            switch (axis)
            {
                case (moveAxis.x):

                    for(int i = ghostvector.x + 1; i < _map.GetLength(0); i++)
                    {
                        if (_map[i, ghostvector.y].Box != null)
                        {
                            return new Vector2Int(i-1, ghostvector.y);
                        }
                    }
                    return new Vector2Int(_map.GetLength(0)-1, ghostvector.y);

                case (moveAxis.y):

                    for (int i = ghostvector.y + 1; i < _map.GetLength(1); i++)
                    {
                        if (_map[ghostvector.x, i].Box != null)
                        {
                            return new Vector2Int(ghostvector.x, i-1);
                        }
                    }
                    return new Vector2Int(ghostvector.x, _map.GetLength(1)-1);
            }
            throw new Exception("在计算终点坐标时出现未知错误");
        }
        Vector2Int Negative(Vector2Int ghostvector, moveAxis axis)
        {
            switch (axis)
            {
                case (moveAxis.x):
                    for (int i = ghostvector.x-1; i >-1; i--)
                    {
                        if (_map[i, ghostvector.y].Box != null)
                        {
                            Debug.Log(i + 1);
                            return new Vector2Int(i+1, ghostvector.y);
                        }
                    }
                    return new Vector2Int(0, ghostvector.y);
                case (moveAxis.y):
                    for (int i = ghostvector.y-1; i > -1; i--)
                    {
                        if (_map[ghostvector.x, i].Box != null)
                        {
                            Debug.Log(i + 1);
                            return new Vector2Int(ghostvector.x, i + 1);
                        }
                    }
                    return new Vector2Int(ghostvector.x, 0);
            }
            throw new Exception("在计算终点坐标时出现未知错误");
        }

        //计算Ghost的附身对象
        Vector2Int GhostForward(Vector2Int ghostvector, moveAxis axis)
        {
            switch (axis)
            {
                case (moveAxis.x):

                    for (int i = ghostvector.x; i < _map.GetLength(0); i++)
                    {
                        if (_map[i, ghostvector.y].Box != null)
                        {                          
                            return new Vector2Int(i, ghostvector.y);
                        }
                    }
                    return new Vector2Int(_map.GetLength(0) - 1, ghostvector.y);

                case (moveAxis.y):

                    for (int i = ghostvector.y; i < _map.GetLength(1); i++)
                    {
                        if (_map[ghostvector.x, i].Box != null)
                        {
                            return new Vector2Int(ghostvector.x, i);
                        }
                    }
                    return new Vector2Int(ghostvector.x, _map.GetLength(1) - 1);
            }
            throw new Exception("在计算终点坐标时出现未知错误");
        }
        Vector2Int GhostNegative(Vector2Int ghostvector, moveAxis axis)
        {
            switch (axis)
            {
                case (moveAxis.x):
                    for (int i = ghostvector.x; i > 0; i--)
                    {
                        if (_map[i, ghostvector.y].Box != null)
                        {
                            return new Vector2Int(i, ghostvector.y);
                        }
                    }
                    return new Vector2Int(0, ghostvector.y);
                case (moveAxis.y):
                    for (int i = ghostvector.y; i > 0; i--)
                    {
                        if (_map[ghostvector.x, i].Box != null)
                        {
                            return new Vector2Int(ghostvector.x, i);
                        }
                    }
                    return new Vector2Int(ghostvector.x, 0);
            }
            throw new Exception("在计算终点坐标时出现未知错误");
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

    public enum State
    {
        _Box,
        _Ghost,
    }


    public class Anim
    {
        public GameObject playobject { get; set; }
        public Vector3 Target { get; set; }
        public Play playState;
        private Vector3 currentVelocity = Vector3.zero;
        public float smoothTime;
        public enum Play
        {
            _MoveCeil,
            _MoveFloor,
            _Stay,
        }

        public Anim (GameObject boxLevel)
        {
            //smoothTime = 1f;
            playobject = boxLevel;
            playState = Play._Stay;
            Target = Floorvectorint(playobject.transform.position);
        }

        public void Anim_Move()
        {
            if (playState == Play._MoveCeil)
            {
                smoothTime += 0.5f * Time.deltaTime;
                playobject.transform.position = Vector3.Lerp(Ceilvectorint(playobject.transform.position), Target, smoothTime);
            }
            if (playState == Play._MoveFloor)
            {
                smoothTime += 0.5f * Time.deltaTime;
                playobject.transform.position = Vector3.Lerp(Floorvectorint(playobject.transform.position), Target, smoothTime);
            }
            if (Eque(playState))
            {
                //Debug.Log(playState);
                playState = Play._Stay;
            }
        }
        //向上取整
        public Vector3 Ceilvectorint(Vector3 _vector)
        {
            return new Vector3(Mathf.CeilToInt( _vector.x), Mathf.CeilToInt(_vector.y),-1);
        }
        //向下取整
        public Vector3 Floorvectorint(Vector3 _vector)
        {
            return new Vector3(Mathf.Floor(_vector.x), Mathf.Floor(_vector.y), -1);
        }

        bool Eque(Play T)
        {
            switch (T)
            {
                case (Play._MoveCeil):
                    if (Ceilvectorint(playobject.transform.position) == Ceilvectorint(Target))
                    {
                        return true;
                    }
                    break;
                case (Play._MoveFloor):
                    if (Floorvectorint(playobject.transform.position) == Floorvectorint(Target))
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }
    }
}
