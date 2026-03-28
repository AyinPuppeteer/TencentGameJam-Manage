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
    /// 当前抓取的棋子
    /// </summary>
    private Chess HoldChess;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ClickPlot()
    {

    }

    public void CatchChess(Chess chess)
    {
        HoldChess = chess;
    }
}