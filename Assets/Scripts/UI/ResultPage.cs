using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPage : MonoBehaviour
{
    [SerializeField]
    private Text Text;

    private static int Winner;

    public static void SetText(int winner)
    {
        Winner = winner;
    }

    private void Start()
    {
        if(Winner == 1)
        {
            Text.text = "红方胜利！";
            Text.color = Color.red;
        }
        else if(Winner == 2)
        {
            Text.text = "蓝方胜利！";
            Text.color = Color.blue;
        }
        else if(Winner == 0)
        {
            Text.text = "平局！？\n你俩是真把这游戏玩明白了";
            Text.color = Color.white;
        }
    }

    public void Return()
    {
        FadeEvent.Instance.Fadeto("MainScene");
    }
}