using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static Tile;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private Sprite[] spriteList;//贴图列表

    public int maxHeight { get; private set; }
    public int maxWidth { get; private set; }

    private Tile[,] tileList;//地图格子实体列表
    public Tile GetTile(int x, int y)
    {
        if (x > 0 && y > 0 && maxHeight >= x && maxWidth >= y)
        {
            return tileList[x - 1, y - 1];
        }
        else
        {
            return null;
        }
    }

    public Tile GetTile(Vector2 pos)
    {
        int SelectIndexX = Mathf.FloorToInt(maxHeight / 2.0f - (pos.y - transform.position.y) / 0.16f);
        int SelectIndexY = Mathf.FloorToInt((pos.x - transform.position.x) / 0.16f + maxWidth / 2.0f);
        return GetTile(SelectIndexX + 1, SelectIndexY + 1);
    }

    private Camera main2DCamera;

    public static TileManager Instance { get; private set; }

    private void Awake()
    {
        main2DCamera = Camera.main;
        Instance = this;

        //测试用地图生成
        MapPack map = new();
        map.Tiles = new int[13, 8]
        {
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0}

        };
        GenerateMap(map);
    }

    private void Update()
    {
        TileChoose();
    }

    public void GenerateMap(MapPack pack)
    {
        if (tileList != null)
        {
            Debug.LogError("重复加载地图！");
        }
        else if (pack == null || pack.Tiles == null)
        {
            Debug.LogWarning("地图规格未设置！");
        }
        else
        {
            maxHeight = pack.Tiles.GetLength(0);
            maxWidth = pack.Tiles.GetLength(1);

            tileList = new Tile[maxHeight, maxWidth];

            for (int i = 0; i < maxHeight; i++)
            {
                for (int j = 0; j < maxWidth; j++)
                {
                    GameObject ob = Instantiate(tilePrefab, transform.position + new Vector3((j - maxWidth / 2.0f) * 0.16f + 0.08f, (maxHeight / 2.0f - i) * 0.16f - 0.08f, 0), Quaternion.identity, transform);
                    Tile tile = ob.GetComponent<Tile>();
                    tile.Initialize((TileType)pack.Tiles[i, j], i, j, spriteList[pack.Tiles[i, j]]);
                    tileList[i, j] = tile;
                    //Debug.Log($"生成了一个{(TileType)pack.Tiles[i, j]}格子，坐标为({i + 1}, {j + 1})");
                }
            }
        }
    }

    private void TileChoose()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x < 380 || Input.mousePosition.x > 1220) return;
            Vector3 mouseWorldPos = main2DCamera.ScreenToWorldPoint(Input.mousePosition);

            Tile tile = GetTile(mouseWorldPos);
            //if (tile != null) tile.whenChosen(true);
        }
    }

    public bool RangeJudge(Tile tile, int sx, int sy, int lx, int ly)
    {
        if (tile.Row_ >= sx && tile.Row_ < sx + lx && tile.Column_ >= sy && tile.Column_ < sy + ly)
        {
            return true;
        }
        return false;
    }

    public List<Tile> ReturnTiles(int sx, int sy, int lx, int ly)
    {
        List<Tile> tiles = new();
        for (int i = sx; i < sx + lx; i++)
        {
            for (int j = sy; j < sy + ly; j++)
            {
                Tile tile = GetTile(i, j);
                if (tile != null) tiles.Add(tile);
            }
        }
        return tiles;
    }

    //地图配置包
    public class MapPack
    {
        public int[,] Tiles;
    }
}
