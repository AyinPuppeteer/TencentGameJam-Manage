using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int Row, Column;
    public int Row_ => Row;
    public int Column_ => Column;

    [HideInInspector]
    public Chess Chess;//格子里的棋子
    [HideInInspector]
    public Elementor Elementor;//格子里的元素颗粒

    [SerializeField]
    private SpriteRenderer Highlight;//高光特效

    public void Initialize(int row, int column)
    {
        Row = row + 1;
        Column = column + 1;
    }

    //获取与另一个图块的曼哈顿距离
    public int ManDis(Tile another)
    {
        return Math.Abs(Row - another.Row) + Math.Abs(Column - another.Column);
    }
    //获取最长距离（正方形距离）
    public int MaxDis(Tile another)
    {
        return Math.Max(Math.Abs(Row - another.Row), Math.Abs(Column - another.Column));
    }

    public void CreateSlime(int belonging, Element e)
    {
        if (Chess != null) return;
        Chess = Instantiate(GameManager.Instance.ChessPrefabs[(int)e], transform.position, Quaternion.identity, transform).GetComponent<Chess>();
        Chess.SetTile(this);
        Chess.SetBelonging(belonging);
        GameManager.Instance.ChessSet.Add(Chess);
    }

    public void CreateElementor(Element e)
    {
        Elementor = Instantiate(GameManager.Instance.ElementorPrefab, transform.position, Quaternion.identity, transform).GetComponent<Elementor>();
        Elementor.Init(e);
    }

    public void OnMouseDown()
    {
        GameManager.Instance.ClickTile(this);
    }

    public void OpenHighlight(bool b)
    {
        Highlight.enabled = b;
    }
}