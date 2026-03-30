using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeEvent : MonoBehaviour
{
    [SerializeField]
    private Animator Anim;

    private string SceneName;

    public static FadeEvent Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ShiftScene()
    {
        if(SceneName != null) SceneManager.LoadScene(SceneName);
    }

    public void Fadeto(string name)
    {
        SceneName = name;
        Anim.SetBool("Fade", true);
    }
}