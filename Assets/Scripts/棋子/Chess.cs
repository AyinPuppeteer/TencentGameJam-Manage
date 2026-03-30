using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 所有棋子的父类
/// </summary>
public class Chess : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer Icon;

    public Animator Anim;

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

    public int Level { get; protected set; } = 1;
    public void LevelUp()
    {
        Level++;
        LevelBar[0].enabled = Level > 0;
        LevelBar[1].enabled = Level > 1;
        LevelBar[2].enabled = Level > 2;
        LevelBar[3].enabled = Level > 3;
        if (Level >= 5) Kill();
    }

    public Image SplitTag;//可分裂记号

    public Image MoveableBar;//体力条
    /// <summary>
    /// 能否行动
    /// </summary>
    public bool Moveable { get; protected set; }
    public void SetMovable(bool b)
    {
        Moveable = b;
        MoveableBar.enabled = b;
    }

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
            InTile.Elementor.WhenPick(this);
            InTile.Elementor = null;
        }
    }

    public void SetBelonging(int belonging)
    {
        Belonging = belonging;
        BelongingCircle.color = belonging == 1 ? Color.red : Color.blue;
    }

    public void MoveTo(Tile tile)
    {
        Anim.SetBool("移动", true);
        transform.DOMove(tile.transform.position, 0.3f).OnComplete(() =>
        {
            Anim.SetBool("移动", false);
            if (tile.Chess != null)
            {
                if(Combat(this, tile.Chess))
                {
                    tile.Chess.Kill();
                    Anim.SetTrigger("吞噬");
                    LevelUp();
                    SetTile(tile);
                }
                else
                {
                    Kill();
                    tile.Chess.Anim.SetTrigger("吞噬");
                    tile.Chess.LevelUp();
                }
            }
            else
            {
                SetTile(tile);
            }
        });
    }

    /// <summary>
    /// 判断战斗结果（前者为进攻方）
    /// </summary>
    public static bool Combat(Chess a, Chess b)
    {
        return a.Level + a.Element.Jugde(b.Element) >= b.Level;
    }

    public void ObtainElementor(Element e)
    {
        Anim.SetTrigger("吞噬");
        DOTween.To(() => 0, x => { }, 0, 0.5f).OnComplete(() =>
        {
            if (Element == e)
            {
                LevelUp();
            }
            else
            {
                Kill();
                if (Element == Element.无)
                {
                    InTile.CreateSlime(Belonging, e);
                }
            }
        });
    }

    public void SetMat(Material mat)
    {
        Icon.material = new Material(mat);
    }

    private void Update()
    {
        SplitTag.enabled = Level >= 4 && Moveable;
    }

    public void Split(Tile tile)
    {
        Level = 2;
        LevelBar[2].enabled = false;
        LevelBar[3].enabled = false;
        tile.CreateSlime(Belonging, Element.无);
    }

    /// <summary>
    /// 被杀死
    /// </summary>
    public void Kill()
    {
        if (InTile != null && InTile.Chess == this) InTile.Chess = null;//消除引用
        GameManager.Instance.ChessSet.Remove(this);
        Destroy(gameObject);
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