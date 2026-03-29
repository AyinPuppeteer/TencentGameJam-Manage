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
    private SpriteRenderer Highlight, SplitHightlight;//高光特效、繁殖标识

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
        Chess newchess = Instantiate(GameManager.Instance.ChessPrefabs[(int)e], transform.position, Quaternion.identity, transform).GetComponent<Chess>();
        newchess.SetBelonging(belonging);
        GameManager.Instance.ChessSet.Add(newchess);
        if (Chess != null)
        {
            if (Chess.Combat(newchess, Chess))
            {
                Chess.Kill();
                newchess.Anim.SetTrigger("吞噬");
                newchess.LevelUp();
                Chess = newchess;
                Chess.SetTile(this);
            }
            else
            {
                newchess.Kill();
                Chess.Anim.SetTrigger("吞噬");
                Chess.LevelUp();
            }
        }
        else
        {
            Chess = newchess;
            Chess.SetTile(this);
        }
    }

    public void CreateElementor(Element e)
    {
        Elementor = Instantiate(GameManager.Instance.ElementorPrefab, transform.position, Quaternion.identity, transform).GetComponent<Elementor>();
        Elementor.Init(e);
    }

    private float PressTimer;//按压计时器

    public void OnMouseDrag()
    {
        if (PressTimer < 1f && (PressTimer += Time.deltaTime) >= 1f)
        {
            GameManager.Instance.LongPressTile(this);//触发长按
        }
    }

    public void OnMouseUp()
    {
        if (PressTimer < 1f)
        {
            GameManager.Instance.ClickTile(this);//触发短按
        }
        PressTimer = 0;
    }

    public void OnMouseExit()
    {
        PressTimer = 0;
    }

    public void OpenHighlight(bool b, bool split)
    {
        Highlight.enabled = b && !split;
        SplitHightlight.enabled = b && split;
    }
}