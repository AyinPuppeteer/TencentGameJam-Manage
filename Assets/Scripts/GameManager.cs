using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 当前回合数
    /// </summary>
    private int TurnNum;
    [SerializeField]
    private Text TurnText;
    /// <summary>
    /// 回合玩家
    /// </summary>
    public int TurnPlayer { get; private set; } = 2;//初始为2，因为上来就要反转
    public SpriteRenderer BoardImage;

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
    private bool Spliting;//分裂状态

    [SerializeField]
    private Material BaseMat, HighlightMat, SplitMat;//（棋子的）普通材质/高光材质/繁殖高光材质

    /// <summary>
    /// 元素颗粒物体
    /// </summary>
    public GameObject[] ElementorPrefabs;

    [SerializeField]
    private Animator Anim;

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

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            CatchChess(null);
        }

        bool p1 = false, p2 = false;
        foreach(var chess in ChessSet)
        {
            if (chess.Belonging == 1) p1 = true;
            else if (chess.Belonging == 2) p2 = true;
        }
        if (!p1) GameOver(2);
        else if (!p2) GameOver(1);
        else
        {
            bool actable = false;//还有没有能行动的棋子
            foreach (var chess in ChessSet)
            {
                if(chess.Moveable)
                {
                    actable = true;
                    break;
                }
            }
            if (!actable) PhaseShift();
        }
    }

    public void PhaseShift()
    {
        Spliting = false;
        CatchChess(null);

        TurnPlayer = 3 - TurnPlayer;
        BoardImage.material.SetColor("_RimColor", TurnPlayer == 1 ? new(3, 0, 0) : new(0, 0, 3));
        foreach(var chess in ChessSet)
        {
            if (chess.Belonging == TurnPlayer)
            {
                chess.SetMovable(true);//回复行动点
            }
            else chess.SetMovable(false);
        }

        if (TurnNum % 10 == 0)
        {
            Anim.SetTrigger("过场");
        }

        TurnNum++;
        TurnText.text = "Turn " + TurnNum;
    }

    /// <summary>
    /// 重新生成元素颗粒
    /// </summary>
    public void ResetElementor()
    {
        foreach (var elementor in TileManager.Instance.GetComponentsInChildren<Elementor>())
        {
            Destroy(elementor.gameObject);
        }
        List<Tile> tiles = TileManager.Instance.ReturnEmptyTiles();
        if (tiles.Count < 20)
        {
            //无法生成新资源直接判定胜利
            int p1 = 0, p2 = 0;
            foreach (var chess in ChessSet)
            {
                if (chess.Belonging == 1) p1++;
                else if (chess.Belonging == 2) p2++;
            }
            if (p1 == p2)
            {
                GameOver(0);
            }
            else GameOver(p1 > p2 ? 1 : 2);
        }
        for (int i = 0; i < 20; i++)
        {
            int t = UnityEngine.Random.Range(0, tiles.Count);
            tiles[t].CreateElementor((Element)(i % 4 + 1));
            tiles.RemoveAt(t);
        }
    }

    public void ClickTile(Tile tile)
    {
        if (HoldChess != null)
        {
            if (HoldChess.Moveable)
            {
                if(tile.ManDis(HoldChess.InTile) == 1)
                {
                    if (Spliting)
                    {
                        HoldChess.Split(tile);
                        HoldChess.SetMovable(false);
                    }
                    else
                    {
                        HoldChess.MoveTo(tile);
                        HoldChess.SetMovable(false);
                    }
                    CatchChess(null);
                    return;
                }
            }
            CatchChess(null);
        }
        if (tile.Chess != null && tile.Chess.Belonging == TurnPlayer)
        {
            CatchChess(tile.Chess);
        }
    }

    public void LongPressTile(Tile tile)
    {
        if (tile.Chess != null && tile.Chess.Belonging == TurnPlayer && tile.Chess.Level == 4)
        {
            CatchChess(tile.Chess, true);
        }
    }

    public void CatchChess(Chess chess, bool split = false)
    {
        Spliting = split;
        TileManager.Instance.DimAll();
        if (HoldChess != null) HoldChess.SetMat(BaseMat);
        HoldChess = chess;
        if (chess != null && chess.Moveable)
        {
            HoldChess.SetMat(Spliting ? SplitMat : HighlightMat);
            TileManager.Instance.GetTile(chess.InTile.Row_, chess.InTile.Column_ + 1)?.OpenHighlight(true, Spliting);
            TileManager.Instance.GetTile(chess.InTile.Row_, chess.InTile.Column_ - 1)?.OpenHighlight(true, Spliting);
            TileManager.Instance.GetTile(chess.InTile.Row_ + 1, chess.InTile.Column_)?.OpenHighlight(true, Spliting);
            TileManager.Instance.GetTile(chess.InTile.Row_ - 1, chess.InTile.Column_)?.OpenHighlight(true, Spliting);
        }
    }

    public void GameOver(int winner)
    {
        ResultPage.SetText(winner);
        FadeEvent.Instance.Fadeto("ResultScene");
    }
}