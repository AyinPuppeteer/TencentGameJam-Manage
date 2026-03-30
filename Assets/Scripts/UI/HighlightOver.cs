using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  Û±Í–¸Õ£∏ﬂπ‚
/// </summary>
public class HighlightOver : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer Image;
    [SerializeField]
    private Image Image2;

    [SerializeField]
    private Material BaseMat, HightlightMaat;

    private void OnMouseEnter()
    {
        if(Image != null) Image.material = new(HightlightMaat);
        if(Image2 != null) Image2.material = new(HightlightMaat);
    }

    private void OnMouseExit()
    {
        if (Image != null) Image.material = new(BaseMat);
        if (Image2 != null) Image2.material = new(BaseMat);
    }
}