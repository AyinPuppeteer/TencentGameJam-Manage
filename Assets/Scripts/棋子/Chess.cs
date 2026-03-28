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

    /// <summary>
    /// 所属阵营
    /// </summary>
    public int Belonging { get; protected set; }

    /// <summary>
    /// 元素
    /// </summary>
    private Element Element;

    private int Level;

    /// <summary>
    /// 判断战斗结果（前者为进攻方）
    /// </summary>
    public static bool Combat(Chess a, Chess b)
    {
        return false;
    }
}

public enum Element
{
    无, 水, 火, 土, 草
}