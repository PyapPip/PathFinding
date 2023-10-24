using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //길찾기, 맵 관리

    public GameObject Player;
    public int[,] Map = new int[11, 10]
    {
        {8,8,8,6,4,7,4,3,2,0},
        {8,8,8,6,4,4,4,3,2,0},
        {8,8,0,0,4,3,3,3,2,0},
        {6,6,0,0,4,2,2,2,2,0},
        {4,4,4,4,4,2,2,2,1,1},
        {4,4,4,4,4,3,3,2,1,1},
        {4,4,4,4,4,3,3,2,2,1},
        {5,5,4,4,4,3,3,2,2,1},
        {5,4,4,4,4,3,3,2,2,1},
        {4,0,4,4,0,3,2,5,2,1},
        {4,4,3,3,3,3,2,2,1,1}
    };

    private int count;
    private List<Pos> openList;
    private bool[,] vist;
    private int[,] route;

    private struct Pos
    {
        public int x, y, z; //좌표 
        public int g;       //현재까지의 코스트
        public int h;       //예상 코스트
        public int f;       //윗 코스트들의 총합, 가중치
        public int p;       //탐색 과정에서 이전 타일의 방향을 나타내는 변수. 0은 이전 타일이 없음, 1은 y--, 2는 x--, 3은 y++, 4는 y++. z축 차이가 있으면 +4

        public static bool operator ==(Pos p1, Pos p2) => p1.x == p2.x && p1.y == p2.y;
        public static bool operator !=(Pos p1, Pos p2) => p1.x != p2.x || p1.y != p2.y;

        public Pos(int x, int y, int z)
        {
            this.x = x; this.y = y; this.z = z;
            f = 0; g = 0; h = 0; p = 0;
        }
        public void total(Pos _goal)
        {
            this.h = Mathf.Abs(_goal.x - this.x) + Mathf.Abs(_goal.y - this.y);
            f = g + h;
        }
    }

    private void Explore(Pos _search_Point, Pos _goal)
    {

        vist[_search_Point.y, _search_Point.x] = true;
        route[_search_Point.y, _search_Point.x] = _search_Point.p;
        _search_Point.g++;
        Pos temp = _search_Point;
        temp.z = Map[temp.y, temp.x];
        int c = 1;

        // x, y-1, z
        _search_Point.y--;
        if (_search_Point.y >= 0)
        {
            _search_Point.z = Map[_search_Point.y, _search_Point.x];
            if (_search_Point.z != 0 && Mathf.Abs(_search_Point.z - temp.z) <= Player.GetComponent<Player>().Jump && !vist[_search_Point.y, _search_Point.x])
            {
                _search_Point.total(_goal);
                _search_Point.p = c;
                openList.Add(_search_Point);
            }
        }
        c++;

        // x-1, y, z
        _search_Point.x--; _search_Point.y++;
        if (_search_Point.x >= 0)
        {
            _search_Point.z = Map[_search_Point.y, _search_Point.x];
            if (_search_Point.z != 0 && Mathf.Abs(_search_Point.z - temp.z) <= Player.GetComponent<Player>().Jump && !vist[_search_Point.y, _search_Point.x])
            {
                _search_Point.total(_goal);
                _search_Point.p = c;
                openList.Add(_search_Point);
            }
        }
        c++;

        // x+1, y, z
        _search_Point.x += 2;
        if (_search_Point.x < Map.GetLength(1))
        {
            _search_Point.z = Map[_search_Point.y, _search_Point.x];
            if (_search_Point.z != 0 && Mathf.Abs(_search_Point.z - temp.z) <= Player.GetComponent<Player>().Jump && !vist[_search_Point.y, _search_Point.x])
            {
                _search_Point.total(_goal);
                _search_Point.p = c;
                openList.Add(_search_Point);
            }
        }
        c++;

        // x, y+1, z
        _search_Point.x--; _search_Point.y++;
        if (_search_Point.y < Map.GetLength(0))
        {
            _search_Point.z = Map[_search_Point.y, _search_Point.x];
            if (_search_Point.z != 0 && Mathf.Abs(_search_Point.z - temp.z) <= Player.GetComponent<Player>().Jump && !vist[_search_Point.y, _search_Point.x])
            {
                _search_Point.total(_goal);
                _search_Point.p = c;
                openList.Add(_search_Point);
            }
        }

        count++;
    }

    /// <summary>
    /// 플레이어에게 루트 전달
    /// </summary>
    private void SendRouteToPlayer(int _x, int _y)
    {
        List<int> list = new List<int>();
        while (route[_y, _x] != 0)
        {
            switch (route[_y, _x])
            {
                case 1:
                    {
                        list.Add(Map[_y, _x] - Map[_y + 1, _x]);
                        list.Add(route[_y, _x]);
                        _y++;
                        break;
                    }
                case 2:
                    {
                        list.Add(Map[_y, _x] - Map[_y, _x + 1]);
                        list.Add(route[_y, _x]);
                        _x++;
                        break;
                    }
                case 3:
                    {
                        list.Add(Map[_y, _x] - Map[_y, _x - 1]);
                        list.Add(route[_y, _x]);
                        _x--;
                        break;
                    }
                case 4:
                    {
                        list.Add(Map[_y, _x] - Map[_y - 1, _x]);
                        list.Add(route[_y, _x]);
                        _y--;
                        break;
                    }
            }
        }
        Debug.Log(list.Count);
        Player.GetComponent<PlayerAnimetion>().FollowRoute(list);
        list.Clear();
    }

    public void Path_Finding(int _x, int _y, int _z)
    {
        Pos goal = new Pos(_x, _y, _z);
        vist = new bool[Map.GetLength(0), Map.GetLength(1)];
        route = new int[Map.GetLength(0), Map.GetLength(1)];
        openList = new List<Pos>();

        Pos search_Point = new Pos(Player.GetComponent<Player>().X, Player.GetComponent<Player>().Y, Player.GetComponent<Player>().Z);
        vist[search_Point.y, search_Point.x] = true;
        Explore(search_Point, goal);

        while (true)
        {
            //길찾기 실패
            if (count > 1000)
            {
                Player.GetComponent<PlayerAnimetion>().NavigationError();
                Debug.Log("Loop");
                count = 0;
                return;
            }

            Pos temp = new Pos();
            temp.f = 9999999;
            for (int i = 0; i < openList.Count; i++)
            {
                //도착
                if (openList[i] == goal)
                {
                    route[openList[i].y, openList[i].x] = openList[i].p;
                    SendRouteToPlayer(openList[i].x, openList[i].y);   //과정을 리스트, 배열에 저장하여 플레이어에게 옮겨야함.
                    openList.Clear();
                    vist = new bool[Map.GetLength(0), Map.GetLength(1)];
                    return;
                }

                if (temp.f > openList[i].f && !vist[openList[i].y, openList[i].x])
                {
                    temp = openList[i];
                }
            }
            Explore(temp, goal);
        }
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        count = 0;
        //Instance = this;
    }
}


