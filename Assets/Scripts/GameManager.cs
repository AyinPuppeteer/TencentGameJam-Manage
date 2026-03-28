using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 回合玩家
    /// </summary>
    private int TurnPlayer = 1;

    /// <summary>
    /// 所有种类的棋子
    /// </summary>
    public GameObject[] ChessPrefabs;

    /// <summary>
    /// 当前抓取的棋子
    /// </summary>
    private Chess HoldChess;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
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