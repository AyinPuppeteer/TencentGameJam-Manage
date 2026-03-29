using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有棋子的父类
/// </summary>
public class Chess : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] LevelBar;

    /// <summary>
    /// 所属阵营
    /// </summary>
    public int Belonging { get; protected set; }
    [SerializeField]
    private SpriteRenderer BelongingCircle;

    /// <summary>
    /// 元素
    /// </summary>
    protected Element Element = Element.无;

    protected int Level = 1;
    public void LevelUp()
    {
        LevelBar[Level++].enabled = true;
        if (Level >= 5) Kill();
    }

    /// <summary>
    /// 能否行动
    /// </summary>
    public bool Moveable;

    private void Start()
    {
        LevelBar[0].enabled = true;
        LevelBar[1].enabled = false;
        LevelBar[2].enabled = false;
        LevelBar[3].enabled = false;
    }

    public Tile InTile { get; protected set; } 

    public void SetTile(Tile tile)
    {
        if(InTile != null) InTile.Chess = null;
        InTile = tile;
        InTile.Chess = this;
        if(InTile.Elementor != null)
        {
            ObtainElementor(InTile.Elementor.Element);
            InTile.Elementor = null;
            Destroy(InTile.Elementor);
        }
    }

    public void SetBelonging(int belonging)
    {
        Belonging = belonging;
        BelongingCircle.color = belonging == 1 ? Color.red : Color.blue;
    }

    public void MoveTo(Tile tile)
    {
        transform.DOMove(tile.transform.position, 0.3f).OnComplete(() =>
        {
            if(tile.Chess != null)
            {
                if(Combat(this, tile.Chess))
                {
                    tile.Chess.Kill();
                    LevelUp();
                    SetTile(tile);
                }
                else
                {
                    Kill();
                }
            }
        });
    }

    /// <summary>
    /// 判断战斗结果（前者为进攻方）
    /// </summary>
    public static bool Combat(Chess a, Chess b)
    {
        return a.Level + a.Element.Jugde(b.Element) > b.Level;
    }

    public void ObtainElementor(Element e)
    {
        if(Element == Element.无)
        {
            Element = e;
        }
        else if(Element == e)
        {
            LevelUp();
        }
        else
        {
            Kill();
        }
    }

    /// <summary>
    /// 被杀死
    /// </summary>
    public void Kill()
    {
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        if(InTile != null && InTile.Chess == this) InTile.Chess = null;//消除引用
        GameManager.Instance.ChessSet.Remove(this);
    }
}

public enum Element
{
    无, 水, 火, 土, 草
}

public static class ElementJudge
{
    //判断元素克制关系
    public static int Jugde(this Element a, Element b)
    {
        if (a == b) return 0;
        else if (a == Element.无) return -1;
        else if (b == Element.无) return 1;
        else if (a == Element.水)
        {
            if (b == Element.火) return 1;
            else if (b == Element.土) return -1;
        }
        else if (a == Element.火)
        {
            if (b == Element.草) return 1;
            else if (b == Element.水) return -1;
        }
        else if (a == Element.土)
        {
            if (b == Element.水) return 1;
            else if (b == Element.草) return -1;
        }
        else if (a == Element.草)
        {
            if (b == Element.土) return 1;
            else if (b == Element.火) return -1;
        }
        return 0;
    }
}