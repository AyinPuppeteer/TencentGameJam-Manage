using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可拾取的元素
/// </summary>
public class Elementor : PickableItem
{
    public Element Element { get; private set; }

    [SerializeField]
    private Sprite[] Icons;

    public void Init(Element e)
    {
        Element = e;
        Icon.sprite = Icons[(int)Element - 1];
    }

    public override void WhenPick(Chess picker)
    {
        picker.ObtainElementor(Element);
        Destroy(gameObject);
    }
}