using UnityEngine;

public class TileManager : MonoBehaviour
{
    //¸Ê »ý¼º

    public GameObject Tile;

    private void Create_Map(int[,] _Map, GameObject _Player)
    {
        for (int y = 0; y < _Map.GetLength(0); y++)
        {
            for (int x = 0; x < _Map.GetLength(1); x++)
            {
                if (_Map[y, x] != 0)
                {
                    for (int z = 1; z <= _Map[y, x]; z++)
                    {
                        GameObject instance = Instantiate(Tile, new Vector3(x * -0.3f, -0.2f + (z * 0.1f), y * -0.3f), Quaternion.identity);
                        instance.transform.parent = this.transform;
                        instance.name = x + "," + y + "," + z;
                        instance.GetComponent<Tile>().x = x;
                        instance.GetComponent<Tile>().y = y;
                        instance.GetComponent<Tile>().z = z;

                        if (z == _Map[y, x])
                        {
                            if (x == 0 && y == 0)
                            {
                                _Player.transform.position = new Vector3(x, z * 0.1f, y);
                                _Player.GetComponent<Player>().Z = _Map[0, 0];
                            }

                            instance.GetComponent<BoxCollider>().enabled = true;
                        }
                    }
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Create_Map(this.GetComponentInParent<MapManager>().Map, this.GetComponentInParent<MapManager>().Player);
    }
}
