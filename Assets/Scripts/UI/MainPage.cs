using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPage : MonoBehaviour
{
    public void StartGame()
    {
        FadeEvent.Instance.Fadeto("BattleScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}