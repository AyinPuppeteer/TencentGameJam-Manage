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

    /// <summary>
    /// 所属阵营
    /// </summary>
    public int Belonging { get; protected set; }

    /// <summary>
    /// 元素
    /// </summary>
    private Element Element;

    private int Level;
    public void LevelUp()
    {
        Level++;
        if (Level >= 5) Kill();
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