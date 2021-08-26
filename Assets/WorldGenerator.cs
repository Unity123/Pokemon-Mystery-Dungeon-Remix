using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    /*class SubDungeon
    {

    }
    class Room
    {
        public RectInt rect;
        public List<Room> connections = new List<Room>();
        public Room(RectInt rect)
        {
            this.rect = rect;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        List<Room> rooms = new List<Room>();
        for (int i = 0; i < 8; i++)
        {
            rooms.Add(new Room(new RectInt(Random.Range(32, size - 32), Random.Range(32, size - 32), Random.Range(0, 32), Random.Range(0, 32))));
            BoxFill(map, ground, new Vector3Int(rooms[i].rect.x, rooms[i].rect.y, 0), new Vector3Int(rooms[i].rect.x + rooms[i].rect.width, rooms[i].rect.y + rooms[i].rect.height, 0));
        }
        for (int i = 0; i < 8; i++)
        {
            do {
                rooms[i].connections.Add(rooms[Random.Range(0, rooms.ToArray().Length)]);
            } while (Random.Range(0, 2) == 1);
            foreach (Room r in rooms[i].connections)
            {
                for (int j = rooms[i].rect.x; (rooms[i].rect.x - r.rect.x) > 0 ? j < r.rect.x : j > r.rect.x;)
                {
                    for (int k = rooms[i].rect.x; (rooms[i].rect.y - r.rect.y) > 0 ? k < r.rect.x : k > r.rect.x;)
                    {
                        map.SetTile(new Vector3Int(j, k, 0), ground);
                        if ((rooms[i].rect.y - r.rect.y) > 0) {
                            k++;
                        } else
                        {
                            k--;
                        }
                    }
                    if ((rooms[i].rect.x - r.rect.x) > 0)
                    {
                        j++;
                    }
                    else
                    {
                        j--;
                    }
                }
            }
        }
        map.FloodFill(new Vector3Int(0, 0, 0), wall);
    }*/
    public int size = 128;
    public Tilemap map;
    public TileBase ground;
    public TileBase wall;
    public void BoxFill(Tilemap map, TileBase tile, Vector3Int start, Vector3Int end)
    {
        //Determine directions on X and Y axis
        var xDir = start.x < end.x ? 1 : -1;
        var yDir = start.y < end.y ? 1 : -1;
        //How many tiles on each axis?
        int xCols = 1 + Mathf.Abs(start.x - end.x);
        int yCols = 1 + Mathf.Abs(start.y - end.y);
        //Start painting
        for (var x = 0; x < xCols; x++)
        {
            for (var y = 0; y < yCols; y++)
            {
                var tilePos = start + new Vector3Int(x * xDir, y * yDir, 0);
                map.SetTile(tilePos, tile);
            }
        }
    }

    //Small override, to allow for world position to be passed directly
    public void BoxFill(Tilemap map, TileBase tile, Vector3 start, Vector3 end)
    {
        BoxFill(map, tile, map.WorldToCell(start), map.WorldToCell(end));
    }
    class SubDungeon
    {
        public SubDungeon child1;
        public SubDungeon child2;
        public SubDungeon root;
        public RectInt rect;
        public RectInt room;
        public bool isConnectedLeft = false;
        public bool isConnectedRight = false;
        Tilemap map; TileBase ground; TileBase wall;
        public SubDungeon(Tilemap map, TileBase ground, TileBase wall)
        {
            root = this;
            rect = new RectInt(0, 0, 128, 128);
            this.ground = ground;
            this.map = map;
            this.wall = wall;
            Split();
        }
        public RectInt GetRoom()
        {
            if (child1 == null && child2 == null)
            {
                return room;
            }
            if (child1 != null)
            {
                return child1.GetRoom();
            }
            if (child2 != null)
            {
                return child2.GetRoom();
            }
            return new RectInt(-1, -1, 0, 0);
        }
        public SubDungeon(RectInt r, Tilemap map, TileBase ground, TileBase wall, SubDungeon ro)
        {
            this.ground = ground;
            this.map = map;
            this.wall = wall;
            root = ro;
            rect = r;
            if (rect.width < 5 || rect.height < 5)
            {
                child1 = null;
                child2 = null;
                return;
            }
            Split();
        }

        public void Split()
        {
            RectInt r1, r2;
            float splitVal = Random.Range(2f, 8f);
            if (rect.width < rect.height)
            {
                r1 = new RectInt(rect.xMin, rect.yMin, rect.width, (int)((float)rect.height / splitVal));
                r2 = new RectInt(rect.xMin, rect.yMin + (int)((float)rect.height / splitVal), rect.width, Mathf.Abs((int)((float)rect.height / (1 - splitVal))));
            } else
            {
                r1 = new RectInt(rect.xMin, rect.yMin, (int)((float)rect.width / splitVal), rect.height);
                r2 = new RectInt(rect.xMin + (int)((float)rect.width / splitVal), rect.yMin, Mathf.Abs((int)(((float)rect.width / (1 - splitVal)))), rect.height);
            }
            print("Splitting into parts " + r1 + " " + r2);
            child1 = new SubDungeon(r1, map, ground, wall, root);
            child2 = new SubDungeon(r2, map, ground, wall, root);
        }
    }
    List<SubDungeon> rooms;
    private void Start()
    {
        SubDungeon s = new SubDungeon(map, ground, wall);
        rooms = new List<SubDungeon>();
        MakeRooms(s);
        for (int i = -128; i < 128; i++)
        {
            for (int j = -128; j < 128; j++)
            {
                if (map.GetTile(new Vector3Int(i, j, 0)) != ground)
                {
                    map.SetTile(new Vector3Int(i, j, 0), wall);
                }
            }
        }
    }

    void MakeRooms(SubDungeon s)
    {
        SubDungeon sub = s;
        if (sub.child1 == null)
        {
            int roomWidth = (int)Random.Range(sub.rect.width / 2,sub.rect.width - 2);
            int roomHeight = (int)Random.Range(sub.rect.height / 2, sub.rect.height - 2);
            int roomX = (int)Random.Range(1, sub.rect.width - roomWidth - 1);
            int roomY = (int)Random.Range(1, sub.rect.height - roomHeight - 1);
            sub.room = new RectInt(sub.rect.xMin + roomX, sub.rect.yMin + roomY, roomWidth, roomHeight);
            BoxFill(map, ground, new Vector3Int(sub.room.xMin, sub.room.yMin, 0), new Vector3Int(sub.room.xMax, sub.room.yMax, 0));
            rooms.Add(sub);
            print(rooms.ToArray().Length);
            if (rooms.ToArray().Length == 2)
            {
                List<RectInt> corridors = CreateCorridorBetween(rooms[0], rooms[1]);
                foreach (RectInt r in corridors)
                {
                    if (r.height > r.width)
                    {
                        for (int i = 0; i <= r.height; i++)
                        {
                            map.SetTile(new Vector3Int(r.xMin, r.yMin + i, 0), ground);
                        }
                    }
                    else if (r.width > r.height)
                    {
                        for (int i = 0; i <= r.width; i++)
                        {
                            map.SetTile(new Vector3Int(r.xMin + i, r.yMin, 0), ground);
                        }
                    } else
                    {
                        map.SetTile(new Vector3Int(r.xMin, r.yMin, 0), ground);
                    }
                }
                rooms.RemoveAt(0);
            } 
        } else
        {
            MakeRooms(sub.child1);
            MakeRooms(sub.child2);
        }
    }
    List<RectInt> CreateCorridorBetween(SubDungeon left, SubDungeon right)
    {
        RectInt lroom = left.GetRoom();
        RectInt rroom = right.GetRoom();
        List<RectInt> corridors = new List<RectInt>();

        // attach the corridor to a random point in each room
        Vector2 lpoint = new Vector2((int)Random.Range(lroom.x + 1, lroom.xMax - 1), (int)Random.Range(lroom.y + 1, lroom.yMax - 1));
        Vector2 rpoint = new Vector2((int)Random.Range(rroom.x + 1, rroom.xMax - 1), (int)Random.Range(rroom.y + 1, rroom.yMax - 1));
        print("Creating corridors between " + lpoint + " " + rpoint);
        // always be sure that left point is on the left to simplify the code
        if (lpoint.x > rpoint.x)
        {
            Vector2 temp = lpoint;
            lpoint = rpoint;
            rpoint = temp;
        }

        int xDist = (int)(lpoint.x - rpoint.x);
        int yDist = (int)(lpoint.y - rpoint.y);

        // if the points are not aligned horizontally
        if (xDist != 0)
        {
            // choose at random to go horizontal then vertical or the opposite
            if (Random.Range(0, 5) > 2)
            {
                // add a corridor to the right
                corridors.Add(new RectInt((int)lpoint.x, (int)lpoint.y, Mathf.Abs(xDist) + 1, 0));

                // if left point is below right point go up
                // otherwise go down
                if (yDist < 0)
                {
                    corridors.Add(new RectInt((int)rpoint.x, (int)lpoint.y, 1, Mathf.Abs(yDist)));
                }
                else
                {
                    corridors.Add(new RectInt((int)rpoint.x, (int)lpoint.y, 1, -Mathf.Abs(yDist)));
                }
            }
            else
            {
                // go up or down
                if (yDist < 0)
                {
                    corridors.Add(new RectInt((int)lpoint.x, (int)lpoint.y, 1, Mathf.Abs(yDist)));
                }
                else
                {
                    corridors.Add(new RectInt((int)lpoint.x, (int)rpoint.y, 1, Mathf.Abs(yDist)));
                }

                // then go right
                corridors.Add(new RectInt((int)lpoint.x, (int)rpoint.y, Mathf.Abs(xDist) + 1, 1));
            }
        }
        else
        {
            // if the points are aligned horizontally
            // go up or down depending on the positions
            if (yDist < 0)
            {
                corridors.Add(new RectInt((int)lpoint.x, (int)lpoint.y, 1, Mathf.Abs(yDist)));
            }
            else
            {
                corridors.Add(new RectInt((int)rpoint.x, (int)rpoint.y, 1, Mathf.Abs(yDist)));
            }
        }
        foreach (RectInt corridor in corridors)
        {
            print(corridor);
        }
        return corridors;
    }
        // Update is called once per frame
        void Update()
    {
        
    }
}
