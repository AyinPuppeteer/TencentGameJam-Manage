using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Û±Í–¸Õ£∏ﬂπ‚
/// </summary>
public class HighlightOver : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer Image;

    [SerializeField]
    private Material BaseMat, HightlightMaat;

    private void OnMouseEnter()
    {
        Image.material = new(HightlightMaat);
    }

    private void OnMouseExit()
    {
        Image.material = new(BaseMat);
    }
}