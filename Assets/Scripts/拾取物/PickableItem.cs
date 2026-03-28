using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可拾取物的父类
/// </summary>
public abstract class PickableItem : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer Icon;

    /// <summary>
    /// 被拾取时
    /// </summary>
    public abstract void WhenPick(Chess picker);
}