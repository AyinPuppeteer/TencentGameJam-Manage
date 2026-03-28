using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.LudiqRootObjectEditor;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private TileType Type;

    private int Row, Column;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public int Row_ => Row;
    public int Column_ => Column;

    public void Initialize(TileType type, int row, int column, Sprite tileSprite)
    {
        Type = type;
        Row = row + 1;
        Column = column + 1;
        spriteRenderer.sprite = tileSprite;
    }

    //获取与另一个图块的曼哈顿距离
    public int ManDis(Tile another)
    {
        return Math.Abs(Row - another.Row) + Math.Abs(Column - another.Column);
    }
    //获取最长距离（正方形距离）
    public int MaxDis(Tile another)
    {
        return Math.Max(Math.Abs(Row - another.Row), Math.Abs(Column - another.Column));
    }

    //方格类型
    public enum TileType
    {
        陆地, 水
    }
}
