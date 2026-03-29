using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 可拾取的元素
/// </summary>
public class Elementor : PickableItem
{
    public Element Element;

    public override void WhenPick(Chess picker)
    {
        picker.ObtainElementor(Element);
        Destroy(gameObject);
    }
}