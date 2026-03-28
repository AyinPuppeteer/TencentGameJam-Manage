using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有棋子的父类
/// </summary>
public class Chess : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer Icon;
    [SerializeField]
    private Sprite[] Icons;

    [SerializeField]
    private SpriteRenderer LevelFrame;
    [SerializeField]
    private Sprite[] Frames;

    [SerializeField]
    private SpriteRenderer[] LevelBar;
    private Color[] BarColor = new Color[5] { new(0.65f, 1, 1, 1), new(0.65f, 0.7f, 1, 1), new(1f, 0.65f, 0.65f, 1), new(0.65f, 1, 1, 1), new(0.6f, 0.4f, 0.1f, 1) };

    /// <summary>
    /// 所属阵营
    /// </summary>
    public int Belonging { get; protected set; }

    /// <summary>
    /// 元素
    /// </summary>
    private Element Element;

    private int Level = 1;
    public void LevelUp()
    {
        LevelBar[Level++].enabled = true;
        if (Level >= 5) Kill();
    }

    private void Start()
    {
        LevelBar[0].enabled = true;
        LevelBar[1].enabled = false;
        LevelBar[2].enabled = false;
        LevelBar[3].enabled = false;
    }

    private Tile InTile;

    public void SetTile(Tile tile)
    {
        InTile = tile;
    }

    /// <summary>
    /// 判断战斗结果（前者为进攻方）
    /// </summary>
    public static bool Combat(Chess a, Chess b)
    {
        switch (a.Element.Jugde(b.Element))
        {
            case 0:
                {
                    return a.Level >= b.Level;
                }
            case 1:
                {
                    return a.Level != 4 || b.Level != 1;
                }
            case -1:
                {
                    return a.Level == 4 && b.Level == 1;
                }
        }
        return true;
    }

    public void ObtainElementor(Element e)
    {
        if(Element == Element.无)
        {
            Element = e;
            Icon.sprite = Icons[(int)e];
            LevelFrame.sprite = Frames[(int)e];
            LevelBar[0].color = LevelBar[1].color = LevelBar[2].color = LevelBar[3].color = BarColor[(int)e];
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
        InTile.Chess = null;//消除引用
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