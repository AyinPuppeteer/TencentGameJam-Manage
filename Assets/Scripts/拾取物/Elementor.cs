using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可拾取的元素
/// </summary>
public class Elementor : PickableItem
{
    private Element Element;

    [SerializeField]
    private Sprite[] Icons;

    public void Init(Element e)
    {
        Element = e;
        Icon.sprite = Icons[(int)Element];
    }

    public override void WhenPick(Chess picker)
    {
        picker.ObtainElementor(Element);
        Destroy(gameObject);
    }
}