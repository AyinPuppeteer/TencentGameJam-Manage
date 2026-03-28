using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 当前回合数
    /// </summary>
    private int TurnNum;
    /// <summary>
    /// 回合玩家
    /// </summary>
    private int TurnPlayer = 2;//初始为2，因为上来就要反转

    /// <summary>
    /// 所有种类的棋子
    /// </summary>
    public GameObject[] ChessPrefabs;
    /// <summary>
    /// 储存所有的棋子
    /// </summary>
    public readonly HashSet<Chess> ChessSet = new();
    /// <summary>
    /// 当前抓取的棋子
    /// </summary>
    private Chess HoldChess;

    /// <summary>
    /// 元素颗粒物体
    /// </summary>
    public GameObject ElementorPrefab;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        Instance = this;
    }

    private void Start()
    {
        TileManager.Instance.GenerateMap();

        //生成初始史莱姆
        TileManager.Instance.GetTile(1, 1).CreateSlime(1, Element.无);
        TileManager.Instance.GetTile(8, 1).CreateSlime(1, Element.无);
        TileManager.Instance.GetTile(1, 13).CreateSlime(2, Element.无);
        TileManager.Instance.GetTile(8, 13).CreateSlime(2, Element.无);

        PhaseShift();
    }

    public void PhaseShift()
    {
        TurnPlayer = 3 - TurnPlayer;
        foreach(var chess in ChessSet)
        {
            if(chess.Belonging == TurnPlayer)
            {
                //回复行动点
            }
        }

        foreach(var elementor in TileManager.Instance.GetComponentsInChildren<Elementor>())
        {
            Destroy(elementor);
        }
        List<Tile> tiles = TileManager.Instance.ReturnEmptyTiles();
        if (TurnNum % 10 == 0)
        {
            if(tiles.Count < 20)
            {
                //无法生成新资源直接判定胜利
            }
            for (int i = 0; i < 20; i++)
            {
                int t = UnityEngine.Random.Range(0, tiles.Count);
                tiles[t].CreateElementor((Element)(i % 4 + 1));
                tiles.RemoveAt(t);
            }
        }

        TurnNum++;
    }

    public void ClickTile(Tile tile)
    {
        if(HoldChess == null)
        {
            if(tile.Chess != null && tile.Chess.Belonging == TurnPlayer)
            {
                HoldChess = tile.Chess;
            }
        }
    }

    public void CatchChess(Chess chess)
    {
        HoldChess = chess;
    }
}